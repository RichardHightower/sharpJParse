namespace sharpJParse;

public interface CollectionNode
{
    
     bool IsScalar() {
        return false;
    }

    bool IsCollection() {
        return true;
    }

    Node GetNode(object key);
     

    List<List<Token>> ChildrenTokens();

    // ArrayNode AsArray() {
    //     return (ArrayNode ) this;
    // }
    //
    // ObjectNode AsObject() {
    //     return (ObjectNode) this;
    // }
}