using System;
using System.Collections.Generic;
using System.Text;

namespace SmallJson
{
    /// <summary>
    /// 数组
    /// </summary>
    public sealed class JArray : ISerialized
    {
        /// <summary>
        /// 长度
        /// </summary>
        public int Length
        {
            get
            {
                return mValues.Count;
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        public JValue this[int index]
        {
            get
            {
                return mValues[index];
            }
        }

        /// <summary>
        /// 转JSON
        /// </summary>
        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for(int i = 0; i < mValues.Count; ++i)
            {
                if(i > 0)
                {
                    sb.Append(",");
                }
                sb.Append(mValues[i].ToJson());
            }
            sb.Append("]");

            return sb.ToString();
        }
        
        /// <summary>
        /// 添加一个值
        /// </summary>
        public void Add(JValue v)
        {
            mValues.Add(v);
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        public object ToDeserialize(Type type)
        {
            if (!JUtil.CanInstance(type)) return null;

            if (type.IsArray)
            {
                object defaultValue = JUtil.CreateInstance(type, mValues.Count);
                Type eleType = type.GetElementType();
                for (int i = 0; i < mValues.Count; ++i)
                {
                    type.GetMethod("SetValue", new Type[] { typeof(object), typeof(int) }).Invoke(defaultValue, new object[] { mValues[i].ToDeserialize(eleType), i });
                }
                return defaultValue;
            }
            else
            {
                object defaultValue = JUtil.CreateInstance(type);

                if(JUtil.IsListGenericType(type))
                {
                    if (JUtil.IsListGenericType(type))
                    {
                        Type t = type.GetGenericArguments()[0];
                        for (int i = 0; i < mValues.Count; ++i)
                        {
                            type.GetMethod("Insert", new Type[] { typeof(int), t }).Invoke(defaultValue, new object[] { i, mValues[i].ToDeserialize(t) });
                        }
                        return defaultValue;
                    }
                }
                
                if(JUtil.IsDictionaryGenericType(type))
                {
                    Type keyType = type.GetGenericArguments()[0];
                    Type valueType = type.GetGenericArguments()[1];

                    Type keyValueType = typeof(KeyValuePair<,>).MakeGenericType(new Type[] { keyType, valueType });

                    for (int i = 0; i < mValues.Count; ++i)
                    {
                        object keyVlaue = mValues[i].ToDeserialize(keyValueType);

                        object key = null, value = null;
                        if (JUtil.GetProperty(keyValueType, "Key",keyVlaue, ref key))
                        {
                            JUtil.GetProperty(keyValueType, "Value", keyVlaue, ref value);
                        }

                        if(null != key)
                        {
                            type.GetMethod("Add", new Type[] { keyType, valueType }).Invoke(defaultValue, new object[] { key,value });
                        }
                    }
                }

                return defaultValue;
            }
            
        }
        
        /// <summary>
        /// 添加一个值
        /// </summary>
        internal void AddFront(JValue v)
        {
            mValues.Insert(0, v);
        }

        /// <summary>
        /// 值列表
        /// </summary>
        List<JValue> mValues = new List<JValue>();
    }
}
