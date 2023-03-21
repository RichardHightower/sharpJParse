using sharpJParse.node.support;

namespace sharpJParse.node;

public interface CollectionNode : INode
{
    INode? GetNode(object key);


    List<TokenSubList>? ChildrenTokens();

    ArrayNode AsArray()
    {
        return (ArrayNode)this;
    }

    ObjectNode AsObject()
    {
        return (ObjectNode)this;
    }
}