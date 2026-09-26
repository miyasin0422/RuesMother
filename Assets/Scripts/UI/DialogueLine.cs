using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public Transform speakerAnchor;

    [TextArea(2, 4)]
    public string text;
}