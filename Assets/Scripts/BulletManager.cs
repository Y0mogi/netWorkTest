using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletManager : NetworkBehaviour
{
    [SerializeField] private NetworkObject bulletPrefab;
    private List<GameObject> gameObjects = new List<GameObject>();

    public void GenerateBullet(GameObject bullet)
    {
        gameObjects.Add(bullet);
    }

    public void DeleteBullet(GameObject bullet)
    {
        var network = bullet.GetComponent<NetworkObject>();
        network.Despawn();
        gameObjects.Remove(bullet);
    }

    //簡易的なシングルトン
    private static BulletManager instance;

    [System.Obsolete]
    public static BulletManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (BulletManager)FindObjectOfType(typeof(BulletManager));
            }

            return instance;
        }
    }
}
