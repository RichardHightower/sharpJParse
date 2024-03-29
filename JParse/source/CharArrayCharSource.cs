using System.Numerics;
using sharpJParse.JsonParser.parser;
using sharpJParse.JsonParser.source.support;
using sharpJParse.JsonParser.support;

namespace sharpJParse.JsonParser.source;

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

    public int Next()
    {
        if (_index + 1 >= _length)
        {
            _index = _length;
            return Etx;
        }

        return _data[++_index];
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
        if (_index >= _length) return (char)Etx;
        return _data[_index];
    }

    public char SkipWhiteSpace()
    {
        var index = _index;
        var data = _data;
        var length = data.Length;

        int ch;


        for (; index < length; index++)
        {
            ch = data[index];
            switch (ch)
            {
                case NewLineWs:
                case CarriageReturnWs:
                case TabWs:
                case SpaceWs:
                    continue;
                default:
                    goto loop;
            }
        }

        loop:
        _index = index;
        return data[index];
    }

    public char GetChartAt(int index)
    {
        return _data[index];
    }

    public string GetString(int startIndex, int endIndex)
    {
        return new string(_data, startIndex, endIndex - startIndex);
    }

    public double GetDouble(int startIndex, int endIndex)
    {
        return double.Parse(GetString(startIndex, endIndex));
    }

    public float GetFloat(int startIndex, int endIndex)
    {
        return float.Parse(GetString(startIndex, endIndex));
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
            negative = false;
        }

        c = digitChars[offset];
        var num = c - '0';
        offset++;

        int digit;

        for (; offset < to; offset++)
        {
            c = digitChars[offset];
            digit = c - '0';
            num = num * 10 + digit;
        }

        return negative ? num * -1 : num;
    }

    public long GetLong(int offset, int to)
    {
        var digitChars = _data;

        long num;
        var negative = false;
        var c = digitChars[offset];
        if (c == '-')
        {
            offset++;
            negative = true;
        }

        c = digitChars[offset];
        num = c - '0';
        offset++;

        long digit;

        for (; offset < to; offset++)
        {
            c = digitChars[offset];
            digit = c - '0';
            num = num * 10 + digit;
        }

        return negative ? num * -1 : num;
    }

    public ICharSequence GetCharSequence(int startIndex, int endIndex)
    {
        return new CharArraySegment(startIndex, endIndex - startIndex, _data);
    }

    public char[] GetArray(int startIndex, int endIndex)
    {
        var length = endIndex - startIndex;
        var array = new char[length];

        Array.Copy(_data, startIndex, array, 0, length);
        return array;
    }

    public string GetEncodedString(int start, int end)
    {
        return CharArrayUtils.DecodeJsonString(_data, start, end);
    }

    public string ToEncodedStringIfNeeded(int start, int end)
    {
        if (CharArrayUtils.HasEscapeChar(_data, start, end))
            return GetEncodedString(start, end);
        return GetString(start, end);
    }

    public BigInteger GetBigInteger(int startIndex, int endIndex)
    {
        return BigInteger.Parse(GetString(startIndex, endIndex));
    }

    public int FindEndOfEncodedString()
    {
        var i = ++_index;
        var data = _data;
        var length = _length;
        var ch = 0;
        for (; i < length; i++)
        {
            ch = data[i];
            switch (ch)
            {
                case ControlEscapeToken:
                    i = FindEndOfstringControlEncode(i + 1);
                    continue;
                case StringEndToken:
                    _index = i + 1;
                    return i;
                default:
                    if (ch >= SpaceWs) continue;
                    throw new UnexpectedCharacterException("Parsing JSON string",
                        "Unexpected character while finding closing for string", this, ch, i);
            }
        }

        throw new UnexpectedCharacterException("Parsing JSON Encoded string", "Unable to find closing for string", this,
            ch, i);
    }

    public int FindEndString()
    {
        var i = ++_index;
        var data = _data;
        var length = _length;
        var ch = 0;

        for (; i < length; i++)
        {
            ch = data[i];
            switch (ch)
            {
                case StringEndToken:
                    _index = i;
                    return i;
                default:
                    if (ch >= SpaceWs) continue;
                    throw new UnexpectedCharacterException("Parsing JSON string",
                        "Unexpected character while finding closing for string", this, ch, i);
            }
        }

        throw new UnexpectedCharacterException("Parsing JSON string", "Unable to find closing for string", this, ch, i);
    }

    public NumberParseResult FindEndOfNumber()
    {
        var startCh = (int)GetCurrentChar();
        var startIndex = _index;
        int ch;


        var i = startIndex + 1;

        var data = _data;
        var length = _length;


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
                            throw new UnexpectedCharacterException("Parsing JSON Number", "Unexpected character", this,
                                ch, i);
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
                    throw new UnexpectedCharacterException("Parsing JSON Int Number",
                        "Int can't start with a 0 ", this, startCh, startIndex);
                break;
            case Plus:
                throw new UnexpectedCharacterException("Parsing JSON Int Number",
                    "Int can't start with a plus ", this, startCh, startIndex);

            case Minus:
                switch (numLength)
                {
                    case 1:
                        throw new UnexpectedCharacterException("Parsing JSON Int Number",
                            "Int can't be only a minus, number is missing", this, startCh, startIndex);

                    case 2:
                        break;
                    default:
                        if (data[startIndex + 1] == Num0)
                            throw new UnexpectedCharacterException("Parsing JSON Int Number",
                                "0 can't be after minus sign", this, startCh, startIndex);
                        break;
                }

                break;
        }

        return new NumberParseResult(i, false);
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

    public bool MatchChars(int startIndex, int endIndex, string key)
    {
        var length = endIndex - startIndex;
        var idx = startIndex;
        var data = _data;

        switch (length)
        {
            case 1:
                return key[0] == data[idx];
            case 2:
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1];
            case 3:
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2];
            case 4:
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2] &&
                       key[3] == data[idx + 3];

            case 5:
                return key[1] == data[idx + 1] &&
                       key[3] == data[idx + 3] &&
                       key[0] == data[idx] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 6:
                return key[0] == data[idx] &&
                       key[5] == data[idx + 5] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 7:
                return key[0] == data[idx] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[5] == data[idx + 5] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 8:
                return key[0] == data[idx] &&
                       key[7] == data[idx + 7] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[5] == data[idx + 5] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[4] == data[idx + 4];


            case 9:
                return key[0] == data[idx] &&
                       key[8] == data[idx + 8] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1];

            case 10:
                return key[0] == data[idx] &&
                       key[9] == data[idx + 9] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            case 11:
                return key[0] == data[idx] &&
                       key[10] == data[idx + 10] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[9] == data[idx + 9] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            case 12:
                return key[0] == data[idx] &&
                       key[11] == data[idx + 11] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[9] == data[idx + 9] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[10] == data[idx + 10] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            default:
                var start = 0;
                var end = length - 1;
                var middle = length / 2;

                if (key[start] == data[idx] &&
                    key[end] == data[idx + end] &&
                    key[middle] == data[idx + middle])
                {
                    for (var i = 1; i < length; i++)
                        if (key[i] != data[idx + i])
                            return false;
                    return true;
                }

                return false;
        }
    }

    public bool MatchChars(int startIndex, int endIndex, char[] key)
    {
        var length = endIndex - startIndex;
        var idx = startIndex;
        var data = _data;

        switch (length)
        {
            case 1:
                return key[0] == data[idx];
            case 2:
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1];
            case 3:
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2];
            case 4:
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2] &&
                       key[3] == data[idx + 3];

            case 5:
                return key[1] == data[idx + 1] &&
                       key[3] == data[idx + 3] &&
                       key[0] == data[idx] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 6:
                return key[0] == data[idx] &&
                       key[5] == data[idx + 5] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 7:
                return key[0] == data[idx] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[5] == data[idx + 5] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 8:
                return key[0] == data[idx] &&
                       key[7] == data[idx + 7] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[5] == data[idx + 5] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[4] == data[idx + 4];


            case 9:
                return key[0] == data[idx] &&
                       key[8] == data[idx + 8] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1];

            case 10:
                return key[0] == data[idx] &&
                       key[9] == data[idx + 9] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            case 11:
                return key[0] == data[idx] &&
                       key[10] == data[idx + 10] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[9] == data[idx + 9] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            case 12:
                return key[0] == data[idx] &&
                       key[11] == data[idx + 11] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[9] == data[idx + 9] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[10] == data[idx + 10] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            default:
                var start = 0;
                var end = length - 1;
                var middle = length / 2;

                if (key[start] == data[idx] &&
                    key[end] == data[idx + end] &&
                    key[middle] == data[idx + middle])
                {
                    for (var i = 1; i < length; i++)
                        if (key[i] != data[idx + i])
                            return false;
                    return true;
                }

                return false;
        }
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
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1];
            case 3:
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2];
            case 4:
                return key[0] == data[idx] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2] &&
                       key[3] == data[idx + 3];

            case 5:
                return key[1] == data[idx + 1] &&
                       key[3] == data[idx + 3] &&
                       key[0] == data[idx] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 6:
                return key[0] == data[idx] &&
                       key[5] == data[idx + 5] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 7:
                return key[0] == data[idx] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[5] == data[idx + 5] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4];

            case 8:
                return key[0] == data[idx] &&
                       key[7] == data[idx + 7] &&
                       key[3] == data[idx + 3] &&
                       key[1] == data[idx + 1] &&
                       key[5] == data[idx + 5] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[4] == data[idx + 4];


            case 9:
                return key[0] == data[idx] &&
                       key[8] == data[idx + 8] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1];

            case 10:
                return key[0] == data[idx] &&
                       key[9] == data[idx + 9] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            case 11:
                return key[0] == data[idx] &&
                       key[10] == data[idx + 10] &&
                       key[6] == data[idx + 6] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[9] == data[idx + 9] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            case 12:
                return key[0] == data[idx] &&
                       key[11] == data[idx + 11] &&
                       key[3] == data[idx + 3] &&
                       key[7] == data[idx + 7] &&
                       key[2] == data[idx + 2] &&
                       key[6] == data[idx + 6] &&
                       key[9] == data[idx + 9] &&
                       key[4] == data[idx + 4] &&
                       key[5] == data[idx + 5] &&
                       key[10] == data[idx + 10] &&
                       key[1] == data[idx + 1] &&
                       key[8] == data[idx + 8];

            default:
                var start = 0;
                var end = length - 1;
                var middle = length / 2;

                if (key[start] == data[idx] &&
                    key[end] == data[idx + end] &&
                    key[middle] == data[idx + middle])
                {
                    for (var i = 1; i < length; i++)
                        if (key[i] != data[idx + i])
                            return false;
                    return true;
                }

                return false;
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

    public int NextSkipWhiteSpace()
    {
        var index = _index + 1;
        var data = _data;
        var length = _length;
        var ch = Etx;


        for (; index < length; index++)
        {
            ch = data[index];
            switch (ch)
            {
                case NewLineWs:
                case CarriageReturnWs:
                case TabWs:
                case SpaceWs:
                    continue;
                default:
                    goto loop;
            }
        }

        loop:
        _index = index;
        return index == length ? Etx : ch;
    }

    public string ErrorDetails(string message, int index, int ch)
    {
        return "coming soon";
    }

    public bool FindCommaOrEndForArray()
    {
        var i = _index;
        var ch = 0;
        var data = _data;
        var length = _length;

        for (; i < length; i++)
        {
            ch = data[i];
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
                    throw new UnexpectedCharacterException("Parsing Object Key", "Finding object end or separator",
                        this, ch, i);
            }
        }


        throw new UnexpectedCharacterException("Parsing Array", "Finding list end or separator", this);
    }

    public bool FindObjectEndOrAttributeSep()
    {
        var i = _index;
        var ch = 0;
        var data = _data;
        var length = _length;

        for (; i < length; i++)
        {
            ch = data[i];
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

        throw new UnexpectedCharacterException("Parsing Object Key", "Finding object end or separator", this);
    }

    public void CheckForJunk()
    {
        var index = _index;
        var data = _data;
        var length = _length;
        var ch = Etx;

        for (; index < length; index++)
        {
            ch = data[index];
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


    public NumberParseResult FindEndOfNumberFast()
    {
        var i = _index + 1;
        var ch = 0;
        var data = _data;
        var length = _length;
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

    public int FindEndOfEncodedStringFast()
    {
        var i = ++_index;
        var data = _data;
        var length = _length;
        var controlChar = false;
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

    public bool FindChar(char c)
    {
        var index = _index;
        var data = _data;
        var length = _length;

        for (; index < length; index++)
            if (data[index] == c)
            {
                _index = index;
                return true;
            }

        return false;
    }

    public int FindAttributeEnd()
    {
        var index = _index;
        var data = _data;
        var length = _length;


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
                    goto loop;
            }
        }

        loop:

        return index;
    }

    public decimal GetDecimal(int startIndex, int endIndex)
    {
        return decimal.Parse(GetString(startIndex, endIndex));
    }


    private int FindEndOfstringControlEncode(int i)
    {
        var data = _data;
        var length = _length;
        var ch = 0;


        ch = data[i];
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
                throw new UnexpectedCharacterException("Parsing JSON string",
                    "Unexpected character while finding closing for string", this, ch, i);
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
        switch (datum)
        {
            case 'A':
            case 'B':
            case 'C':
            case 'D':
            case 'E':
            case 'F':
            case 'a':
            case 'b':
            case 'c':
            case 'd':
            case 'e':
            case 'f':
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                return true;
            default:
                return false;
        }
    }

    private NumberParseResult ParseFloatWithExponent()
    {
        var ch = Next();
        if (!IsNumberOrSign(ch))
            throw new UnexpectedCharacterException("Parsing exponent part of float",
                "After exponent expecting number or sign but got", this, ch, _index);

        if (IsSign(ch))
        {
            ch = Next();
            if (!IsNumber(ch))
                throw new UnexpectedCharacterException("Parsing exponent part of float after sign",
                    "After sign expecting number but got", this, ch, _index);
        }

        var i = _index + 1;
        var data = _data;
        var length = _length;

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
                    throw new UnexpectedCharacterException("Parsing Float with exponent",
                        "Unable to find closing for Number", this, ch, i);
            }
        }

        _index = i;
        return new NumberParseResult(i, true);
    }

    private bool IsSign(int ch)
    {
        switch (ch)
        {
            case Minus:
            case Plus:
                return true;
            default:
                return false;
        }
    }

    private bool IsNumberOrSign(int ch)
    {
        switch (ch)
        {
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
            case Minus:
            case Plus:
                return true;
            default:
                return false;
        }
    }

    private NumberParseResult FindEndOfFloat()
    {
        var i = _index + 1;
        var ch = Next();

        if (!IsNumber(ch))
            throw new UnexpectedCharacterException("Parsing float part of number",
                "After decimal point expecting number but got", this, ch, _index);
        var data = _data;
        var length = _length;

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
                    throw new UnexpectedCharacterException("Parsing JSON Float Number", "Unexpected character", this,
                        ch, i);
            }
        }


        _index = i;
        return new NumberParseResult(i, true);
    }

    private bool IsNumber(int ch)
    {
        switch (ch)
        {
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
                return true;
            default:
                return false;
        }
    }


    private NumberParseResult FindEndOfFloatFast()
    {
        var i = _index + 1;
        var ch = 0;
        var data = _data;
        var length = _length;

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
                    return ParseFloatWithExponentFast();


                default:
                    throw new UnexpectedCharacterException("Parsing JSON Float Number", "Unexpected character", this,
                        ch, i);
            }
        }


        _index = i;
        return new NumberParseResult(i, true);
    }

    private NumberParseResult ParseFloatWithExponentFast()
    {
        var i = _index + 1;
        var ch = 0;
        var signOperator = 0;
        var data = _data;
        var length = _length;
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

    public override string ToString()
    {
        return new string(_data);
    }
}