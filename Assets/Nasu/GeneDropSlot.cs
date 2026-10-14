using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GeneDropSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public enum SlotType
    {
        Left,
        Middle,
        Right
    }
    [SerializeField] private Image geneImage;
    [SerializeField] private CreateUI createUI; // UI更新用に参照
    public GeneSO CurrentGene { get; private set; }

    public void OnDrop(PointerEventData eventData)
    {
        GeneDrag geneCard = eventData.pointerDrag?.GetComponent<GeneDrag>();

        if (geneCard == null)
        {
            return;
        }
        Debug.Log("ドロップした");
        geneImage.sprite = geneCard.GeneSprite;
        CurrentGene = geneCard.CurrentGene;

        if (createUI != null)
        {
            createUI.RefreshGeneList();
        }
    }
    //右クリックで外す
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClearSlot();
        }
    }
    public void ClearSlot()
    {
        CurrentGene = null;
        if (geneImage != null)
        {
            geneImage.sprite = null;
        }

        // ★ クリアされたので、予告表示を消して元の表示（x3）に再計算して更新
        if (createUI != null)
        {
            createUI.RefreshGeneList();
        }
    }

}