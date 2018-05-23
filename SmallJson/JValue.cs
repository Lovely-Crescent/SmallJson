using System;

namespace SmallJson
{
    /// <summary>
    /// 值类型
    /// </summary>
    public enum ValueType
    {
        OBJECT,
        ARRAY,
        STRING,
        NUMBER,
        BOOLEAN,
        NULL,
    }

    /// <summary>
    /// 值
    /// </summary>
    public sealed class JValue : ISerialized
    {
        /// <summary>
        /// 值类型
        /// </summary>
        public readonly ValueType ValueType;

        /// <summary>
        /// 构建一个JValue对象
        /// </summary>
        public JValue(ValueType t,object value)
        {
            ValueType = t;
            mValue = value;
        }

        /// <summary>
        /// 转为Json格式
        /// </summary>
        public string ToJson()
        {
            switch(ValueType)
            {
                case ValueType.ARRAY:
                    {
                        return (mValue as JArray).ToJson();
                    }
                case ValueType.OBJECT:
                    {
                        return (mValue as JObject).ToJson();
                    }
                case ValueType.BOOLEAN:
                case ValueType.NUMBER:
                case ValueType.NULL:
                    {
                        return mValue as string;
                    }
                case ValueType.STRING:
                    {
                        return string.Format("\"{0}\"", mValue as string);
                    }
                default:
                    {
                        return "";
                    }
            }
        }

        /// <summary>
        /// 反序列化出一个对象
        /// </summary>
        public object ToDeserialize(Type type)
        {
            switch (ValueType)
            {
                case ValueType.ARRAY:
                    {
                        return (mValue as JArray).ToDeserialize(type);
                    }
                case ValueType.OBJECT:
                    {
                        return (mValue as JObject).ToDeserialize(type);
                    }
                case ValueType.BOOLEAN:
                    {
                        return "true" == (mValue as string) ? true : false;
                    }
                case ValueType.NUMBER:
                    {
                        return JUtil.ToObject(type,mValue as string);
                    }
                case ValueType.NULL:
                    {
                        return null;
                    }
                case ValueType.STRING:
                    {
                        return JUtil.ToObject(type, mValue as string);
                    }
                default:
                    {
                        return JUtil.CreateInstance(type);
                    }
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get
            {
                return mValue;
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        private readonly object mValue;
    }
}
