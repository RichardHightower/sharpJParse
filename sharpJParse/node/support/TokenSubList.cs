using System.Collections;
using sharpJParse.token;

namespace sharpJParse.node.support;

public class TokenSubList : IList<Token>
{
    private readonly int _offset;
    private readonly Token[] _tokens;
    private readonly int _endIndex;


    public TokenSubList(Token[] tokens, int offset, int endIndex)
    {
        Count = endIndex - offset;
        _tokens = tokens;
        _offset = offset;
        _endIndex = endIndex;
    }

    public TokenSubList(Token token)
    {
        Count = _endIndex - _offset;
        _tokens = new[] { token };
        _offset = 0;
        _endIndex = 1;
    }

    public IEnumerator<Token> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Token item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(Token item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(Token[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(Token item)
    {
        throw new NotImplementedException();
    }


    public int Count { get; }

    bool ICollection<Token>.IsReadOnly => false;

    public int IndexOf(Token item)
    {
        throw new NotImplementedException();
    }

    public void Insert(int index, Token item)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new NotImplementedException();
    }

    public Token this[int index]
    {
        get => _tokens[_offset + index];
        set => throw new NotImplementedException();
    }

    public Token Get(int index)
    {
        return _tokens[_offset + index];
    }


    public int Size()
    {
        return Count;
    }


    public TokenSubList SubList(int start, int end)
    {
        return new TokenSubList(_tokens, _offset + start, _offset + end);
    }

    public int CountChildren(int from, Token rootToken)
    {
        var idx = from;
        var count = 0;
        var tokens = _tokens;
        var length = Count;
        var offset = _offset;
        var rootTokenStart = rootToken.startIndex;
        var rootTokenEnd = rootToken.endIndex;
        for (; idx < length; idx++)
        {
            var token = tokens[idx + offset];


            if (token.startIndex >= rootTokenStart
                && token.endIndex <= rootTokenEnd)
                count++;
            else
                break;
        }

        return count;
    }
}