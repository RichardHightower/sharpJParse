using sharpJParse.node.support;

namespace sharpJParse.node;

public interface ICollectionNode : INode
{
    bool INode.IsScalar()
    {
        return false;
    }

    bool INode.IsCollection()
    {
        return true;
    }

    INode GetNode(object key);


    List<TokenSubList> ChildrenTokens();

    ArrayNode AsArray()
    {
        return (ArrayNode)this;
    }

    ObjectNode AsObject()
    {
        return (ObjectNode)this;
    }
}