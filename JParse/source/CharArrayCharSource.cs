using System;
using System.Numerics;
using System.Text;
using sharpJParse.JsonParser.parser;
using sharpJParse.JsonParser.source.support;
using sharpJParse.JsonParser.support;

namespace sharpJParse.JsonParser.source
{
    public class CharArrayCharSource : ParseConstants, ICharSource
    {
        private static readonly char[] MinIntChars = MinIntStr.ToCharArray();
        private static readonly char[] MaxIntChars = MaxIntStr.ToCharArray();
        private readonly char[] _data;
        private readonly int _length;
        private int _index;

        public CharArrayCharSource(char[] data)
        {
            _data = data;
            _index = -1;
            _length = _data.Length;
        }

        public CharArrayCharSource(string str)
        {
            _data = str.ToCharArray();
            _index = -1;
            _length = _data.Length;
        }

        public int Next()
        {
            if (_index + 1 >= _length)
            {
                _index = _length;
                return Etx;
            }
            return _data[++_index];
        }

        public void CheckForJunk()
        {
            int index = _index;
            var data = _data;
            int length = _length;

            for (; index < length; index++)
            {
                int ch = data[index];
                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                        continue;
                    default:
                        throw new UnexpectedCharacterException("Junk", "Unexpected extra characters", this);
                }
            }
        }

        public int NextSkipWhiteSpace()
        {
            int index = _index + 1;
            var data = _data;
            int length = _length;

            for (; index < length; index++)
            {
                int ch = data[index];
                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                        continue;
                    default:
                        _index = index;
                        return ch;
                }
            }

