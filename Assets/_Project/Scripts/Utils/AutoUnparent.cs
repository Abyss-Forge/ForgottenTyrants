using UnityEngine;
using Utils.Extensions;

public class AutoUnparent : MonoBehaviour
{

    void Awake()
    {
        transform.Unparent();
    }
}