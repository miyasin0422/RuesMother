using UnityEngine;
using UnityEngine.InputSystem;

public class SavePoint : MonoBehaviour
{
    private bool canSave = false;

    [SerializeField] private GameObject QuestionPanel;
    [SerializeField] private GameObject SavePanel;

    [SerializeField] InputAction openAction;
    [SerializeField] InputAction closeAction;

    void Start()
    {
        openAction.Enable();
        closeAction.Enable();
    }

    void Update()
    {
        if (!canSave)
        {
            return;
        }

        if (openAction.triggered)
        {
            OpenSavePanel();
        }
        else if (closeAction.triggered)
        {
            CloseQuestionPanel();
        }
    }

    // Eキー・確認画面の「開く」ボタンから呼ぶ
    public void OpenSavePanel()
    {
        if (!canSave)
        {
            return;
        }

        QuestionPanel.SetActive(false);
        SavePanel.SetActive(true);
        canSave = false;
    }

    // Escキー・確認画面の「閉じる」ボタンから呼ぶ
    public void CloseQuestionPanel()
    {
        if (!canSave)
        {
            return;
        }

        QuestionPanel.SetActive(false);
        canSave = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            QuestionPanel.SetActive(true);
            canSave = true;
        }
    }
}