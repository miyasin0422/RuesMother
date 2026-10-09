using UnityEngine;

public class GeneItem : MonoBehaviour
{
    [SerializeField]
    private GeneSO geneData; // ドロップさせる遺伝子データ

    public GeneSO GeneData => geneData;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (geneData != null && Inventory.instance != null)
            {
                Inventory.instance.AddGene(geneData, 1);
                Debug.Log($"遺伝子「{geneData.geneName}」を回収しました！");
            }

            Destroy(gameObject);
        }
    }
}