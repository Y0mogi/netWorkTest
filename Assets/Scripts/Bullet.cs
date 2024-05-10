using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float move = 1;
    public Vector3 direction;
    private int count;
    int ID;

    public void Init(int id)
    {
        ID = id;
    }

    [System.Obsolete]
    private void FixedUpdate()
    {
        direction.y = 0;
        transform.Translate(direction * move * Time.fixedDeltaTime);
        count++;

        if (!IsOwner) return;
        if(count > 50)
        {
            BulletManager.Instance.DeleteBullet(this.gameObject);
        }
    }
}