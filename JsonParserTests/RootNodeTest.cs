using System.Numerics;
using sharpJParse.JsonParser.node;
using sharpJParse.JsonParser.parser;
using sharpJParse.JsonParser.parser.indexoverlay;
using sharpJParse.JsonParser.source;

namespace sharpJParse;

public class RootNodeTest
{
    public IJsonParser JsonParser() {

        IJsonParser build = new JsonFastParser(false);
        return build;
    }


    [Test]
    public void basicTests() {
        IJsonParser parser = JsonParser();
        Assert.That(parser.Parse(Json.NiceJson("1.1")).GetDouble(), Is.EqualTo(1.1).Within(0.02));
        
        Assert.That(parser.Parse(Json.NiceJson("10000")).GetBigIntegerValue(), Is.EqualTo(BigInteger.Parse("10000")));
        
        Assert.That(parser.Parse(Json.NiceJson("1")).GetLong(), Is.EqualTo(1L));

        Assert.That(parser.Parse(Json.NiceJson("1.1")).GetFloat(), Is.EqualTo(1.1f).Within(0.02f));
        Assert.That(parser.Parse(Json.NiceJson("'hi mom'")).GetString(), Is.EqualTo("hi mom"));
        Assert.That(parser.Parse(Json.NiceJson("{'m':'hi mom'}")).GetObjectNode().GetString("m"), 
            Is.EqualTo("hi mom"));
        Assert.That(parser.Parse(Json.NiceJson("{'m':'hi mom'}")).AsObject().GetString("m"), 
            Is.EqualTo("hi mom"));
        Assert.That(parser.Parse(Json.NiceJson("{'m':'hi mom'}")).GetNode("m").AsScalar().ToString(), 
            Is.EqualTo("hi mom"));
        Assert.That(parser.Parse(Json.NiceJson("['m','hi mom']")).GetNode(1).AsScalar().ToString(), 
            Is.EqualTo("hi mom"));
        Assert.That(parser.Parse(Json.NiceJson("['m','hi mom']")).AsArray().GetString(1), 
            Is.EqualTo("hi mom"));
        
        Assert.That(parser.Parse(Json.NiceJson("null")).GetNullNode().ToString(), 
            Is.EqualTo("null"));
        
        Assert.That(parser.Parse(Json.NiceJson("null")).Type(), 
            Is.EqualTo(NodeType.ROOT));
        
        Assert.That(parser.Parse(Json.NiceJson("null")).RootType(), 
            Is.EqualTo(NodeType.NULL));
        
        Assert.That(parser.Parse(Json.NiceJson("null")).CharSource().ToString(), 
            Is.EqualTo("null"));
        
        Assert.That(parser.Parse(Json.NiceJson("null")).RootElementToken().StartIndex, 
            Is.EqualTo(0));
        
        Assert.That(parser.Parse(Json.NiceJson("null")).Tokens().Count, 
            Is.EqualTo(1));
        
        Assert.That(parser.Parse(Json.NiceJson("null")), 
            Is.EqualTo(parser.Parse(Json.NiceJson("null"))));
        
        Assert.That(parser.Parse(Json.NiceJson("'hi'")).GetHashCode(), 
            Is.EqualTo(parser.Parse(Json.NiceJson("'hi'")).GetHashCode()));

        var childrenTokens = ((ICollectionNode)parser.Parse(Json.NiceJson("{'m':'hi mom'}"))).ChildrenTokens();

        var tokens = parser.Parse(Json.NiceJson("{'m':'hi mom'}")).AsObject().ChildrenTokens();
        

        Assert.That(childrenTokens.Count, 
             Is.EqualTo(tokens.Count));

        var tokenSubList = childrenTokens[0];
        var subList = tokens[0];
        
        Assert.That(tokenSubList[0], 
            Is.EqualTo(subList[0]));
        
        Assert.That(tokenSubList[0].Type, 
            Is.EqualTo(subList[0].Type));
    }
}