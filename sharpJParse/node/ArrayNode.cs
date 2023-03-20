using System.Collections.ObjectModel;
using System.Numerics;
using sharpJParse.support;

namespace sharpJParse;


public class ArrayNode : Collection<Node>, CollectionNode {

    private  TokenSubList _tokens;
    private  CharSource _source;
    private  Token _rootToken;
    private  bool _objectsKeysCanBeEncoded;
    private int _hashCode;
    private List<IList<Token>> _childrenTokens;
    private Node[] _elements;
    private bool _hashCodeSet;

    public ArrayNode( TokenSubList tokens,  CharSource source, bool objectsKeysCanBeEncoded) {
        this._tokens = tokens;
        this._rootToken = tokens[0];
        this._source = source;
        this._objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
    }


    public List<IList<Token>> ChildrenTokens() {
        if (_childrenTokens == null) {
            _childrenTokens = NodeUtils.GetChildrenTokens(_tokens);
        }
        return _childrenTokens;
    }

    Node[] Elements() {
        if (_elements == null) {
            _elements = new Node[ChildrenTokens().Count()];
        }
        return _elements;
    }

    public Node GetNode(Object key) {
        return key is string ?
                this.GetNodeAt(int.Parse ((string)key)) :
                this.GetNodeAt((int) key);
    }

    public Node GetNodeAt(int index) {
        Node element = Elements()[index];
        if (element == null) {
            IList<Token> tokens = ChildrenTokens()[index];
            Elements()[index] = NodeUtils.CreateNode(tokens, _source, _objectsKeysCanBeEncoded);
        }
        return Elements()[index];
    }
    

    public long GetLong(int index) {
        return GetNumberNode(index).longValue();
    }

    public double GetDouble(int index) {
        return GetNumberNode(index).doubleValue();
    }

    public double[] GetDoubleArray() {
        int length = Length();
        double[] array = new double[length];
        for (int i = 0; i < length; i++) {
             Token token = _tokens[i + 1];
            array[i] = double.Parse(_source.GetString(token.startIndex, token.endIndex),  System.Globalization.CultureInfo.InvariantCulture) ;
        }
        return array;
    }

    public float[] GetFloatArray() {
        int length = Length();
        float[] array = new float[length];
        for (int i = 0; i < length; i++) {
             Token token = _tokens.Get(i + 1);
             array[i] = float.Parse(_source.GetString(token.startIndex, token.endIndex),  System.Globalization.CultureInfo.InvariantCulture) ;

        }
        return array;
    }



    public BigInteger[] GetBigIntegerArray() {
        int length = Length();
        BigInteger[] array = new BigInteger[length];
        for (int i = 0; i < length; i++) {
             Token token = _tokens.Get(i + 1);
             
             array[i] = BigInteger.Parse(_source.GetString(token.startIndex, token.endIndex),  System.Globalization.CultureInfo.InvariantCulture) ;
        }
        return array;
    }

    public int[] GetIntArray() {
        int length = Length();
        int[] array = new int[length];
        for (int i = 0; i < length; i++) {
            Token token = _tokens.Get(i + 1);
            array[i] = _source.GetInt(token.startIndex, token.endIndex);
        }
        return array;
    }

    public long[] GetLongArray() {
        int length = Length();
        long[] array = new long[length];
        for (int i = 0; i < length; i++) {
            Token token = _tokens.Get(i + 1);
            array[i] = _source.GetLong(token.startIndex, token.endIndex);
        }
        return array;
    }

    public NullNode GetNullNode(int index) {
        return (NullNode) GetNodeAt(index);
    }

    public int GetInt(int index) {
        return GetNumberNode(index).IntValue();
    }

    public float GetFloat(int index) {
        return GetNumberNode(index).floatValue();
    }

    public NumberNode GetNumberNode(int index) {
        return (NumberNode) GetNodeAt(index);
    }


    public BigInteger GetBigInteger(int i) {
        return GetNumberNode(i).bigIntegerValue();
    }

    public StringNode GetStringNode(int index) {
        return (StringNode) GetNodeAt(index);
    }

    public string GetString(int index) {
        return GetStringNode(index).toString();
    }

    public ObjectNode GetObject(int index) {
        return (ObjectNode) GetNodeAt(index);
    }

    public ArrayNode GetArray(int index) {
        return (ArrayNode) GetNodeAt(index);
    }

    public BooleanNode GetBooleanNode(int index) {
        return (BooleanNode) GetNodeAt(index);
    }

    public bool GetBoolean(int index) {
        return GetBooleanNode(index).booleanValue();
    }

    public int Length() {
        return Elements().Length;
    }

    public NodeType Type() {
        return NodeType.ARRAY;
    }

    public IList<Token> Tokens() {
        return _tokens;
    }

    public Token RootElementToken() {
        return _rootToken;
    }

    public CharSource CharSource()
    {
        throw new NotImplementedException();
    }

    public bool IsScalar()
    {
        return false;
    }

    public bool IsCollection()
    {
        return true;
    }

    public CharSource charSource() {
        return _source;
    }


    public Node Get(int index) {
         Node node = GetNodeAt(index);
        return node.Type() == NodeType.NULL ? null : node;
    }


    public bool Equals(Object o) {
        if (this == o) return true;
        if (!(o is ArrayNode)) return false;

        ArrayNode other = (ArrayNode) o;

        if (this._tokens.Size() != other._tokens.Size()) {
            return false;
        }

        for (int index = 0; index < this._tokens.Size(); index++) {
            Token thisValue = this._tokens[index];
            Token otherValue = other._tokens[index];
            if (otherValue == null && thisValue == null) continue;
            String thisStr = thisValue.AsString(this._source);
            String otherStr = otherValue.AsString(other._source);
            if (!thisStr.Equals(otherStr)) {
                return false;
            }
        }
        return true;
    }


    public int HashCode() {
        if (_hashCodeSet) {
            return _hashCode;
        }
        //TODO _hashCode = _tokens.Map(tok => tok.AsString(this._source)).ToList().HashCode();
        _hashCode = 1;
        _hashCodeSet = true;
        return _hashCode;
    }


    public int Size() {
        return ChildrenTokens().Count;
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

    public int GetLength()
    {
        throw new NotImplementedException();
    }

    public CharSequence SubSequence(int start, int end)
    {
        throw new NotImplementedException();
    }
}