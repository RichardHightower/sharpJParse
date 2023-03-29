namespace sharpJParse.support;

public interface ICharSequence
{
    char this[int index] => CharAt(index);

    string? ToString();
    char CharAt(int index);
    int Length();

    ICharSequence SubSequence(int start, int end);
}