using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;
public class ClientNetworkTransform : NetworkTransform
{
    bool serverAuthority = true;
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

    public void SetserverAuthority(bool vlaue)
    {
        serverAuthority = vlaue;
    }
}
