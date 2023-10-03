//using ParrelSync;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    private async void Awake()
    {
        var options = new InitializationOptions();
//#if UNITY_EDITOR
//        options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
//#endif
        await UnityServices.InitializeAsync(options);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
