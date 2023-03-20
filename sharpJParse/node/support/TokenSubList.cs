using System.Collections;

namespace sharpJParse.support;

public class TokenSubList : IList<Token>
{
    private readonly int _size;
    private int endIndex;
    private readonly int offset;
    private readonly Token[] tokens;


    public TokenSubList(Token[] tokens, int offset, int endIndex)
    {
        _size = endIndex - offset;
        this.tokens = tokens;
        this.offset = offset;
        this.endIndex = endIndex;
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


    public int Count
    {
        get
        {
            return _size;
        }
    }

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
        get => tokens[offset + index];
        set => throw new NotImplementedException();
    }

    public Token Get(int index)
    {
        return tokens[offset + index];
    }


    public int Size()
    {
        return _size;
    }


    public TokenSubList SubList(int start, int end)
    {
        return new TokenSubList(tokens, offset + start, offset + end);
    }

    public int CountChildren(int from, Token rootToken)
    {
        var idx = from;
        var count = 0;
        var tokens = this.tokens;
        var length = _size;
        var offset = this.offset;
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