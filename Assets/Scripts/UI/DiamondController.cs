using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondController : MonoBehaviour
{
    public int diamondID;

    private void Start()
    {
        if (Puntaje.Instance.IsDiamondCollected(diamondID))
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Puntaje.Instance.AgregarDiamante();
            Puntaje.Instance.CollectDiamond(diamondID);
            gameObject.SetActive(false);
        }
    }

}
