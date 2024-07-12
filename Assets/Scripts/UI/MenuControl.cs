using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private Canvas menu;
    [SerializeField] private GameObject puntaje;
    private bool gameRunning;

    private void Start()
    {
        gameRunning = true;
        Time.timeScale = 1f;

        if (menu != null)
        {
            menu.enabled = false;
        }
    }

    private void Update()
    {
        if (menu != null)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ChangeGameRun();
            }
        }
    }

    public void ReStart()
    {
        
        string nombreEscena = SceneManager.GetActiveScene().name;
        Puntaje.Instance.puntos = 0;
        Puntaje.Instance.vidas = Puntaje.Instance.vidasActuales;
        SceneManager.LoadScene(nombreEscena);
    }

    public void NextLvl(string nivel)
    {
        Puntaje.Instance.puntos = 0;
        Puntaje.Instance.vidasActuales = Puntaje.Instance.vidas;

        SceneManager.LoadScene(nivel);
    }

    public void MoveMenu()
    {
        Puntaje.Instance.lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Menu");
    }

    public void MovePerdiste()
    {
        Puntaje.Instance.lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Perdiste");
    }

    public void ReintentarUltimo()
    {
        Puntaje.Instance.puntos = 0;
        Puntaje.Instance.vidas = Puntaje.Instance.vidasActuales;
        SceneManager.LoadScene(Puntaje.Instance.lastScene);
    }

    public void MoveGanaste()
    {
        SceneManager.LoadScene("Ganaste");
    }

    public void ChangeGameRun()
    {
        gameRunning = !gameRunning;

        if (gameRunning)
        {
            Time.timeScale = 1f;
            if (menu  != null)
            {
                menu.enabled = false;
            }
        }
        else
        {
            Time.timeScale = 0f;
            if (menu != null)
            {
                menu.enabled = true;
            }
        }
    }

}
