using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScopeSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject scopeUI;
    [SerializeField]
    private GameObject playerUI;
    [SerializeField]
    private float zoomSpeed = 2f;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Camera scopeCamera;
    private bool isScoped;
    private float originalFOV;
    
    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {   
            isScoped = !isScoped;
            scopeUI.SetActive(isScoped);

            if(isScoped)
            {
                playerUI.SetActive(false);
                mainCamera.gameObject.SetActive(false);
                scopeCamera.gameObject.SetActive(true);

                scopeCamera.enabled = true;
                originalFOV = scopeCamera.fieldOfView;
                scopeCamera.fieldOfView /= zoomSpeed;
            }
            else
            {
                playerUI.SetActive(true);
                mainCamera.gameObject.SetActive(true);
                scopeCamera.gameObject.SetActive(false);

                scopeCamera.enabled = false;
                scopeCamera.fieldOfView = originalFOV;
            }
        }

        if (!isScoped)
            return;
        
        var zoom = Input.GetAxis("Mouse ScrollWheel");
        scopeCamera.fieldOfView -= zoom * zoomSpeed * 10f;
        scopeCamera.fieldOfView = Mathf.Clamp(scopeCamera.fieldOfView, 10f, 80f);
    }
}
