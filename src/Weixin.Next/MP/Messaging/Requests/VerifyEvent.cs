using System.Xml.Linq;

namespace Weixin.Next.MP.Messaging.Requests
{
    public abstract class VerifyEvent : EventMessage
    {
        protected VerifyEvent(XElement xml) : base(xml)
        {
        }
    }

    public abstract class VerifyExpireEvent : VerifyEvent
    {
        protected VerifyExpireEvent(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 有效期 (整形)，指的是时间戳，认证过期时间
        /// </summary>
        public long ExpiredTime { get { return long.Parse(_xml.Element("ExpiredTime").Value); } }
    }

    public abstract class VerifyFailEvent : VerifyEvent
    {
        public VerifyFailEvent(XElement xml) : base(xml)
        {
        }

        /// <summary>
        /// 失败发生时间 (整形)，时间戳
        /// </summary>
        public long FailTime { get { return long.Parse(_xml.Element("FailTime").Value); } }
        /// <summary>
        /// 认证失败的原因
        /// </summary>
        public string FailReason { get { return _xml.Element("FailReason").Value; } }
    }

    /// <summary>
    /// 资质认证成功
    /// </summary>
    public class QualificationVerifySuccessEvent : VerifyExpireEvent
    {
        public QualificationVerifySuccessEvent(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// 资质认证失败
    /// </summary>
    public class QualificationVerifyFailEvent : VerifyFailEvent
    {
        public QualificationVerifyFailEvent(XElement xml) : base(xml)
        {
        }

    }

    /// <summary>
    /// 名称认证成功
    /// </summary>
    public class NamingVerifySuccessEvent : VerifyExpireEvent
    {
        public NamingVerifySuccessEvent(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// 名称认证失败
    /// </summary>
    public class NamingVerifyFailEvent : VerifyFailEvent
    {
        public NamingVerifyFailEvent(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// 年审通知
    /// </summary>
    public class AnnualRenewEvent : VerifyExpireEvent
    {
        public AnnualRenewEvent(XElement xml) : base(xml)
        {
        }
    }

    /// <summary>
    /// 认证过期失效通知
    /// </summary>
    public class VerifyExpiredEvent : VerifyExpireEvent
    {
        public VerifyExpiredEvent(XElement xml) : base(xml)
        {
        }
    }
}
