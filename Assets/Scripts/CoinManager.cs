using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinManager : NetworkBehaviour
{
    //�R�C���̃v���n�u
    [SerializeField] private NetworkObject m_coinPrefab;
    //���������R�C���̃��X�g
    private List<GameObject> m_coinObjects = new List<GameObject>();

    //�X�|�[�����ꂽ�Ƃ�
    public override void OnNetworkSpawn()
    {
        //�z�X�g�̏ꍇ
        if (IsHost)
        {
            GenerateCoin();
        }
    }

    // �R�C������
    public void GenerateCoin()
    {
        //�R�C������
        for (int x = 0; x < 10; x++)
        {
            NetworkObject coin = Instantiate(m_coinPrefab);
            int posX = UnityEngine.Random.Range(0, 10) - 5;
            int posZ = UnityEngine.Random.Range(0, 10) - 5;
            coin.transform.position = new Vector3(posX, 0, posZ);
            coin.Spawn();
            m_coinObjects.Add(coin.gameObject);
        }
    }

    //�ȈՓI�ȃV���O���g��
    private static CoinManager instance;

    [System.Obsolete]
    public static CoinManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (CoinManager)FindObjectOfType(typeof(CoinManager));
            }

            return instance;
        }
    }

    // �R�C���폜�i�z�X�g����Ăяo�����j
    public void DeleteCoin(GameObject coinObj)
    {
        var network = coinObj.GetComponent<NetworkObject>();
        network.Despawn();
        m_coinObjects.Remove(coinObj);
        //�S����������R�C���Đ���
        if (m_coinObjects.Count == 0)
        {
            GenerateCoin();
        }
    }

}
