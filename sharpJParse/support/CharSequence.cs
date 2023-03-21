namespace sharpJParse.support;

public interface ICharSequence
{
    string ToString();
    char CharAt(int index);
    int Length();

    ICharSequence SubSequence(int start, int end);
}