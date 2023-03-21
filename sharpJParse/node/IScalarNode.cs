using System.Numerics;
using sharpJParse.support;

namespace sharpJParse.node;

public interface IScalarNode : INode
{
    new bool IsScalar()
    {
        return true;
    }

    new bool IsCollection()
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

    CharSequence CharSequenceValue()
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