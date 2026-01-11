using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostCurrencyController : MonoBehaviour
{
    public int Currency;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.GetComponent<Player>() != null)
        {
            PlayerManager.Instance.UpdateCurrency(Currency);
            Destroy(gameObject);
        }
    }
}
