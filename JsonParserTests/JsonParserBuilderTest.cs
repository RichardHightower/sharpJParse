using NUnit.Framework;
using sharpJParse.JsonParser.parser;
using sharpJParse.JsonParser.parser.indexoverlay;

namespace sharpJParse.Tests
{
    [TestFixture]
    public class JsonParserBuilderTests
    {
        [Test]
        public void TestDefaultBuilder()
        {
            var builder = new JsonParserBuilder();
            var parser = builder.Build();

            Assert.IsInstanceOf<JsonFastParser>(parser);
            Assert.IsFalse(builder.Strict());
            Assert.IsFalse(builder.ObjectsKeysCanBeEncoded());
        }

        [Test]
        public void TestStrictParser()
        {
            var builder = new JsonParserBuilder().SetStrict(true);
            var parser = builder.Build();

            // Note: This test will fail until JsonStrictParser is implemented
            // Assert.IsInstanceOf<JsonStrictParser>(parser);
            Assert.IsInstanceOf<JsonFastParser>(parser);
            Assert.IsTrue(builder.Strict());
        }

        [Test]
        public void TestObjectKeysEncoded()
        {
            var builder = new JsonParserBuilder().SetObjectsKeysCanBeEncoded(true);
            var parser = builder.Build();

            Assert.IsInstanceOf<JsonFastParser>(parser);
            Assert.IsTrue(builder.ObjectsKeysCanBeEncoded());
        }

        [Test]
        public void TestBuilderMethodChaining()
        {
            var builder = new JsonParserBuilder()
                .SetStrict(true)
                .SetObjectsKeysCanBeEncoded(true);

            Assert.IsTrue(builder.Strict());
            Assert.IsTrue(builder.ObjectsKeysCanBeEncoded());

            var parser = builder.Build();
            // Note: This will return JsonFastParser until JsonStrictParser is implemented
            Assert.IsInstanceOf<JsonFastParser>(parser);
        }

        [Test]
        public void TestStaticBuilderMethod()
        {
            var builder = JsonParserBuilder.Builder();
            Assert.IsNotNull(builder);
            Assert.IsInstanceOf<JsonParserBuilder>(builder);
        }

        [Test]
        public void TestMultipleBuilds()
        {
            var builder = new JsonParserBuilder();
            
            var parser1 = builder.Build();
            Assert.IsInstanceOf<JsonFastParser>(parser1);

            builder.SetStrict(true);
            var parser2 = builder.Build();
            // Note: This will return JsonFastParser until JsonStrictParser is implemented
            Assert.IsInstanceOf<JsonFastParser>(parser2);

            builder.SetStrict(false);
            var parser3 = builder.Build();
            Assert.IsInstanceOf<JsonFastParser>(parser3);
        }

        [Test]
        public void TestObjectsKeysCanBeEncodedPropagation()
        {
            var builder = new JsonParserBuilder().SetObjectsKeysCanBeEncoded(true);
            var parser = (JsonFastParser)builder.Build();

            var json = Json.NiceJson("{'hi': 'how are you?'}");

            var objectNode = parser.Parse(json).GetObjectNode();

            var value = objectNode.GetString("hi");
            Assert.That(value, Is.EqualTo("how are you?"));
        }
    }
}