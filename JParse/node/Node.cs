using sharpJParse.JsonParser.source;
using sharpJParse.JsonParser.support;
using sharpJParse.JsonParser.token;

namespace sharpJParse.JsonParser.node;

public interface INode : ICharSequence
{
    int ICharSequence.Length
    {
        get
        {
            var token = RootElementToken();
            return token.EndIndex - token.StartIndex;
        }
    }


    char ICharSequence.CharAt(int index)
    {
        return CharSource().GetChartAt(RootElementToken().StartIndex + index);
    }


    ICharSequence ICharSequence.SubSequence(int start, int end)
    {
        var token = RootElementToken();
        return CharSource().GetCharSequence(start + token.StartIndex, end + token.StartIndex);
    }

    string? ICharSequence.ToString()
    {
        return OriginalString();
    }

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

    string OriginalString()
    {
        return CharSource().GetString(RootElementToken().StartIndex, RootElementToken().EndIndex);
    }

    string ToJsonString()
    {
        return OriginalString();
    }

    ICharSequence OriginalCharSequence()
    {
        return CharSource().GetCharSequence(RootElementToken().StartIndex, RootElementToken().EndIndex);
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