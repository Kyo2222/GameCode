using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Recoil Settings")]
    [SerializeField]
    private float recoilX = 0.1f; // Horizontal recoil

    [SerializeField]
    private float recoilY = 0.3f; // Vertical recoil

    [SerializeField]
    private float recoilZ = 0.05f; // Offset

    [Header("Recovery Settings")]
    [SerializeField]
    private float returnSpeed = 5f; // Recovery speed

    private Vector3 currentRecoil;
    private Vector3 targetRecoil;
    [SerializeField]
    private Vector3 maxRecoil;

    private void Update()
    {
        // Recoil
        if (Input.GetButton("Fire1"))
        {
            ApplyRecoil();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            ResetRecoil();
        }
        
        currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero,
            returnSpeed * Time.deltaTime);
        
        transform.localRotation = Quaternion.Euler(currentRecoil);
    }
    
    private void ResetRecoil() 
    {
        currentRecoil = targetRecoil = Vector3.zero;
    }

    private void ApplyRecoil()
    {
        float randomX = Random.Range(-recoilX, recoilX);
        float randomY = Random.Range(recoilY * 0.5f, recoilY);
        float randomZ = Random.Range(-recoilZ, recoilZ);

        targetRecoil += new Vector3(randomX, randomY, randomZ);
        // Clamp target recoil to max limits
        targetRecoil.x = Mathf.Clamp(targetRecoil.x, -maxRecoil.x, maxRecoil.x);
        targetRecoil.y = Mathf.Clamp(targetRecoil.y, 0f, maxRecoil.y);
        targetRecoil.z = Mathf.Clamp(targetRecoil.z, -maxRecoil.z, maxRecoil.z);
        
        currentRecoil = targetRecoil;
    }
}
