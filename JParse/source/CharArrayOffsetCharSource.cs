using System.Numerics;
using System.Text;
using sharpJParse.JsonParser.parser;
using sharpJParse.JsonParser.source.support;
using sharpJParse.JsonParser.support;

namespace sharpJParse.JsonParser.source;

public class CharArrayOffsetCharSource : ParseConstants, ICharSource
{
    private static readonly char[] MinIntChars = MinIntStr.ToCharArray();
    private static readonly char[] MaxIntChars = MaxIntStr.ToCharArray();
    private readonly char[] _data;
    private readonly int _length;
    private readonly int _sourceEndIndex;
    private readonly int _sourceStartIndex;
    private int index;

    public CharArrayOffsetCharSource(int startIndex, int endIndex, char[] chars)
    {
        index = startIndex - 1;
        _data = chars;
        _sourceStartIndex = startIndex;
        _sourceEndIndex = endIndex;
        _length = endIndex - startIndex;
    }

    public int Next()
    {
        if (index + 1 >= _sourceEndIndex)
        {
            index = _sourceEndIndex;
            return Etx;
        }

        return _data[++index];
    }

    public int GetIndex()
    {
        return index - _sourceStartIndex;
    }

    public char GetCurrentChar()
    {
        return _data[index];
    }

    public char GetCurrentCharSafe()
    {
        return index >= _sourceEndIndex ? Etx : _data[index];
    }

    public char SkipWhiteSpace()
    {
        for (; index < _sourceEndIndex; index++)
        {
            var ch = _data[index];
            if (ch != NewLineWs && ch != CarriageReturnWs && ch != TabWs && ch != SpaceWs)
                break;
        }

        return _data[index];
    }

    public char GetChartAt(int index)
    {
        return _data[index + _sourceStartIndex];
    }

    public string GetString(int startIndex, int endIndex)
    {
        var from = startIndex + _sourceStartIndex;
        return new string(_data, from, endIndex - startIndex);
    }

    public double GetDouble(int startIndex, int endIndex)
    {
        return ParseDoubleSupport.ParseDouble(_data, startIndex + _sourceStartIndex, endIndex + _sourceStartIndex);
    }

    public float GetFloat(int startIndex, int endIndex)
    {
        return ParseFloatSupport.ParseFloat(_data, startIndex + _sourceStartIndex, endIndex + _sourceStartIndex);
    }

    public int GetInt(int startIndex, int endIndex)
    {
        var from = startIndex + _sourceStartIndex;
        var to = endIndex + _sourceStartIndex;
        var num = 0;
        var negative = false;

        if (_data[from] == '-')
        {
            negative = true;
            from++;
        }
        else if (_data[from] == '+')
        {
            from++;
        }

        for (; from < to; from++) num = num * 10 + (_data[from] - '0');

        return negative ? -num : num;
    }

    public long GetLong(int startIndex, int endIndex)
    {
        var from = startIndex + _sourceStartIndex;
        var to = endIndex + _sourceStartIndex;
        long num = 0;
        var negative = false;

        if (_data[from] == '-')
        {
            negative = true;
            from++;
        }
        else if (_data[from] == '+')
        {
            from++;
        }

        for (; from < to; from++) num = num * 10 + (_data[from] - '0');

        return negative ? -num : num;
    }

    ICharSequence ICharSource.GetCharSequence(int startIndex, int endIndex)
    {
        return GetCharSequence(startIndex, endIndex);
    }

    public char[] GetArray(int startIndex, int endIndex)
    {
        var length = endIndex - startIndex;
        var array = new char[length];
        Array.Copy(_data, startIndex + _sourceStartIndex, array, 0, length);
        return array;
    }

    public string GetEncodedString(int start, int end)
    {
        return CharArrayUtils.DecodeJsonString(_data, start + _sourceStartIndex, end + _sourceStartIndex);
    }

    public string ToEncodedStringIfNeeded(int start, int end)
    {
        var from = start + _sourceStartIndex;
        var to = end + _sourceStartIndex;
        return CharArrayUtils.HasEscapeChar(_data, from, to) ? GetEncodedString(start, end) : GetString(start, end);
    }

