namespace sharpJParse.source;

public class UnexpectedCharacterException : Exception
{
    public UnexpectedCharacterException(string whileDoing, string message, ICharSource source, int ch, int index) :
        base(
            $"Unexpected character while {whileDoing}, " +
            $"Error is '{message}'. \n Details \n {source.ErrorDetails(message, index, ch)}")
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
}