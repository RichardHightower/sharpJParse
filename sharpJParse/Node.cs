namespace sharpJParse;

public interface Node : CharSequence
{
    
    NodeType Type();

    List<Token> Tokens();

    Token RootElementToken();

    CharSource CharSource();

    bool isScalar();
    bool isCollection();
    
    ScalarNode AsScalar() {
        return (ScalarNode) this;
    }

    CollectionNode AsCollection() {
        return (CollectionNode) this;
    }

    int Length() {
        Token token = RootElementToken();
        return token.endIndex - token.startIndex;
    }

    
    char GetCharAt(int index) {
        return CharSource().GetChartAt(RootElementToken().startIndex + index);
    }


    
    CharSequence SubSequence(int start, int end) {
        Token token = RootElementToken();
        return CharSource().GetCharSequence(start + token.startIndex, end + token.startIndex);
    }

    String OriginalString() {
        return CharSource().GetString(RootElementToken().startIndex, RootElementToken().endIndex);
    }

    string ToJsonString() {
        return OriginalString();
    }

    CharSequence OriginalCharSequence() {
        return CharSource().GetCharSequence(RootElementToken().startIndex, RootElementToken().endIndex);
    }

    CharSequence ToJsonCharSequence() {
        return OriginalCharSequence();
    }

    bool EqualsContent(string content) {
        return Equals(content);
    }
}