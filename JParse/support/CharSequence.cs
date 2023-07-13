namespace sharpJParse.JsonParser.support;

public interface ICharSequence
{
    char this[int index] => CharAt(index);

    int Length { get; }

    string? ToString();
    char CharAt(int index);

    ICharSequence SubSequence(int start, int end);
}