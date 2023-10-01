using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AuthenticationManagerSimple : MonoBehaviour
{
    public async void LoginAnonymously()
    {
        using (new Load("Logging you in..."))
        {
            await AuthenticationService.Login();
            // perfect place for Persistant player data
            // not really
            SceneManager.LoadSceneAsync("Lobby");
        }
    }
}
