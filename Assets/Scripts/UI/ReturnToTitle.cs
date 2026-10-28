using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ReturnToTitle : MonoBehaviour
{
    void Update()
    {
        if (
            Keyboard.current == null ||
            !Keyboard.current.pKey.isPressed ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (SceneManager.GetActiveScene().name == "Title")
        {
            return;
        }

        PlayerActionBlocker.ResetForSceneChange();
        SceneManager.LoadScene("Title");
    }
}