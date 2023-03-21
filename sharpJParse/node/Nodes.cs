using System.Numerics;
using sharpJParse.support;

namespace sharpJParse;




public class RootNode : CollectionNode {

    private  TokenList _tokens;
    private  CharSource _source;
    private  Token _rootToken;
    private  bool _objectsKeysCanBeEncoded;

    private Node _root;

    public RootNode(TokenList tokens, CharSource source, bool objectsKeysCanBeEncoded) {
        this._tokens = tokens;
        this._source = source;
        this._rootToken = tokens[0];
        this._objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
    }

    public NodeType GetType() {
        return NodeTypeUtil.TokenTypeToElement(_rootToken.type);
    }
    
    public Node GetNode(Object key) {
        switch (_rootToken.type) {
            case TokenTypes.OBJECT_TOKEN:
                return GetObjectNode().getNode(key);
            case TokenTypes.ARRAY_TOKEN:
                return GetArrayNode().GetNode(key);
            default:
                return doGetNode(key);
        }
    }


    public List<IList<Token>> ChildrenTokens() {
        switch (_rootToken.type) {
            case TokenTypes.OBJECT_TOKEN:
                return GetObjectNode().ChildrenTokens();
            case TokenTypes.ARRAY_TOKEN:
                return GetArrayNode().ChildrenTokens();
            default:
                return DoGetChildrenTokens();
        }
    }

    private List<IList<Token>> DoGetChildrenTokens() {
            return ((CollectionNode) GetNode()).ChildrenTokens();
    }

    private Node doGetNode(Object key) {
            return ((CollectionNode) GetNode()).GetNode(key);
    }

    public Node GetNode() {
        if (_root == null) {
            _root = NodeUtils.CreateNode(new TokenSubList(_tokens.GetTokens(), 0, _tokens.Count()), _source, _objectsKeysCanBeEncoded);
        }
        return _root;
    }

    public PathNode GetPathNode() {
        if (_root == null) {
            _root = new PathNode((TokenSubList) _tokens.SubList(0, _tokens.size()), charSource());
        }
        return (PathNode) _root;
    }

    public ObjectNode GetObjectNode() {
        return (ObjectNode) getNode();
    }


    public ArrayNode AsArray() {
        return GetArrayNode();
    }


    public ObjectNode AsObject() {
        return GetObjectNode();
    }

    public Dictionary<String, Object> GetMap() {
        return (Dictionary<String, Object>) (Object) GetObjectNode();
    }

    public StringNode getStringNode() {
        return (StringNode) this.GetNode();
    }

    public string GetString() {
        return getStringNode().ToString();
    }

    public int GetInt() {
        return GetNumberNode().intValue();
    }

    public float GetFloat() {
        return GetNumberNode().floatValue();
    }

    public long GetLong() {
        return GetNumberNode().longValue();
    }

    public double GetDouble() {
        return GetNumberNode().doubleValue();
    }
    
    public BigInteger GetBigIntegerValue() {
        return GetNumberNode().bigIntegerValue();
    }

    public NumberNode GetNumberNode() {
        return (NumberNode) GetNode();
    }

    public BooleanNode GetBooleanNode() {
        return (BooleanNode) GetNode();
    }

    public NullNode GetNullNode() {
        return (NullNode) GetNode();
    }

    public bool GetBoolean() {
        return GetBooleanNode().booleanValue();
    }

    public ArrayNode GetArrayNode() {
        return (ArrayNode) GetNode();
    }
    
    public NodeType Type() {
        return NodeType.ROOT;
    }

    IList<Token> Node.Tokens()
    {
        return _tokens;
    }

    public IList<Token> Tokens() {
        return this._tokens;
    }


    public Token RootElementToken() {
        return _rootToken;
    }

    public CharSource CharSource() {
        return _source;
    }

    public bool IsScalar()
    {
        throw new NotImplementedException();
    }

    public bool IsCollection()
    {
        throw new NotImplementedException();
    }

    public bool Equals(Object o) {
        var other = o as RootNode;
        if (other != null) {
            return GetNode().Equals(other.GetNode());
        }
        return false;
    }


    public int GetHashCode() {
        //return GetNode().GetHashCode(); TODO fix 
        return 1;
    }


    public char GetCharAt(int index)
    {
        throw new NotImplementedException();
    }

    public int GetLength()
    {
        throw new NotImplementedException();
    }

    public CharSequence SubSequence(int start, int end)
    {
        throw new NotImplementedException();
    }
}



public class StringNode : ScalarNode, CharSequence {

    private readonly Token _token;
    private readonly CharSource _source;
    private readonly int _length;
    private readonly int _start;
    private readonly int _end;
    private readonly bool _encodeStringByDefault;
    private int _hashCode = 0;
    private  bool _hashCodeSet = false;

    public StringNode(Token token, CharSource source, bool encodeStringByDefault) {
        _token = token;
        _source = source;
        _start = token.startIndex;
        _end = token.endIndex;
        _encodeStringByDefault = encodeStringByDefault;
        _length = token.endIndex - token.startIndex;
    }

