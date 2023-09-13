using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class FacebookLiner : MonoBehaviour
{
    public string Token;
    public string Error;

    private void Awake()
    {
        if(!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }
    }
    void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Factbook SDK");
        }
    }
    void OnHideUnity(bool isGameShown)
    {
        if(!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1.00f;
        }
    }

    public void Login()
    {
        var perms = new List<string>() { "public_Profile", "email" };
        FB.LogInWithReadPermissions(perms, result =>
        {
            if(FB.IsLoggedIn)
            {
                Token = AccessToken.CurrentAccessToken.TokenString;
                Debug.Log($"Facebook loginToken : {Token}");
            }
            else
            {
                Error = "User canclled login";
                Debug.Log("[Facebool Login] User canclled login");
            }
        });
    }

    // Signin a returning player or create a new player

    async Task SigninWithFacebookAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
            Debug.Log("Signin Succeeded");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch(RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }

    // Update a player from anonymous to facebool account
    async Task LinkWithFacebookAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithFacebookAsync(accessToken);
            Debug.Log("Upgrade success Anonymous -> Facebook");
        }
        catch(AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.Log("Account is already linked with facebook");
        }
        catch(AuthenticationException ex)
        {
            Debug.LogException(ex);
        }
        catch(RequestFailedException ex)
        {
            Debug.LogException(ex);
        }
    }
    // Generate an access token from facebook with current user
    // Use the id to mostly Link the Anonymous account with Facebook account
    // Link Account WIth Facebook is the method we will probably use.

}