            _index = length;
            return Etx;
        }

        public char SkipWhiteSpace()
        {
            int index = _index;
            var data = _data;
            int length = _length;

            for (; index < length; index++)
            {
                int ch = data[index];
                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                        continue;
                    default:
                        _index = index;
                        return (char)ch;
                }
            }

            _index = length;
            return (char)Etx;
        }

        public int GetIndex()
        {
            return _index;
        }

        public char GetCurrentChar()
        {
            return _data[_index];
        }

        public char GetCurrentCharSafe()
        {
            if (_index >= _length)
                return (char)Etx;
            return _data[_index];
        }

        public char GetChartAt(int index)
        {
            return _data[index];
        }

        public string GetString(int startIndex, int endIndex)
        {
            return new string(_data, startIndex, endIndex - startIndex);
        }

        public ICharSequence GetCharSequence(int startIndex, int endIndex)
        {
            return new CharArraySegment(startIndex, endIndex - startIndex, _data);
        }

        public char[] GetArray(int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            var array = new char[length];
            Array.Copy(_data, startIndex, array, 0, length);
            return array;
        }

        public BigInteger GetBigInteger(int startIndex, int endIndex)
        {
            var len = endIndex - startIndex;
            if (len > MaxLongStrLength)
                return BigInteger.Parse(GetString(startIndex, endIndex));
            else
                return new BigInteger(GetLong(startIndex, endIndex));
        }

        public decimal GetDecimal(int startIndex, int endIndex)
        {
            return decimal.Parse(GetString(startIndex, endIndex));
        }

        public double GetDouble(int startIndex, int endIndex)
        {
            return ParseDoubleSupport.ParseDouble(_data, startIndex, endIndex);
        }

        public float GetFloat(int startIndex, int endIndex)
        {
            return ParseFloatSupport.ParseFloat(_data, startIndex, endIndex);
        }

        public int GetInt(int offset, int to)
        {
            var digitChars = _data;
            var negative = false;
            var c = digitChars[offset];

            if (c == '-')
            {
                offset++;
                negative = true;
            }
            else if (c == '+')
            {
                offset++;
            }

            c = digitChars[offset];
            var num = c - '0';
            offset++;

            for (; offset < to; offset++)
            {
                c = digitChars[offset];
                num = num * 10 + (c - '0');
            }

            return negative ? -num : num;
        }

        public long GetLong(int offset, int to)
        {
            var digitChars = _data;
            var negative = false;
            var c = digitChars[offset];

            if (c == '-')
            {
                offset++;
                negative = true;
            }

            c = digitChars[offset];
            long num = c - '0';
            offset++;

            for (; offset < to; offset++)
            {
                c = digitChars[offset];
                num = num * 10 + (c - '0');
            }

            return negative ? -num : num;
        }

        public int FindEndOfEncodedStringFast()
        {
            int i = ++_index;
            var data = _data;
            int length = _length;
            bool controlChar = false;

            for (; i < length; i++)
            {
                int ch = data[i];
                switch (ch)
                {
                    case ControlEscapeToken:
                        controlChar = !controlChar;
                        continue;
                    case StringEndToken:
                        if (!controlChar)
                        {
                            _index = i + 1;
                            return i;
                        }
                        controlChar = false;
                        break;
                    default:
                        controlChar = false;
                        break;
                }
            }

            throw new InvalidOperationException("Unable to find closing for string");
        }

        public int FindEndString()
        {
            int i = ++_index;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                int ch = data[i];
                switch (ch)
                {
                    case StringEndToken:
                        _index = i;
                        return i;
                    default:
                        if (ch >= SpaceWs)
                            continue;
                        throw new UnexpectedCharacterException("Parsing JSON string",
                            "Unexpected character while finding closing for string", this, ch, i);
                }
            }

            throw new UnexpectedCharacterException("Parsing JSON string", "Unable to find closing for string", this);
        }

        public int FindEndOfEncodedString()
        {
            int i = ++_index;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                int ch = data[i];
                switch (ch)
                {
                    case ControlEscapeToken:
                        i = FindEndOfStringControlEncode(i + 1);
                        continue;
                    case StringEndToken:
                        _index = i + 1;
                        return i;
                    default:
                        if (ch >= SpaceWs)
                            continue;
                        throw new UnexpectedCharacterException("Parsing JSON string",
                            "Unexpected character while finding closing for string", this, ch, i);
                }
            }

            throw new UnexpectedCharacterException("Parsing JSON string", "Unable to find closing for string", this);
        }

        public int FindFalseEnd()
        {
            var index = _index;
            if (_data[++index] == 'a' && _data[++index] == 'l' && _data[++index] == 's' && _data[++index] == 'e')
            {
                ++index;
                _index = index;
                return index;
            }

            throw new UnexpectedCharacterException("Parsing JSON False Boolean", "Unexpected character", this);
        }

        public int FindTrueEnd()
        {
            var index = _index;
            if (_data[++index] == 'r' && _data[++index] == 'u' && _data[++index] == 'e')
            {
                ++index;
                _index = index;
                return index;
            }

            throw new UnexpectedCharacterException("Parsing JSON True Boolean", "Unexpected character", this);
        }

        public int FindNullEnd()
        {
            var index = _index;
            if (_data[++index] == 'u' && _data[++index] == 'l' && _data[++index] == 'l')
            {
                _index = ++index;
                return index;
            }

            throw new UnexpectedCharacterException("Parsing JSON Null", "Unexpected character", this);
        }

        public bool MatchChars(int startIndex, int endIndex, ICharSequence key)
        {
            var length = endIndex - startIndex;
            var idx = startIndex;
            var data = _data;

            switch (length)
            {
                case 1:
                    return key[0] == data[idx];
                case 2:
                    return key[0] == data[idx] && key[1] == data[idx + 1];
                case 3:
                    return key[0] == data[idx] && key[1] == data[idx + 1] && key[2] == data[idx + 2];
                case 4:
                    return key[0] == data[idx] && key[1] == data[idx + 1] && key[2] == data[idx + 2] && key[3] == data[idx + 3];
                case 5:
                    return key[0] == data[idx] && key[1] == data[idx + 1] && key[2] == data[idx + 2] && key[3] == data[idx + 3] && key[4] == data[idx + 4];
                case 6:
                    return key[0] == data[idx] && key[1] == data[idx + 1] && key[2] == data[idx + 2] && key[3] == data[idx + 3] && key[4] == data[idx + 4] && key[5] == data[idx + 5];
                // Add more cases as needed
                default:
                    for (int i = 0; i < length; i++)
                    {
                        if (key[i] != data[idx + i])
                            return false;
                    }
                    return true;
            }
        }

        public bool IsInteger(int offset, int end)
        {
            var len = end - offset;
            var digitChars = _data;
            var negative = digitChars[offset] == '-';
            var cmpLen = negative ? MinIntStrLength : MaxIntStrLength;
            if (len < cmpLen) return true;
            if (len > cmpLen) return false;
            var cmpStr = negative ? MinIntChars : MaxIntChars;
            for (var i = 0; i < cmpLen; ++i)
            {
                var diff = digitChars[offset + i] - cmpStr[i];
                if (diff != 0) return diff < 0;
            }
            return true;
        }

        public bool FindCommaOrEndForArray()
        {
            int i = _index;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                int ch = data[i];
                switch (ch)
                {
                    case ArrayEndToken:
                        _index = i + 1;
                        return true;
                    case ArraySep:
                        _index = i;
                        return false;
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                        continue;
                    default:
                        throw new UnexpectedCharacterException("Parsing Array", "Unexpected character in array", this, ch, i);
                }
            }

            throw new UnexpectedCharacterException("Parsing Array", "Array end not found", this);
        }

        public bool FindObjectEndOrAttributeSep()
        {
            int i = _index;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                int ch = data[i];
                switch (ch)
                {
                    case ObjectEndToken:
                        _index = i + 1;
                        return true;
                    case AttributeSep:
                        _index = i;
                        return false;
                }
            }

            throw new UnexpectedCharacterException("Parsing Object", "Unexpected character in object", this);
        }

        public override string ToString()
        {
            return new string(_data);
        }

        public NumberParseResult FindEndOfNumberFast()
        {
            int i = _index + 1;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                int ch = data[i];
                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                    case AttributeSep:
                    case ArraySep:
                    case ObjectEndToken:
                    case ArrayEndToken:
                        _index = i;
                        return new NumberParseResult(i, false);

                    case Num0:
                    case Num1:
                    case Num2:
                    case Num3:
                    case Num4:
                    case Num5:
                    case Num6:
                    case Num7:
                    case Num8:
                    case Num9:
                        break;

                    case DecimalPoint:
                        _index = i;
                        return FindEndOfFloatFast();

                    case ExponentMarker:
                    case ExponentMarker2:
                        _index = i;
                        return ParseFloatWithExponentFast();

                    default:
                        throw new InvalidOperationException("Unexpected character " + ch + " at index " + _index);
                }
            }

            _index = i;
            return new NumberParseResult(i, false);
        }

        public NumberParseResult FindEndOfNumber()
        {
            var startCh = (int)GetCurrentChar();
            var startIndex = _index;
            int ch;

            var i = startIndex + 1;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                ch = data[i];

                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                    case AttributeSep:
                    case ArraySep:
                    case ObjectEndToken:
                    case ArrayEndToken:
                        goto loop;

                    case Num0:
                    case Num1:
                    case Num2:
                    case Num3:
                    case Num4:
                    case Num5:
                    case Num6:
                    case Num7:
                    case Num8:
                    case Num9:
                        break;

                    case DecimalPoint:
                        if (startCh == Minus)
                        {
                            var numLenSoFar = i - startIndex;
                            if (numLenSoFar == 1)
                                throw new UnexpectedCharacterException("Parsing JSON Number", "Unexpected character", this, ch, i);
                        }

                        _index = i;
                        return FindEndOfFloat();

                    case ExponentMarker:
                    case ExponentMarker2:
                        _index = i;
                        return ParseFloatWithExponent();

                    default:
                        throw new UnexpectedCharacterException("Parsing JSON Number", "Unexpected character", this, ch, i);
                }
            }

        loop:
            _index = i;
            var numLength = i - startIndex;

            switch (startCh)
            {
                case Num0:
                    if (numLength != 1)
                        throw new UnexpectedCharacterException("Parsing JSON Int Number", "Int can't start with a 0", this, startCh, startIndex);
                    break;
                case Plus:
                    throw new UnexpectedCharacterException("Parsing JSON Int Number", "Int can't start with a plus", this, startCh, startIndex);
                case Minus:
                    if (numLength == 1)
                        throw new UnexpectedCharacterException("Parsing JSON Int Number", "Int can't be only a minus, number is missing", this, startCh, startIndex);
                    if (numLength == 2 && data[startIndex + 1] == Num0)
                        throw new UnexpectedCharacterException("Parsing JSON Int Number", "0 can't be after minus sign", this, startCh, startIndex);
                    break;
            }

            return new NumberParseResult(i, false);
        }

        private NumberParseResult FindEndOfFloatFast()
        {
            int i = _index + 1;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                int ch = data[i];
                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                    case AttributeSep:
                    case ArraySep:
                    case ObjectEndToken:
                    case ArrayEndToken:
                        _index = i;
                        return new NumberParseResult(i, true);

                    case Num0:
                    case Num1:
                    case Num2:
                    case Num3:
                    case Num4:
                    case Num5:
                    case Num6:
                    case Num7:
                    case Num8:
                    case Num9:
                        break;

                    case ExponentMarker:
                    case ExponentMarker2:
                        _index = i;
                        return ParseFloatWithExponentFast();

                    default:
                        throw new UnexpectedCharacterException("Parsing JSON Float Number", "Unexpected character", this, ch, i);
                }
            }

            _index = i;
            return new NumberParseResult(i, true);
        }

        private NumberParseResult ParseFloatWithExponent()
        {
            int ch = Next();
            if (!IsNumberOrSign(ch))
                throw new UnexpectedCharacterException("Parsing exponent part of float", "After exponent expecting number or sign but got", this, ch, _index);

            if (IsSign(ch))
            {
                ch = Next();
                if (!IsNumber(ch))
                    throw new UnexpectedCharacterException("Parsing exponent part of float after sign", "After sign expecting number but got", this, ch, _index);
            }

            int i = _index + 1;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                ch = data[i];

                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                    case AttributeSep:
                    case ArraySep:
                    case ObjectEndToken:
                    case ArrayEndToken:
                        _index = i;
                        return new NumberParseResult(i, true);

                    case Num0:
                    case Num1:
                    case Num2:
                    case Num3:
                    case Num4:
                    case Num5:
                    case Num6:
                    case Num7:
                    case Num8:
                    case Num9:
                        break;

                    default:
                        throw new UnexpectedCharacterException("Parsing Float with exponent", "Unable to find closing for Number", this, ch, i);
                }
            }

            _index = i;
            return new NumberParseResult(i, true);
        }

        private NumberParseResult ParseFloatWithExponentFast()
        {
            int i = _index + 1;
            int signOperator = 0;
            var data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                int ch = data[i];
                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                    case AttributeSep:
                    case ArraySep:
                    case ObjectEndToken:
                    case ArrayEndToken:
                        _index = i;
                        return new NumberParseResult(i, true);

                    case Minus:
                    case Plus:
                        signOperator++;
                        if (signOperator > 1)
                            throw new InvalidOperationException("Too many sign operators when parsing exponent of float");
                        break;

                    case Num0:
                    case Num1:
                    case Num2:
                    case Num3:
                    case Num4:
                    case Num5:
                    case Num6:
                    case Num7:
                    case Num8:
                    case Num9:
                        break;

                    default:
                        throw new InvalidOperationException("Unexpected character " + ch + " at index " + _index);
                }
            }

            _index = i;
            return new NumberParseResult(i, true);
        }

        private int FindEndOfStringControlEncode(int i)
        {
            var data = _data;

            int ch = data[i];
            switch (ch)
            {
                case ControlEscapeToken:
                case StringEndToken:
                case 'n':
                case 'b':
                case '/':
                case 'r':
                case 't':
                case 'f':
                    return i;
                case 'u':
                    return FindEndOfHexEncoding(i);
                default:
                    throw new UnexpectedCharacterException("Parsing JSON string", "Unexpected character while finding closing for string", this, ch, i);
            }
        }

        private int FindEndOfHexEncoding(int index)
        {
            var data = _data;

            if (IsHex(data[++index]) && IsHex(data[++index]) && IsHex(data[++index]) && IsHex(data[++index]))
                return index;
            throw new UnexpectedCharacterException("Parsing hex encoding in a string", "Unexpected character", this);
        }

        private bool IsHex(char datum)
        {
            return (datum >= '0' && datum <= '9') || (datum >= 'a' && datum <= 'f') || (datum >= 'A' && datum <= 'F');
        }

        private bool IsNumber(int ch)
        {
            return ch >= '0' && ch <= '9';
        }

        private bool IsNumberOrSign(int ch)
        {
            return IsNumber(ch) || IsSign(ch);
        }

        private bool IsSign(int ch)
        {
            return ch == Minus || ch == Plus;
        }

        public string ErrorDetails(string message, int index, int ch)
        {
            StringBuilder buf = new StringBuilder(255);
            var array = _data;

            buf.Append(message).Append("\n\n");
            buf.Append("The current character read is ").Append(DebugCharDescription(ch)).Append('\n');

            int line = 0;
            int lastLineIndex = 0;

            for (int i = 0; i < index && i < array.Length; i++)
            {
                if (array[i] == '\n')
                {
                    line++;
                    lastLineIndex = i + 1;
                }
            }

            int count = 0;
            for (int i = lastLineIndex; i < array.Length; i++, count++)
            {
                if (array[i] == '\n')
                    break;
            }

            buf.Append("line number ").Append(line + 1).Append('\n');
            buf.Append("index number ").Append(index).Append('\n');

            try
            {
                buf.Append(new string(array, lastLineIndex, count)).Append('\n');
            }
            catch (Exception)
            {
                try
                {
                    int start = index - 10 < 0 ? 0 : index - 10;
                    buf.Append(new string(array, start, index)).Append('\n');
                }
                catch (Exception)
                {
                    buf.Append(new string(array)).Append('\n');
                }
            }

            for (int i = 0; i < index - lastLineIndex; i++)
            {
                buf.Append('.');
            }
            buf.Append('^');

            return buf.ToString();
        }

        private static string DebugCharDescription(int c)
        {
            string charString = c switch
            {
                ' ' => "[SPACE]",
                '\t' => "[TAB]",
                '\n' => "[NEWLINE]",
                Etx => "ETX",
                _ => "'" + (char)c + "'"
            };

            return charString + " with an int value of " + c;
        }
        
        public string GetEncodedString(int start, int end) {
            return CharArrayUtils.DecodeJsonString(_data, start, end);
        }
        
        public String ToEncodedStringIfNeeded(int start, int end) {
            if (CharArrayUtils.HasEscapeChar(_data, start, end)) {
                return GetEncodedString(start, end);
            } else {
                return this.GetString(start, end);
            }
        }
        
        public bool FindChar(char c) {
            int index = _index;
            char[] data = _data;
            int length = _data.Length;

            for (; index < length; index++) {
                if (data[index] == c) {
                    _index = index;
                    return true;
                }
            }
            return false;
        }
        
        public int FindAttributeEnd()
        {
            int index = _index;
            char[] data = _data;
            int length = _length;

            for (; index < length; index++)
            {
                int ch = data[index];
                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                    case AttributeSep:
                        _index = index;
                        goto loop;  // break out of the loop
                }
            }

            loop:
            return index;
        }

        private NumberParseResult FindEndOfFloat()
        {
            int i = _index + 1;
            int ch = (char)Next();

            if (!IsNumber(ch))
            {
                throw new UnexpectedCharacterException("Parsing float part of number", "After decimal point expecting number but got", this, ch, _index);
            }

            char[] data = _data;
            int length = _length;

            for (; i < length; i++)
            {
                ch = data[i];
                switch (ch)
                {
                    case NewLineWs:
                    case CarriageReturnWs:
                    case TabWs:
                    case SpaceWs:
                    case AttributeSep:
                    case ArraySep:
                    case ObjectEndToken:
                    case ArrayEndToken:
                        _index = i;
                        return new NumberParseResult(i, true);

                    case Num0:
                    case Num1:
                    case Num2:
                    case Num3:
                    case Num4:
                    case Num5:
                    case Num6:
                    case Num7:
                    case Num8:
                    case Num9:
                        break;

                    case ExponentMarker:
                    case ExponentMarker2:
                        _index = i;
                        return ParseFloatWithExponent();

                    default:
                        throw new UnexpectedCharacterException("Parsing JSON Float Number", "Unexpected character", this, ch, i);
                }
            }

            _index = i;
            return new NumberParseResult(i, true);
        }


    }
}
