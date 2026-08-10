#if UNITY_EDITOR

using UnityEngine;

public struct PatternPreviewElement
{
    public string name;
    public PatternPreviewDetail left;
    public PatternPreviewDetail oper;
    public PatternPreviewDetail right;
    public PatternPreviewDescription desc;

    public PatternPreviewElement(
        string name, 
        PatternPreviewDetail left, 
        PatternPreviewDetail oper, 
        PatternPreviewDetail right,
        PatternPreviewDescription desc)
    {
        this.name = name;

        this.left = left;
        this.oper = oper;
        this.right = right;

        this.desc = desc;
    }
}

public struct PatternPreviewDetail
{
    public string name;
    public Color clr;

    public PatternPreviewDetail(string name, Color clr)
    {
        this.name = name;
        this.clr = clr;
    }

    public PatternPreviewDetail(string name)
    {
        this.name = name;
        this.clr = new Color(0.8f, 0.8f, 0.8f, 1f);
    }
}

public struct PatternPreviewDescription
{
    public string desc;

    public PatternPreviewDescription(string desc)
    {
        this.desc = desc;
    }
}

#endif