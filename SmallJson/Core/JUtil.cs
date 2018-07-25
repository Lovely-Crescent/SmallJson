using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

namespace SmallJson
{
    static class JUtil
    {
        /// <summary>  
        /// 字符串转Unicode  
        /// </summary>  
        /// <param name="source">源字符串</param>  
        /// <returns>Unicode编码后的字符串</returns>  
        public static string String2Unicode(string source)
        {
            var bytes = Encoding.Unicode.GetBytes(source);
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0:x2}{1:x2}", bytes[i + 1], bytes[i]);
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 字符串序列化
        /// </summary>
        public static string SerializeString(string str)
        {
            StringBuilder sb = new StringBuilder();
            char[] array = str.ToCharArray();


            for(int i = 0; i < array.Length; ++i)
            {
                char ch = array[i];
                if (ch >= 0 && ch < 32)
                {
                    if(ch == '\b')
                    {
                        sb.Append("\\b");
                    }
                    else if(ch == '\t')
                    {
                        sb.Append("\\t");
                    }
                    else if(ch == '\n')
                    {
                        sb.Append("\\n");
                    }
                    else if (ch == '\f')
                    {
                        sb.Append("\\f");
                    }
                    else if (ch == '\r')
                    {
                        sb.Append("\\r");
                    }
                    else
                    {
                        sb.AppendFormat("\\u00{0:x2}", (int)ch);
                    }
                }
                else
                {
                    if(ch == '"')
                    {
                        sb.Append("\\\"");
                    }
                    else if(ch == '\\')
                    {
                        sb.Append("\\\\");
                    }
                    else if('\u0085' == ch)
                    {
                        sb.Append("\\u0085");
                    }
                    else if ('\u2028' == ch)
                    {
                        sb.Append("\\u2028");
                    }
                    else if ('\u2029' == ch)
                    {
                        sb.Append("\\u2029");
                    }
                    else
                    {
                        sb.Append(ch);
                    }
                }
                    
            }

            return sb.ToString();
        }

