using TMPro;
using UnityEngine;

public class LobbyID : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_lobbyIdText;

    private void Start()
    {
        //���r�[�쐬or�������ɋL�����Ă�����LobbyID��ݒ�
        m_lobbyIdText.text = SteamLobby.Instance.LobbyID.ToString();
    }
}