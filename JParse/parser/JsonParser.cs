using JsonParser.node;
using JsonParser.node.support;
using JsonParser.source;
using JsonParser.token;

namespace JsonParser.parser;

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