using UnityEngine;

public enum DialogueSpeaker
{
    Rue,
    Ray
}

[System.Serializable]
public class DialogueLine
{
    public DialogueSpeaker speaker;

    [TextArea(2, 4)]
    public string text;

    public DialogueLine(DialogueSpeaker speaker, string text)
    {
        this.speaker = speaker;
        this.text = text;
    }
}