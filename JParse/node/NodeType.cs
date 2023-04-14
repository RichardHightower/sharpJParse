using JsonParser.token;

namespace JsonParser.node;

public enum NodeType : short
{
    ROOT = -1,
    OBJECT = TokenTypes.OBJECT_TOKEN,
    ARRAY = TokenTypes.ARRAY_TOKEN,
    INT = TokenTypes.INT_TOKEN,
    FLOAT = TokenTypes.FLOAT_TOKEN,
    STRING = TokenTypes.STRING_TOKEN,
    BOOLEAN = TokenTypes.BOOLEAN_TOKEN,
    NULL = TokenTypes.NULL_TOKEN,
    PATH_KEY = TokenTypes.PATH_KEY_TOKEN,
    PATH_INDEX = TokenTypes.PATH_INDEX_TOKEN,
    OTHER = -2
}

public class NodeTypeUtil
{
    public static NodeType TokenTypeToElement(int tokenType)
    {
        switch (tokenType)
        {
            case TokenTypes.OBJECT_TOKEN:
                return NodeType.OBJECT;
            case TokenTypes.ARRAY_TOKEN:
                return NodeType.ARRAY;
            case TokenTypes.INT_TOKEN:
                return NodeType.INT;
            case TokenTypes.FLOAT_TOKEN:
                return NodeType.FLOAT;
            case TokenTypes.STRING_TOKEN:
                return NodeType.STRING;
            case TokenTypes.BOOLEAN_TOKEN:
                return NodeType.BOOLEAN;
            case TokenTypes.NULL_TOKEN:
                return NodeType.NULL;
            case TokenTypes.PATH_KEY_TOKEN:
                return NodeType.PATH_KEY;
            case TokenTypes.PATH_INDEX_TOKEN:
                return NodeType.PATH_INDEX;
            default:
                throw new InvalidOperationException("" + tokenType);
        }
    }
}