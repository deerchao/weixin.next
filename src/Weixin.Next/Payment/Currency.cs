namespace Weixin.Next.Payment
{
    /// <summary>
    /// ISO 4217 货币 https://en.wikipedia.org/wiki/ISO_4217
    /// </summary>
    public class Currency
    {
        private static readonly Currency _cny = new Currency("CNY", 156, 2, "人民币");

        private readonly string _code;
        private readonly int _num;
        private readonly int _e;
        private readonly string _name;

        /// <summary>
        /// 创建一个 Currency 对象
        /// </summary>
        /// <param name="code">ISO 4217 货币代码</param>
        /// <param name="num">ISO 4217 货币编号</param>
        /// <param name="e">小数点后位数</param>
        /// <param name="name">名称</param>
        public Currency(string code, int num, int e, string name)
        {
            _code = code;
            _num = num;
            _e = e;
            _name = name;
        }

        /// <summary>
        /// 货币代码, 例: CNY
        /// </summary>
        public string Code
        {
            get { return _code; }
        }

        /// <summary>
        /// 货币编号, 例: 156
        /// </summary>
        public int Num
        {
            get { return _num; }
        }

        /// <summary>
        /// 小数点后位数, 例: 2
        /// </summary>
        public int E
        {
            get { return _e; }
        }

        /// <summary>
        /// 货币名称, 例: 人民币
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        public override string ToString()
        {
            return _code;
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        /// 人民币
        /// </summary>
        public static Currency CNY { get { return _cny; } }

        public static Currency Find(string code)
        {
            //目前只支持人民币
            return code == "CNY"
                ? CNY
                : null;
        }
    }
}
