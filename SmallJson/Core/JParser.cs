using System;
using System.Collections.Generic;

namespace SmallJson
{
    /// <summary>
    /// 解析器
    /// </summary>
    static class JParser
    {
        class CharReader
        {
            public CharReader(string str)
            {
                mChars = str.ToCharArray();
            }

            public long Seek
            {
                get
                {
                    return mSeek;
                }
            }
            
            public bool Read(ref char ch)
            {
                if (null == mChars || mSeek >= mChars.LongLength)
                {
                    return false;
                }

                ch = mChars[mSeek++];
                return true;
            }

            long mSeek;
            readonly char[] mChars = null;
        }
        
        /// <summary>
        /// 从字符串解析Json对象
        /// </summary>
        public static JValue Parse(string json)
        {
            // 从0开始
            long seek = 0;
            CharReader reader = new CharReader(json);
            Stack<JToken> stack = new Stack<JToken>();
            // 当前的字符对象
            char ch = ' ';

            // 转义中
            bool transition = false;

            stack.Push(new JToken(TokenType.NONE, 0));

            while(reader.Read(ref ch))
            {
                seek = reader.Seek;

                if(transition)
                {
                    stack.Peek().AddChar(ch);
                    transition = false;
                    continue;
                }

                switch(ch)
                {
                    case '\\':
                        {
                            transition = true;
                        }
                        break;
                    case '{':
                        {
                            if(
                                TokenType.NONE == stack.Peek().TType
                                || TokenType.ARRAY == stack.Peek().TType
                                || TokenType.DATA_MARK == stack.Peek().TType
                                || TokenType.PROPERTY_MARK == stack.Peek().TType
                             )
                            {
                                stack.Push(new JToken(TokenType.OBJECT, seek));
                            }
                        }
                        break;
                    case '}':
                        {
                            if (TokenType.NUMBER == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.NUMBER_CLOSE, seek));
                            }
                            if (TokenType.BOOLEAN == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.BOOLEAN_CLOSE, seek));
                            }
                            if (TokenType.NULL == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.NULL_CLOSE, seek));
                            }

                            if (TokenType.PROPERTY_MARK == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.NUMBER, seek));
                                stack.Push(new JToken(TokenType.NUMBER_CLOSE, seek));
                            }

                            if (
                                TokenType.STRING_CLOSE == stack.Peek().TType
                                || TokenType.OBJECT_CLOSE == stack.Peek().TType
                                || TokenType.ARRAY_CLOSE == stack.Peek().TType
                                || TokenType.NUMBER_CLOSE == stack.Peek().TType
                                || TokenType.BOOLEAN_CLOSE == stack.Peek().TType
                                || TokenType.NULL_CLOSE == stack.Peek().TType
                                || TokenType.OBJECT == stack.Peek().TType
                                )
                            {
                                stack.Push(new JToken(TokenType.OBJECT_CLOSE, seek));
                            }
                        }
                        break;
                    case '[':
                        {
                            if (
                                TokenType.NONE == stack.Peek().TType
                                || TokenType.ARRAY == stack.Peek().TType
                                || TokenType.DATA_MARK == stack.Peek().TType
                                || TokenType.PROPERTY_MARK == stack.Peek().TType
                             )
                            {
                                stack.Push(new JToken(TokenType.ARRAY, seek));
                            }
                        }
                        break;
                    case ']':
                        {
                            if(TokenType.NUMBER == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.NUMBER_CLOSE, seek));
                            }
                            if (TokenType.BOOLEAN == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.BOOLEAN_CLOSE, seek));
                            }
                            if (TokenType.NULL == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.NULL_CLOSE, seek));
                            }

                            if (TokenType.DATA_MARK == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.NUMBER, seek));
                                stack.Push(new JToken(TokenType.NUMBER_CLOSE, seek));
                            }

                            if (
                                TokenType.STRING_CLOSE == stack.Peek().TType
                                || TokenType.OBJECT_CLOSE == stack.Peek().TType
                                || TokenType.ARRAY_CLOSE == stack.Peek().TType
                                || TokenType.NUMBER_CLOSE == stack.Peek().TType
                                || TokenType.BOOLEAN_CLOSE == stack.Peek().TType
                                || TokenType.NULL_CLOSE == stack.Peek().TType
                                || TokenType.ARRAY == stack.Peek().TType
                                )
                            {
                                stack.Push(new JToken(TokenType.ARRAY_CLOSE, seek));
                            }
                        }
                        break;
                    case ',':
                        {
                            if (TokenType.NUMBER == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.NUMBER_CLOSE, seek));
                            }
                            if(TokenType.BOOLEAN == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.BOOLEAN_CLOSE, seek));
                            }
                            if (TokenType.NULL == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.NULL_CLOSE, seek));
                            }

                            if (
                                TokenType.DATA_MARK == stack.Peek().TType
                                || TokenType.PROPERTY_MARK == stack.Peek().TType
                                )
                            {
                                stack.Push(new JToken(TokenType.NUMBER, seek));
                                stack.Push(new JToken(TokenType.NUMBER_CLOSE, seek));
                            }

                            if (
                                TokenType.NUMBER_CLOSE == stack.Peek().TType
                                || TokenType.STRING_CLOSE == stack.Peek().TType
                                || TokenType.ARRAY_CLOSE == stack.Peek().TType 
                                || TokenType.OBJECT_CLOSE == stack.Peek().TType
                                || TokenType.BOOLEAN_CLOSE == stack.Peek().TType
                                || TokenType.NULL_CLOSE == stack.Peek().TType
                                )
                            {
                                stack.Push(new JToken(TokenType.DATA_MARK, seek));
                            }
                        }
                        break;
                    case ':':
                        {
                            if (TokenType.STRING_CLOSE == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.PROPERTY_MARK, seek));
                            }
                        }
                        break;
                    case '"':
                        {
                            if (TokenType.STRING == stack.Peek().TType)
                            {
                                stack.Push(new JToken(TokenType.STRING_CLOSE, seek));
                            }
                            
                            if (
                                TokenType.DATA_MARK == stack.Peek().TType
                                ||TokenType.PROPERTY_MARK == stack.Peek().TType
                                || TokenType.OBJECT == stack.Peek().TType
                                || TokenType.ARRAY == stack.Peek().TType
                            )
                            {
                                stack.Push(new JToken(TokenType.STRING, seek));
                            }
                        }
                        break;
                    case '-':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                    case '0':
                        {
                            if (
                                    TokenType.ARRAY == stack.Peek().TType
                                    || TokenType.DATA_MARK == stack.Peek().TType
                                    || TokenType.PROPERTY_MARK == stack.Peek().TType
                                    )
                            {
                                stack.Push(new JToken(TokenType.NUMBER, seek));
                            }
                        }
                        break;
                    case 't':
                    case 'f':
                        {
                            if (
                                    TokenType.ARRAY == stack.Peek().TType
                                    || TokenType.DATA_MARK == stack.Peek().TType
                                    || TokenType.PROPERTY_MARK == stack.Peek().TType
                                    )
                            {
                                stack.Push(new JToken(TokenType.BOOLEAN, seek));
                            }
                        }
                        break;
                    case 'n':
                        {
                            if (
                                    TokenType.ARRAY == stack.Peek().TType
                                    || TokenType.DATA_MARK == stack.Peek().TType
                                    || TokenType.PROPERTY_MARK == stack.Peek().TType
                                    )
                            {
                                stack.Push(new JToken(TokenType.NULL, seek));
                            }
                        }
                        break;
                    default:
                        break;
                }
                stack.Peek().AddChar(ch);
            }
            
            // 返回结果
            return Parse(stack);
        }

        /// <summary>
        /// 解析标识
        /// </summary>
        private static JValue Parse(Stack<JToken> tokens)
        {

            switch(tokens.Pop().TType)
            {
                case TokenType.ARRAY_CLOSE:
                    {
                        JArray tempArray = new JArray();
                        Parse(tokens, tempArray);
                        return new JValue(ValueType.ARRAY, tempArray);
                    }
                case TokenType.OBJECT_CLOSE:
                    {
                        JObject tempObject = new JObject();
                        Parse(tokens, tempObject);
                        return new JValue(ValueType.OBJECT, tempObject);
                    }
                default:
                    {
                        throw new Exception("丢失结束标记");
                    }
            }
        }

        private static void Parse(Stack<JToken> tokens,JArray container)
        {
            while(tokens.Count > 0)
            {
                JToken temp = tokens.Pop();

                switch (temp.TType)
                {
                    case TokenType.ARRAY:
                        {
                            return;
                        }
                    case TokenType.OBJECT_CLOSE:
                        {
                            JObject tempObject = new JObject();
                            Parse(tokens, tempObject);
                            container.AddFront(new JValue(ValueType.OBJECT, tempObject));
                        }
                        break;
                    case TokenType.ARRAY_CLOSE:
                        {
                            JArray tempArray = new JArray();
                            Parse(tokens, tempArray);
                            container.AddFront(new JValue(ValueType.ARRAY, tempArray));
                        }
                        break;
                    case TokenType.NUMBER:
                        {
                            container.AddFront(new JValue(ValueType.NUMBER, temp.GetString()));
                        }
                        break;
                    case TokenType.STRING:
                        {
                            container.AddFront(new JValue(ValueType.STRING, temp.GetString()));
                        }
                        break;
                    case TokenType.BOOLEAN:
                        {
                            container.AddFront(new JValue(ValueType.BOOLEAN, temp.GetString()));
                        }
                        break;
                    case TokenType.NULL:
                        {
                            container.AddFront(new JValue(ValueType.NULL, temp.GetString()));
                        }
                        break;
                    default:
                        {
                            continue;
                        }
                }
            }
        }

        private static void Parse(Stack<JToken> tokens,JObject container)
        {
            while (tokens.Count > 0)
            {
                JToken temp = tokens.Pop();
                switch(temp.TType)
                {
                    case TokenType.OBJECT:
                        {
                            return;
                        }
                    case TokenType.ARRAY_CLOSE:
                        {
                            // 值
                            JArray tempArray = new JArray();
                            Parse(tokens, tempArray);

                            // 名
                            string name = GetPropertyName(tokens, temp.Seek);

                            container.AddFront(name, new JValue(ValueType.ARRAY, tempArray));
                        }
                        break;
                    case TokenType.OBJECT_CLOSE:
                        {

                            // 值
                            JObject tempObject = new JObject();
                            Parse(tokens, tempObject);

                            // 名
                            string name = GetPropertyName(tokens, temp.Seek);

                            container.AddFront(name, new JValue(ValueType.OBJECT, tempObject));
                        }
                        break;
                    case TokenType.STRING:
                        {
                            // 名
                            string name = GetPropertyName(tokens, temp.Seek);

                            container.AddFront(name, new JValue(ValueType.STRING, temp.GetString()));
                        }
                        break;
                    case TokenType.NUMBER:
                        {

                            // 名
                            string name = GetPropertyName(tokens, temp.Seek);

                            container.AddFront(name, new JValue(ValueType.NUMBER, temp.GetString()));
                        }
                        break;
                    case TokenType.BOOLEAN:
                        {

                            // 名
                            string name = GetPropertyName(tokens, temp.Seek);

                            container.AddFront(name, new JValue(ValueType.BOOLEAN, temp.GetString()));
                        }
                        break;
                    case TokenType.NULL:
                        {

                            // 名
                            string name = GetPropertyName(tokens, temp.Seek);

                            container.AddFront(name, new JValue(ValueType.NULL, temp.GetString()));
                        }
                        break;
                    default:
                        {
                            continue;
                        }
                }
            }
        }

        /// <summary>
        /// 获取属性名
        /// </summary>
        private static string GetPropertyName(Stack<JToken> tokens,long seek)
        {
            // 属性标记
            if (tokens.Count < 3)
                throw new Exception("解析错误 位置:" + seek);

            // 标记
            JToken mark = tokens.Pop();
            if (TokenType.PROPERTY_MARK != mark.TType)
                throw new Exception("解析错误 位置:" + mark.Seek);

            // 名称结束标记
            JToken name_close = tokens.Pop();
            if (TokenType.STRING_CLOSE != name_close.TType)
                throw new Exception("解析错误 位置:" + name_close.Seek);

            // 名称
            JToken name = tokens.Pop();
            if (TokenType.STRING != name.TType)
                throw new Exception("解析错误 位置:" + name.Seek);

            return name.GetString();
        }
        
    }
}