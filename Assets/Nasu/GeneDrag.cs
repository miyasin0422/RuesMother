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

    private GameObject dragObject;
    //ボタン表示処理
    public void Setup(GeneSO gene, string countDisplayText, Transform layerTransform)
    {
        //GeneSO設定
        CurrentGene = gene;
        //ドラッグ用レイヤー
        dragLayer = layerTransform;
        //遺伝子名表示
        if(geneNameText != null)
        {
            geneNameText.text = gene.geneName;
        }
        //遺伝子数表示
        if(countText != null)
        {
            countText.text = countDisplayText;
        }

    }

    // ドラッグ開始
    public void OnBeginDrag(PointerEventData eventData)
    {
        // ドラッグ用オブジェクトを生成
        dragObject = Instantiate(dragObjectPrefab, dragLayer);

        // ★ドラッグ用ゴースト画像にもカードのSpriteを反映させる
        if (dragObject.TryGetComponent<Image>(out var ghostImage))
        {
            ghostImage.sprite = GeneSprite;
        }

        // ★ゴーストがレイキャスト（ドロップ判定）を遮らないように非判定化
        if (dragObject.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.blocksRaycasts = false;
        }

        // マウス位置に配置
        dragObject.transform.position = eventData.position;
    }

    // ドラッグ中
    public void OnDrag(PointerEventData eventData)
    {
        if (dragObject != null)
        {
            dragObject.transform.position = eventData.position;
        }
    }

    // ドラッグ終了
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragObject != null)
        {
            Destroy(dragObject);
        }
        dragObject = null;
    }
    //Destroyされた場合にも対応
    private void OnDestroy()
    {
        if (dragObject != null)
        {
            Destroy(dragObject);
        }
        dragObject = null;
    }
}