    public decimal GetDecimal(int startIndex, int endIndex)
    {
        return decimal.Parse(GetString(startIndex, endIndex));
    }

    public BigInteger GetBigInteger(int startIndex, int endIndex)
    {
        return BigInteger.Parse(GetString(startIndex, endIndex));
    }

    public int FindEndOfEncodedString()
    {
        var i = ++index;
        var controlChar = false;
        for (; i < _sourceEndIndex; i++)
        {
            int ch = _data[i];
            switch (ch)
            {
                case ControlEscapeToken:
                    controlChar = !controlChar;
                    continue;
                case StringEndToken:
                    if (!controlChar)
                    {
                        index = i + 1;
                        return i - _sourceStartIndex;
                    }

                    controlChar = false;
                    break;
                default:
                    controlChar = false;
                    break;
            }
        }

        throw new InvalidOperationException("Unable to find closing for String");
    }

    public int FindEndString()
    {
        var i = ++index;
        for (; i < _sourceEndIndex; i++)
        {
            var ch = _data[i];
            if (ch == StringEndToken)
            {
                index = i;
                return i - _sourceStartIndex;
            }

            if (ch < SpaceWs)
                throw new UnexpectedCharacterException("Parsing JSON String",
                    "Unexpected character while finding closing for String", this, ch, i - _sourceStartIndex);
        }

        throw new UnexpectedCharacterException("Parsing JSON String", "Unable to find closing for String", this, Etx,
            i - _sourceStartIndex);
    }

    public NumberParseResult FindEndOfNumber()
    {
        int startCh = GetCurrentChar();
        var startIndex = index;
        var i = index + 1;

        for (; i < _sourceEndIndex; i++)
        {
            int ch = _data[i];
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
                    if (startCh == Minus && i - startIndex == 1)
                        throw new UnexpectedCharacterException("Parsing JSON Number", "Unexpected character", this, ch,
                            i - _sourceStartIndex);
                    index = i;
                    return FindEndOfFloat();
                case ExponentMarker:
                case ExponentMarker2:
                    index = i;
                    return ParseFloatWithExponent();
                default:
                    throw new UnexpectedCharacterException("Parsing JSON Number", "Unexpected character", this, ch,
                        i - _sourceStartIndex);
            }
        }

        loop:
        index = i;
        var numLength = i - startIndex;

        switch (startCh)
        {
            case Num0 when numLength != 1:
                throw new UnexpectedCharacterException("Parsing JSON Int Number", "Int can't start with a 0 ", this,
                    startCh, startIndex - _sourceStartIndex);
            case Plus:
                throw new UnexpectedCharacterException("Parsing JSON Int Number", "Int can't start with a plus ", this,
                    startCh, startIndex - _sourceStartIndex);
            case Minus:
                switch (numLength)
                {
                    case 1:
                        throw new UnexpectedCharacterException("Parsing JSON Int Number",
                            "Int can't be only a minus, number is missing", this, startCh,
                            startIndex - _sourceStartIndex);
                    case 2:
                        break;
                    default:
                        if (_data[startIndex + 1] == Num0)
                            throw new UnexpectedCharacterException("Parsing JSON Int Number",
                                "0 can't be after minus sign", this, startCh, startIndex - _sourceStartIndex);
                        break;
                }

                break;
        }

