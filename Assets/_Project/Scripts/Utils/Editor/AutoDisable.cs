using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    void OnValidate()
    {
        gameObject.SetActive(false);
    }
}