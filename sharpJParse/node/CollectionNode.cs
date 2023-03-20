namespace sharpJParse;

public interface CollectionNode : Node
{
    
     bool IsScalar() {
        return false;
    }

    bool IsCollection() {
        return true;
    }

    Node GetNode(object key);
     

    List<IList<Token>> ChildrenTokens();

    // ArrayNode AsArray() {
    //     return (ArrayNode ) this;
    // }
    //
    // ObjectNode AsObject() {
    //     return (ObjectNode) this;
    // }
}