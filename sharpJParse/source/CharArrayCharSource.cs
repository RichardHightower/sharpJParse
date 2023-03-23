using System.Numerics;
using sharpJParse.parser;
using sharpJParse.support;

namespace sharpJParse.source;

public class CharArrayCharSource : ParseConstants, ICharSource 
{
    private readonly char[] _data;
    private int _index;
    private readonly int _length;


    public CharArrayCharSource(char[] data)
    {
        _data = data;
        _index = -1;
        _length = _data.Length;
    }
    
    public int Next()
    {
        if (_index + 1 >= _length) {
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
        throw new NotImplementedException();
    }

    public char GetCurrentCharSafe()
    {
        throw new NotImplementedException();
    }

    public char SkipWhiteSpace()
    {
        throw new NotImplementedException();
    }

    public char GetChartAt(int index)
    {
        throw new NotImplementedException();
    }

    public string GetString(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
    }

    public double GetDouble(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
    }

    public float GetFloat(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
    }

    public int GetInt(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public int FindEndString()
    {
        throw new NotImplementedException();
    }

    public NumberParseResult FindEndOfNumber()
    {
        throw new NotImplementedException();
    }

    public int FindFalseEnd()
    {
        throw new NotImplementedException();
    }

    public int FindTrueEnd()
    {
        throw new NotImplementedException();
    }

    public int FindNullEnd()
    {
        throw new NotImplementedException();
    }

    public bool MatchChars(int startIndex, int endIndex, ICharSequence key)
    {
        throw new NotImplementedException();
    }

    public bool IsInteger(int startIndex, int endIndex)
    {
        throw new NotImplementedException();
    }

    public int NextSkipWhiteSpace()
    {

        int index = this._index + 1;
        char[] data = _data;
        int length = _length;
        int ch = Etx;


        for (; index < length; index++) {
            ch = data[index];
            switch (ch) {
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
        _index = index ;
        return index == length ? Etx : ch;
    }

    public string ErrorDetails(string message, int index, int ch)
    {
        throw new NotImplementedException();
    }

    public bool FindCommaOrEndForArray()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public bool FindChar(char c)
    {
        throw new NotImplementedException();
    }

    public int FindAttributeEnd()
    {
        throw new NotImplementedException();
    }
}