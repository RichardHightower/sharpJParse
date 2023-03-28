using System.Numerics;
using sharpJParse.support;

namespace sharpJParse.source;

public class NumberParseResult
{
    private readonly int _endIndex;
    private readonly bool _wasFloat;

    public NumberParseResult(int endIndex, bool wasFloat)
    {
        _endIndex = endIndex;
        _wasFloat = wasFloat;
    }

    public int EndIndex()
    {
        return _endIndex;
    }

    public bool WasFloat()
    {
        return _wasFloat;
    }

    public override bool Equals(object? obj)
    {
        if (obj == this) return true;
        if (obj == null || obj.GetType() != GetType()) return false;
        var that = (NumberParseResult)obj;
        return _endIndex == that._endIndex &&
               _wasFloat == that._wasFloat;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }


    public override string ToString()
    {
        return "NumberParseResult[" +
               "endIndex=" + _endIndex + ", " +
               "wasFloat=" + _wasFloat + ']';
    }
}

public interface ICharSource
{
    int Next();

    int GetIndex();

    char GetCurrentChar();

    char GetCurrentCharSafe();

    char SkipWhiteSpace();

    char GetChartAt(int index);

    string GetString(int startIndex, int endIndex);

    double GetDouble(int startIndex, int endIndex);

    float GetFloat(int startIndex, int endIndex);

    int GetInt(int startIndex, int endIndex);

    long GetLong(int startIndex, int endIndex);

    ICharSequence GetCharSequence(int startIndex, int endIndex);

    char[] GetArray(int startIndex, int endIndex);

    string GetEncodedString(int start, int end);

    string ToEncodedStringIfNeeded(int start, int end);

    BigInteger GetBigInteger(int startIndex, int endIndex);

    int FindEndOfEncodedString();

    int FindEndString();

    NumberParseResult FindEndOfNumber();

    int FindFalseEnd();

    int FindTrueEnd();

    int FindNullEnd();


    bool MatchChars(int startIndex, int endIndex, string key);

    bool MatchChars(int startIndex, int endIndex, char[] key);

    bool MatchChars(int startIndex, int endIndex, ICharSequence key);

    bool IsInteger(int startIndex, int endIndex);

    int NextSkipWhiteSpace();

    string ErrorDetails(string message, int index, int ch);


    bool FindCommaOrEndForArray();

    bool FindObjectEndOrAttributeSep();

    void CheckForJunk();

    NumberParseResult FindEndOfNumberFast();

    int FindEndOfEncodedStringFast();

    bool FindChar(char c);

    int FindAttributeEnd();
}