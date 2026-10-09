using UnityEngine;
using System.Collections.Generic;

public enum ElementType { Base, Acid, Plasma, Fire, Ice }

[CreateAssetMenu(menuName ="Gene")]

public class GeneSO : ScriptableObject
{
    public string geneId;
    public string geneName;
    public Sprite icon;
    public ElementType element; // 付与属性

    [Header("ステータス補正値")]
    public float atkBonus;
    public float atkSpeedBonus;
}
