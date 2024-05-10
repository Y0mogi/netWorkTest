using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Copy : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyIdText;
    public void OnClick()
    {
        GUIUtility.systemCopyBuffer = lobbyIdText.text;
    }
}
