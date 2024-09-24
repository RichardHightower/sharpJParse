using System.Numerics;
using sharpJParse.JsonParser.parser;
using sharpJParse.JsonParser.source;

namespace sharpJParse.Tests;

[TestFixture]
public class CharArrayOffsetCharSourceTests
{
    [Test]
    public void Next_BasicTest()
    {
        const string json = "     01 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        var next = (char)source.Next();
        Assert.AreEqual('0', next);
        Assert.AreEqual(0, source.GetIndex());

        next = (char)source.Next();
        Assert.AreEqual('1', next);
        Assert.AreEqual(1, source.GetIndex());

        next = (char)source.Next();
        Assert.AreEqual(ParseConstants.Etx, next);
        Assert.AreEqual(2, source.GetIndex());
    }

    [Test]
    public void NextSkipWhiteSpace_BasicTest()
    {
        const string json = "     \t\n01 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        var next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual('0', next);
        Assert.AreEqual(2, source.GetIndex());

        next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual('1', next);
        Assert.AreEqual(3, source.GetIndex());

        next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual(ParseConstants.Etx, next);
        Assert.AreEqual(4, source.GetIndex());
    }

    [Test]
    public void SkipWhiteSpace_BasicTest()
    {
        const string json = "     \t\n01 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        source.SkipWhiteSpace();

        var next = source.GetCurrentChar();
        Assert.AreEqual('0', next);
        Assert.AreEqual(2, source.GetIndex());

        next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual('1', next);
        Assert.AreEqual(3, source.GetIndex());
        Assert.AreEqual('1', source.GetCurrentChar());
        Assert.AreEqual('1', source.GetCurrentCharSafe());

        next = (char)source.NextSkipWhiteSpace();
        Assert.AreEqual(ParseConstants.Etx, next);
        Assert.AreEqual(4, source.GetIndex());

        Assert.AreEqual(ParseConstants.Etx, source.GetCurrentCharSafe());
        Assert.AreEqual(' ', source.GetCurrentChar());
    }

    [Test]
    public void GetChartAt_Test()
    {
        const string json = "     01 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        Assert.AreEqual('0', source.GetChartAt(0));
        Assert.AreEqual('1', source.GetChartAt(1));
        Assert.AreEqual(' ', source.GetChartAt(2));
    }

    [Test]
    public void ToString_Test()
    {
        const string json = "     01 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        Assert.AreEqual("01", source.ToString());
    }

    [Test]
    public void FindEndOfNumberFast_Test()
    {
        const string json = "     01 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        var result = source.FindEndOfNumberFast();
        Assert.AreEqual(2, result.EndIndex());
        Assert.IsFalse(result.WasFloat());
    }

    [Test]
    public void FindEndOfNumberFastFloat_Test()
    {
        const string json = "     0.2 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        var result = source.FindEndOfNumberFast();
        Assert.AreEqual(3, result.EndIndex());
        Assert.IsTrue(result.WasFloat());
    }

    // Add more tests following the same pattern...

    [Test]
    public void ParseDoubleSimple_Test()
    {
        const string json = "      1.2 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        Assert.AreEqual(1.2, source.GetDouble(1, 4), 0.000001);
    }

    [Test]
    public void ParseIntSimple_Test()
    {
        const string json = "      100 ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        Assert.AreEqual("100", source.GetString(1, 4));
        Assert.AreEqual(100, source.GetInt(1, 4));
    }

    // Continue adding tests for other methods...

    [Test]
    public void FindCommaOrEnd_Test()
    {
        const string json = "       ,2] ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        Assert.IsFalse(source.FindCommaOrEndForArray());
        Assert.AreEqual(2, source.GetIndex());
    }

    [Test]
    public void FindObjectEndOrAttributeSep_Test()
    {
        const string json = "       : 2 } ";
        var source = new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        Assert.IsFalse(source.FindObjectEndOrAttributeSep());
        Assert.AreEqual(2, source.GetIndex());
    }

    [Test]
    public void CheckForJunk_Test()
    {
        // .......................012345
        //...................01234567890123456789
        const string json = "      } ";
        var source =
            new CharArrayOffsetCharSource(5, json.Length - 1, json.ToCharArray());

        try
        {
            source.CheckForJunk();
            Assert.Fail();
        }
        catch (Exception ex)
        {
        }
    }

    [Test]
    public void Encoding()
    {
        //.................0123456789
        var json = "'`u0003'";
        var source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        var end = source.FindEndOfEncodedString();
        Assert.AreEqual(7, end);
        Assert.AreEqual(8, source.GetIndex());
    }

    [Test]
    public void Encoding2()
    {
        //.................0123456789012
        var json = "'abc`n`u0003'";
        var source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        var end = source.FindEndOfEncodedString();
        Assert.AreEqual(12, end);
        Assert.AreEqual(13, source.GetIndex());
    }

    [Test]
    public void Encoding3()
    {
        //.................0123456789
        var json = "'abc`n`b`r`u1234'";
        var source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        var end = source.FindEndOfEncodedString();
        Assert.AreEqual(16, end);
    }

