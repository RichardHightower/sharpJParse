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
                return doGetChildrenTokens();
        }
    }

    private List<IList<Token>> doGetChildrenTokens() {
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

    public PathNode getPathNode() {
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

    public String GetString() {
        return getStringNode().toString();
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


    public int HashCode() {
        //return GetNode().HashCode(); TODO fix 
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
