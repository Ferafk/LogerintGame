using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{
    [SerializeField] private Canvas confirmationPanel;
    [SerializeField] private Button continuar;
    [SerializeField] private Button niveles;
    [SerializeField] private GameObject puntaje;
    [SerializeField] private UnlockLevels _unlock;

    private void Start()
    {
        Time.timeScale = 1f;

        if (confirmationPanel != null)
        {
            confirmationPanel.enabled = false;
        }

        puntaje = GameObject.FindGameObjectWithTag("Puntaje");

        if (continuar != null)
        {
            continuar.interactable = (puntaje == null) ? false : true;
            niveles.interactable = (puntaje == null) ? false : true;
        }

        if (_unlock != null)
        {
            _unlock.Unlock();
        }
    }

    public void ShowConfirmation()
    {
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

    public void NextLvl(string nivel)
    {
        Puntaje.Instance.puntos = 0;
        Puntaje.Instance.vidasActuales = Puntaje.Instance.vidas;

        SceneManager.LoadScene(nivel);
    }

    public void MoveNiveles()
    {
        SceneManager.LoadScene("Niveles");
    }

    public void MoveMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ReintentarUltimo()
    {
        Puntaje.Instance.puntos = 0;
        Puntaje.Instance.vidas = Puntaje.Instance.vidasActuales;
        SceneManager.LoadScene(Puntaje.Instance.lastScene);
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
