namespace sharpJParse;

public interface CharSequence
{
    string ToString();
    char CharAt(int index);
    int Length();

    CharSequence SubSequence(int start, int end);
}