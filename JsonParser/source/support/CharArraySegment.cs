using sharpJParse.support;

namespace JsonParser.source.support;

public class CharArraySegment : ICharSequence
{
    private readonly char[] _data;

    private readonly int _offset;

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
        if (o is CharArraySegment)
        {
            var other = (CharArraySegment)o;

            if (other.Length != Length) return false;

            var end = Length + _offset;
            for (int i = _offset, j = other._offset; i < end; i++, j++)
            {
                var cOther = other._data[j];
                var cThis = _data[i];
                if (cOther != cThis) return false;
            }

            return true;
        }

        if (o is ICharSequence)
        {
            var other = (ICharSequence)o;

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

        if (o is string)
        {
            var other = (string)o;

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

        return false;
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public override string ToString()
    {
        return new string(_data, _offset, Length);
    }
}