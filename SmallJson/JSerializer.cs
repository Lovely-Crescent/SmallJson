using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace SmallJson
{
    /// <summary>
    /// 序列化对象
    /// </summary>
    public interface ISerialized
    {
        /// <summary>
        /// 转为JSON格式
        /// </summary>
        string ToJson();

        /// <summary>
        /// 反序列化出对象
        /// </summary>
        object ToDeserialize(Type t);
    }

    /// <summary>
    /// 序列化工具
    /// </summary>
    public static class JSerializer
    {
        /// <summary>
        /// 序列化为字符串
        /// </summary>
        public static string SerializeToString(object obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException();
            }

            JValue jvalue = ConvertToJValue(obj);
            if (null != jvalue)
            {
                return jvalue.ToJson();
            }

            return string.Empty;
        }

        /// <summary>
        /// 序列化为JArray对象
        /// </summary>
        public static JArray SerializeToJArray(object obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException();
            }

            JValue jvalue = ConvertToJValue(obj);
            if(null != jvalue)
            {
                if(ValueType.ARRAY == jvalue.ValueType)
                {
                    return jvalue.Value as JArray;
                }
            }

            return null;
        }

        /// <summary>
        /// 序列化为JObject对象
        /// </summary>
        public static JObject SerializeToJObject(object obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException();
            }

            JValue jvalue = ConvertToJValue(obj);
            if (null != jvalue)
            {
                if (ValueType.OBJECT == jvalue.ValueType)
                {
                    return jvalue.Value as JObject;
                }
            }

            return null;
        }

        /// <summary>
        /// 从Json字符串反序列化
        /// </summary>
        public static T Deserialize<T>(string json) 
        {
            if(typeof(T) == typeof(string))
            {
                throw new Exception("T 类型为String");
            }

            JValue jValue = JParser.Parse(json);

            if(null == jValue)
            {
                return default(T);
            }

            return (T)jValue.ToDeserialize(typeof(T));
        }

        /// <summary>
        /// 从Json字符串反序列化出JArray
        /// </summary>
        public static JArray DeserializeToArray(string json)
        {
            JValue jValue = JParser.Parse(json);

            if (null != jValue)
            {
                if (ValueType.ARRAY == jValue.ValueType)
                {
                    return jValue.Value as JArray;
                }
            }

            return null;

        }

        /// <summary>
        /// 从Json字符串反序列化出JObject
        /// </summary>
        public static JObject DeserializeToObject(string json)
        {
            JValue jValue = JParser.Parse(json);

            if (null != jValue)
            {
                if (ValueType.OBJECT == jValue.ValueType)
                {
                    return jValue.Value as JObject;
                }
            }

            return null;
        }

        /// <summary>
        /// 转为JValue
        /// </summary>
        private static JValue ConvertToJValue(object obj)
        {
            if (null == obj)
            {
                return new JValue(ValueType.NULL, "null");
            }

            if (typeof(JArray) == obj.GetType())
            {
                return new JValue(ValueType.ARRAY, obj);
            }

            if (typeof(JObject) == obj.GetType())
            {
                return new JValue(ValueType.OBJECT, obj);
            }
            if(obj.GetType().IsEnum)
            {
                return new JValue(ValueType.NUMBER, ((int)obj).ToString());
            }
            if (typeof(bool) == obj.GetType() || typeof(Boolean) == obj.GetType())
            {
                return new JValue(ValueType.BOOLEAN, ((bool)obj) ? "true" : "false");
            }

            if(typeof(NameValueCollection) == obj.GetType() || typeof(NameValueCollection).IsAssignableFrom(obj.GetType()))
            {
                return new JValue(ValueType.OBJECT,ConvertNameValueCollectionToObject(obj as NameValueCollection));
            }

            if (JUtil.IsNumberType(obj.GetType()))
            {
                return new JValue(ValueType.NUMBER, obj.ToString());
            }

            if (JUtil.IsStringType(obj.GetType()))
            {
                if (typeof(DateTime) == obj.GetType())
                {
                    return new JValue(ValueType.STRING, ((DateTime)obj).ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK"));
                }
                if(typeof(string) == obj.GetType())
                {
                    return new JValue(ValueType.STRING,JUtil.SerializeString(obj.ToString()));
                }
                return new JValue(ValueType.STRING, obj.ToString());
            }

            if (JUtil.IsValueKeyDictionaryGenericType(obj.GetType()))
            {
                return new JValue(ValueType.OBJECT, ConvertDictionaryGenericToObject(obj));
            }

            if (JUtil.IsKeyValuePairGenericType(obj.GetType()))
            {
                return new JValue(ValueType.OBJECT, ConvertKeyValuePairGenericToObject(obj));
            }

            if (JUtil.IsListType(obj.GetType()))
            {
                return new JValue(ValueType.ARRAY, ConvertListToArray(obj as IList));
            }

            if (JUtil.IsIEnumerableType(obj.GetType()))
            {
                return new JValue(ValueType.ARRAY, ConvertIEnumerableToArray(obj as IEnumerable));
            }

            return new JValue(ValueType.OBJECT, ConvertToObject(obj));
        }

        /// <summary>
        /// 转IList为
        /// </summary>
        private static JArray ConvertListToArray(IList list)
        {
            JArray res = new JArray();

            for (int i = 0; i < list.Count; ++i)
            {
                res.Add(ConvertToJValue(list[i]));
            }

            return res;
        }

        /// <summary>
        /// 转IEnumerable为JArray
        /// </summary>
        private static JArray ConvertIEnumerableToArray(IEnumerable obj)
        {
            JArray res = new JArray();
            foreach(object temp in obj)
            {
                res.Add(ConvertToJValue(temp));
            }
            return res;
        }

        /// <summary>
        /// 转NameValueCollection为JObject
        /// </summary>
        private static JObject ConvertNameValueCollectionToObject(NameValueCollection nameValue)
        {
            JObject res = new JObject();
            foreach(string key in nameValue.AllKeys)
            {
                if(!string.IsNullOrEmpty(key))
                {
                    res.Add(key, new JValue(ValueType.STRING, nameValue[key]));
                }
            }
            return res;
        }

        /// <summary>
        /// 转 IDictionary<,>为JObject
        /// </summary>
        private static JObject ConvertDictionaryGenericToObject(object obj)
        {
            JObject res = new JObject();
            IEnumerable tempEnumerable = obj as IEnumerable;
            IEnumerator tempEnumerator = tempEnumerable.GetEnumerator();
            while (tempEnumerator.MoveNext())
            {
                object tempKV = tempEnumerator.Current;
                object key = null;
                if(JUtil.GetProperty(tempKV.GetType(),"Key",tempKV, ref key))
                {
                    if (null != key)
                    {
                        object value = null;
                        if (JUtil.GetProperty(tempKV.GetType(), "Value", tempKV, ref value))
                        {
                            res.Add(key.ToString(), ConvertToJValue(value));
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 转指定类型的KeyValuePair<,>为JObject
        /// </summary>
        private static JObject ConvertKeyValuePairGenericToObject(object obj)
        {
            JObject res = new JObject();

            Type keyType = obj.GetType().GetGenericArguments()[0];
            Type valueType = obj.GetType().GetGenericArguments()[1];

            object key = null, value = null;
            if (JUtil.GetProperty(obj.GetType(), "Key", obj, ref key))
            {
                if (JUtil.GetProperty(obj.GetType(), "Value", obj, ref value))
                {
                    if (JUtil.IsValueType(keyType))
                    {
                        res.Add(key.ToString(), ConvertToJValue(value));
                    }
                    else
                    {
                        res.Add("Key", ConvertToJValue(key));
                        res.Add("Value", ConvertToJValue(value));
                    }
                }
            }
            return res;
        }
        
        /// <summary>
        /// 转object为JObject
        /// </summary>
        private static JObject ConvertToObject(object obj)
        {
            JObject res = new JObject();

            PropertyInfo[] propertys = JUtil.GetSerializableProperties(obj.GetType());

            if(null != propertys)
            {
                for(long i = 0; i < propertys.LongLength; ++i)
                {
                    try
                    {
                        if (propertys[i].CanRead)
                        {
                            res.Add(propertys[i].Name, ConvertToJValue(propertys[i].GetValue(obj, null)));
                        }
                    }
                    catch { }
                }
            }

            FieldInfo[] fields = JUtil.GetSerializableFields(obj.GetType());

            if (null != fields)
            {
                for (long i = 0; i < fields.LongLength; ++i)
                {
                    try
                    {
                        res.Add(fields[i].Name, ConvertToJValue(fields[i].GetValue(obj)));
                    }
                    catch { }
                }
            }

            return res;
        }
    }
}