        /// <summary>
        /// 符串解码
        /// </summary>
        public static string DeserializeString(string str)
        {
            StringBuilder sb = new StringBuilder();

            char[] chArray = str.ToCharArray();

            bool transition = false;

            StringBuilder tran = new StringBuilder();

            for (int i = 0; i < chArray.Length; ++i)
            {
                char ch = chArray[i];

                if(transition)
                {
                    if (0 == tran.Length)
                    {
                        if ('b' == ch)
                        {
                            transition = false;
                            sb.Append('\b');
                            continue;
                        }
                        if ('t' == ch)
                        {
                            transition = false;
                            sb.Append('\t');
                            continue;
                        }
                        if ('n' == ch)
                        {
                            transition = false;
                            sb.Append('\n');
                            continue;
                        }
                        if ('f' == ch)
                        {
                            transition = false;
                            sb.Append('\f');
                            continue;
                        }
                        if ('r' == ch)
                        {
                            transition = false;
                            sb.Append('\r');
                            continue;
                        }
                        if ('"' == ch)
                        {
                            transition = false;
                            sb.Append('\"');
                            continue;
                        }
                        if ('\\' == ch)
                        {
                            transition = false;
                            sb.Append('\\');
                            continue;
                        }

                        tran.Append(ch);
                        continue;
                    }
                    else
                    {
                        tran.Append(ch);
                        if (5 == tran.Length)
                        {
                            string tranString = tran.ToString();
                            switch (tranString)
                            {
                                case "u0085": sb.Append('\u0085'); break;
                                case "u2028": sb.Append('\u2028'); break;
                                case "u2029": sb.Append('\u2029'); break;
                                default:
                                    {
                                        for (int c = 0; c < 32; ++c)
                                        {
                                            if (tranString == string.Format("u00{0:x2}", c))
                                            {
                                                sb.Append((char)c); break;
                                            }
                                        }
                                    }
                                    break;

                            }
                            tran.Remove(0, tran.Length);
                            transition = false;
                        }
                        continue;
                    }
                }
                
                switch(ch)
                {
                    case '\\':
                        {
                            transition = true;
                        }
                        break;
                    default:
                        {
                            sb.Append(ch);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 转成字符串格式
        /// </summary>
        public static string ToString(object obj)
        {
            if(typeof(DateTime) == obj.GetType())
            {
                return ((DateTime)obj).ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK");
            }

            if(typeof(Guid) == obj)
            {
                return ((Guid)obj).ToString("N");
            }
            if(obj.GetType().IsEnum)
            {
                return ((int)obj).ToString();
            }
            return Convert.ToString(obj);
        }

        public static object ToObject(Type targetType,string str)
        {
            if(typeof(string) == targetType)
            {
                return DeserializeString(str);
            }
            if (typeof(DateTime) == targetType)
            {
                return DateTime.Parse(str);
            }
            if (typeof(Guid) == targetType)
            {
                return new Guid(str);
            }
            if(typeof(Char) == targetType)
            {
                return Convert.ToChar(str);
            }
            if (typeof(Int32) == targetType)
            {
                return Convert.ToInt32(str);
            }
            if (typeof(Int16) == targetType)
            {
                return Convert.ToInt16(str);
            }
            if (typeof(SByte) == targetType)
            {
                return Convert.ToSByte(str);
            }
            if (typeof(UInt32) == targetType)
            {
                return Convert.ToUInt32(str);
            }
            if (typeof(UInt16) == targetType)
            {
                return Convert.ToUInt16(str);
            }
            if (typeof(Int64) == targetType)
            {
                return Convert.ToInt64(str);
            }
            if (typeof(Double) == targetType)
            {
                return Convert.ToDouble(str);
            }
            if (typeof(UInt64) == targetType)
            {
                return Convert.ToUInt64(str);
            }
            if (typeof(Single) == targetType)
            {
                return Convert.ToSingle(str);
            }
            if (typeof(Decimal) == targetType)
            {
                return Convert.ToDecimal(str);
            }
            if (typeof(Byte) == targetType)
            {
                return Convert.ToByte(str);
            }
            if(typeof(bool) == targetType)
            {
                if(string.IsNullOrEmpty(str))
                {
                    return false;
                }

                if("true" == str.ToLower())
                {
                    return true;
                }

                if ("false" == str.ToLower())
                {
                    return false;
                }

                return bool.Parse(str);
            }
            if(targetType.IsEnum)
            {
                return Enum.Parse(targetType, str);
            }

            return str;
        }

        /// <summary>
        /// 是否是值类型
        /// </summary>
        public static bool IsValueType(Type type)
        {
            return IsStringType(type) || IsNumberType(type) || typeof(bool) == type || typeof(Boolean) == type || type.IsEnum;
        }

        /// <summary>
        /// 生成一个对象
        /// </summary>
        public static object CreateInstance(Type type,params object[] args)
        {
            return Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// 是否可以实例化
        /// </summary>
        public static bool CanInstance(Type type)
        {
            return !type.IsAbstract;
        }
        /// <summary>
        /// 获取可序列化的属性
        /// </summary>
        public static PropertyInfo[] GetSerializableProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }

        /// <summary>
        /// 获取可序列化的属性
        /// </summary>
        public static FieldInfo[] GetSerializableFields(Type type)
        {
            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }

        /// <summary>
        /// 设置属性值
        /// </summary>
        public static bool SetProperty(Type type,string name,object target,object value)
        {
            PropertyInfo property = type.GetProperty(name);
            if (null != property)
            {
                if(property.CanWrite)
                {
                    property.SetValue(target, value,null);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <returns></returns>
        public static bool GetProperty(Type type,string name,object target,ref object value)
        {
            PropertyInfo property = type.GetProperty(name);
            if (null != property)
            {
                if (property.CanRead)
                {
                    value = property.GetValue(target, null);
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// 是否IList类型
        /// </summary>
        public static bool IsListType(Type type)
        {
            return typeof(IList) == type
                || typeof(IList).IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否IList<>类型
        /// </summary>
        public static bool IsListGenericType(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(IList<>) || type.GetGenericTypeDefinition() == typeof(List<>);
            }
            return false;
        }

        /// <summary>
        /// 是否是字符串形式
        /// </summary>
        public static bool IsStringType(Type type)
        {
            return type == typeof(string)
                || type == typeof(String)
                || type == typeof(DateTime)
                || type == typeof(Guid);
        }

        /// <summary>
        /// 是否是数字形式
        /// </summary>
        public static bool IsNumberType(Type type)
        {
            return type == typeof(char)
                || type == typeof(int)
                || type == typeof(short)
                || type == typeof(sbyte)
                || type == typeof(uint)
                || type == typeof(ushort)
                || type == typeof(long)
                || type == typeof(double)
                || type == typeof(ulong)
                || type == typeof(float)
                || type == typeof(decimal)
                || type == typeof(byte)
                || type == typeof(Char)
                || type == typeof(Int32)
                || type == typeof(Int16)
                || type == typeof(SByte)
                || type == typeof(UInt32)
                || type == typeof(UInt16)
                || type == typeof(Int64)
                || type == typeof(Double)
                || type == typeof(UInt64)
                || type == typeof(Single)
                || type == typeof(Decimal)
                || type == typeof(Byte);
        }

        /// <summary>
        /// 是否是NameValueCollection类型
        /// </summary>
        public static bool IsNameValueCollectionType(Type type)
        {
            return typeof(NameValueCollection) == type || typeof(NameValueCollection).IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否是键为值类型的Dictionary<,>泛型
        /// </summary>
        public static bool IsValueKeyDictionaryGenericType(Type type)
        {
            if (type.IsGenericType)
            {
                if(type.GetGenericTypeDefinition() == typeof(Dictionary<,>) || type.GetGenericTypeDefinition() == typeof(SortedDictionary<,>) || type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    return JUtil.IsValueType(type.GetGenericArguments()[0]);
                }
            }
            return false;
        }

        /// <summary>
        /// 是否是IDictionary<,>类型
        /// </summary>
        public static bool IsDictionaryGenericType(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(Dictionary<,>) || type.GetGenericTypeDefinition() == typeof(SortedDictionary<,>) || type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
            }
            return false;
        }


        /// <summary>
        /// 是否是指定KeyValuePair<,>类型
        /// </summary>
        public static bool IsKeyValuePairGenericType(Type type)
        {
            if (type.IsGenericType)
            {
                return type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
            }
            return false;
        }

        /// <summary>
        /// 是否是IEnumerable 类型
        /// </summary>
        public static bool IsIEnumerableType(Type type)
        {
            return typeof(IEnumerable) == type
                || type.IsSubclassOf(typeof(IEnumerable))
                || typeof(IEnumerable).IsAssignableFrom(type);
        }
    }
}
