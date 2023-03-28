using System.Numerics;
using sharpJParse.parser;
using sharpJParse.support;

namespace sharpJParse.source;

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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
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

    public long GetLong(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
    }

    public ICharSequence GetCharSequence(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
    }

    public char[] GetArray(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
    }

    public string GetEncodedString(int start, int end)
    {
        throw new NotImplementedException();
    }

    public string ToEncodedStringIfNeeded(int start, int end)
    {
        throw new NotImplementedException();
    }

    public BigInteger GetBigInteger(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public NumberParseResult FindEndOfNumber()
    {
        var startCh = GetCurrentChar();
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

        return null;
    }

    public int FindFalseEnd()
    {
        int index = _index;
        if (_data[++index] == 'a' && _data[++index] == 'l' && _data[++index] == 's' && _data[++index] == 'e') {
            ++index;
            _index = index;
            return index;
        } else {
            throw new UnexpectedCharacterException("Parsing JSON False Boolean", "Unexpected character", this);

        }
    }

    public int FindTrueEnd()
    {
        int index = _index;
        if (_data[++index] == 'r' && _data[++index] == 'u' && _data[++index] == 'e') {
             ++index;
             _index = index;
             return index;
        } else {

            throw new UnexpectedCharacterException("Parsing JSON True Boolean", "Unexpected character", this);
        }
        
    }

    public int FindNullEnd()
    {
        int index = _index;
        if (_data[++index] == 'u' && _data[++index] == 'l' && _data[++index] == 'l') {
            _index = ++index;
            return index;
        } else {
            throw new UnexpectedCharacterException("Parsing JSON Null", "Unexpected character", this);
        }
    }

    public bool MatchChars(int startIndex, int endIndex, string key)
    {
        int length = endIndex - startIndex;
        int idx = startIndex;
        char[] data = _data;

        switch (length) {
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
                        key[3]== data[idx + 3] &&
                        key[7] == data[idx + 7] &&
                        key[2] == data[idx + 2] &&
                        key[6] == data[idx + 6] &&
                        key[9] == data[idx + 9] &&
                        key[4] == data[idx + 4] &&
                        key[5] == data[idx + 5] &&
                        key[10] == data[idx + 10] &&
                        key[1]== data[idx + 1] &&
                        key[8] == data[idx + 8];

            default:
                int start = 0;
                int end = length - 1;
                int middle = length / 2;

                if (key[start] == data[idx] &&
                        key[end] == data[idx + end] &&
                        key[middle] == data[idx + middle]) {
                    for (int i = 1; i < length; i++) {
                        if (key[i] != data[idx + i]) {
                            return false;
                        }
                    }
                    return true;
                } else {
                    return false;
                }
        }

    }

    public bool MatchChars(int startIndex, int endIndex, char[] key)
    {

        int length = endIndex - startIndex;
        int idx = startIndex;
        char[] data = _data;

        switch (length) {
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
                        key[3]== data[idx + 3] &&
                        key[7] == data[idx + 7] &&
                        key[2] == data[idx + 2] &&
                        key[6] == data[idx + 6] &&
                        key[9] == data[idx + 9] &&
                        key[4] == data[idx + 4] &&
                        key[5] == data[idx + 5] &&
                        key[10] == data[idx + 10] &&
                        key[1]== data[idx + 1] &&
                        key[8] == data[idx + 8];

            default:
                int start = 0;
                int end = length - 1;
                int middle = length / 2;

                if (key[start] == data[idx] &&
                        key[end] == data[idx + end] &&
                        key[middle] == data[idx + middle]) {
                    for (int i = 1; i < length; i++) {
                        if (key[i] != data[idx + i]) {
                            return false;
                        }
                    }
                    return true;
                } else {
                    return false;
                }
        }
    }
    
    
    public bool MatchChars(int startIndex, int endIndex, ICharSequence key)
    {

        int length = endIndex - startIndex;
        int idx = startIndex;
        char[] data = _data;

        switch (length) {
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
                        key[3]== data[idx + 3] &&
                        key[7] == data[idx + 7] &&
                        key[2] == data[idx + 2] &&
                        key[6] == data[idx + 6] &&
                        key[9] == data[idx + 9] &&
                        key[4] == data[idx + 4] &&
                        key[5] == data[idx + 5] &&
                        key[10] == data[idx + 10] &&
                        key[1]== data[idx + 1] &&
                        key[8] == data[idx + 8];

            default:
                int start = 0;
                int end = length - 1;
                int middle = length / 2;

                if (key[start] == data[idx] &&
                        key[end] == data[idx + end] &&
                        key[middle] == data[idx + middle]) {
                    for (int i = 1; i < length; i++) {
                        if (key[i] != data[idx + i]) {
                            return false;
                        }
                    }
                    return true;
                } else {
                    return false;
                }
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
        throw new NotImplementedException();
    }

    public void CheckForJunk()
    {
        throw new NotImplementedException();
    }

    public NumberParseResult FindEndOfNumberFast()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public int FindAttributeEnd()
    {
        throw new NotImplementedException();
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
        int ch =   Next();
        if (!IsNumberOrSign(ch)) {
            throw new UnexpectedCharacterException("Parsing exponent part of float", "After exponent expecting number or sign but got", this, (int) ch, _index);
        }

        if (IsSign(ch)) {
            ch =  Next();
            if (!IsNumber(ch)) {
                throw new UnexpectedCharacterException("Parsing exponent part of float after sign", "After sign expecting number but got", this, (int) ch, _index);
            }
        }

        int i = _index + 1;
        char[] data = _data;
        int length = _length;

        for (; i < length; i++) {
            ch = data[i];

            switch (ch) {

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
                    throw new UnexpectedCharacterException("Parsing Float with exponent", "Unable to find closing for Number", this,  ch, i);

            }
        }
        _index = i;
        return new NumberParseResult(i, true);
    }

    private bool IsSign(int ch)
    {
        switch (ch) {
            case Minus:
            case Plus:
                return true;
            default:
                return false;
        }

    }

    private bool IsNumberOrSign(int ch)
    {
        switch (ch) {
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
}