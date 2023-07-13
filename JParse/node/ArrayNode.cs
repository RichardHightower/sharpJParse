using System.Collections.ObjectModel;
using System.Globalization;
using System.Numerics;
using sharpJParse.JsonParser.node.support;
using sharpJParse.JsonParser.source;
using sharpJParse.JsonParser.support;
using sharpJParse.JsonParser.token;

namespace sharpJParse.JsonParser.node;

public class ArrayNode : Collection<INode>, ICollectionNode
{
    private readonly bool _objectsKeysCanBeEncoded;
    private readonly Token _rootToken;
    private readonly ICharSource _source;

    private readonly TokenSubList _tokens;
    private List<TokenSubList>? _childrenTokens;
    private INode?[]? _elements;
    private int _hashCode;
    private bool _hashCodeSet;
    private int _length;

    public ArrayNode(TokenSubList tokens, ICharSource source, bool objectsKeysCanBeEncoded)
    {
        _tokens = tokens;
        _rootToken = tokens[0];
        _source = source;
        _objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
    }


    public List<TokenSubList> ChildrenTokens()
    {
        if (_childrenTokens == null) _childrenTokens = NodeUtils.GetChildrenTokens(_tokens);
        return _childrenTokens ?? throw new InvalidOperationException();
    }

    public INode GetNode(object key)
    {
        return key is string ? GetNodeAt(int.Parse((string)key)) : GetNodeAt((int)key);
    }

    public char CharAt(int index)
    {
        throw new NotImplementedException();
    }

    int ICharSequence.Length => Elements().Length;

