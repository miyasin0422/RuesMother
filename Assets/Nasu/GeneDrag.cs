using Gene;
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

    // ★プロパティに修正（geneIcon.sprite を安全に返す）
    public Sprite GeneSprite => geneIcon != null ? geneIcon.sprite : null;

    public GameObject DragObjectPrefab => dragObjectPrefab;

    private GameObject dragObject;

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
    }
}