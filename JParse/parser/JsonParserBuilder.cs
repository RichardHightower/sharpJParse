using sharpJParse.JsonParser.parser.indexoverlay;

namespace sharpJParse.JsonParser.parser;

/// <summary>
///     Builder class for creating instances of JsonParser.
/// </summary>
public class JsonParserBuilder
{
    private bool _objectsKeysCanBeEncoded;
    private bool _strict;

    /// <summary>
    ///     Constructs a new instance of JsonParserBuilder.
    /// </summary>
    public JsonParserBuilder()
    {
    }

    /// <summary>
    ///     Returns a new instance of JsonParserBuilder.
    /// </summary>
    /// <returns>A new instance of JsonParserBuilder</returns>
    public static JsonParserBuilder Builder()
    {
        return new JsonParserBuilder();
    }

    /// <summary>
    ///     Gets whether to use strict parsing when parsing JSON.
    /// </summary>
    /// <returns>True if strict parsing is used, false otherwise</returns>
    public bool Strict()
    {
        return _strict;
    }

    /// <summary>
    ///     Sets whether to use strict parsing when parsing JSON.
    /// </summary>
    /// <param name="strict">True to use strict parsing, false otherwise</param>
    /// <returns>The modified builder</returns>
    public JsonParserBuilder SetStrict(bool strict)
    {
        this._strict = strict;
        return this;
    }

    /// <summary>
    ///     Gets whether object keys can be encoded when parsing JSON.
    /// </summary>
    /// <returns>True if object keys can be encoded, false otherwise</returns>
    public bool ObjectsKeysCanBeEncoded()
    {
        return _objectsKeysCanBeEncoded;
    }

    /// <summary>
    ///     Sets whether object keys can be encoded when parsing JSON.
    /// </summary>
    /// <param name="objectsKeysCanBeEncoded">True to allow encoded keys, false otherwise</param>
    /// <returns>The modified builder</returns>
    public JsonParserBuilder SetObjectsKeysCanBeEncoded(bool objectsKeysCanBeEncoded)
    {
        this._objectsKeysCanBeEncoded = objectsKeysCanBeEncoded;
        return this;
    }

    /// <summary>
    ///     Builds and returns a new instance of JsonParser based on the current configuration.
    /// </summary>
    /// <returns>A new instance of JsonParser</returns>
    public IJsonParser Build()
    {
        if (Strict())
            //return new JsonStrictParser(ObjectsKeysCanBeEncoded()); //TODO create JSON Strict Parser 
            return new JsonFastParser(ObjectsKeysCanBeEncoded());
        return new JsonFastParser(ObjectsKeysCanBeEncoded());
    }
}