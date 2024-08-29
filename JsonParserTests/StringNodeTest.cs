using sharpJParse.JsonParser.node;
using sharpJParse.JsonParser.parser;
using sharpJParse.JsonParser.parser.indexoverlay;

namespace sharpJParse;

public class StringNodeTest
{
    public IJsonParser JsonParser() {

        IJsonParser build = new JsonFastParser(false);
        return build;
    }


    [Test]
    public void BasicTests()
    {
        IJsonParser parser = JsonParser();

        var stringNode1 = parser.Parse(Json.NiceJson("'hello'")).GetStringNode();
        var stringNode2 = parser.Parse(Json.NiceJson("'hello'")).GetStringNode();

        Assert.That(stringNode2, Is.EqualTo(stringNode1));
        Assert.That(stringNode2.ToString(), Is.EqualTo(stringNode1.ToString()));
        Assert.That(stringNode2.GetHashCode(), Is.EqualTo(stringNode1.GetHashCode()));
        
        Assert.That(stringNode2.Type(), Is.EqualTo(NodeType.STRING));
        
        Assert.That(stringNode2.Tokens().Count, Is.EqualTo(1));
        Assert.That(stringNode2.IsScalar(), Is.EqualTo(true));
        Assert.That(stringNode2.IsCollection(), Is.EqualTo(false));
    }
}