using System.Numerics;
using sharpJParse.node.support;
using sharpJParse.source;
using sharpJParse.support;
using sharpJParse.token;

namespace sharpJParse.node;

public class RootNode : ICollectionNode
{
    private readonly bool _objectsKeysCanBeEncoded;
    private readonly Token _rootToken;
    private readonly ICharSource _source;

    private readonly TokenList _tokens;

    private INode _root;

    public RootNode(TokenList tokens, ICharSource source, bool objectsKeysCanBeEncoded)
    {
        _tokens = tokens;
        _source = source;
        _rootToken = tokens[0];
        _objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
    }

    public INode GetNode(object key)
    {
        switch (_rootToken.type)
        {
            case TokenTypes.OBJECT_TOKEN:
                return GetObjectNode().GetNode(key);
            case TokenTypes.ARRAY_TOKEN:
                return GetArrayNode().GetNode(key);
            default:
                return DoGetNode(key);
        }
    }

    List<TokenSubList>? ICollectionNode.ChildrenTokens()
    {
        throw new NotImplementedException();
    }


    public ArrayNode AsArray()
    {
        return GetArrayNode();
    }


    public ObjectNode AsObject()
    {
        return GetObjectNode();
    }

    public NodeType Type()
    {
        return NodeType.ROOT;
    }

    IList<Token> INode.Tokens()
    {
        return _tokens;
    }


    public Token RootElementToken()
    {
        return _rootToken;
    }

    public ICharSource CharSource()
    {
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


    public char GetCharAt(int index)
    {
        throw new NotImplementedException();
    }

    public char CharAt(int index)
    {
        throw new NotImplementedException();
    }

    public int Length()
    {
        throw new NotImplementedException();
    }

    public char this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public ICharSequence SubSequence(int start, int end)
    {
        throw new NotImplementedException();
    }

    public NodeType GetType()
    {
        return NodeTypeUtil.TokenTypeToElement(_rootToken.type);
    }


    public List<TokenSubList>? ChildrenTokens()
    {
        switch (_rootToken.type)
        {
            case TokenTypes.OBJECT_TOKEN:
                return GetObjectNode().ChildrenTokens();
            case TokenTypes.ARRAY_TOKEN:
                return GetArrayNode().ChildrenTokens();
            default:
                return DoGetChildrenTokens();
        }
    }

    private List<TokenSubList>? DoGetChildrenTokens()
    {
        return ((ICollectionNode)GetNode()).ChildrenTokens();
    }

    private INode DoGetNode(object key)
    {
        return ((ICollectionNode)GetNode()).GetNode(key);
    }

    public INode GetNode()
    {
        if (_root == null)
            _root = NodeUtils.CreateNode(new TokenSubList(_tokens.GetTokens(), 0, _tokens.Count()), _source,
                _objectsKeysCanBeEncoded);
        return _root;
    }

    //TODO add back when we add Path support
    // public PathNode GetPathNode() {
    //     if (_root == null) {
    //         _root = new PathNode((TokenSubList) _tokens.SubList(0, _tokens.size()), charSource());
    //     }
    //     return (PathNode) _root;
    // }

    public ObjectNode GetObjectNode()
    {
        return (ObjectNode)GetNode();
    }

    public Dictionary<string, object> GetMap()
    {
        return (Dictionary<string, object>)(object)GetObjectNode();
    }

    public StringNode getStringNode()
    {
        return (StringNode)GetNode();
    }

    public string GetString()
    {
        return getStringNode().ToString();
    }

    public int GetInt()
    {
        return GetNumberNode().IntValue();
    }

    public float GetFloat()
    {
        return GetNumberNode().FloatValue();
    }

    public long GetLong()
    {
        return GetNumberNode().LongValue();
    }

    public double GetDouble()
    {
        return GetNumberNode().DoubleValue();
    }

    public BigInteger GetBigIntegerValue()
    {
        return GetNumberNode().BigIntegerValue();
    }

    public NumberNode GetNumberNode()
    {
        return (NumberNode)GetNode();
    }

    public BooleanNode GetBooleanNode()
    {
        return (BooleanNode)GetNode();
    }

    public NullNode GetNullNode()
    {
        return (NullNode)GetNode();
    }

    public bool GetBoolean()
    {
        return GetBooleanNode().BooleanValue();
    }

    public ArrayNode GetArrayNode()
    {
        return (ArrayNode)GetNode();
    }

    public IList<Token> Tokens()
    {
        return _tokens;
    }

    public bool Equals(object o)
    {
        var other = o as RootNode;
        if (other != null) return GetNode().Equals(other.GetNode());
        return false;
    }


    public int GetHashCode()
    {
        //return GetNode().GetHashCode(); TODO fix 
        return 1;
    }

    public int GetLength()
    {
        throw new NotImplementedException();
    }
}

public class StringNode : IScalarNode, ICharSequence
{
    private readonly bool _encodeStringByDefault;
    private readonly int _end;
    private readonly int _length;
    private readonly ICharSource _source;
    private readonly int _start;

