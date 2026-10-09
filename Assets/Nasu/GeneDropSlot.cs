using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GeneDropSlot : MonoBehaviour, IDropHandler
{
    public enum SlotType
    {
        Left,
        Middle,
        Right
    }
    [SerializeField] private Image geneImage;

    public void OnDrop(PointerEventData eventData)
    {
        GeneDrag geneCard = eventData.pointerDrag?.GetComponent<GeneDrag>();

        if (geneCard == null)
        {
            return;
        }
       
    }
}