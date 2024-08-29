using NUnit.Framework;
using sharpJParse.JsonParser.source.support;

namespace sharpJParse.support
{
    [TestFixture]
    public class ParseFloatTest
    {
        [Test]
        public void TestParseFloat()
        {
            // Test normal case
            char[] input1 = { '1', '.', '2', '3', 'e', '4' };
            float result1 = ParseFloatSupport.ParseFloat(input1, 0, input1.Length);
            Assert.AreEqual(1.23e4f, result1, 0.001f);

            // Test negative number
            char[] input2 = { '-', '5', '.', '6', '7', 'e', '-', '4' };
            float result2 = ParseFloatSupport.ParseFloat(input2, 0, input2.Length);
            Assert.AreEqual(-5.67e-4f, result2, 0.001f);

            // Test zero
            char[] input3 = { '0' };
            float result3 = ParseFloatSupport.ParseFloat(input3, 0, input3.Length);
            Assert.AreEqual(0f, result3, 0.001f);

            // Test integer
            char[] input4 = { '4', '2' };
            float result4 = ParseFloatSupport.ParseFloat(input4, 0, input4.Length);
            Assert.AreEqual(42f, result4, 0.001f);
        }

        [Test]
        public void TestParseFloatWithIntegerPartOnly()
        {
            char[] chars = { '1', '2', '3', '4', '5' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithFractionalPartOnly()
        {
            char[] chars = { '.', '1', '2', '3', '4', '5' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithExponentOnly()
        {
            char[] chars = { '1', 'e', '2' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithPositiveExponent()
        {
            char[] chars = { '1', '.', '2', 'e', '2' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithNegativeExponent()
        {
            char[] chars = { '1', '.', '2', 'e', '-', '2' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithNegativeSign()
        {
            char[] chars = { '-', '1', '.', '2', 'e', '2' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithNegativeExponentAndFractionalPart()
        {
            char[] chars = { '-', '1', '.', '2', '3', 'e', '-', '2' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithPositiveExponentAndFractionalPart()
        {
            char[] chars = { '1', '.', '2', '3', 'e', '2' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithMultipleExponents()
        {
            Assert.Throws<UnexpectedCharacterException>(() =>
            {
                char[] chars = { '1', 'e', '2', 'e', '3' };
                float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            });
        }

        [Test]
        public void TestParseFloatWithMultipleFractions()
        {
            Assert.Throws<UnexpectedCharacterException>(() =>
            {
                char[] chars = { '1', '.', '2', '.', '3', '.', '4', '.', '5' };
                float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            });
        }

        [Test]
        public void TestParseFloatWithLeadingZeros()
        {
            char[] chars = { '0', '0', '1', '.', '2', '3' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestParseFloatWithTrailingZeros()
        {
            char[] chars = { '1', '.', '2', '3', '0', '0', '0', '0' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(new string(chars)), result, 0.001f);
        }

        [Test]
        public void TestNormalCase()
        {
            char[] chars = { '1', '.', '2', '3', 'e', '4' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(1.23e4f, result, 0.001f);
        }

        [Test]
        public void TestPositiveExponent()
        {
            char[] chars = { '1', '0', 'e', '3' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(1e4f, result, 0.001f);
        }

        [Test]
        public void TestNegativeExponent()
        {
            char[] chars = { '1', 'e', '-', '2' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(0.01f, result, 0.001f);
        }

        [Test]
        public void TestExponentZero()
        {
            char[] chars = { '1', 'e', '0' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(1f, result, 0.001f);
        }

        [Test]
        public void TestLargePositiveExponent()
        {
            char[] chars = { '1', '0', 'e', '9' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(1e10f, result, 0.001f);
        }

        [Test]
        public void TestLargeNegativeExponent()
        {
            char[] chars = { '1', 'e', '-', '9' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(0.0001f, result, 0.001f);
        }

        [Test]
        public void TestExponent18()
        {
            char[] chars = { '1', '.', '0', 'e', '1', '8' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(1e18f, result, 0.001f);
        }

        [Test]
        public void TestExponentNegative18()
        {
            char[] chars = { '1', '.', '0', 'e', '-', '1', '8' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(1e-18f, result, 0.001f);
        }

        [Test]
        public void TestExponent19()
        {
            char[] chars = { '1', '.', '0', 'e', '1', '9' };
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(1e19f, result, 0.001f);
        }

        [Test]
        public void TestExponentCrazy()
        {
            const string str = "-9087.123456e-40";
            char[] chars = str.ToCharArray();
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(str), result, 0.00001f);
        }

        [Test]
        public void TestExponentCrazy2()
        {
            const string str = "9087.123456e40";
            char[] chars = str.ToCharArray();
            float result = ParseFloatSupport.ParseFloat(chars, 0, chars.Length);
            Assert.AreEqual(float.Parse(str), result, 0.00001f);
        }
    }
}
