using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SocialPlatforms.Impl;

public class Puntaje : MonoBehaviour
{
    public static Puntaje Instance { get; private set; }

    public int puntos;
    public int vidas;
    public int vidasActuales;

    public string lastScene;
    public int desbloquear = 0;
    public int diamantes { get; private set; }
    public Dictionary<int, bool> Diamonds { get; private set; } = new Dictionary<int, bool>
    {
        {01, false },
        {02, false },
        {03, false },
        {04, false },
        {05, false }
    };

    public int total = 10;

    [SerializeField] private TextMeshProUGUI monedaTexto;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            LoadDiamondsState();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AgregarMoneda(int cantidad)
    {
        puntos += cantidad;
        ActualizarMonedaTexto();
    }

    public void AgregarVida()
    {
        vidas += 1;
    }

    public void QuitarVida()
    {
        vidas -= 1;
    }

    public void AgregarDiamante()
    {
        diamantes += 1;
    }

    public void QuitarMoneda(int cantidad)
    {
        puntos = Mathf.Max(0, puntos - cantidad);
        ActualizarMonedaTexto();
    }

    private void ActualizarMonedaTexto()
    {
        if (monedaTexto != null)
        {
            monedaTexto.text = $"{puntos}";
        }
        else
        {
            monedaTexto = GameObject.FindGameObjectWithTag("PuntajeTxt").GetComponent<TextMeshProUGUI>();

            if (monedaTexto == null) return;

            monedaTexto.text = $"{puntos}/{total}";
        }

    }

    public void CollectDiamond(int id)
    {
        if (Diamonds.ContainsKey(id))
        {
            Diamonds[id] = true;
            SaveDiamondsState();
        }
    }

    public bool IsDiamondCollected(int id)
    {
        return Diamonds.ContainsKey(id) && Diamonds[id];
    }

    public int CollectedDiamondsCount
    {
        get
        {
            int count = 0;
            foreach (var diamond in Diamonds.Values)
            {
                if (diamond) count++;
            }
            return count;
        }
    }

    public void ResetAllData()
    {
        puntos = 0;
        vidas = 2;
        diamantes = 0;
        foreach (var key in Diamonds.Keys.ToList())
        {
            Diamonds[key] = false;
        }
        SaveDiamondsState();
    }

    private void LoadDiamondsState()
    {
        int diamondCount = PlayerPrefs.GetInt("DiamondCount", 0);
        for (int i = 0; i < diamondCount; i++)
        {
            int id = PlayerPrefs.GetInt($"Diamond_{i}_ID", -1);
            bool collected = PlayerPrefs.GetInt($"Diamond_{i}_Collected", 0) == 1;
            if (id != -1)
            {
                Diamonds[id] = collected;
            }
        }
    }

    private void SaveDiamondsState()
    {
        PlayerPrefs.SetInt("DiamondCount", Diamonds.Count);
        int index = 0;
        foreach (var diamond in Diamonds)
        {
            PlayerPrefs.SetInt($"Diamond_{index}_ID", diamond.Key);
            PlayerPrefs.SetInt($"Diamond_{index}_Collected", diamond.Value ? 1 : 0);
            index++;
        }
        PlayerPrefs.Save();
    }

}
