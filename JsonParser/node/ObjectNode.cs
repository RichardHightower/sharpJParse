using System.Numerics;
using sharpJParse.node.support;
using sharpJParse.source;
using sharpJParse.support;
using sharpJParse.token;

#pragma warning disable CS8619

namespace sharpJParse.node;

public class ObjectNode : ICollectionNode
{
    private readonly bool _objectsKeysCanBeEncoded;
    private readonly Token _rootToken;
    private readonly ICharSource _source;


    private readonly TokenSubList _tokens;
    private List<TokenSubList>? _childrenTokens;
    private Dictionary<object, INode?>? _elementMap;
    private int _hashCode;
    private bool _hashCodeSet;
    private List<ICharSequence> _keys = null!;


    public ObjectNode(TokenSubList tokens, ICharSource source, bool objectsKeysCanBeEncoded)
    {
        _tokens = tokens;
        _source = source;
        _rootToken = tokens[0];
        _objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
    }

    public List<TokenSubList> ChildrenTokens()
    {
        return _childrenTokens ??= NodeUtils.GetChildrenTokens(_tokens);
    }

    public INode GetNode(object key)
    {
        return LookupElement((ICharSequence)key);
    }

    public char CharAt(int index)
    {
        throw new NotImplementedException();
    }

    public int Length()
    {
        return ChildrenTokens()!.Count() / 2;
    }

    public NodeType Type()
    {
        return NodeType.OBJECT;
    }


