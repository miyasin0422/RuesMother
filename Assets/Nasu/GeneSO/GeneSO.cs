using UnityEngine;

public enum ElementType { Base, Acid, Plasma, Fire, Ice }

[CreateAssetMenu(menuName = "Gene")]
public class GeneSO : ScriptableObject
{
    public string geneId;
    public string geneName;
    public ElementType element; // 付与属性

    [Header("UI表示用")]
    public GameObject buttonPrefab;

    [Header("ステータス補正値")]
    public float atkBonus = 5f;        // 攻撃力加算
    public float atkSpeedBonus = 0.1f; // 攻撃速度加算
    public float rangeBonus = 0f;      // 飛距離/射程加算
    public float elementDmgBonus = 0f; // 属性ダメージ加算

    [Header("自動命名・テキスト生成用")]
    [Tooltip("変異因子の接頭辞 (例: 酸->'酸の', 炎->'火炎の', プラズマ->'プラズマ')")]
    public string weaponPrefix = "";

    [Tooltip("説明文追加テキスト (例: '強烈な酸で対象を溶かす。')")]
    public string elementDescription = "";
}