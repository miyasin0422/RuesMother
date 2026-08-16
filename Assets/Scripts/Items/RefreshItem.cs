using UnityEngine;

public class RefreshItem : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (PlayerStatus.refreshItemStock < 3)
            {
                PlayerStatus.refreshItemStock += 1;
                uiManager.RefreshItemUpdate();
                Destroy(gameObject);
            }
        }
    }
}
