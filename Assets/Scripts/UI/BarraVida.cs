using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image marco;

    public Color colorInicial;
    private bool parpadeo = false;
    public float pvel = 0.2f;

    void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void CambiarVidaMax(float vidaMax)
    {
        slider.maxValue = vidaMax;
    }

    public void CambiarVidaActual(float cantidadVida)
    {
        slider.value = cantidadVida;
    }

    public void InizializarBV(float cantidadVida)
    {
        CambiarVidaMax(cantidadVida);
        CambiarVidaActual(cantidadVida);
    }

    private void Update()
    {
        if (slider.value < 7 && !parpadeo)
        {
            parpadeo = true;
            StartCoroutine(EfectoParpadeo());
        }
        else if(slider.value >= 9 && parpadeo)
        {
            parpadeo = false;
            marco.color = colorInicial;
        }
    }

    IEnumerator EfectoParpadeo()
    {
        while (parpadeo)
        {
            marco.color = Color.red;
            yield return new WaitForSeconds(pvel);

            marco.color = colorInicial;
            yield return new WaitForSeconds(pvel);
        }
    }

    /*IEnumerator EfectoParpadeo()
    {
        while (parpadeo)
        {
            // Transición de color al rojo
            float elapsedTime = 0f;
            while (elapsedTime < 0.5f)
            {
                marco.color = Color.Lerp(colorInicial, Color.red, elapsedTime / 0.5f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Espera antes de volver al color inicial
            yield return new WaitForSeconds(0.5f);

            // Transición de color de regreso al color inicial
            elapsedTime = 0f;
            while (elapsedTime < 0.5f)
            {
                marco.color = Color.Lerp(Color.red, colorInicial, elapsedTime / 0.5f);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Espera antes del próximo parpadeo
            yield return new WaitForSeconds(0.5f);
        }
    }*/


}
