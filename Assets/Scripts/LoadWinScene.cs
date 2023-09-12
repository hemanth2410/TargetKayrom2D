using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadWinScene : MonoBehaviour
{
    public async void TestWinLoad()
    {
        await SceneManager.UnloadSceneAsync(2);
        await SceneManager.LoadSceneAsync(3, LoadSceneMode.Additive);
    }
    public async void LoadMenu()
    {
        await SceneManager.UnloadSceneAsync(3);
        await SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }
}
