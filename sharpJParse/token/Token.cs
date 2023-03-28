using sharpJParse.source;

namespace sharpJParse.token;

/*
 * Copyright 2013-2023 Richard M. Hightower
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *  		http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

public readonly struct Token
{
    public readonly int endIndex;

    public readonly int startIndex;
    public readonly int type;

    public Token(int startIndex, int endIndex, int type)
    {
        this.startIndex = startIndex;
        this.endIndex = endIndex;
        this.type = type;
    }

    public string AsString(string buffer)
    {
        return buffer.Substring(startIndex, endIndex);
    }

    public string AsString(ICharSource source)
    {
        return source.GetString(startIndex, endIndex);
    }

    public int Length()
    {
        return endIndex - startIndex;
    }

    public override string ToString()
    {
        return "Token{" +
               "startIndex=" + startIndex +
               ", endIndex=" + endIndex +
               ", type=" + TokenTypes.GetTypeName(type) + " " + type +
               '}';
    }
}