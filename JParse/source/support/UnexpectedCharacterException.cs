namespace sharpJParse.JsonParser.source.support;

public class UnexpectedCharacterException : Exception
{
    
    public UnexpectedCharacterException(string whileDoing, string message, ICharSource? source, int ch, int index)
        : base(FormatMessage(whileDoing, message, source, ch, index))
    {
    }

    public UnexpectedCharacterException(string whileDoing, string message, ICharSource source, int ch)
        : this(whileDoing, message, source, ch, source.GetIndex())
    {
    }

    public UnexpectedCharacterException(string whileDoing, string message, ICharSource source)
        : this(whileDoing, message, source, source.GetCurrentCharSafe(), source.GetIndex())
    {
    }
    


    private static string FormatMessage(string whileDoing, string message, ICharSource? source, int ch, int index)
    {
        string details = source != null ? source.ErrorDetails(message, index, ch) : $"Character: {(char)ch}, Index: {index}";
        return $"Unexpected character while {whileDoing}. Error: {message}. Details: {details}";
    }
}