using sharpJParse.node.support;

namespace sharpJParse.node;

public interface ICollectionNode : INode
{
    INode? GetNode(object key);


    List<TokenSubList> ChildrenTokens();

    ArrayNode AsArray()
    {
        return (ArrayNode)this;
    }

    ObjectNode AsObject()
    {
        return (ObjectNode)this;
    }

    bool INode.IsScalar() => false;

    bool INode.IsCollection() => true;
}