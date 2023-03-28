namespace sharpJParse.support;

public interface ICharSequence
{
    string ToString();
    char CharAt(int index);
    int Length();
    
    char this[int index]
    {
        get;
        set;
    }

    ICharSequence SubSequence(int start, int end);
}