    public IList<Token> Tokens()
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
        return false;
    }

    public bool IsCollection()
    {
        return true;
    }


    public override string ToString()
    {
        return _source.GetString(RootElementToken().startIndex, RootElementToken().endIndex);
    }

    public List<ICharSequence> GetKeys()
    {
        return Keys();
    }


    public INode Get(object key)
    {
        var value = GetNode(key);
        return value is NullNode ? null : value;
    }

    //
    // public Set<Entry<CharSequence, Node>> entrySet() {
    //
    //     return new AbstractSet<Entry<CharSequence, Node>>() {
    //
    //         @Override
    //         public boolean contains(Object o) {
    //             return keys().contains(o);
    //         }
    //
    //         @Override
    //         public Iterator<Entry<CharSequence, Node>> iterator() {
    //             final Iterator<CharSequence> iterator = keys().iterator();
    //             return new Iterator<Entry<CharSequence, Node>>() {
    //                 @Override
    //                 public boolean hasNext() {
    //                     return iterator.hasNext();
    //                 }
    //
    //                 @Override
    //                 public Entry<CharSequence, Node> next() {
    //                     CharSequence nextKey = iterator.next();
    //                     return new Entry<CharSequence, Node>() {
    //                         @Override
    //                         public CharSequence getKey() {
    //                             return nextKey;
    //                         }
    //
    //                         @Override
    //                         public Node getValue() {
    //                             return getObjectNode(nextKey);
    //                         }
    //
    //                         @Override
    //                         public Node setValue(Node value) {
    //                             throw new UnsupportedOperationException();
    //                         }
    //                     };
    //                 }
    //             };
    //         }
    //
    //
    //         public int size() {
    //             return keys().size();
    //         }
    //     };
    //
    // }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (!(o is ObjectNode)) return false;

        var other = (ObjectNode)o;

        if (_tokens.Size() != other._tokens.Size()) return false;


        var keys = Keys();
        var otherKeys = other.Keys();

        if (keys.Count() != otherKeys.Count()) return false;

        foreach (object key in keys)
        {
            var otherElementValue = other.GetNode(key);
            var thisElementValue = GetNode(key);

            if (otherElementValue == null) return false;

            if (!otherElementValue.Equals(thisElementValue)) return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        if (_hashCodeSet) return _hashCode;
        var keys = Keys();
        foreach (object key in keys) GetNode(key);
        _hashCode = _elementMap.GetHashCode();
        _hashCodeSet = true;
        return _hashCode;
    }


    public NumberNode GetNumberNode(ICharSequence key)
    {
        return (NumberNode)GetNode(key);
    }

    public NullNode GetNullNode(ICharSequence key)
    {
        return (NullNode)GetNode(key);
    }

    public long GetLong(ICharSequence key)
    {
        return ((NumberNode)GetNode(key)).LongValue();
    }

    public double GetDouble(ICharSequence key)
    {
        return ((NumberNode)GetNode(key)).DoubleValue();
    }

    public int GetInt(ICharSequence key)
    {
        return ((NumberNode)GetNode(key)).IntValue();
    }

    public float GetFloat(ICharSequence key)
    {
        return ((NumberNode)GetNode(key)).FloatValue();
    }

    public decimal GetDecimal(ICharSequence key)
    {
        return 1; // TODO fix //getNumberNode(key).bigDecimalValue();
    }

    public BigInteger GetBigInteger(ICharSequence key)
    {
        return GetNumberNode(key).BigIntegerValue();
    }

    public StringNode GetStringNode(ICharSequence key)
    {
        return (StringNode)GetNode(key);
    }

    public string GetString(ICharSequence key)
    {
        return GetStringNode(key).ToString();
    }

    public ObjectNode GetObjectNode(ICharSequence key)
    {
        return (ObjectNode)GetNode(key);
    }

    public ArrayNode GetArrayNode(ICharSequence key)
    {
        return (ArrayNode)GetNode(key);
    }

    public BooleanNode GetBooleanNode(ICharSequence key)
    {
        return (BooleanNode)GetNode(key);
    }

    public bool GetBoolean(ICharSequence key)
    {
        return GetBooleanNode(key).BooleanValue();
    }

    // public Optional<Node> GetNode(final Node key) {
    //     List<List<Token>> childrenTokens = childrenTokens();
    //     Node node = null;
    //     for (int index = 0; index < childrenTokens.size(); index += 2) {
    //         List<Token> itemKey = childrenTokens.get(index);
    //         if (keyMatch(itemKey, key)) {
    //             node = NodeUtils.createNodeForObject(childrenTokens.get(index + 1), _source, _objectsKeysCanBeEncoded);
    //             break;
    //         }
    //     }
    //     return Optional.ofNullable(node);
    // }


    private bool KeyMatch(List<Token> tokens, INode key)
    {
        return NodeUtils.CreateNodeForObject(_tokens, _source, _objectsKeysCanBeEncoded).Equals(key);
    }

    private INode? LookupElement(ICharSequence key)
    {
        if (_elementMap == null) _elementMap = new Dictionary<object, INode>();
        var node = _elementMap[key];


        if (node == null)
        {
            var childrenTokens = ChildrenTokens();
            for (var index = 0; index < childrenTokens.Count; index += 2)
            {
                IList<Token> itemKey = childrenTokens[index];

                if (DoesMatchKey(itemKey, key))
                {
                    node = NodeUtils.CreateNodeForObject(childrenTokens[index + 1], _source, _objectsKeysCanBeEncoded);
                    _elementMap[key] = node;
                    break;
                }
            }
        }

        return node;
    }

    private bool DoesMatchKey(IList<Token> itemKey, ICharSequence key)
    {
        var keyToken = itemKey[1];

        if (keyToken.type == TokenTypes.STRING_TOKEN)
        {
            if (keyToken.Length() != key.Length()) return false;
            if (_objectsKeysCanBeEncoded)
            {
                var stringNode = new StringNode(keyToken, _source, _objectsKeysCanBeEncoded);
                var str = stringNode.ToString();
                for (var index = 0; index < key.Length(); index++)
                    if (str[index] != key.CharAt(index))
                        return false;
                return true;
            }

            return _source.MatchChars(keyToken.startIndex, keyToken.endIndex, key);
        }

        return false;
    }


    private List<ICharSequence> Keys()
    {
        if (_keys == null)
        {
            var childrenTokens = ChildrenTokens();
            _keys = new List<ICharSequence>(childrenTokens.Count / 2);
            for (var index = 0; index < childrenTokens.Count; index += 2)
            {
                IList<Token> itemKey = childrenTokens[index];
                var keyToken = itemKey[1];
                switch (keyToken.type)
                {
                    case TokenTypes.STRING_TOKEN:
                        var element = new StringNode(keyToken, _source, _objectsKeysCanBeEncoded);
                        _keys.Add(element);
                        break;
                    default:
                        throw new InvalidOperationException("Only String are allowed for keys " +
                                                            TokenTypes.GetTypeName(keyToken.type));
                }

                ;
            }
        }

        return _keys;
    }
}