    [Test]
    public void NextSkipWhiteSpaceSample()
    {
        //.....................01234
        var str = "[ 1 , 3 ]";
        var charSource = Sources.StringSource(str);

        Assert.AreEqual('[', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('1', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(',', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('3', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(']', (char)charSource.NextSkipWhiteSpace());
    }

    [Test]
    public void TestSimpleObjectTwoItemsWeirdSpacing()
    {
        //.................. 0123456789012345678901234567890123456789012345
        const string json = "   {'h':   'a',\n\t 'i':'b'\n\t } \n\t    \n";

        var charSource = Sources.StringSource(Json.NiceJson(json));
        Assert.AreEqual('{', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('h', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(':', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('a', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(',', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('i', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual(':', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('b', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('"', (char)charSource.NextSkipWhiteSpace());
        Assert.AreEqual('}', (char)charSource.NextSkipWhiteSpace());
    }

    [Test]
    public void NextSkipWhiteSpace()
    {
        //..................01234
        var str = "   12";
        var charSource = Sources.StringSource(str);

        var ch = (char)charSource.NextSkipWhiteSpace();

        Assert.AreEqual('1', ch);
        Assert.AreEqual('1', charSource.GetCurrentChar());
        Assert.AreEqual(3, charSource.GetIndex());
        Assert.AreEqual('2', charSource.Next());
        Assert.AreEqual(4, charSource.GetIndex());
    }

    [Test]
    public void ParseDouble()
    {
        var s = "1.0e9";
        var charSource = Sources.StringSource(s);
        Assert.AreEqual(1000000000.0, charSource.GetDouble(0, s.Length));

        s = "1e9";
        charSource = Sources.StringSource(s);
        Assert.AreEqual(1000000000.0, charSource.GetDouble(0, s.Length));

        s = "1.1";
        charSource = Sources.StringSource(s);
        Assert.AreEqual(1.1, charSource.GetDouble(0, s.Length));

        s = "1.0e12";
        charSource = Sources.StringSource(s);
        Assert.AreEqual(1e12, charSource.GetDouble(0, s.Length));
    }

    [Test]
    public void IsIntegerButLong()
    {
        var s = "" + long.MaxValue;
        var charSource = Sources.StringSource(s);
        Assert.IsFalse(charSource.IsInteger(0, s.Length));

        var value = int.MaxValue + 1L;
        s = "" + value;
        charSource = Sources.StringSource(s);
        Assert.IsFalse(charSource.IsInteger(0, s.Length));

        value = int.MaxValue;
        s = "" + value;
        charSource = Sources.StringSource(s);
        Assert.IsTrue(charSource.IsInteger(0, s.Length));

        value = int.MinValue;
        s = "" + value;
        charSource = Sources.StringSource(s);
        Assert.IsTrue(charSource.IsInteger(0, s.Length));

        value = int.MinValue - 1L;
        s = "" + value;
        charSource = Sources.StringSource(s);
        Assert.IsFalse(charSource.IsInteger(0, s.Length));
    }

    [Test]
    public void ParseFloatSimple()
    {
        // .......................012345
        //...................01234567890123456789
        const string json = "1.2";
        var charSource = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(1.2f, charSource.GetFloat(0, 3), 0.000001f);
    }

    [Test]
    public void ParseFloatExp()
    {
        // .......................012345
        //...................01234567890123456789
        const string json = "1.2e12";
        var charSource = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(1.2e12f, charSource.GetFloat(0, 6), 0.0001e12f);
    }

    [Test]
    public void GetBigInt()
    {
        // .......................012345
        //...................01234567890123456789
        const string json = "100";
        var source = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual("100", source.GetString(0, 3));
        Assert.AreEqual(new BigInteger(100), source.GetBigInteger(0, 3));
    }

    [Test]
    public void GetEncodedString()
    {
        // ....................................012345678901
        //................................01234567890123456789
        var json = Json.NiceJson("`b`n`r`t ");
        var source = Sources.StringSource(Json.NiceJson(json));

        Assert.AreEqual(Json.NiceJson("`b`n`r`t"), source.GetString(0, 8));
        Assert.AreEqual("\b\n\r\t", source.GetEncodedString(0, 8));
    }

    [Test]
    public void FindEndOfNumberFastFloatExponentInArray()
    {
        //...................01234567890123456789
        const string json = "0.2e12] ";
        var source = Sources.StringSource(Json.NiceJson(json));

        var result = source.FindEndOfNumberFast();
        Assert.AreEqual(6, result.EndIndex());
        Assert.IsTrue(result.WasFloat());
    }

    [Test]
    public void FindEndOfNumberFastFloatExponentInArray2()
    {
        //...................01234567890123456789
        const string json = "0.2e12] ";
        var source = Sources.StringSource(Json.NiceJson(json));

        var result = source.FindEndOfNumberFast();
        Assert.AreEqual(6, result.EndIndex());
        Assert.IsTrue(result.WasFloat());
    }

    [Test]
    public void ReadNumberWithNothingBeforeDecimal()
    {
        //.................0123456789
        const string json = "-.123";
        var source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        try
        {
            source.FindEndOfNumber();
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("" + ex);
            Assert.Pass("Expected exception was thrown.");
        }
    }

    [Test]
    public void ReadNumberNormalNegativeFloat()
    {
        //.................0123456789
        const string json = "-0.123";
        var source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        var result = source.FindEndOfNumber();
        Assert.IsTrue(result.WasFloat());
        Assert.AreEqual(6, result.EndIndex());
    }

    [Test]
    public void EncodingFast()
    {
        //.................0123456789
        var json = "'`u0003'";
        var source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        var end = source.FindEndOfEncodedStringFast();
        Assert.AreEqual(7, end);
        Assert.AreEqual(8, source.GetIndex());
    }

    [Test]
    public void Encoding2Fast()
    {
        //.................0123456789012
        var json = "'abc`n`u0003'";
        var source = Sources.StringSource(Json.NiceJson(json));
        source.Next();
        var end = source.FindEndOfEncodedStringFast();
        Assert.AreEqual(12, end);
        Assert.AreEqual(13, source.GetIndex());
    }
}