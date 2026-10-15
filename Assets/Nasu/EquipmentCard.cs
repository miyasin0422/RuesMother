using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class EquipmentCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int id; // 0: 左手, 1: 右手
    [SerializeField] private Image myFrame;
    [SerializeField] private Image oppositeFrame;

    public static int SelectedSlotId { get; private set; } = 0;
    public static event Action<int> OnCardSelected;

    private void Start()
    {
        if (id == 0)
        {
            SelectThisSlot();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectThisSlot();
    }

    public void SelectThisSlot()
    {
        SelectedSlotId = id;
        ColorChange();
        OnCardSelected?.Invoke(id);
    }

    public void ColorChange()
    {
        if (myFrame != null) myFrame.color = Color.green;
        if (oppositeFrame != null) oppositeFrame.color = Color.gray;
    }
}