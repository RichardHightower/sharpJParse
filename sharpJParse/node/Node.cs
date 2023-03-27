using sharpJParse.source;
using sharpJParse.support;
using sharpJParse.token;

namespace sharpJParse.node;

public interface INode : ICharSequence
{
    NodeType Type();

    IList<Token> Tokens();

    Token RootElementToken();

    ICharSource CharSource();

    bool IsScalar();
    bool IsCollection();

    IScalarNode AsScalar()
    {
        return (IScalarNode)this;
    }

    ICollectionNode AsCollection()
    {
        return (ICollectionNode)this;
    }

    public int Length()
    {
        var token = RootElementToken();
        return token.endIndex - token.startIndex;
    }


    char GetCharAt(int index)
    {
        return CharSource().GetChartAt(RootElementToken().startIndex + index);
    }


    ICharSequence SubSequence(int start, int end)
    {
        var token = RootElementToken();
        return CharSource().GetCharSequence(start + token.startIndex, end + token.startIndex);
    }

    string OriginalString()
    {
        return CharSource().GetString(RootElementToken().startIndex, RootElementToken().endIndex);
    }

    string ToJsonString()
    {
        return OriginalString();
    }

    ICharSequence OriginalCharSequence()
    {
        return CharSource().GetCharSequence(RootElementToken().startIndex, RootElementToken().endIndex);
    }

    ICharSequence ToJsonCharSequence()
    {
        return OriginalCharSequence();
    }

    bool EqualsContent(string content)
    {
        return Equals(content);
    }
}