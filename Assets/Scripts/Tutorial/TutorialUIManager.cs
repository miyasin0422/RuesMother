using TMPro;
using UnityEngine;

public class TutorialUIManager : MonoBehaviour
{
    [SerializeField] GameObject tutorialUI;
    [SerializeField] TextMeshProUGUI tutorialText;

    private void Awake()
    {
        tutorialUI.SetActive(false);
    }

    public void Show()
    {
        tutorialUI.SetActive(true);
    }

    public void Hide()
    {
        tutorialUI.SetActive(false);
    }
}