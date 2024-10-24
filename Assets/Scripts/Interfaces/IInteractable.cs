using System;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform raySource;   //camera
    [SerializeField] private float interactRange;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // input system
        {
            Ray ray = new Ray(raySource.position, raySource.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, interactRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }

}