using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact();
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _raySource;   //camera
    [SerializeField] private float _interactRange;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // input system
        {
            Ray ray = new Ray(_raySource.position, _raySource.forward);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, _interactRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }

}