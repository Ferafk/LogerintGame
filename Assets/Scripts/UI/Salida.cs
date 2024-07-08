using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salida : MonoBehaviour
{
    [SerializeField] private MenuControl _menu;
    [SerializeField] private string irNivel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _menu.NextLvl(irNivel);
        }
    }

}
