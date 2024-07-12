using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockLevels : MonoBehaviour
{
    [SerializeField] private Button[] unlock;
    private int _desbloquear;

    public void Unlock()
    {
        string lastLevel = Puntaje.Instance.lastScene;
        char unlockedLevels = lastLevel[5];
        int numero = (unlockedLevels - '0');

        _desbloquear = Puntaje.Instance.desbloquear;
        if (numero > _desbloquear)
        {
            _desbloquear = numero;
        }
        Puntaje.Instance.desbloquear = _desbloquear;

        for (int i = 0; i < _desbloquear; i++)
        {
            unlock[i].interactable = true;
        }
    }

}