    private readonly Token _token;
    private int _hashCode;
    private bool _hashCodeSet;

    public StringNode(Token token, ICharSource source, bool encodeStringByDefault)
    {
        _token = token;
        _source = source;
        _start = token.startIndex;
        _end = token.endIndex;
        _encodeStringByDefault = encodeStringByDefault;
        _length = token.endIndex - token.startIndex;
    }

    public StringNode(Token token, ICharSource source)
    {
        _token = token;
        _source = source;
        _start = token.startIndex;
        _end = token.endIndex;
        _encodeStringByDefault = true;
        _length = token.endIndex - token.startIndex;
    }

    public NodeType Type()
    {
        return NodeType.STRING;
    }

    public IList<Token> Tokens()
    {
        return new SingletonList<Token>(_token);
    }


    public Token RootElementToken()
    {
        return _token;
    }

    public ICharSource CharSource()
    {
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


    public object Value()
    {
        return ToString();
    }


    public int Length()
    {
        return _length;
    }

    public char this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }


    public char CharAt(int index)
    {
        return _source.GetChartAt(_token.startIndex + index);
    }


    public ICharSequence SubSequence(int start, int end)
    {
        return _source.GetCharSequence(start + _start, end + _start);
    }


    public string ToString()
    {
        return _encodeStringByDefault ? _source.ToEncodedStringIfNeeded(_start, _end) : _source.GetString(_start, _end);
    }

    public ICharSequence CharSequence()
    {
        return _source.GetCharSequence(_start, _end);
    }

    public string ToEncodedString()
    {
        return _source.GetEncodedString(_start, _end);
    }

    public string ToUnencodedString()
    {
        return _source.GetString(_start, _end);
    }


    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (o is ICharSequence)
        {
            var other = (ICharSequence)o;
            return CharSequenceUtils.Equals(this, other);
        }

        return false;
    }


    public override int GetHashCode()
    {
        if (_hashCodeSet) return _hashCode;
        _hashCode = CharSequenceUtils.GetHashCode(this);
        _hashCodeSet = true;
        return _hashCode;
    }
}

public class NullNode : IScalarNode
{
    private readonly ICharSource _source;


    private readonly Token _token;


    public NullNode(Token token, ICharSource source)
    {
        _token = token;
        _source = source;
    }

    public NodeType Type()
    {
        return NodeType.NULL;
    }


    public IList<Token> Tokens()
    {
        return new SingletonList<Token>(_token);
    }

    public Token RootElementToken()
    {
        return _token;
    }


