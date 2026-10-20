using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Gene;

public class GeneDropSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public enum SlotType { Left, Middle, Right }

    [SerializeField] private Image geneImage;
    [SerializeField] private CreateUI createUI;

    // ★ 型を Gene.CraftingManager に明示して同名クラスとの衝突を解消
    [SerializeField] private Gene.CraftingManager craftingManager;

    public GeneSO CurrentGene { get; private set; }

    public void OnDrop(PointerEventData eventData)
    {
        GeneDrag geneCard = eventData.pointerDrag?.GetComponent<GeneDrag>();
        if (geneCard == null) return;

        geneImage.sprite = geneCard.GeneSprite;
        CurrentGene = geneCard.CurrentGene;

        if (createUI != null) createUI.RefreshGeneList();
        if (craftingManager != null) craftingManager.UpdateCraftPreview();
    }

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
        if (geneImage != null) geneImage.sprite = null;

        if (createUI != null) createUI.RefreshGeneList();
        if (craftingManager != null) craftingManager.UpdateCraftPreview();
    }

    /// <summary>
    /// CraftingManagerから直接遺伝子をセットする（初期表示用）
    /// </summary>
    public void SetGeneDirectly(GeneSO gene)
    {
        CurrentGene = gene;
        if (geneImage != null && gene != null)
        {   GeneDrag geneCard = gene.buttonPrefab.GetComponent<GeneDrag>();
            geneImage.sprite = geneCard.GeneSprite;
        }
    }

    public void ClearSlotWithoutRefresh()
    {
        CurrentGene = null;
        if (geneImage != null) geneImage.sprite = null;
    }
}