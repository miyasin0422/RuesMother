using UnityEngine;

public class TutorialStartTrigger : MonoBehaviour
{
    [SerializeField] TutorialEventBase tutorialEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerControll playerMovement =
            other.GetComponentInParent<PlayerControll>();

        if (playerMovement == null)
        {
            return;
        }

        tutorialEvent.StartTutorial(
            playerMovement.gameObject
        );
    }
}