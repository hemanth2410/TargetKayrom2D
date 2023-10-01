using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHelperSIngletom : MonoBehaviour
{
    static NetworkHelperSIngletom instance;
    public static NetworkHelperSIngletom Instance => instance ?? (instance = FindAnyObjectByType<NetworkHelperSIngletom>());

    // public events
    public event Action InitializeRelayService;

    public void InvokeRelayInitialization()
    {
        InitializeRelayService ?.Invoke();
    }
}
