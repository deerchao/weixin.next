using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Weixin.Next.Utilities
{
    // Modified version of http://stackoverflow.com/questions/2196767/c-implementing-networkstream-peek/7281113#7281113
    /// <summary>
    /// PeekableStream wraps a Stream and can be used to peek ahead in the underlying stream,
    /// without consuming the bytes. In other words, doing Peek() will allow you to look ahead in the stream,
    /// but it won't affect the result of subsequent Read() calls.
    /// 
    /// This is sometimes necessary, e.g. for peeking at the magic number of a stream of bytes and decide which
    /// stream processor to hand over the stream.
    /// </summary>
    internal class PeekableStream : Stream
    {
        private readonly Stream _underlyingStream;
        private readonly byte[] _lookAheadBuffer;

        private int _lookAheadIndex;

        public PeekableStream(Stream underlyingStream, int maxPeekBytes)
        {
            this._underlyingStream = underlyingStream;
            _lookAheadBuffer = new byte[maxPeekBytes];
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _underlyingStream.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Peeks at a maximum of count bytes, or less if the stream ends before that number of bytes can be read.
        /// 
        /// Calls to this method do not influence subsequent calls to Read() and Peek().
        /// 
        /// Please note that this method will always peek count bytes unless the end of the stream is reached before that - in contrast to the Read()
        /// method, which might read less than count bytes, even though the end of the stream has not been reached.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and
        /// (offset + number-of-peeked-bytes - 1) replaced by the bytes peeked from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data peeked from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be peeked from the current stream.</param>
        /// <returns>The total number of bytes peeked into the buffer. If it is less than the number of bytes requested then the end of the stream has been reached.</returns>
        public virtual int Peek(byte[] buffer, int offset, int count)
        {
            if (count > _lookAheadBuffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "must be smaller than peekable size, which is " + _lookAheadBuffer.Length);

            while (_lookAheadIndex < count)
            {
                int bytesRead = _underlyingStream.Read(_lookAheadBuffer, _lookAheadIndex, count - _lookAheadIndex);

                if (bytesRead == 0) // end of stream reached
                    break;

                _lookAheadIndex += bytesRead;
            }

            int peeked = Math.Min(count, _lookAheadIndex);
            Array.Copy(_lookAheadBuffer, 0, buffer, offset, peeked);
            return peeked;
        }

        /// <summary>
        /// Peeks at a maximum of count bytes, or less if the stream ends before that number of bytes can be read.
        /// 
        /// Calls to this method do not influence subsequent calls to Read() and Peek().
        /// 
        /// Please note that this method will always peek count bytes unless the end of the stream is reached before that - in contrast to the Read()
        /// method, which might read less than count bytes, even though the end of the stream has not been reached.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and
        /// (offset + number-of-peeked-bytes - 1) replaced by the bytes peeked from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data peeked from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be peeked from the current stream.</param>
        /// <returns>The total number of bytes peeked into the buffer. If it is less than the number of bytes requested then the end of the stream has been reached.</returns>
        public virtual async Task<int> PeekAsync(byte[] buffer, int offset, int count)
        {
            if (count > _lookAheadBuffer.Length)
                throw new ArgumentOutOfRangeException(nameof(count), "must be smaller than peekable size, which is " + _lookAheadBuffer.Length);

            while (_lookAheadIndex < count)
            {
                int bytesRead = await _underlyingStream.ReadAsync(_lookAheadBuffer, _lookAheadIndex, count - _lookAheadIndex).ConfigureAwait(false);

                if (bytesRead == 0) // end of stream reached
                    break;

                _lookAheadIndex += bytesRead;
            }

            int peeked = Math.Min(count, _lookAheadIndex);
            Array.Copy(_lookAheadBuffer, 0, buffer, offset, peeked);
            return peeked;
        }

        public override bool CanRead { get { return true; } }

        public override long Position
        {
            get
            {
                return _underlyingStream.Position - _lookAheadIndex;
            }
            set
            {
                _underlyingStream.Position = value;
                _lookAheadIndex = 0; // this needs to be done AFTER the call to underlyingStream.Position, as that might throw NotSupportedException, 
                                     // in which case we don't want to change the lookAhead status
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesTakenFromLookAheadBuffer = 0;
            if (count > 0 && _lookAheadIndex > 0)
            {
                bytesTakenFromLookAheadBuffer = Math.Min(count, _lookAheadIndex);
                Array.Copy(_lookAheadBuffer, 0, buffer, offset, bytesTakenFromLookAheadBuffer);
                count -= bytesTakenFromLookAheadBuffer;
                offset += bytesTakenFromLookAheadBuffer;
                _lookAheadIndex -= bytesTakenFromLookAheadBuffer;
                if (_lookAheadIndex > 0) // move remaining bytes in lookAheadBuffer to front
                                         // copying into same array should be fine, according to http://msdn.microsoft.com/en-us/library/z50k9bft(v=VS.90).aspx :
                                         // "If sourceArray and destinationArray overlap, this method behaves as if the original values of sourceArray were preserved
                                         // in a temporary location before destinationArray is overwritten."
                    Array.Copy(_lookAheadBuffer, _lookAheadBuffer.Length - bytesTakenFromLookAheadBuffer + 1, _lookAheadBuffer, 0, _lookAheadIndex);
            }

            return count > 0
                ? bytesTakenFromLookAheadBuffer + _underlyingStream.Read(buffer, offset, count)
                : bytesTakenFromLookAheadBuffer;
        }

        public override int ReadByte()
        {
            if (_lookAheadIndex > 0)
            {
                _lookAheadIndex--;
                byte firstByte = _lookAheadBuffer[0];
                if (_lookAheadIndex > 0) // move remaining bytes in lookAheadBuffer to front
                    Array.Copy(_lookAheadBuffer, 1, _lookAheadBuffer, 0, _lookAheadIndex);
                return firstByte;
            }
            else
            {
                return _underlyingStream.ReadByte();
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long ret = _underlyingStream.Seek(offset, origin);
            _lookAheadIndex = 0; // this needs to be done AFTER the call to underlyingStream.Seek(), as that might throw NotSupportedException,
                                 // in which case we don't want to change the lookAhead status
            return ret;
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            int bytesTakenFromLookAheadBuffer = 0;
            if (count > 0 && _lookAheadIndex > 0)
            {
                bytesTakenFromLookAheadBuffer = Math.Min(count, _lookAheadIndex);
                Array.Copy(_lookAheadBuffer, 0, buffer, offset, bytesTakenFromLookAheadBuffer);
                count -= bytesTakenFromLookAheadBuffer;
                offset += bytesTakenFromLookAheadBuffer;
                _lookAheadIndex -= bytesTakenFromLookAheadBuffer;
                if (_lookAheadIndex > 0) // move remaining bytes in lookAheadBuffer to front
                                         // copying into same array should be fine, according to http://msdn.microsoft.com/en-us/library/z50k9bft(v=VS.90).aspx :
                                         // "If sourceArray and destinationArray overlap, this method behaves as if the original values of sourceArray were preserved
                                         // in a temporary location before destinationArray is overwritten."
                    Array.Copy(_lookAheadBuffer, _lookAheadBuffer.Length - bytesTakenFromLookAheadBuffer + 1, _lookAheadBuffer, 0, _lookAheadIndex);
            }

            return count > 0
                ? bytesTakenFromLookAheadBuffer + await _underlyingStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false)
                : bytesTakenFromLookAheadBuffer;
        }


        // from here on, only simple delegations to underlyingStream

        public override bool CanSeek { get { return _underlyingStream.CanSeek; } }
        public override bool CanWrite { get { return _underlyingStream.CanWrite; } }
        public override bool CanTimeout { get { return _underlyingStream.CanTimeout; } }
        public override int ReadTimeout { get { return _underlyingStream.ReadTimeout; } set { _underlyingStream.ReadTimeout = value; } }
        public override int WriteTimeout { get { return _underlyingStream.WriteTimeout; } set { _underlyingStream.WriteTimeout = value; } }
        public override void Flush() { _underlyingStream.Flush(); }
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _underlyingStream.FlushAsync(cancellationToken);
        }
        public override long Length { get { return _underlyingStream.Length; } }
        public override void SetLength(long value) { _underlyingStream.SetLength(value); }
        public override void Write(byte[] buffer, int offset, int count) { _underlyingStream.Write(buffer, offset, count); }
        public override void WriteByte(byte value) { _underlyingStream.WriteByte(value); }
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return _underlyingStream.WriteAsync(buffer, offset, count, cancellationToken);
        }
    }
}
