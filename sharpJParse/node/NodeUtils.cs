using sharpJParse.support;

namespace sharpJParse;

public class NodeUtils
{
    
    public static List<IList<Token>> GetChildrenTokens( TokenSubList tokens) {

         Token root = tokens[0];
         List<IList<Token>> childrenTokens;
        childrenTokens = new List<IList<Token>>(16);

        for (int index = 1; index < tokens.Size(); index++) {
            Token token = tokens[index];

            if (token.startIndex > root.endIndex) {
                break;
            }

            if (token.type <= TokenTypes.ARRAY_ITEM_TOKEN) {

                int childCount = tokens.CountChildren(index, token);
                int endIndex = index + childCount;
                childrenTokens.Add(tokens.SubList(index, endIndex));
                index = endIndex - 1;

            } else {
                childrenTokens.Add(new SingletonList<Token>(token) );
            }

        }

        return childrenTokens;
    }

    public static Node CreateNode( IList<Token> tokens,  CharSource source, bool objectsKeysCanBeEncoded) {

         NodeType nodeType = NodeTypeUtil.TokenTypeToElement(tokens[0].type);
         Token token = tokens[0];
        switch (nodeType) {
            case NodeType.ARRAY:
                return new ArrayNode((TokenSubList) tokens, source, objectsKeysCanBeEncoded);
            case NodeType.INT:
                return new NumberNode(token, source, NodeType.INT);
            case NodeType.FLOAT:
                return new NumberNode(token, source, NodeType.FLOAT);
            case NodeType.OBJECT:
                return new ObjectNode((TokenSubList) tokens, source, objectsKeysCanBeEncoded);
            case NodeType.STRING:
                return new StringNode(token, source);
            case NodeType.BOOLEAN:
                return new BooleanNode(token, source);
            case NodeType.NULL:
                return new NullNode(token, source);
            case NodeType.PATH_INDEX:
                return new IndexPathNode(token, source);
            case NodeType.PATH_KEY:
                return new KeyPathNode(token, source);
            default:
                throw new InvalidOperationException();
        }
    }


    public static Node CreateNodeForObject( TokenSubList theTokens,  CharSource source, bool objectsKeysCanBeEncoded) {

         Token rootToken = theTokens[1];
         TokenSubList tokens = theTokens.SubList(1, theTokens.Count());
         Token token = tokens[0];
         NodeType nodeType = NodeTypeUtil.TokenTypeToElement(rootToken.type);

        switch (nodeType) {
            case NodeType.ARRAY:
                return new ArrayNode((TokenSubList) tokens, source, objectsKeysCanBeEncoded);
            case NodeType.INT:
                return new NumberNode(token, source, NodeType.INT);
            case NodeType.FLOAT:
                return new NumberNode(token, source, NodeType.FLOAT);
            case NodeType.OBJECT:
                return new ObjectNode((TokenSubList) tokens, source, objectsKeysCanBeEncoded);
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