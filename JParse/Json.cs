namespace sharpJParse;

public class Json
{
    public static string NiceJson(string json)
    {
        return json.Replace("'", "\"").Replace('`', '\\');
    }
    
}