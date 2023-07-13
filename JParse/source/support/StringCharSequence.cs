using sharpJParse.JsonParser.support;

namespace sharpJParse.JsonParser.source.support;

public class StringCharSequence : ICharSequence
{
    private readonly string _str;

    public StringCharSequence(string str)
    {
        _str = str;
        Length = str.Length;
    }

    public int Length { get; }


    char ICharSequence.CharAt(int index)
    {
        return CharAt(index);
    }
    public char CharAt(int index)
    {
        return _str[index];
    }

    public ICharSequence SubSequence(int start, int end)
    {
        return new CharArraySegment(start, end - start, _str.ToCharArray());
    }
    
    string ICharSequence.ToString()
    {
        return ToString();
    }
    
    public override string ToString()
    {
        return _str;
    }
    
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
    
    
    public override bool Equals(object? o)
    {
     
        if (this == o) return true;
        
        switch (o)
        {
            case ICharSequence sequence when sequence.Length != Length:
                return false;
            case ICharSequence sequence:
            {
                var end = Length;
                for (int i = 0, j = 0; i < end; i++, j++)
                {
                    var cOther = sequence.CharAt(j);
                    var cThis = _str[i];
                    if (cOther != cThis) return false;
                }

                return true;
            }
            case string other:
                return _str.Equals(other);
            default:
                return false;
        }
    }
}