    public StringNode(Token token, CharSource source) {
        this._token = token;
        this._source = source;
        _start = token.startIndex;
        _end = token.endIndex;
        this._encodeStringByDefault = true;
        this._length = token.endIndex - token.startIndex;
    }

    public NodeType Type() {
        return NodeType.STRING;
    }
    
    public IList<Token> Tokens() {
        return new SingletonList<Token>(this._token);
    }


    public Token RootElementToken() {
        return _token;
    }

    public CharSource CharSource() {
        return _source;
    }

    public bool IsScalar()
    {
        return true;
    }

    public bool IsCollection()
    {
        return false;
    }


    public Object Value() {
        return ToString();
    }


    public int Length() {
        return _length;
    }


    public char CharAt(int index) {
        return _source.GetChartAt(_token.startIndex + index);
    }

  
    public CharSequence SubSequence(int start, int end) {
        return _source.GetCharSequence(start + this._start, end + this._start);
    }

    public CharSequence CharSequence() {
        return _source.GetCharSequence(this._start, this._end);
    }

  
    public string ToString() {
        return _encodeStringByDefault ? _source.ToEncodedStringIfNeeded(_start, _end) : _source.GetString(_start, _end);
    }

    public string ToEncodedString() {
        return _source.GetEncodedString(_start, _end);
    }

    public string ToUnencodedString() {
        return _source.GetString(_start, _end);
    }


    public override bool Equals(object o) {
        if (this == o) return true;
        if (o is CharSequence) {
            CharSequence other = (CharSequence) o;
            return CharSequenceUtils.Equals(this, other);
        } else {
            return false;
        }
    }


    public override int GetHashCode() {
        if (_hashCodeSet) {
            return _hashCode;
        }
        _hashCode = CharSequenceUtils.GetHashCode(this);
        _hashCodeSet = true;
        return _hashCode;
    }
}


public class NullNode : ScalarNode {


    private readonly Token _token;
    private readonly CharSource _source;


    public NullNode( Token token,  CharSource source) {
        _token = token;
        _source = source;
    }

    public NodeType Type() {
        return NodeType.NULL;
    }


    public IList<Token> Tokens() {
        return new SingletonList<Token>(_token);
    }
    
    public Token RootElementToken() {
        return _token;
    }

    
    public CharSource CharSource() {
        return _source;
    }

    public bool IsScalar()
    {
        return true;
    }

    public bool IsCollection()
    {
        return false;
    }


    public int Length() {
        return 4;
    }

    public CharSequence SubSequence(int start, int end)
    {
        throw new NotImplementedException();
    }

    public char CharAt(int index) {
        switch (index) {
            case 0:
                return 'n';
            case 1:
                return 'u';
            case 2:
            case 3:
                return 'l';
            default:
                throw new InvalidOperationException();
        }
    }

    public object Value() {
        return null;
    }

    public override string ToString() {
        return "null";
    }

    public override bool Equals(object o) {
        if (this == o) return true;
        if (o is null || GetType() != o.GetType()) return false;
        return true;
    }


    public override int GetHashCode() {
        return ToString().GetHashCode();
    }

}


public class BooleanNode : ScalarNode {

    private readonly Token _token;
    private readonly CharSource _source;
    private readonly bool _value;

    public BooleanNode( Token token,  CharSource source) {
        _token = token;
        _source = source;
        _value = _source.GetChartAt(_token.startIndex) == 't';
    }

    public NodeType Type() {
        return NodeType.BOOLEAN;
    }

    public IList<Token> Tokens() {
        return new SingletonList<Token>(_token);
    }

    public Token RootElementToken() {
        return _token;
    }

    public CharSource CharSource() {
        return _source;
    }

    public bool IsScalar()
    {
        return true;
    }

    public bool IsCollection()
    {
        return false;
    }


    public bool BooleanValue() {
        return _value;
    }



    public int Length() {
        return _value ? 4 : 5;
    }

    public CharSequence SubSequence(int start, int end)
    {
        throw new NotImplementedException();
    }


    public object Value() {
        return BooleanValue();
    }

    public char CharAt(int index) {
        if (_value) {
            switch (index) {
                case 0:
                    return 't';
                case 1:
                    return 'r';
                case 2:
                    return 'u';
                case 3:
                    return 'e';
                default:
                    throw new InvalidOperationException();
            }
        } else {
            switch (index) {
                case 0:
                    return 'f';
                case 1:
                    return 'a';
                case 2:
                    return 'l';
                case 3:
                    return 's';
                case 4:
                    return 'e';
                default:
                    throw new InvalidOperationException();
            }
        }
    }


    public override string ToString() {
        return _value ? "true" : "false";
    }


    public override bool Equals(Object o) {
        if (this == o) return true;
        if (o == null || GetType() != o.GetType()) return false;
        BooleanNode that = (BooleanNode) o;
        return _value == that._value;
    }


    public override int GetHashCode() {
        return ToString().GetHashCode();
    }
}