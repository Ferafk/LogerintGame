using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CambiarRio : MonoBehaviour
{
    [SerializeField] private PushEffect[] _river;
    [SerializeField] private int pago = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Puntaje.Instance.puntos >= pago)
            {
                AudioManager.instance.PlaySFX("Button");
                foreach (PushEffect rios in _river)
                {
                    rios.cambiararFlujo();
                }
                Puntaje.Instance.QuitarMoneda(pago);
                
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Te faltan monedas");
            }
        }
    }
}