        return new NumberParseResult(i - _sourceStartIndex, false);
    }

    public int FindFalseEnd()
    {
        if (_data[++index] == 'a' && _data[++index] == 'l' && _data[++index] == 's' && _data[++index] == 'e')
            return ++index - _sourceStartIndex;
        throw new UnexpectedCharacterException("Parsing JSON False Boolean", "Unexpected character", this);
    }

    public int FindTrueEnd()
    {
        if (_data[++index] == 'r' && _data[++index] == 'u' && _data[++index] == 'e') return ++index - _sourceStartIndex;
        throw new UnexpectedCharacterException("Parsing JSON True Boolean", "Unexpected character", this);
    }

    public int FindNullEnd()
    {
        if (_data[++index] == 'u' && _data[++index] == 'l' && _data[++index] == 'l') return ++index - _sourceStartIndex;
        throw new UnexpectedCharacterException("Parsing JSON Null", "Unexpected character", this);
    }


    public bool MatchChars(int startIndex, int endIndex, ICharSequence key)
    {
        var length = endIndex - startIndex;
        var idx = startIndex + _sourceStartIndex;

        if (length > 12)
        {
            var start = 0;
            var end = length - 1;
            var middle = length / 2;

            if (key[start] == _data[idx] && key[end] == _data[idx + end] && key[middle] == _data[idx + middle])
            {
                for (var i = 1; i < length; i++)
                    if (key[i] != _data[idx + i])
                        return false;
                return true;
            }

            return false;
        }

        switch (length)
        {
            case 1:
                return key[0] == _data[idx];
            case 2:
                return key[0] == _data[idx] && key[1] == _data[idx + 1];
            case 3:
                return key[0] == _data[idx] && key[1] == _data[idx + 1] && key[2] == _data[idx + 2];
            case 4:
                return key[0] == _data[idx] && key[1] == _data[idx + 1] && key[2] == _data[idx + 2] &&
                       key[3] == _data[idx + 3];
            case 5:
                return key[1] == _data[idx + 1] && key[3] == _data[idx + 3] && key[0] == _data[idx] &&
                       key[2] == _data[idx + 2] && key[4] == _data[idx + 4];
            case 6:
                return key[0] == _data[idx] && key[5] == _data[idx + 5] && key[3] == _data[idx + 3] &&
                       key[1] == _data[idx + 1] && key[2] == _data[idx + 2] && key[4] == _data[idx + 4];
            case 7:
                return key[0] == _data[idx] && key[6] == _data[idx + 6] && key[3] == _data[idx + 3] &&
                       key[1] == _data[idx + 1] && key[5] == _data[idx + 5] && key[2] == _data[idx + 2] &&
                       key[4] == _data[idx + 4];
            case 8:
                return key[0] == _data[idx] && key[7] == _data[idx + 7] && key[3] == _data[idx + 3] &&
                       key[1] == _data[idx + 1] && key[5] == _data[idx + 5] && key[2] == _data[idx + 2] &&
                       key[6] == _data[idx + 6] && key[4] == _data[idx + 4];
            case 9:
                return key[0] == _data[idx] && key[8] == _data[idx + 8] && key[2] == _data[idx + 2] &&
                       key[6] == _data[idx + 6] && key[3] == _data[idx + 3] && key[7] == _data[idx + 7] &&
                       key[4] == _data[idx + 4] && key[5] == _data[idx + 5] && key[1] == _data[idx + 1];
            case 10:
                return key[0] == _data[idx] && key[9] == _data[idx + 9] && key[6] == _data[idx + 6] &&
                       key[3] == _data[idx + 3] && key[7] == _data[idx + 7] && key[2] == _data[idx + 2] &&
                       key[4] == _data[idx + 4] && key[5] == _data[idx + 5] && key[1] == _data[idx + 1] &&
                       key[8] == _data[idx + 8];
            case 11:
                return key[0] == _data[idx] && key[10] == _data[idx + 10] && key[6] == _data[idx + 6] &&
                       key[3] == _data[idx + 3] && key[7] == _data[idx + 7] && key[2] == _data[idx + 2] &&
                       key[9] == _data[idx + 9] && key[4] == _data[idx + 4] && key[5] == _data[idx + 5] &&
                       key[1] == _data[idx + 1] && key[8] == _data[idx + 8];

            default:
                return false;
        }
    }

    public bool IsInteger(int startIndex, int endIndex)
    {
        var len = endIndex - startIndex;
        var digitChars = _data;
        var negative = digitChars[startIndex + _sourceStartIndex] == '-';
        var cmpLen = negative ? MinIntStrLength : MaxIntStrLength;
        if (len < cmpLen) return true;
        if (len > cmpLen) return false;
        var cmpStr = negative ? MinIntChars : MaxIntChars;
        for (var i = 0; i < cmpLen; ++i)
        {
            var diff = digitChars[startIndex + _sourceStartIndex + i] - cmpStr[i];
            if (diff != 0) return diff < 0;
        }

        return true;
    }

    public int NextSkipWhiteSpace()
    {
        var index = this.index + 1;
        var data = _data;
        var length = _sourceEndIndex;
        int ch = Etx;

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
        this.index = index;
        return index == length ? Etx : ch;
    }

    public string ErrorDetails(string message, int index, int ch)
    {
        var buf = new StringBuilder(255);

        buf.AppendLine(message);
        buf.AppendLine();
        buf.AppendLine($"The current character read is {DebugCharDescription(ch)}");

        var line = 0;
        var lastLineIndex = 0;

        for (var i = 0; i < index && i < _data.Length; i++)
            if (_data[i] == '\n')
            {
                line++;
                lastLineIndex = i + 1;
            }

        var count = 0;
        for (var i = lastLineIndex; i < _data.Length; i++, count++)
            if (_data[i] == '\n')
                break;

        buf.AppendLine($"line number {line + 1}");
        buf.AppendLine($"index number {index}");
        buf.AppendLine($"offset index number {index + _sourceStartIndex}");

        try
        {
            buf.AppendLine(new string(_data, lastLineIndex, count));
        }
        catch (Exception)
        {
            try
            {
                var start = index = index - 10 < 0 ? 0 : index - 10;
                buf.AppendLine(new string(_data, start, index));
            }
            catch (Exception)
            {
                buf.AppendLine(new string(_data));
            }
        }

        for (var i = 0; i < index - lastLineIndex; i++) buf.Append('.');
        buf.Append('^');

        return buf.ToString();
    }

    public bool FindCommaOrEndForArray()
    {
        var i = index;
        int ch;
        var data = _data;
        var end = _sourceEndIndex;

        for (; i < end; i++)
        {
            ch = data[i];
            switch (ch)
            {
                case ArrayEndToken:
                    index = i + 1;
                    return true;
                case ArraySep:
                    index = i;
                    return false;
                case NewLineWs:
                case CarriageReturnWs:
                case TabWs:
                case SpaceWs:
                    continue;
                default:
                    throw new UnexpectedCharacterException("Parsing Object Key", "Finding object end or separator",
                        this, ch, i - _sourceStartIndex);
            }
        }

        throw new UnexpectedCharacterException("Parsing Array", "Finding list end or separator", this);
    }

    public bool FindObjectEndOrAttributeSep()
    {
        var i = index;
        int ch;
        var data = _data;
        var end = _sourceEndIndex;

        for (; i < end; i++)
        {
            ch = data[i];
            switch (ch)
            {
                case ObjectEndToken:
                    index = i + 1;
                    return true;
                case AttributeSep:
                    index = i;
                    return false;
            }
        }

        throw new UnexpectedCharacterException("Parsing Object Key", "Finding object end or separator", this);
    }

    public void CheckForJunk()
    {
        var index = this.index;
        var data = _data;
        var end = _sourceEndIndex;
        int ch = Etx;

        for (; index < end; index++)
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
                    throw new UnexpectedCharacterException("Junk", "Unexpected extra characters", this, ch);
            }
        }
    }

    public NumberParseResult FindEndOfNumberFast()
    {
        var i = index + 1;
        int ch;
        var data = _data;
        var end = _sourceEndIndex;
        for (; i < end; i++)
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
                    index = i;
                    return new NumberParseResult(i - _sourceStartIndex, false);
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
                    index = i;
                    return FindEndOfFloatFast();
                case ExponentMarker:
                case ExponentMarker2:
                    index = i;
                    return ParseFloatWithExponentFast();
                default:
                    throw new InvalidOperationException($"Unexpected character {ch} at index {index}");
            }
        }

        index = i;
        return new NumberParseResult(i - _sourceStartIndex, false);
    }

    public int FindEndOfEncodedStringFast()
    {
        var i = ++index;
        var data = _data;
        var end = _sourceEndIndex;
        var controlChar = false;
        for (; i < end; i++)
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
                        index = i + 1;
                        return i - _sourceStartIndex;
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
        var index = this.index;
        var data = _data;
        var end = _sourceEndIndex;

        for (; index < end; index++)
            if (data[index] == c)
            {
                this.index = index;
                return true;
            }

        return false;
    }

    public int FindAttributeEnd()
    {
        var index = this.index;
        var data = _data;
        var end = _sourceEndIndex;

        for (; index < end; index++)
        {
            int ch = data[index];
            switch (ch)
            {
                case NewLineWs:
                case CarriageReturnWs:
                case TabWs:
                case SpaceWs:
                case AttributeSep:
                    this.index = index;
                    goto loop;
            }
        }

        loop:
        return index - _sourceStartIndex;
    }

    public static string DebugCharDescription(int c)
    {
        var charString = c switch
        {
            ' ' => "[SPACE]",
            '\t' => "[TAB]",
            '\n' => "[NEWLINE]",
            Etx => "ETX",
            _ => $"'{(char)c}'"
        };
        return $"{charString} with an int value of {c}";
    }

    public ICharSequence GetCharSequence(int startIndex, int endIndex)
    {
        return new CharArraySegment(startIndex + _sourceStartIndex, endIndex - startIndex, _data);
    }

    public override string ToString()
    {
        return new string(_data, _sourceStartIndex, _length);
    }

    private NumberParseResult FindEndOfFloatFast()
    {
        var i = index + 1;
        int ch = (char)0;
        var data = _data;
        var length = _sourceEndIndex;

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
                    index = i;
                    return new NumberParseResult(i - _sourceStartIndex, true);

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
                    index = i;
                    return ParseFloatWithExponentFast();

                default:
                    throw new UnexpectedCharacterException("Parsing JSON Float Number", "Unexpected character", this,
                        ch, i - _sourceStartIndex);
            }
        }

        index = i;
        return new NumberParseResult(i - _sourceStartIndex, true);
    }

    private NumberParseResult ParseFloatWithExponentFast()
    {
        var i = index + 1;
        int ch = (char)0;
        var signOperator = 0;
        var data = _data;
        var length = _sourceEndIndex;
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
                    index = i;
                    return new NumberParseResult(i - _sourceStartIndex, true);

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
                    throw new InvalidOperationException($"Unexpected character {ch} at index {index}");
            }
        }

        index = i;
        return new NumberParseResult(i - _sourceStartIndex, true);
    }

    private NumberParseResult FindEndOfFloat()
    {
        var i = index + 1;
        int ch = (char)Next();

        if (!IsNumber(ch))
            throw new UnexpectedCharacterException("Parsing float part of number",
                "After decimal point expecting number but got", this, ch, index - _sourceStartIndex);
        var data = _data;
        var length = _sourceEndIndex;

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
                    index = i;
                    return new NumberParseResult(i - _sourceStartIndex, true);

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
                    index = i;
                    return ParseFloatWithExponent();

                default:
                    throw new UnexpectedCharacterException("Parsing JSON Float Number", "Unexpected character", this,
                        ch, i - _sourceStartIndex);
            }
        }

        index = i;
        return new NumberParseResult(i - _sourceStartIndex, true);
    }

    private NumberParseResult ParseFloatWithExponent()
    {
        int ch = (char)Next();
        if (!IsNumberOrSign(ch))
            throw new UnexpectedCharacterException("Parsing exponent part of float",
                "After exponent expecting number or sign but got", this, ch, index - _sourceStartIndex);

        if (IsSign(ch))
        {
            ch = (char)Next();
            if (!IsNumber(ch))
                throw new UnexpectedCharacterException("Parsing exponent part of float after sign",
                    "After sign expecting number but got", this, ch, index - _sourceStartIndex);
        }

        var i = index + 1;
        var data = _data;
        var length = _sourceEndIndex;

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
                    index = i;
                    return new NumberParseResult(i - _sourceStartIndex, true);

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
                        "Unable to find closing for Number", this, ch, i - _sourceStartIndex);
            }
        }

        index = i;
        return new NumberParseResult(i - _sourceStartIndex, true);
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
}