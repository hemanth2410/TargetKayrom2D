using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;

public class AuthenticationHandler : MonoBehaviour
{
    [SerializeField] GameObject m_MainMenuObject;
    [SerializeField] GameObject m_AccountSetupObject;
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private async void Start()
    {
        // check for cached players
        await signinCachedPlayer();
    }

    async Task signinCachedPlayer()
    {
        // checking if a session token exists
        if (!AuthenticationService.Instance.SessionTokenExists)
        {
            // session token doesnt exist any more
            return;
        }
        // perfect place to perform a anonymous signin (Background signin)
        // later we can call signinCachedPlayer
        await signInAnonymouslyAsync();
    }

    public async void PerformAnonumousSignin()
    {
        await signInAnonymouslyAsync();
    }

    void CheckForPlayerDetails()
    {
        if(string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
        {
            m_AccountSetupObject.SetActive(true);
        }
    }

    public async void UpdatePlayerName(string playerName)
    {
        await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
    }

    async Task signInAnonymouslyAsync()
    {
        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Signed in anonymously");
            Debug.Log($"PLayer ID : {AuthenticationService.Instance.PlayerId}");
            m_MainMenuObject.SetActive(false);
            CheckForPlayerDetails();
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError(ex);
        }
        catch (RequestFailedException ex)
        {
            Debug.LogError(ex);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}
