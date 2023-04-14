using JsonParser.node.support;
using JsonParser.source;
using JsonParser.token;

namespace JsonParser.node;

public abstract class NodeUtils
{
    public static List<TokenSubList> GetChildrenTokens(TokenSubList tokens)
    {
        var root = tokens[0];
        List<TokenSubList> childrenTokens = new(16);

        for (var index = 1; index < tokens.Size(); index++)
        {
            var token = tokens[index];

            if (token.StartIndex > root.EndIndex) break;

            if (token.Type <= TokenTypes.ARRAY_ITEM_TOKEN)
            {
                var childCount = tokens.CountChildren(index, token);
                var endIndex = index + childCount;
                childrenTokens.Add(tokens.SubList(index, endIndex));
                index = endIndex - 1;
            }
            else
            {
                childrenTokens.Add(new TokenSubList(token));
            }
        }

        return childrenTokens;
    }

    public static INode? CreateNode(IList<Token> tokens, ICharSource source, bool objectsKeysCanBeEncoded)
    {
        var nodeType = NodeTypeUtil.TokenTypeToElement(tokens[0].Type);
        var token = tokens[0];
        switch (nodeType)
        {
            case NodeType.ARRAY:
                return new ArrayNode((TokenSubList)tokens, source, objectsKeysCanBeEncoded);
            case NodeType.INT:
                return new NumberNode(token, source, NodeType.INT);
            case NodeType.FLOAT:
                return new NumberNode(token, source, NodeType.FLOAT);
            case NodeType.OBJECT:
                return new ObjectNode((TokenSubList)tokens, source, objectsKeysCanBeEncoded);
            case NodeType.STRING:
                return new StringNode(token, source);
            case NodeType.BOOLEAN:
                return new BooleanNode(token, source);
            case NodeType.NULL:
                return new NullNode(token, source);

            //TODO fix when you add path support
            // case NodeType.PATH_INDEX:
            //     return new IndexPathNode(token, source);
            // case NodeType.PATH_KEY:
            //     return new KeyPathNode(token, source);
            default:
                throw new InvalidOperationException();
        }
    }


    public static INode CreateNodeForObject(TokenSubList theTokens, ICharSource source, bool objectsKeysCanBeEncoded)
    {
        var rootToken = theTokens[1];
        var tokens = theTokens.SubList(1, theTokens.Count());
        var token = tokens[0];
        var nodeType = NodeTypeUtil.TokenTypeToElement(rootToken.Type);

        switch (nodeType)
        {
            case NodeType.ARRAY:
                return new ArrayNode(tokens, source, objectsKeysCanBeEncoded);
            case NodeType.INT:
                return new NumberNode(token, source, NodeType.INT);
            case NodeType.FLOAT:
                return new NumberNode(token, source, NodeType.FLOAT);
            case NodeType.OBJECT:
                return new ObjectNode(tokens, source, objectsKeysCanBeEncoded);
            case NodeType.STRING:
                return new StringNode(token, source);
            case NodeType.BOOLEAN:
                return new BooleanNode(token, source);
            case NodeType.NULL:
                return new NullNode(token, source);
            default:
                throw new InvalidOperationException();
        }
    }
}