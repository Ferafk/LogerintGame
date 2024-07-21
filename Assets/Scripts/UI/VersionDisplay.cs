using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionDisplay : MonoBehaviour
{
    private TextMeshProUGUI versionText;

    private void Start()
    {
        versionText = GetComponent<TextMeshProUGUI>();
        string version = Application.version;
        versionText.text = "V." + version;
    }

}
