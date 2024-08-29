
using sharpJParse.JsonParser.source.support;

namespace sharpJParse.support;

[TestFixture]
public class ParseDoubleTest
{
     [Test]
        public void TestParseDouble()
        {
            char[] input1 = "123.456".ToCharArray();
            double result1 = ParseDoubleSupport.ParseDouble(input1, 0, input1.Length);
            Assert.AreEqual(123.456, result1, 1e-6);

            char[] input2 = "-0.5".ToCharArray();
            double result2 = ParseDoubleSupport.ParseDouble(input2, 0, input2.Length);
            Assert.AreEqual(-0.5, result2, 1e-6);

            char[] input3 = "1.23e4".ToCharArray();
            double result3 = ParseDoubleSupport.ParseDouble(input3, 0, input3.Length);
            Assert.AreEqual(1.23e4, result3, 1e-6);

            char[] input4 = "-1.23e-4".ToCharArray();
            double result4 = ParseDoubleSupport.ParseDouble(input4, 0, input4.Length);
            Assert.AreEqual(-1.23e-4, result4, 1e-10);
        }

        [Test]
        public void TestParseDoubleInvalidInput()
        {
            char[] input1 = "123a.456".ToCharArray();
            Assert.Throws<UnexpectedCharacterException>(() => ParseDoubleSupport.ParseDouble(input1, 0, input1.Length));

            char[] input2 = "123..456".ToCharArray();
            Assert.Throws<UnexpectedCharacterException>(() => ParseDoubleSupport.ParseDouble(input2, 0, input2.Length));

            char[] input3 = "1.23e4.5".ToCharArray();
            Assert.Throws<UnexpectedCharacterException>(() => ParseDoubleSupport.ParseDouble(input3, 0, input3.Length));
        }

        [Test]
        public void TestParseDoubleWithIntegerPartOnly()
        {
            char[] chars = { '1', '2', '3', '4', '5' };
            double d = ParseDoubleSupport.ParseDouble(chars, 0, chars.Length);
            Assert.AreEqual(double.Parse(new string(chars)), d, 0.001);
        }

        [Test]
        public void TestParseDoubleWithFractionalPartOnly()
        {
            char[] chars = { '.', '1', '2', '3', '4', '5' };
            double d = ParseDoubleSupport.ParseDouble(chars, 0, chars.Length);
            Assert.AreEqual(double.Parse(new string(chars)), d, 0.001);
        }

        [Test]
        public void TestParseDoubleWithExponentOnly()
        {
            char[] chars = { '1', 'e', '2' };
            double d = ParseDoubleSupport.ParseDouble(chars, 0, chars.Length);
            Assert.AreEqual(double.Parse(new string(chars)), d, 0.001);
        }

        [Test]
        public void TestParseDoubleWithPositiveExponent()
        {
            char[] chars = { '1', '2', '3', 'e', '4' };
            double d = ParseDoubleSupport.ParseDouble(chars, 0, chars.Length);
            Assert.AreEqual(double.Parse(new string(chars)), d, 0.001);
        }

        [Test]
        public void TestParseDoubleWithNegativeExponent()
        {
            char[] chars = { '1', '2', '3', 'e', '-', '4' };
            double d = ParseDoubleSupport.ParseDouble(chars, 0, chars.Length);
            Assert.AreEqual(double.Parse(new string(chars)), d, 0.001);
        }
}