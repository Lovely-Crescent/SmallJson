using System.Text;

namespace SmallJson
{
    /// <summary>
    /// 标识类型
    /// </summary>
    enum TokenType
    {
        NONE,

        OBJECT,
        OBJECT_CLOSE,

        ARRAY,
        ARRAY_CLOSE,

        STRING,
        STRING_CLOSE,

        NUMBER,
        NUMBER_CLOSE,

        BOOLEAN,
        BOOLEAN_CLOSE,

        NULL,
        NULL_CLOSE,

        DATA_MARK,
        PROPERTY_MARK,

    }

    /// <summary>
    /// 标识
    /// </summary>
    class JToken
    {
        /// <summary>
        /// 一个标识对象
        /// </summary>
        public JToken(TokenType type,long seek)
        {
            TType = type;
            Seek = seek;

            if(TokenType.STRING == TType || TokenType.NUMBER == TType || TokenType.BOOLEAN == TType || TokenType.NULL == TType)
            {
                mStringBuilder = new StringBuilder();
            }
        }

        /// <summary>
        /// 添加一个字符
        /// </summary>
        public void AddChar(char ch)
        {
            if(null != mStringBuilder)
            {
                mStringBuilder.Append(ch);
            }
        }

        /// <summary>
        /// 获取自符串
        /// </summary>
        public string GetString()
        {
            if(null != mStringBuilder)
            {
                if(TokenType.STRING == TType )
                {
                    if(mStringBuilder.Length > 0 && mStringBuilder[0] == '"')
                    {
                        mStringBuilder.Remove(0, 1);
                    }

                    if (mStringBuilder.Length > 0 && mStringBuilder[mStringBuilder.Length - 1] == '"')
                    {
                        mStringBuilder.Remove(mStringBuilder.Length - 1, 1);
                    }
                }

                if (0 == mStringBuilder.Length)
                {
                    if(TokenType.NUMBER == TType)
                    {
                        return "0";
                    }
                    if(TokenType.BOOLEAN == TType)
                    {
                        return "false";
                    }
                }

                return mStringBuilder.ToString();
            }
            return string.Empty;
        }
        /// <summary>
        /// 位置
        /// </summary>
        public readonly long Seek;

        /// <summary>
        /// 类型
        /// </summary>
        public readonly TokenType TType;

        /// <summary>
        /// 内容
        /// </summary>
        private readonly StringBuilder mStringBuilder = null;
    }
}
