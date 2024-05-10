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
    private void FixedUpdate()
    {
        direction.y = 0;
        transform.Translate(direction * move * Time.fixedDeltaTime);
        count++;

        if (count > 50)
        {
            Destroy(this.gameObject);
        }

    }
}