using UnityEngine;

public class EnemySensor : MonoBehaviour
{
    [SerializeField] float detectionRange = 8f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] LayerMask playerLayer;

    public Transform Player { get; private set; }

    public void SearchPlayer()
    {
        Collider2D foundPlayer =
            Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);

        if (foundPlayer != null)
        {
            Player = foundPlayer.transform;
        }
        else
        {
            Player = null;
        }
    }

    public bool IsPlayerInAttackRange()
    {
        if (Player == null)
        {
            return false;
        }

        float distance =
            Vector2.Distance(transform.position, Player.position);

        return distance <= attackRange;
    }
}