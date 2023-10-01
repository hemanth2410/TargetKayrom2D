using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClientControl : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ClientNetworkTransform>().SetserverAuthority(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner)
        {
            transform.Translate(Vector3.up * Time.deltaTime);
        }
    }
}
