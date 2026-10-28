using TMPro;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;

    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(Transform newTarget, string text)
    {
        Debug.Log("DialogueBubble Show");
        dialogueText.text = text;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}