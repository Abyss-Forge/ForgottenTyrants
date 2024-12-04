using UnityEngine;

public sealed class AutoDisable : MonoBehaviour
{
    void OnValidate()
    {
        gameObject.SetActive(false);
    }
}