using UnityEngine;
using UnityEngine.InputSystem;

public class SavePoint : MonoBehaviour
{
    private bool canSave = false;
    [SerializeField] private GameObject QuestionPanel;
    [SerializeField] private GameObject SavePanel;
    [SerializeField] InputAction openAction;
    [SerializeField] InputAction closeAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        openAction.Enable();
        closeAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (canSave)
        {
            if (openAction.triggered)
            {
                QuestionPanel.SetActive(false);
                SavePanel.SetActive(true);
                canSave = false;
            }
            if (closeAction.triggered)
            {
                QuestionPanel.SetActive(false);
                canSave = false;
            }
        }
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
