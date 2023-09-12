using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum CurrentPlayer { player1, player2 }


public class PlayerAvaterHandlerOffline : MonoBehaviour
{
    [SerializeField] Sprite[] m_DefaultAvatars;
    [SerializeField] GameObject m_AvatarButtonPrefab;
    [SerializeField] Transform m_DefaultAvatarsHolder;

    [Header("PLayer 1 Data")]
    [SerializeField] Image m_Player1AvatarImage;
    [SerializeField] TextMeshProUGUI m_Player1Name;

    [Header("Player 2 Data")]
    [SerializeField] Image m_Player2AvatarImage;
    [SerializeField] TextMeshProUGUI m_Player2Name;
    // this data will be used to construct a new faction for game to run
    int player1Index;
    int player2Index;
    List<GameObject> instantiatedButtons = new List<GameObject>();
    CurrentPlayer player;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_DefaultAvatars.Length; i++)
        {
            var k = Instantiate(m_AvatarButtonPrefab, m_DefaultAvatarsHolder);
            k.GetComponent<ProfileAvatar>().SetAvatarInfo(m_DefaultAvatars[i], i,true);
            instantiatedButtons.Add(k);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerAvatar(int index)
    {
        switch(player)
        {
            case CurrentPlayer.player1:
                m_Player1AvatarImage.sprite = m_DefaultAvatars[index];
                player1Index = index;
                break;
            case CurrentPlayer.player2:
                m_Player2AvatarImage.sprite = m_DefaultAvatars[index];
                player2Index = index;
                break;
        }
    }

    public void PerformDelelect()
    {
        for (int i = 0; i < instantiatedButtons.Count; i++)
        {
            instantiatedButtons[i].GetComponent<ProfileAvatar>().Deselect();
        }
    }

    public void SetPlayer1()
    {
        player = CurrentPlayer.player1;
    }
    public void SetPlayer2()
    {
        player = CurrentPlayer.player2;
    }

    internal void SetSelectedIndex(int v)
    {
        switch (player)
        {
            case CurrentPlayer.player1:
                player1Index = 0;
                m_Player1AvatarImage.sprite = m_DefaultAvatars[0];
                break;
            case CurrentPlayer.player2:
                player2Index = 0;
                m_Player2AvatarImage.sprite = m_DefaultAvatars[0];
                break;
        }
    }

    public async void PerformPlayerRegistration()
    {
        Player player1 = new Player(m_Player1Name.text, CoinType.Faction1, player1Index);
        Player player2 = new Player(m_Player2Name.text, CoinType.Faction2, player2Index);
        PersistantPlayerData.Instance.RegisterPlayer1(player1);
        PersistantPlayerData.Instance.RegisterPlayer2(player2);
        await SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive);
        await SceneManager.UnloadSceneAsync(1);
    }
}
