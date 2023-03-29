using sharpJParse.support;

namespace sharpJParse;

#pragma warning disable NUnit2005
public class CharArrayUtilsTests
{
    [Test]
    public void DecodeJsonStringTest()
    {
        var encodedString = Json.NiceJson("hello `b `n `b `u1234 ");
        var result = CharArrayUtils.DecodeJsonString(encodedString.ToArray(), 0, encodedString.Length);

        var expectedCount = encodedString.Length - 3 - 5;

        Assert.AreEqual(expectedCount, result.Length);
        Assert.AreEqual("hello \b \n \b \u1234 ", result);
    }
}
#pragma warning restore NUnit2005