using JsonParser.source;

namespace sharpJParse.source;

public class Sources
{
    public static ICharSource StringSource(string json)
    {
        return new CharArrayCharSource(json.ToCharArray());
    }
}