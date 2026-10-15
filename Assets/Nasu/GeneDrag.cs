using Gene;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GeneDrag : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [SerializeField] private Transform dragLayer;
    [SerializeField] private GameObject dragObjectPrefab;
    [SerializeField] private Image geneIcon;
    [SerializeField] private TMP_Text geneNameText;
    [SerializeField] private TMP_Text countText;

    public GeneSO CurrentGene { get; private set; }
    public Sprite GeneSprite => geneIcon != null ? geneIcon.sprite : null;
    public GameObject DragObjectPrefab => dragObjectPrefab;

    public int AvailableCount { get; private set; } // ★ 現在使用可能な残数

    private GameObject dragObject;

    // ボタン表示処理（残数 availableCount を受け取るように拡張）
    public void Setup(GeneSO gene, int availableCount, string countDisplayText, Transform layerTransform)
    {
        CurrentGene = gene;
        AvailableCount = availableCount;
        dragLayer = layerTransform;

        if (geneNameText != null) geneNameText.text = gene.geneName;
        if (countText != null) countText.text = countDisplayText;

    }

    // ドラッグ開始
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ★ 残数が 0 個の場合はドラッグ不可
        if (AvailableCount <= 0) return;

        dragObject = Instantiate(dragObjectPrefab, dragLayer);

        if (dragObject.TryGetComponent<Image>(out var ghostImage))
        {
            ghostImage.sprite = GeneSprite;
        }

        if (dragObject.TryGetComponent<CanvasGroup>(out var ghostGroup))
        {
            ghostGroup.blocksRaycasts = false;
        }

        dragObject.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragObject != null)
        {
            dragObject.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragObject != null)
        {
            Destroy(dragObject);
        }
        dragObject = null;
    }

    private void OnDestroy()
    {
        if (dragObject != null)
        {
            Destroy(dragObject);
        }
        dragObject = null;
    }
}