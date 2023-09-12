using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenDetails : MonoBehaviour
{
    [SerializeField] Sprite[] m_Avatars;
    [SerializeField] TextMeshProUGUI m_WinnerName;
    [SerializeField] Image m_ProfilePicture;
    LoadWinScene loadWinScene;
    // Start is called before the first frame update
    void Start()
    {
        m_WinnerName.text = PersistantPlayerData.Instance.Player1.PlayerWon ? PersistantPlayerData.Instance.Player1.PlayerName : PersistantPlayerData.Instance.Player2.PlayerName;
        m_ProfilePicture.sprite = m_Avatars[PersistantPlayerData.Instance.Player1.PlayerWon ? PersistantPlayerData.Instance.Player1.SelectedIndex : PersistantPlayerData.Instance.Player2.SelectedIndex];
        StartCoroutine(LoadMenu());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator LoadMenu()
    {
        yield return new WaitForSecondsRealtime(60.0f);
        loadWinScene.LoadMenu();
    }
}
