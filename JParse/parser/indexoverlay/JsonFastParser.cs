using System.Xml;
using JsonParser.node;
using JsonParser.node.support;
using JsonParser.source;
using JsonParser.source.support;
using JsonParser.token;
using sharpJParse.token;

namespace JsonParser.parser.indexoverlay;

public class JsonFastParser : ParseConstants, IJsonParser
{
    
    private bool objectsKeysCanBeEncoded;


    public JsonFastParser(bool objectsKeysCanBeEncoded) {
        this.objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
    }
    
    public TokenList Scan(ICharSource source)
    {
        return Scan(source, new TokenList());
    }

    public RootNode Parse(ICharSource source)
    {
        throw new NotImplementedException();
    }
    private TokenList Scan(ICharSource source, TokenList tokens)
    {
        
        int ch = source.NextSkipWhiteSpace();

        switch (ch) {
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
                throw new UnexpectedCharacterException("Scanning JSON", "Unexpected character", source, (char) ch);

        }

        return tokens;
    }

    private void ParseString(ICharSource source, TokenList tokens)
    {
        throw new NotImplementedException();
    }

    private void ParseNumber(ICharSource source, TokenList tokens)
    {
        throw new NotImplementedException();
    }

    private void ParseNull(ICharSource source, TokenList tokens)
    {
        throw new NotImplementedException();
    }

    private void ParseFalse(ICharSource source, TokenList tokens)
    {
        throw new NotImplementedException();
    }

    private void ParseTrue(ICharSource source, TokenList tokens)
    {
        int start = source.GetIndex();
        int end = source.FindTrueEnd();
        tokens.Add(new Token(start, end, TokenTypes.BOOLEAN_TOKEN));
    }

    private void ParseArray(ICharSource source, TokenList tokens)
    {
        throw new NotImplementedException();
    }

    private void ParseObject(ICharSource source, TokenList tokens)
    {
        throw new NotImplementedException();
    }


}