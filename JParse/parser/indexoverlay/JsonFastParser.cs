using JsonParser.node;
using JsonParser.node.support;
using JsonParser.source;
using JsonParser.source.support;
using JsonParser.token;

namespace JsonParser.parser.indexoverlay;

public class JsonFastParser : ParseConstants, IJsonParser
{
    private bool objectsKeysCanBeEncoded;


    public JsonFastParser(bool objectsKeysCanBeEncoded)
    {
        this.objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
    }

    public TokenList Scan(ICharSource source)
    {
        return Scan(source, new TokenList());
    }

    public RootNode Parse(ICharSource source)
    {
        return new RootNode(Scan(source), source, objectsKeysCanBeEncoded);
    }

    private TokenList Scan(ICharSource source, TokenList tokens)
    {
        var ch = source.NextSkipWhiteSpace();

        switch (ch)
        {
            case ObjectStartToken:
                ParseObject(source, tokens);
                break;

            case ArrayStartToken:
                ParseArray(source, tokens);
                break;

            case TrueBooleanStart:
                ParseTrue(source, tokens);
                break;

            case FalseBooleanStart:
                ParseFalse(source, tokens);
                break;

            case NullStart:
                ParseNull(source, tokens);
                break;

            case StringStartToken:
                ParseString(source, tokens);
                break;

            case Num0:
            case Num1:
            case Num2:
            case Num3:
            case Num4:
            case Num5:
            case Num6:
            case Num7:
            case Num8:
            case Num9:
            case Minus:
            case Plus:
                ParseNumber(source, tokens);
                break;

            default:
                throw new UnexpectedCharacterException("Scanning JSON", "Unexpected character", source, (char)ch);
        }

        return tokens;
    }

    private void ParseString(ICharSource source, TokenList tokens)
    {
        int startIndex = source.GetIndex();
        int endIndex = source.FindEndOfEncodedStringFast();
        tokens.Add(new Token(startIndex + 1, endIndex, TokenTypes.STRING_TOKEN));

    }

    private void ParseNumber(ICharSource source, TokenList tokens)
    {
        int startIndex = source.GetIndex();
        NumberParseResult numberParse = source.FindEndOfNumberFast();
        tokens.Add(new Token(startIndex, numberParse.EndIndex(), numberParse.WasFloat() ? TokenTypes.FLOAT_TOKEN : TokenTypes.INT_TOKEN));

    }

    private void ParseNull(ICharSource source, TokenList tokens)
    {
        var start = source.GetIndex();
        var end = source.FindNullEnd();
        tokens.Add(new Token(start, end, TokenTypes.NULL_TOKEN));
    }

    private void ParseFalse(ICharSource source, TokenList tokens)
    {
        var start = source.GetIndex();
        var end = source.FindFalseEnd();
        tokens.Add(new Token(start, end, TokenTypes.BOOLEAN_TOKEN));
    }

    private void ParseTrue(ICharSource source, TokenList tokens)
    {
        var start = source.GetIndex();
        var end = source.FindTrueEnd();
        tokens.Add(new Token(start, end, TokenTypes.BOOLEAN_TOKEN));
    }

    private void ParseArray(ICharSource source, TokenList tokens)
    {
        var startSourceIndex = source.GetIndex();
        var tokenListIndex = tokens.Count;
        tokens.PlaceHolder();

        bool done = false;
        while (!done)
        {
            done = ParseArrayItem(source, tokens);
        }

        var arrayToken = new Token(startSourceIndex, source.GetIndex(), TokenTypes.ARRAY_TOKEN);
        tokens[tokenListIndex] = arrayToken;
    }

