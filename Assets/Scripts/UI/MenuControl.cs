using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    [SerializeField] private Canvas menu;
    [SerializeField] private Canvas confirmationPanel;
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

        if(confirmationPanel != null)
        {
            confirmationPanel.enabled = false;
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

    public void ShowConfirmation()
    {
        puntaje = GameObject.FindGameObjectWithTag("Puntaje");

        if (puntaje == null)
        {
            ConfirmNewGame();
        }
        else
        {
            confirmationPanel.enabled = true;
        }
    }

    public void ConfirmNewGame()
    {
        confirmationPanel.enabled = false;

        if (puntaje != null)
            Puntaje.Instance.ResetAllData();

        SceneManager.LoadScene("Nivel1");
    }

    public void CancelNewGame()
    {
        confirmationPanel.enabled = false;
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

    public void SalirJuego()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }

}