    public NodeType Type()
    {
        return NodeType.ARRAY;
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

    public ICharSequence SubSequence(int start, int end)
    {
        throw new NotImplementedException();
    }

    public int Length()
    {
        return Elements().Length;
    }
    //
    // public string ToString() {
    //     return this.OriginalString();
    // }
    //
    //
    // public <R> List<R> mapObjectNode(Function<ObjectNode, ? extends R> mapper) {
    //     return map(node -> mapper.apply(node.asCollection().asObject()));
    // }
    //
    // public <R> List<R> map(Function<Node, ? extends R> mapper) {
    //     List<R> list = new ArrayList<>(this.size());
    //     Node[] elements = elements();
    //     for (int i = 0; i < elements.length; i++) {
    //         Node element = elements[i];
    //         if (element == null) {
    //             element = getNodeAt(i);
    //             elements[i] = element;
    //         }
    //         list.add(mapper.apply(element));
    //     }
    //     return list;
    // }
    //
    // public Optional<ObjectNode> findObjectNode(Predicate<ObjectNode> predicate) {
    //     final Node[] elements = elements();
    //     ObjectNode node = null;
    //     for (int i = 0; i < elements.length; i++) {
    //         Node element = elements[i];
    //         if (element == null) {
    //             element = getNodeAt(i);
    //         }
    //         if (element.type() == NodeType.OBJECT) {
    //             ObjectNode objectNode = element.asCollection().asObject();
    //             if (predicate.test(objectNode)) {
    //                 node = objectNode;
    //                 break;
    //             }
    //         }
    //     }
    //     return Optional.ofNullable(node);
    // }
    //
    // public Optional<Node> find(Predicate<Node> predicate) {
    //     Node[] elements = elements();
    //     Node node = null;
    //     for (int i = 0; i < elements.length; i++) {
    //         Node element = elements[i];
    //         if (element == null) {
    //             element = getNodeAt(i);
    //         }
    //         if (predicate.test(element)) {
    //             node = element;
    //             break;
    //         }
    //     }
    //     return Optional.ofNullable(node);
    // }
    //
    // public List<ObjectNode> filterObjects(Predicate<ObjectNode> predicate) {
    //     Node[] elements = elements();
    //     final int length = elements.length;
    //     final List<ObjectNode> arrayList = new ArrayList<>(length / 2);
    //     for (int i = 0; i < length; i++) {
    //         Node element = elements[i];
    //         if (element == null) {
    //             element = getNodeAt(i);
    //         }
    //         if (element.type() == NodeType.OBJECT) {
    //             ObjectNode objectNode = element.asCollection().asObject();
    //             if (predicate.test(objectNode)) {
    //                 arrayList.add(objectNode);
    //             }
    //         }
    //     }
    //     return arrayList;
    // }
    //
    // public List<Node> filter(Predicate<Node> predicate) {
    //     Node[] elements = elements();
    //     final int length = elements.length;
    //     final List<Node> arrayList = new ArrayList<>(length / 2);
    //     for (int i = 0; i < length; i++) {
    //         Node element = elements[i];
    //         if (element == null) {
    //             element = getNodeAt(i);
    //         }
    //         if (predicate.test(element)) {
    //             arrayList.add(element);
    //         }
    //     }
    //     return arrayList;
    // }


    public char GetCharAt(int index)
    {
        throw new NotImplementedException();
    }

    private INode?[]? Elements()
    {
        if (_elements == null) _elements = new INode[ChildrenTokens().Count()];
        return _elements;
    }

    public INode GetNodeAt(int index)
    {
        var element = Elements()[index];
        if (element == null)
        {
            IList<Token> tokens = ChildrenTokens()[index];
            Elements()[index] = NodeUtils.CreateNode(tokens, _source, _objectsKeysCanBeEncoded);
        }

        return Elements()[index];
    }


    public long GetLong(int index)
    {
        return GetNumberNode(index).LongValue();
    }

    public double GetDouble(int index)
    {
        return GetNumberNode(index).DoubleValue();
    }

    public double[] GetDoubleArray()
    {
        var length = Length();
        var array = new double[length];
        for (var i = 0; i < length; i++)
        {
            var token = _tokens[i + 1];
            array[i] = double.Parse(_source.GetString(token.StartIndex, token.EndIndex), CultureInfo.InvariantCulture);
        }

        return array;
    }

    public float[] GetFloatArray()
    {
        var length = Length();
        var array = new float[length];
        for (var i = 0; i < length; i++)
        {
            var token = _tokens.Get(i + 1);
            array[i] = float.Parse(_source.GetString(token.StartIndex, token.EndIndex), CultureInfo.InvariantCulture);
        }

        return array;
    }


    public BigInteger[] GetBigIntegerArray()
    {
        var length = Length();
        var array = new BigInteger[length];
        for (var i = 0; i < length; i++)
        {
            var token = _tokens.Get(i + 1);

            array[i] = BigInteger.Parse(_source.GetString(token.StartIndex, token.EndIndex),
                CultureInfo.InvariantCulture);
        }

        return array;
    }

    public int[] GetIntArray()
    {
        var length = Length();
        var array = new int[length];
        for (var i = 0; i < length; i++)
        {
            var token = _tokens.Get(i + 1);
            array[i] = _source.GetInt(token.StartIndex, token.EndIndex);
        }

        return array;
    }

    public long[] GetLongArray()
    {
        var length = Length();
        var array = new long[length];
        for (var i = 0; i < length; i++)
        {
            var token = _tokens.Get(i + 1);
            array[i] = _source.GetLong(token.StartIndex, token.EndIndex);
        }

        return array;
    }

    public NullNode GetNullNode(int index)
    {
        return (NullNode)GetNodeAt(index);
    }

    public int GetInt(int index)
    {
        return GetNumberNode(index).IntValue();
    }

    public float GetFloat(int index)
    {
        return GetNumberNode(index).FloatValue();
    }

    public NumberNode GetNumberNode(int index)
    {
        return (NumberNode)GetNodeAt(index);
    }


    public BigInteger GetBigInteger(int i)
    {
        return GetNumberNode(i).BigIntegerValue();
    }

    public StringNode GetStringNode(int index)
    {
        return (StringNode)GetNodeAt(index);
    }

    public string GetString(int index)
    {
        return GetStringNode(index).ToString();
    }

    public ObjectNode GetObject(int index)
    {
        return (ObjectNode)GetNodeAt(index);
    }

    public ArrayNode GetArray(int index)
    {
        return (ArrayNode)GetNodeAt(index);
    }

    public BooleanNode GetBooleanNode(int index)
    {
        return (BooleanNode)GetNodeAt(index);
    }

    public bool GetBoolean(int index)
    {
        return GetBooleanNode(index).BooleanValue();
    }


    public INode Get(int index)
    {
        var node = GetNodeAt(index);
#pragma warning disable CS8603
        return node.Type() == NodeType.NULL ? null : node;
#pragma warning restore CS8603
    }


    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (!(o is ArrayNode)) return false;

        var other = (ArrayNode)o;

        if (_tokens.Size() != other._tokens.Size()) return false;

        for (var index = 0; index < _tokens.Size(); index++)
        {
            var thisValue = _tokens[index];
            var otherValue = other._tokens[index];
            var thisStr = thisValue.AsString(_source);
            var otherStr = otherValue.AsString(other._source);
            if (!thisStr.Equals(otherStr)) return false;
        }

        return true;
    }


    public override int GetHashCode()
    {
        if (_hashCodeSet) return _hashCode;
        //TODO _hashCode = _tokens.Map(tok => tok.AsString(this._source)).ToList().GetHashCode();
        _hashCode = 1;
        _hashCodeSet = true;
        return _hashCode;
    }


    public int Size()
    {
        return ChildrenTokens().Count;
    }

    public int GetLength()
    {
        throw new NotImplementedException();
    }
}