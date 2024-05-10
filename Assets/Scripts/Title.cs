using Steamworks;
using TMPro;
using UnityEngine;

public class Title : MonoBehaviour
{
    [SerializeField]
    TMP_InputField m_joinLobbyID;

    public void StartHost()
    {
        //���r�[�쐬
        SteamLobby.Instance.CreateLobby();
    }

    public void StartClient()
    {
        //���r�[����
        SteamLobby.Instance.JoinLobby((CSteamID)ulong.Parse(m_joinLobbyID.text));
    }
}
