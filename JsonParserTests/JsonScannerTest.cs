using JsonParser.node.support;
using JsonParser.parser;
using JsonParser.parser.indexoverlay;
using JsonParser.source;
using JsonParser.token;
using sharpJParse.token;

namespace sharpJParse;

public class JsonScannerTest
{
    
    public IJsonParser JsonParser() {

        IJsonParser build = new JsonFastParser(false);
        return build;
    }


    [Test]
    public void TestSimpleBooleanTrue() {
        IJsonParser parser = JsonParser();
        //...................01234567890123456789
        const string json = "true";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        TokenList tokens = parser.Scan(source);
        Token trueToken = tokens[0];
        Assert.AreEqual(TokenTypes.BOOLEAN_TOKEN, trueToken.type);
        Assert.AreEqual(0, trueToken.startIndex);
        Assert.AreEqual(4, trueToken.endIndex);
        //Assert.AreEqual(5, source.GetIndex());
    }
}