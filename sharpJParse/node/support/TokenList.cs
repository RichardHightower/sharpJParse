using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace sharpJParse.support;

public class TokenList : IList<Token>
{
    
    private Token[] _tokens;
    private int _index = 0;
    private bool _isReadOnly;

    public TokenList() {
        this._tokens = new Token[32];
    }

    public TokenList(Token[] tokens) {

        _index = tokens.Length;
        this._tokens = tokens;
    }

    class Enumerator : IEnumerator<Token>
    {
        private Token[] tokens;
        private int index = -1;
        public Enumerator(     Token[] tokens)
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

        public Token GetCurrent()
        {
            return tokens[index];
        }

        object IEnumerator.Current => GetCurrent();

        public void Dispose()
        {
            throw new NotImplementedException();
        }
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

        int length = _tokens.Length;
        if (_index >= length) {
            Token[] newTokens = new Token[length * 2];
            Array.Copy(_tokens, 0, newTokens, 0, length);
            _tokens = newTokens;
        }
        _tokens[_index] = token;
        _index++;

        
    }

    public void Clear()
    {
        this._index = 0;
    }

    public bool Contains(Token item)
    {
        foreach (var token in _tokens)
        {
            if (token == null)
            {
                return false;
            }
            if (item == token)
            {
                return true;
            }
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

    public int Count
    {
        get
        {
            return _index;
        }
    }

    bool ICollection<Token>.IsReadOnly => false;

    
    public int IndexOf(Token item)
    {

        for (int i = 0; i < _tokens.Length; i++)
        {

            Token token = _tokens[i];
            if (token is null)
            {
                return -1;
            }

            if (token == item)
            {
                return i;
            }
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
    
    public Token[] GetTokens() {
        return _tokens;
    }
    
    public  TokenSubList SubList( int from, int to) {
        return new TokenSubList(_tokens, from, to);
    }

}