using System.Collections.Generic;
using UnityEngine;

public class SlotReset : MonoBehaviour
{
    [SerializeField] private List<GeneDropSlot> allSlots; // スロット
   
    public void Reset()
    {
        foreach(var slot in allSlots)
        {
            slot.ClearSlot();
        }
    }
}
