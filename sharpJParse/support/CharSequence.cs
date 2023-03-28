namespace sharpJParse.support;

public interface ICharSequence
{
    char this[int index] { get; set; }

    string ToString();
    char CharAt(int index);
    int Length();

    ICharSequence SubSequence(int start, int end);
}