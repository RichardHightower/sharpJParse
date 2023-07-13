using JsonParser.support;

namespace JsonParser.source.support;

public class CharArraySegment : ICharSequence
{
    private readonly char[] _data;

    private readonly int _offset;

    private bool hashCodeSet;

    private int hashCode;

    public CharArraySegment(int offset, int length, char[] data)
    {
        _offset = offset;
        Length = length;
        _data = data;
    }

    public char CharAt(int index)
    {
        return _data[_offset + index];
    }

    // ReSharper disable once ConvertToAutoProperty
    public int Length { get; }

    public ICharSequence SubSequence(int start, int end)
    {
        return new CharArraySegment(start + _offset, end - start, _data);
    }

    string ICharSequence.ToString()
    {
        return ToString();
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        switch (o)
        {
            case CharArraySegment segment when segment.Length != Length:
                return false;
            case CharArraySegment segment:
            {
                var end = Length + _offset;
                for (int i = _offset, j = segment._offset; i < end; i++, j++)
                {
                    var cOther = segment._data[j];
                    var cThis = _data[i];
                    if (cOther != cThis) return false;
                }

                return true;
            }
            case ICharSequence other:
            {
                if (other.Length != Length) return false;
                var end = Length + _offset;
                for (int i = _offset, j = 0; i < end; i++, j++)
                {
                    var cOther = other.CharAt(j);
                    var cThis = _data[i];
                    if (cOther != cThis) return false;
                }
                return true;
            }
            case string s:
            {
                var other = s;
                if (other.Length != Length) return false;
                var end = Length + _offset;
                for (int i = _offset, j = 0; i < end; i++, j++)
                {
                    var cOther = other[j];
                    var cThis = _data[i];
                    if (cOther != cThis) return false;
                }
                return true;
            }
            default:
                return false;
        }
    }

    public override int GetHashCode()
    {
        if (hashCodeSet)
        {
            return hashCode;
        }
        hashCode = ToString().GetHashCode();
        hashCodeSet = true;
        return hashCode;
    }

    public override string ToString()
    {
        return new string(_data, _offset, Length);
    }
}