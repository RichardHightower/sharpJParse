using System.Numerics;
using JsonParser.node.support;
using JsonParser.source;
using JsonParser.support;
using JsonParser.token;

namespace JsonParser.node;

public class RootNode : ICollectionNode
{
    private readonly bool _objectsKeysCanBeEncoded;
    private readonly Token _rootToken;
    private readonly ICharSource _source;

    private readonly TokenList _tokens;

    private INode? _root;

    public RootNode(TokenList tokens, ICharSource source, bool objectsKeysCanBeEncoded)
    {
        _tokens = tokens;
        _source = source;
        _rootToken = tokens[0];
        _objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
    }

    public INode GetNode(object key)
    {
        switch (_rootToken.Type)
        {
            case TokenTypes.OBJECT_TOKEN:
                return GetObjectNode().GetNode(key);
            case TokenTypes.ARRAY_TOKEN:
                return GetArrayNode().GetNode(key);
            default:
                return DoGetNode(key);
        }
    }

    List<TokenSubList> ICollectionNode.ChildrenTokens()
    {
        //TODO
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


    public NodeType RootType()
    {
        return NodeTypeUtil.TokenTypeToElement(_rootToken.Type);
    }


    public List<TokenSubList>? ChildrenTokens()
    {
        switch (_rootToken.Type)
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
        return _root ?? throw new InvalidOperationException();
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

    public StringNode GetStringNode()
    {
        return (StringNode)GetNode();
    }

    public string GetString()
    {
        return GetStringNode().ToString();
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

    public override bool Equals(object? o)
    {
        var other = o as RootNode;
        if (other != null) return GetNode().Equals(other.GetNode());
        return false;
    }


    public override int GetHashCode()
    {
        //return GetNode().GetHashCode(); TODO fix 
        return 1;
    }
}

public class StringNode : IScalarNode
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
        _start = token.StartIndex;
        _end = token.EndIndex;
        _encodeStringByDefault = encodeStringByDefault;
        _length = token.EndIndex - token.StartIndex;
    }

    public StringNode(Token token, ICharSource source)
    {
        _token = token;
        _source = source;
        _start = token.StartIndex;
        _end = token.EndIndex;
        _encodeStringByDefault = true;
        _length = token.EndIndex - token.StartIndex;
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

    public char CharAt(int index)
    {
        return _source.GetChartAt(_token.StartIndex + index);
    }


    public ICharSequence SubSequence(int start, int end)
    {
        return _source.GetCharSequence(start + _start, end + _start);
    }


    public override string ToString()
    {
        return _encodeStringByDefault ? _source.ToEncodedStringIfNeeded(_start, _end) : _source.GetString(_start, _end);
    }


    public int Length()
    {
        return _length;
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


    public override bool Equals(object? o)
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
#pragma warning disable CS8603
        return null;
#pragma warning restore CS8603
    }

    public override string ToString()
    {
        return "null";
    }


    public int Length()
    {
        return 4;
    }

    public override bool Equals(object? o)
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
        _value = _source.GetChartAt(_token.StartIndex) == 't';
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


    public int Length()
    {
        return _value ? 4 : 5;
    }


    public override bool Equals(object? o)
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
        return _source.GetInt(_token.StartIndex, _token.EndIndex);
    }


    public long LongValue()
    {
        return _source.GetLong(_token.StartIndex, _token.EndIndex);
    }

    public double DoubleValue()
    {
        return _source.GetDouble(_token.StartIndex, _token.EndIndex);
    }


    public BigInteger BigIntegerValue()
    {
        return _source.GetBigInteger(_token.StartIndex, _token.EndIndex);
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

    public char CharAt(int index)
    {
        if (index > Length()) throw new IndexOutOfRangeException();
        return _source.GetChartAt(_token.StartIndex + index);
    }

    public ICharSequence SubSequence(int start, int end)
    {
        if (end > Length()) throw new IndexOutOfRangeException();
        return _source.GetCharSequence(start + _token.StartIndex, end + _token.StartIndex);
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

    public override string ToString()
    {
        return _source.GetString(RootElementToken().StartIndex, RootElementToken().EndIndex);
    }

    public int Length()
    {
        return _token.EndIndex - _token.StartIndex;
    }

    public float FloatValue()
    {
        return _source.GetFloat(_token.StartIndex, _token.EndIndex);
    }


    public decimal DecimalValue()
    {
        return 1; // TODO fix _source.getBigDecimal(_token.startIndex, _token.endIndex);
    }

    public override bool Equals(object? o)
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
                return _source.IsInteger(_token.StartIndex, _token.EndIndex);
            default:
                return false;
        }
    }

    public bool IsLong()
    {
        switch (_elementType)
        {
            case NodeType.INT:
                return !_source.IsInteger(_token.StartIndex, _token.EndIndex);
            default:
                return false;
        }
    }
}