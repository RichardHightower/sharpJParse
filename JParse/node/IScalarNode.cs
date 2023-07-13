using System.Numerics;
using sharpJParse.JsonParser.support;

namespace sharpJParse.JsonParser.node;

public interface IScalarNode : INode
{
    bool INode.IsScalar()
    {
        return true;
    }

    bool INode.IsCollection()
    {
        return false;
    }


    object Value();

    bool BooleanValue()
    {
        throw new InvalidOperationException();
    }

    int IntValue()
    {
        throw new InvalidOperationException();
    }

    long LongValue()
    {
        throw new InvalidOperationException();
    }

    double DoubleValue()
    {
        throw new InvalidOperationException();
    }


    BigInteger BigIntegerValue()
    {
        throw new InvalidOperationException();
    }

    ICharSequence CharSequenceValue()
    {
        return OriginalCharSequence();
    }

    string StringValue()
    {
        return OriginalString();
    }

    bool EqualsString(string str)
    {
        return Equals(str);
    }
}