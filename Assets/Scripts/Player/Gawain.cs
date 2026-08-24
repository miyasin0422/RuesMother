using UnityEngine;

public class Gawain : MonoBehaviour
{
    [SerializeField] GameObject attackPrefab;
    [SerializeField] Transform attackPoint;
    [SerializeField] float summonTime = 0.5f;

    void Start()
    {
        GameObject attack = Instantiate(attackPrefab, attackPoint.position, Quaternion.identity);

        Vector3 scale = attack.transform.localScale;
        scale.x *= transform.localScale.x > 0 ? 1 : -1;
        attack.transform.localScale = scale;

        Destroy(gameObject, summonTime);
    }
}