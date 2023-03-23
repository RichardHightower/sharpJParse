using sharpJParse.parser;

namespace sharpJParse.source;

public class CharArrayCharSourceTest
{
    [Test]
    public void NextTest()
    {
        //...................01234567890123456789
        const string json = "01";
        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        char next = (char) source.Next();
        Assert.AreEqual('0', next);
        Assert.AreEqual(0, source.GetIndex());

        next = (char) source.Next();
        Assert.AreEqual('1', next);
        Assert.AreEqual(1, source.GetIndex());

        next = (char) source.Next();
        Assert.AreEqual(ParseConstants.Etx, next);
        Assert.AreEqual(json.Length, source.GetIndex());

    }
    
    /* Basic test to test nextSkipWhiteSpace and getIndex */
    [Test]
    public void NextSkipWhiteSpace2() {

        //...................01234567890123456789
        const String json = "\t\n01";

        ICharSource source =
            Sources.StringSource(Json.NiceJson(json));

        char next = (char) source.NextSkipWhiteSpace();
        Assert.AreEqual('0', next);
        Assert.AreEqual(2, source.GetIndex());

        next = (char) source.NextSkipWhiteSpace();
        Assert.AreEqual('1', next);
        Assert.AreEqual(3, source.GetIndex());

        next = (char) source.NextSkipWhiteSpace();
        Assert.AreEqual(ParseConstants.Etx, next);
        Assert.AreEqual(4, source.GetIndex());

    }
}