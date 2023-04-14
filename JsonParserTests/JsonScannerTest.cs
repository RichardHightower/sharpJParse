using JsonParser.node.support;
using JsonParser.parser;
using JsonParser.parser.indexoverlay;
using JsonParser.source;
using JsonParser.token;
using static sharpJParse.JsonTestUtils;

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
        Assert.AreEqual(TokenTypes.BOOLEAN_TOKEN, trueToken.Type);
        Assert.AreEqual(0, trueToken.StartIndex);
        Assert.AreEqual(4, trueToken.EndIndex);
        //Assert.AreEqual(5, source.GetIndex());
    }
    
    [Test]
    public void TestSimpleBooleanFalse() {
        IJsonParser parser = JsonParser();
        //...................01234567890123456789
        const string json = "false";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        TokenList tokens = parser.Scan(source);
        Token token = tokens[0];
        Assert.AreEqual(TokenTypes.BOOLEAN_TOKEN, token.Type);
        Assert.AreEqual(0, token.StartIndex);
        Assert.AreEqual(5, token.EndIndex);
        Assert.AreEqual(5, source.GetIndex());
    }
    
    [Test]
    public void TestSimpleNull() {
        IJsonParser parser = JsonParser();
        //...................01234567890123456789
        const string json = "null";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        TokenList tokens = parser.Scan(source);
        Token token = tokens[0];
        Assert.AreEqual(TokenTypes.NULL_TOKEN, token.Type);
        Assert.AreEqual(0, token.StartIndex);
        Assert.AreEqual(4,token.EndIndex);
        Assert.AreEqual(4, source.GetIndex());
    }
    
    [Test]
    public void TestSimpleString() {
        IJsonParser parser = JsonParser();
        //...................01234567890123456789
        const string json = "'abcd'";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        TokenList tokens = parser.Scan(source);
        Token token = tokens[0];
        Assert.AreEqual(TokenTypes.STRING_TOKEN, token.Type);
        Assert.AreEqual(1, token.StartIndex);
        Assert.AreEqual(5,token.EndIndex);
        Assert.AreEqual(6, source.GetIndex());
    }
    
    [Test]
    public void TestSimpleNumber() {
        IJsonParser parser = JsonParser();
        //...................0123
        const string json = "1 ";
        TokenList tokens = parser.Scan(Sources.StringSource(json));
        Assert.AreEqual(1, tokens.Count         );
        Token token = tokens[0];
        Assert.AreEqual(0, token.StartIndex);
        Assert.AreEqual(1, token.EndIndex);
        Assert.AreEqual("1", json.Substring(token.StartIndex, token.EndIndex));
    }

    [Test]
    public void TestSimpleNumberNoSpace() {
        IJsonParser parser = JsonParser();

        const string json = "1";
        TokenList tokens = parser.Scan(Sources.StringSource(json));
        Assert.AreEqual(1, tokens.Count);
        Token token = tokens[0];
        Assert.AreEqual(0, token.StartIndex);
        Assert.AreEqual(1, token.EndIndex);
        Assert.AreEqual("1", json.Substring(token.StartIndex, token.EndIndex));
    }
    
    [Test]
    public void TestSimpleListWithInts()
    {
        
        IJsonParser parser = JsonParser();
        var json = "[ 1 , 3 ]";
        TokenList tokens = parser.Scan(Sources.StringSource(json));

        Assert.AreEqual(TokenTypes.ARRAY_TOKEN, tokens[0].Type);
        Assert.AreEqual(0, tokens[0].StartIndex);
        Assert.AreEqual(9, tokens[0].EndIndex);

        Assert.AreEqual(TokenTypes.INT_TOKEN, tokens[1].Type);
        Assert.AreEqual(2, tokens[1].StartIndex);
        Assert.AreEqual(3, tokens[1].EndIndex);

        Assert.AreEqual(TokenTypes.INT_TOKEN, tokens[2].Type);
        Assert.AreEqual(6, tokens[2].StartIndex);
        Assert.AreEqual(7, tokens[2].EndIndex);
    }
    [Test]
    public void TestSimpleListWithIntsNoSpaces()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("[1,3]");
        TokenList tokens = parser.Scan(Sources.StringSource(json));

        Assert.AreEqual(TokenTypes.ARRAY_TOKEN, tokens[0].Type);
        Assert.AreEqual(0, tokens[0].StartIndex);
        Assert.AreEqual(5, tokens[0].EndIndex);

        Assert.AreEqual(TokenTypes.INT_TOKEN, tokens[1].Type);
        Assert.AreEqual(1, tokens[1].StartIndex);
        Assert.AreEqual(2, tokens[1].EndIndex);

        Assert.AreEqual(TokenTypes.INT_TOKEN, tokens[2].Type);
        Assert.AreEqual(3, tokens[2].StartIndex);
        Assert.AreEqual(4, tokens[2].EndIndex);
    }

    [Test]
    public void TestSimpleList()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("['h','a']");
        TokenList tokens = parser.Scan(Sources.StringSource(json));

        Assert.AreEqual(TokenTypes.ARRAY_TOKEN, tokens[0].Type);
        Assert.AreEqual(0, tokens[0].StartIndex);
        Assert.AreEqual(9, tokens[0].EndIndex);

        Assert.AreEqual(TokenTypes.STRING_TOKEN, tokens[1].Type);
        Assert.AreEqual(2, tokens[1].StartIndex);
        Assert.AreEqual(3, tokens[1].EndIndex);

        Assert.AreEqual(TokenTypes.STRING_TOKEN, tokens[2].Type);
        Assert.AreEqual(6, tokens[2].StartIndex);
        Assert.AreEqual(7, tokens[2].EndIndex);
    }
    
    [Test]
    public void TestStringKey()
    {
        IJsonParser parser = JsonParser();
        var json = Json.NiceJson("{'1':1}");
        TokenList tokens = parser.Scan(Sources.StringSource(json));  
        Assert.AreEqual(TokenTypes.OBJECT_TOKEN, tokens[0].Type);
        Assert.AreEqual(TokenTypes.ATTRIBUTE_KEY_TOKEN, tokens[1].Type);
        Assert.AreEqual(TokenTypes.STRING_TOKEN, tokens[2].Type);
        Assert.AreEqual(TokenTypes.ATTRIBUTE_VALUE_TOKEN, tokens[3].Type);
        Assert.AreEqual(TokenTypes.INT_TOKEN, tokens[4].Type);
    }
    [Test]
    public void Test2ItemIntKeyMap()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("{'1':2,'2':3}");

        TokenList tokens = parser.Scan(Sources.StringSource(json));

        ValidateToken(tokens[0], TokenTypes.OBJECT_TOKEN, 0, 13);
        ValidateToken(tokens[1], TokenTypes.ATTRIBUTE_KEY_TOKEN, 1, 4);
        ValidateToken(tokens[2], TokenTypes.STRING_TOKEN, 2, 3);
        ValidateToken(tokens[3], TokenTypes.ATTRIBUTE_VALUE_TOKEN, 5, 6);
        ValidateToken(tokens[4], TokenTypes.INT_TOKEN, 5, 6);
        ValidateToken(tokens[5], TokenTypes.ATTRIBUTE_KEY_TOKEN, 7, 10);
        ValidateToken(tokens[6], TokenTypes.STRING_TOKEN, 8, 9);
        ValidateToken(tokens[7], TokenTypes.ATTRIBUTE_VALUE_TOKEN, 11, 12);
        ValidateToken(tokens[8], TokenTypes.INT_TOKEN, 11, 12);
    }
    
    [Test]
    public void TestComplexMap()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("{'1':2,'2':7,'abc':[1,2,3]}");

        TokenList tokens = parser.Scan(Sources.StringSource(json));

        ValidateToken(tokens[0], TokenTypes.OBJECT_TOKEN, 0, 27);

        ValidateToken(tokens[1], TokenTypes.ATTRIBUTE_KEY_TOKEN, 1, 4);
        ValidateToken(tokens[2], TokenTypes.STRING_TOKEN, 2, 3);
        ValidateToken(tokens[3], TokenTypes.ATTRIBUTE_VALUE_TOKEN, 5, 6);
        ValidateToken(tokens[4], TokenTypes.INT_TOKEN, 5, 6);

        ValidateToken(tokens[5], TokenTypes.ATTRIBUTE_KEY_TOKEN, 7, 10);
        ValidateToken(tokens[6], TokenTypes.STRING_TOKEN, 8, 9);
        ValidateToken(tokens[7], TokenTypes.ATTRIBUTE_VALUE_TOKEN, 11, 12);
        ValidateToken(tokens[8], TokenTypes.INT_TOKEN, 11, 12);

        ValidateToken(tokens[9], TokenTypes.ATTRIBUTE_KEY_TOKEN, 13, 18);
        ValidateToken(tokens[10], TokenTypes.STRING_TOKEN, 14, 17);

        ValidateToken(tokens[11], TokenTypes.ATTRIBUTE_VALUE_TOKEN, 19, 26);
        ValidateToken(tokens[12], TokenTypes.ARRAY_TOKEN, 19, 26);

        ValidateToken(tokens[13], TokenTypes.INT_TOKEN, 20, 21);
        ValidateToken(tokens[14], TokenTypes.INT_TOKEN, 22, 23);
        ValidateToken(tokens[15], TokenTypes.INT_TOKEN, 24, 25);
    }

    [Test]
    public void TestMapWithList()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("{'abc':[1,2,'3']}");

        TokenList tokens = parser.Scan(Sources.StringSource(json));

        ValidateToken(tokens[0], TokenTypes.OBJECT_TOKEN, 0, 17);

        ValidateToken(tokens[1], TokenTypes.ATTRIBUTE_KEY_TOKEN, 1, 6);
        ValidateToken(tokens[2], TokenTypes.STRING_TOKEN, 2, 5);

        ValidateToken(tokens[3], TokenTypes.ATTRIBUTE_VALUE_TOKEN, 7, 16);
        ValidateToken(tokens[4], TokenTypes.ARRAY_TOKEN, 7, 16);

        ValidateToken(tokens[5], TokenTypes.INT_TOKEN, 8, 9);
        ValidateToken(tokens[6], TokenTypes.INT_TOKEN, 10, 11);
        ValidateToken(tokens[7], TokenTypes.STRING_TOKEN, 13, 14);
    }


}