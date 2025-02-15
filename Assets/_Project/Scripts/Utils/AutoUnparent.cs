using UnityEngine;

public class AutoUnparent : MonoBehaviour
{
    [Tooltip("This GameObject will be unparented to the scene hierarchy root on Awake")]

    void Awake()
    {
        transform.SetParent(null);
    }
}