using System.Collections;
using sharpJParse.token;

namespace sharpJParse.node.support;

public class TokenList : IList<Token>
{
    private bool _isReadOnly;

    private Token[] _tokens;

    public TokenList()
    {
        _tokens = new Token[32];
    }

    public TokenList(Token[] tokens)
    {
        Count = tokens.Length;
        _tokens = tokens;
    }

    public IEnumerator<Token> GetEnumerator()
    {
        return new Enumerator(_tokens);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Token token)
    {
        var length = _tokens.Length;
        if (Count >= length)
        {
            var newTokens = new Token[length * 2];
            Array.Copy(_tokens, 0, newTokens, 0, length);
            _tokens = newTokens;
        }

        _tokens[Count] = token;
        Count++;
    }

    public void Clear()
    {
        Count = 0;
    }

    public bool Contains(Token item)
    {
        foreach (var token in _tokens)
        {
            if (token == null) return false;
            if (item == token) return true;
        }

        return false;
    }

    public void CopyTo(Token[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public bool Remove(Token item)
    {
        throw new NotImplementedException();
    }

    public int Count { get; private set; }

    bool ICollection<Token>.IsReadOnly => false;


    public int IndexOf(Token item)
    {
        for (var i = 0; i < _tokens.Length; i++)
        {
            var token = _tokens[i];
            if (token is null) return -1;

            if (token == item) return i;
        }

        return -1;
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
        get => _tokens[index];
        set => _tokens[index] = value;
    }

    public Token[] GetTokens()
    {
        return _tokens;
    }

    public TokenSubList SubList(int from, int to)
    {
        return new TokenSubList(_tokens, from, to);
    }

    private class Enumerator : IEnumerator<Token>
    {
        private int index = -1;
        private readonly Token[] tokens;

        public Enumerator(Token[] tokens)
        {
            this.tokens = tokens;
        }

        public bool MoveNext()
        {
            if (index + 1 < tokens.Length)
            {
                index++;
                return true;
            }

            return false;
        }

        public void Reset()
        {
            index = -1;
        }

        public Token Current { get; }

        object IEnumerator.Current => GetCurrent();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Token GetCurrent()
        {
            return tokens[index];
        }
    }
}