using TMPro;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;

    Transform target;
    Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
        gameObject.SetActive(false);
    }
    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(target.position);

        transform.position = screenPosition;
    }

    public void Show(Transform newTarget, string text)
    {
        Debug.Log("DialogueBubble Show");

        target = newTarget;
        dialogueText.text = text;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        target = null;
    }
}