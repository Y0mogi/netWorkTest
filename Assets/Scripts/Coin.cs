using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(IsServer)
        {
            transform.Rotate(Vector3.up, Space.World);
        }
    }
}