    private bool ParseArrayItem(ICharSource source, TokenList tokens)
    {
        int ch = source.NextSkipWhiteSpace();

        
        for (; ch != Etx; ch =  source.NextSkipWhiteSpace()) { 
            switch (ch)
            {
                case ObjectStartToken:
                    ParseObject(source, tokens);
                    goto forLoop;

                case ArrayStartToken:
                    ParseArray(source, tokens);
                    goto forLoop;

                case TrueBooleanStart:
                    ParseTrue(source, tokens);
                    goto forLoop;

                case FalseBooleanStart:
                    ParseFalse(source, tokens);
                    goto forLoop;

                case NullStart:
                    ParseNull(source, tokens);
                    goto forLoop;

                case StringStartToken:
                    ParseString(source, tokens);
                    goto forLoop;

                case Num0:
                case Num1:
                case Num2:
                case Num3:
                case Num4:
                case Num5:
                case Num6:
                case Num7:
                case Num8:
                case Num9:
                case Minus:
                case Plus:
                    ParseNumber(source, tokens);
                    goto forLoop;

                case ArrayEndToken:
                    source.Next();
                    return true;

                case ArraySep:
                    source.Next();
                    return false;

                default:
                    throw new UnexpectedCharacterException("Parsing Array Item", "Unexpected character", source, (char) ch);

            }

        }
        forLoop:

        if (source.GetCurrentChar() == ArrayEndToken)
        {
            source.Next();
            return true;
        }

        return false;
    }


private bool ParseKey(ICharSource source, TokenList tokens)
{
    int ch = source.NextSkipWhiteSpace();
    int startIndex = source.GetIndex() - 1;
    int tokenListIndex = tokens.Count;
    tokens.PlaceHolder();
    bool found = false;

    switch (ch)
    {
        case StringStartToken:
            int strStartIndex = startIndex + 1;
            int strEndIndex = objectsKeysCanBeEncoded ? source.FindEndOfEncodedString() : source.FindEndString();
            tokens.Add(new Token(strStartIndex + 1, strEndIndex, TokenTypes.STRING_TOKEN));
            found = true;
            break;

        case ObjectEndToken:
            tokens.UndoPlaceHolder();
            return true;

        default:
            throw new UnexpectedCharacterException("Parsing key", "Unexpected character found", source);
    }

    bool done = source.FindObjectEndOrAttributeSep();
    if (!done && found)
    {
        tokens[tokenListIndex] = new Token(startIndex + 1, source.GetIndex(), TokenTypes.ATTRIBUTE_KEY_TOKEN);
    }
    else if (found && done)
    {
        throw new UnexpectedCharacterException("Parsing key", "Not found", source);
    }

    return done;
}

private bool ParseValue(ICharSource source, TokenList tokens)
{
    int ch = source.NextSkipWhiteSpace();
    int startIndex = source.GetIndex();
    int tokenListIndex = tokens.Count;
    tokens.PlaceHolder();

    switch (ch)
    {
        case ObjectStartToken:
            ParseObject(source, tokens);
            break;

        case ArrayStartToken:
            ParseArray(source, tokens);
            break;

        case TrueBooleanStart:
            ParseTrue(source, tokens);
            break;

        case FalseBooleanStart:
            ParseFalse(source, tokens);
            break;

        case NullStart:
            ParseNull(source, tokens);
            break;

        case StringStartToken:
            ParseString(source, tokens);
            break;

        case Num0:
        case Num1:
        case Num2:
        case Num3:
        case Num4:
        case Num5:
        case Num6:
        case Num7:
        case Num8:
        case Num9:
        case Minus:
        case Plus:
            ParseNumber(source, tokens);
            break;

        default:
            throw new UnexpectedCharacterException("Parsing Value", "Unexpected character", source, (char)ch);
    }

    ch = source.SkipWhiteSpace();

    switch (ch)
    {
        case ObjectEndToken:
            if (source.GetIndex() == tokenListIndex)
            {
                throw new UnexpectedCharacterException("Parsing Value", "Key separator before value", source);
            }

            tokens[tokenListIndex] = new Token(startIndex, source.GetIndex(), TokenTypes.ATTRIBUTE_VALUE_TOKEN);
            return true;

        case ObjectAttributeSep:
            if (source.GetIndex() == tokenListIndex)
            {
                throw new UnexpectedCharacterException("Parsing Value", "Key separator before value", source);
            }

            tokens[tokenListIndex] = new Token(startIndex, source.GetIndex(), TokenTypes.ATTRIBUTE_VALUE_TOKEN);
            return false;

        default:
            throw new UnexpectedCharacterException("Parsing Value", "Unexpected character", source, source.GetCurrentChar());
    }
}

private void ParseObject(ICharSource source, TokenList tokens)
{
    int startSourceIndex = source.GetIndex();
    int tokenListIndex = tokens.Count;
    tokens.PlaceHolder();

    bool done = false;
    while (!done)
    {
        done = ParseKey(source, tokens);
        if (!done)
            done = ParseValue(source, tokens);
    }
    source.Next();
    tokens[tokenListIndex] = 
        new Token(startSourceIndex, source.GetIndex(), TokenTypes.OBJECT_TOKEN);
}

}