using sharpJParse.JsonParser.token;

namespace sharpJParse;

public static class JsonTestUtils
{
    public static void ValidateToken(Token token, int type, int start, int end)
    {
        Assert.AreEqual(type, token.Type);
        Assert.AreEqual(start, token.StartIndex);
        Assert.AreEqual(end, token.EndIndex);
    }
}