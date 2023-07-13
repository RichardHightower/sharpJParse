using JsonParser.node;
using JsonParser.parser;
using JsonParser.parser.indexoverlay;
using JsonParser.source;


namespace sharpJParse;

public class JsonParserTest
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

        var rootNode = parser.Parse(source);
        Assert.True(rootNode.GetBoolean());
    }
    
    [Test]
    public void TestSimpleBooleanFalse() {
        IJsonParser parser = JsonParser();
        //...................01234567890123456789
        const string json = "false";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        
        var rootNode = parser.Parse(source);
        Assert.False(rootNode.GetBoolean());
    }
    
    [Test]
    public void TestSimpleNull() {
        IJsonParser parser = JsonParser();
        //...................01234567890123456789
        const string json = "null";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        var rootNode = parser.Parse(source);
        Assert.That(rootNode.GetNode().GetType(), Is.EqualTo(typeof(NullNode)));
    }
    
    [Test]
    public void TestSimpleString() {
        IJsonParser parser = JsonParser();
        //...................01234567890123456789
        const string json = "'abcd'";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        var rootNode = parser.Parse(source);
        
        Assert.That(rootNode.GetString(), Is.EqualTo("abcd"));
    }
    
    [Test]
    public void TestSimpleNumber() {
        IJsonParser parser = JsonParser();
        //...................0123
        const string json = "1 ";
        ICharSource source = Sources.StringSource(Json.NiceJson(json));
        var rootNode = parser.Parse(source);
        
        Assert.That(rootNode.GetInt(), Is.EqualTo(1));
    }

    [Test]
    public void TestSimpleNumberNoSpace() {
        IJsonParser parser = JsonParser();

        const string json = "1";
        var rootNode = parser.Parse(json);
        Assert.That(rootNode.GetInt(), Is.EqualTo(1));

    }
    
    [Test]
    public void TestSimpleListWithInts()
    {
        IJsonParser parser = JsonParser();
        var json = "[ 1 , 3 ]";
        var rootNode = parser.Parse(json);
        Assert.That(rootNode.GetArrayNode().GetInt(0), Is.EqualTo(1));
        Assert.That(rootNode.GetArrayNode().GetInt(1), Is.EqualTo(3));
    }
    [Test]
    public void TestSimpleListWithIntsNoSpaces()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("[1,3]");
        var rootNode = parser.Parse(json);
        Assert.That(rootNode.GetArrayNode().GetInt(0), Is.EqualTo(1));
        Assert.That(rootNode.GetArrayNode().GetInt(1), Is.EqualTo(3));
    }

    [Test]
    public void TestSimpleList()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("['h','a']");
        var rootNode = parser.Parse(json);
        Assert.That(rootNode.GetArrayNode().GetString(0), Is.EqualTo("h"));
        Assert.That(rootNode.GetArrayNode().GetString(1), Is.EqualTo("a"));
    }
    
    [Test]
    public void TestStringKey()
    {
        IJsonParser parser = JsonParser();
        var json = Json.NiceJson("{'1':1}");
        var rootNode = parser.Parse(json);
        //TODO Assert.That(rootNode.GetObjectNode().GetInt("1"), Is.EqualTo(1));
    }
    [Test]
    public void Test2ItemIntKeyMap()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("{'1':2,'2':3}");

        var rootNode = parser.Parse(json);
        //TODO Assert.That(rootNode.GetObjectNode().GetInt("1"), Is.EqualTo(1));
    }
    
    [Test]
    public void TestComplexMap()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("{'1':2,'2':7,'abc':[1,2,3]}");

        var rootNode = parser.Parse(json);
        //TODO Assert.That(rootNode.GetObjectNode().GetInt("1"), Is.EqualTo(1));
    }

    [Test]
    public void TestMapWithList()
    {
        IJsonParser parser = JsonParser();
        string json = Json.NiceJson("{'abc':[1,2,'3']}");

        var rootNode = parser.Parse(json);
        //TODO Assert.That(rootNode.GetObjectNode().GetInt("1"), Is.EqualTo(1));
    }


}