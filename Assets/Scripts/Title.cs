using Steamworks;
using TMPro;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField]
    TMP_InputField m_joinLobbyID;

    public void StartHost()
    {
        //ロビー作成
        SteamLobby.Instance.CreateLobby();
    }

    public void StartClient()
    {
        //ロビー入室
        SteamLobby.Instance.JoinLobby((CSteamID)ulong.Parse(m_joinLobbyID.text));
    }
}
