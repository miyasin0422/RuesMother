using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AssignWeaponButton : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    
    [SerializeField]
    private AssignUI assignUI;
    [SerializeField]
    private Button button;
    [SerializeField]
    private GameObject checkMarkObject;
    [SerializeField]
    private Transform dragLayer;
    [SerializeField]
    private GameObject dragObjectPrefab;
    public GameObject DragObjectPrefab
    {
        get { return dragObjectPrefab; }
    }

    private GameObject dragObject;
    public WeaponRecipeSO weaponRecipe;
    public GameObject weaponPrefab;

    


    // 作成済みかどうかによる表示
    public void CanAssignState(bool isCrafted)
    {
        if (button != null)
        {
            button.image.color = isCrafted ? Color.white : Color.gray;
            button.interactable = isCrafted;
        }
    }
    public void SetAssignedState(bool isAssigned)
    {
        if (checkMarkObject != null)
        {
            checkMarkObject.SetActive(isAssigned);
        }
    }

    // ドラッグ開始
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 作成済みでなければドラッグ不可
        if (!CraftingManager.instance.IsCrafted(weaponRecipe))
        {
            return;
        }

        // ドラッグ用オブジェクトを生成
        dragObject = Instantiate(
            dragObjectPrefab,
            dragLayer
        );

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