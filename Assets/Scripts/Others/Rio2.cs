using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rio2 : MonoBehaviour
{
    public Vector2 riverDirection = Vector2.left; // Direcci�n del r�o
    public float riverForce = 10f; // Fuerza del r�o

    void OnTriggerStay2D(Collider2D other)
    {
        // Verifica si el objeto en el r�o tiene un Rigidbody2D
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Aplica una fuerza al Rigidbody en la direcci�n del r�o
            rb.AddForce(riverDirection * riverForce * Time.deltaTime);
        }
    }
}