    public ICharSource CharSource()
    {
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


    public int Length()
    {
        return 4;
    }

    public char this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public ICharSequence SubSequence(int start, int end)
    {
        throw new NotImplementedException();
    }

    public char CharAt(int index)
    {
        switch (index)
        {
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

    public object Value()
    {
        return null;
    }

    public override string ToString()
    {
        return "null";
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (o is null || GetType() != o.GetType()) return false;
        return true;
    }


    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}

public class BooleanNode : IScalarNode
{
    private readonly ICharSource _source;

    private readonly Token _token;
    private readonly bool _value;

    public BooleanNode(Token token, ICharSource source)
    {
        _token = token;
        _source = source;
        _value = _source.GetChartAt(_token.startIndex) == 't';
    }

    public NodeType Type()
    {
        return NodeType.BOOLEAN;
    }

    public IList<Token> Tokens()
    {
        return new SingletonList<Token>(_token);
    }

    public Token RootElementToken()
    {
        return _token;
    }

    public ICharSource CharSource()
    {
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


    public bool BooleanValue()
    {
        return _value;
    }


    public int Length()
    {
        return _value ? 4 : 5;
    }

    public char this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public ICharSequence SubSequence(int start, int end)
    {
        throw new NotImplementedException();
    }


    public object Value()
    {
        return BooleanValue();
    }

    public char CharAt(int index)
    {
        if (_value)
            switch (index)
            {
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

        switch (index)
        {
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


    public override string ToString()
    {
        return _value ? "true" : "false";
    }


    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (o == null || GetType() != o.GetType()) return false;
        var that = (BooleanNode)o;
        return _value == that._value;
    }


    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}

public class NumberNode : IScalarNode
{
    private readonly NodeType _elementType;
    private readonly ICharSource _source;

    private readonly Token _token;
    private int _hashCode;
    private bool _hashCodeSet;


    public NumberNode(Token token, ICharSource source, NodeType elementType)
    {
        _token = token;
        _source = source;
        _elementType = elementType;
    }


    public int IntValue()
    {
        return _source.GetInt(_token.startIndex, _token.endIndex);
    }


    public long LongValue()
    {
        return _source.GetLong(_token.startIndex, _token.endIndex);
    }

    public double DoubleValue()
    {
        return _source.GetDouble(_token.startIndex, _token.endIndex);
    }


    public BigInteger BigIntegerValue()
    {
        return _source.GetBigInteger(_token.startIndex, _token.endIndex);
    }


    public object Value()
    {
        if (IsInteger())
            return IntValue();
        if (IsLong())
            return LongValue();
        return DoubleValue();
    }

    public NodeType Type()
    {
        return _elementType;
    }

    public int Length()
    {
        return _token.endIndex - _token.startIndex;
    }

    public char this[int index]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }


    public char CharAt(int index)
    {
        if (index > Length()) throw new IndexOutOfRangeException();
        return _source.GetChartAt(_token.startIndex + index);
    }

    public ICharSequence SubSequence(int start, int end)
    {
        if (end > Length()) throw new IndexOutOfRangeException();
        return _source.GetCharSequence(start + _token.startIndex, end + _token.startIndex);
    }

    public IList<Token> Tokens()
    {
        return new SingletonList<Token>(_token);
    }

    public Token RootElementToken()
    {
        return _token;
    }

    public ICharSource CharSource()
    {
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


    public override string ToString()
    {
        return _source.GetString(RootElementToken().startIndex, RootElementToken().endIndex);
    }

    public float FloatValue()
    {
        return _source.GetFloat(_token.startIndex, _token.endIndex);
    }


    public decimal DecimalValue()
    {
        return 1; // TODO fix _source.getBigDecimal(_token.startIndex, _token.endIndex);
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (o == null || GetType() != o.GetType()) return false;
        var other = (NumberNode)o;
        return CharSequenceUtils.Equals(this, other);
    }

    public override int GetHashCode()
    {
        if (_hashCodeSet) return _hashCode;
        _hashCode = CharSequenceUtils.GetHashCode(this);
        _hashCodeSet = true;
        return _hashCode;
    }

    public bool IsInteger()
    {
        switch (_elementType)
        {
            case NodeType.INT:
                return _source.IsInteger(_token.startIndex, _token.endIndex);
            default:
                return false;
        }
    }

    public bool IsLong()
    {
        switch (_elementType)
        {
            case NodeType.INT:
                return !_source.IsInteger(_token.startIndex, _token.endIndex);
            default:
                return false;
        }
    }
}