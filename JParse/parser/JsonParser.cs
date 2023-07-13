

using sharpJParse.JsonParser.node;
using sharpJParse.JsonParser.node.support;
using sharpJParse.JsonParser.source;

namespace sharpJParse.JsonParser.parser;

public interface IJsonParser 
{
    TokenList Scan( ICharSource source);
    RootNode Parse(ICharSource source); 
    RootNode Parse(string source) {
        return Parse(Sources.StringSource(source));
    }
    TokenList Scan(string source) {
        return Scan(Sources.StringSource(source));
    }
}