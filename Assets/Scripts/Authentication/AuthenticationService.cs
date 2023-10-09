
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
#if UNITY_EDITOR
using ParrelSync;
#endif
public static class AuthenticationService
{
   public static string PlayerID { get; private set; }
    public static async Task Login()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            var options = new InitializationOptions();
#if UNITY_EDITOR
            if (ClonesManager.IsClone()) options.SetProfile(ClonesManager.GetArgument());
            else options.SetProfile("Primary");
#endif
            await UnityServices.InitializeAsync(options);
        }
        if(!Unity.Services.Authentication.AuthenticationService.Instance.IsSignedIn)
        {
            await Unity.Services.Authentication.AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerID = Unity.Services.Authentication.AuthenticationService.Instance.PlayerId;

        }
    }
}
