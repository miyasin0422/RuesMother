using UnityEngine;

public class CharacterUIAnchor : MonoBehaviour
{
    [SerializeField]
    private Transform uiAnchor;

    public Transform UIAnchor => uiAnchor;
}