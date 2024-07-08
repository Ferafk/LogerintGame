using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gCheck : MonoBehaviour
{
    public bool Suelo;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Suelo"))
        {
            Suelo = true;
        }
    }
}
