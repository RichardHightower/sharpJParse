namespace sharpJParse;

public class CharArrayUtilsTests
{
    

    [Test]
    public void DecodeJsonStringTest() {
        string encodedString = Json.NiceJson("hello `b `n `b `u1234 ");
        string result = CharArrayUtils.DecodeJsonString(encodedString.ToArray(), 0, encodedString.Length);

        int expectedCount = encodedString.Length - 3 - 5;
        
        Assert.AreEqual(expectedCount, result.Length);
        Assert.AreEqual("hello \b \n \b \u1234 ", result);
    }
}