using UnityEngine;
using UnityEngine.UI;

public class EquipmentCard : MonoBehaviour
{
    [SerializeField] 
    private int id;
    [SerializeField]
    private Image myFrame;
    [SerializeField]
    private Image oppositeFrame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ColorChange()
    {

        myFrame.color = Color.green;
        oppositeFrame.color = Color.gray;
    }
}
