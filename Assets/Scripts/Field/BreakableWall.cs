using UnityEngine;

public class BreakableWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerAttack"))
        {
            Debug.Log("aaa");
            Destroy(gameObject);
        }
    }
}
