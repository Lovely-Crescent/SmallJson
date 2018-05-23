using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

namespace SmallJson
{
    /// <summary>
    /// 对象
    /// </summary>
    public sealed class JObject : ISerialized
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int Count
        {
            get
            {
                return mPropertys.Count;
            }
        }

        /// <summary>
        /// 获取一个值
        /// </summary>
        public JValue this[string key]
        {
            get
            {
                return mPropertys[key];
            }
        }

        /// <summary>
        /// 遍历对象
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            return mPropertys.GetEnumerator();
        }

        /// <summary>
        /// 是否包含键
        /// </summary>
        public bool ContainKey(string name)
        {
            return mPropertys.ContainsKey(name);
        }

        /// <summary>
        /// 转为JSON格式
        /// </summary>
        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            
            bool isFirst = true;
            foreach (KeyValuePair<string,JValue> kv in mSortPropertys)
            {
                if(!isFirst)
                {
                    sb.Append(",");
                }
                isFirst = false;

                sb.Append(string.Format("\"{0}\":{1}", kv.Key, kv.Value.ToJson()));
            }

            sb.Append("}");

            return sb.ToString();
        }
        
        /// <summary>
        /// 添加一个值
        /// </summary>
        public void Add(string name, JValue v)
        {
            mPropertys[name] = v;
            mSortPropertys.Add(new KeyValuePair<string, JValue>(name, v));
        }

        /// <summary>
        /// 添加一个值
        /// </summary>
        internal void AddFront(string name, JValue v)
        {
            mPropertys[name] = v;
            mSortPropertys.Insert(0,new KeyValuePair<string, JValue>(name, v));
        }

        /// <summary>
        /// 序列化对象
        /// </summary>
        public object ToDeserialize(Type type)
        {
            if (!JUtil.CanInstance(type)) return null;
            object defalutValue = JUtil.CreateInstance(type);

            if (JUtil.IsValueKeyDictionaryGenericType(type))
            {
                Type keyType = type.GetGenericArguments()[0];
                Type valueType = type.GetGenericArguments()[1];

                foreach(KeyValuePair<string,JValue> tempKV in mSortPropertys)
                {
                    object key = JUtil.ToObject(keyType,tempKV.Key);
                    if(!(bool)type.GetMethod("ContainsKey", new Type[] { keyType }).Invoke(defalutValue, new object[] { key }))
                    {
                        type.GetMethod("Add", new Type[] { keyType, valueType }).Invoke(defalutValue, new object[] { key, tempKV.Value.ToDeserialize(valueType) });
                    }
                }
                return defalutValue;
            }

            if (JUtil.IsNameValueCollectionType(type))
            {
                NameValueCollection nameValue = defalutValue as NameValueCollection;

                foreach (KeyValuePair<string, JValue> tempKV in mSortPropertys)
                {
                    nameValue[tempKV.Key] = tempKV.Value.Value.ToString();
                }
                return defalutValue;
            }
            
            if(JUtil.IsKeyValuePairGenericType(type))
            {
                Type keyType = type.GetGenericArguments()[0];
                Type valueType = type.GetGenericArguments()[1];

                object key = null, value = null;

                if (JUtil.IsValueType(keyType))
                {
                    if(mSortPropertys.Count > 0)
                    {
                        KeyValuePair<string, JValue> tempKV = mSortPropertys[0];
                        key = JUtil.ToObject(keyType, tempKV.Key);
                        value = tempKV.Value.ToDeserialize(valueType);
                    }
                }
                else
                {
                    if(mPropertys.ContainsKey("Key"))
                    {
                        key = mPropertys["Key"].ToDeserialize(keyType);
                        value = mPropertys["Value"].ToDeserialize(valueType);
                    }
                }

                if(null != key)
                {
                    defalutValue = JUtil.CreateInstance(type, key, value);
                }

                return defalutValue;
            }

            PropertyInfo[] propertyInfo = JUtil.GetSerializableProperties(type);
            
            if(null != propertyInfo)
            {
                for (int i = 0; i < propertyInfo.Length; ++i)
                {
                    if(propertyInfo[i].CanWrite)
                    {
                        string name = propertyInfo[i].Name;
                        if(mPropertys.ContainsKey(name))
                        {
                            propertyInfo[i].SetValue(defalutValue, mPropertys[name].ToDeserialize(propertyInfo[i].PropertyType),null);
                        }
                    }
                }
            }

            FieldInfo[] fieldInfo = JUtil.GetSerializableFields(type);

            if (null != fieldInfo)
            {
                for (int i = 0; i < fieldInfo.Length; ++i)
                {
                    string name = fieldInfo[i].Name;
                    if (mPropertys.ContainsKey(name))
                    {
                        fieldInfo[i].SetValue(defalutValue, mPropertys[name].ToDeserialize(fieldInfo[i].FieldType));
                    }
                }
            }

            return defalutValue;

        }

        /// <summary>
        /// 属性
        /// </summary>
        Dictionary<string, JValue> mPropertys = new Dictionary<string, JValue>();

        /// <summary>
        /// 排序的属性列表
        /// </summary>
        List<KeyValuePair<string,JValue>> mSortPropertys = new List<KeyValuePair<string, JValue>>();
    }
}
