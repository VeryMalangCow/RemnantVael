#if UNITY_EDITOR

using UnityEngine;

public struct PatternPreviewElement
{
    public string name;
    public PatternPreviewDetail left;
    public PatternPreviewDetail oper;
    public PatternPreviewDetail right;

    public PatternPreviewElement(string name, PatternPreviewDetail left, PatternPreviewDetail oper, PatternPreviewDetail right)
    {
        this.name = name;

        this.left = left;
        this.oper = oper;
        this.right = right;
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

#endif