using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Weixin.Next.Utilities
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Use this method to ignore compiler warnings on unawaited tasks
        /// </summary>
        /// <param name="task"></param>
        /// <param name="filePath"></param>
        /// <param name="lineNo"></param>
        /// <param name="callerMember"></param>
        public static void FireAndForget(this Task task, [CallerFilePath]string filePath = null, [CallerLineNumber] int lineNo = 0, [CallerMemberName]string callerMember = null)
        {
            task.ContinueWith(t =>
            {
                // ReSharper disable once PossibleNullReferenceException
                var ex = t.Exception.Flatten().InnerException;
                Trace.WriteLine($"FireAndForget Exception: {ex.Message}, Source File: {filePath}, Line No: {lineNo}, Member: {callerMember}");
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
