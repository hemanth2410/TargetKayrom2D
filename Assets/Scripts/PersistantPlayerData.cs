using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistantPlayerData : MonoBehaviour
{
    private static PersistantPlayerData instance;

    public static PersistantPlayerData Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<PersistantPlayerData>();
            }
            return instance;
        }
    }
    public Player Player1 { get { return player1; } }
    public Player Player2 { get { return player2; } }

    Player player1;
    Player player2;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        Debug.LogWarning("Persistant player data is reloaded, causing scene load ");
        loadSceneAditive(1);
    }
    public void RegisterPlayer1(Player player)
    {
        player1 = player;
    }
    public void RegisterPlayer2(Player player)
    {
        player2 = player;
    }

    void loadSceneAditive(int index)
    {
        SceneManager.LoadScene(index, LoadSceneMode.Additive);
    }
}

public class Player
{
    public string PlayerName { get { return playerName; } }
    public int SelectedIndex { get { return  selectedIndex; } }
    public CoinType PlayerFaction { get { return playerFaction; } }
    public bool PlayerWon { get { return playerWon; } }
    public bool IsHost { get { return isHost; } }
    public Sprite PlayerSprite { get { return playerSprite; } }
    private string playerName;
    private int selectedIndex;
    private CoinType playerFaction;
    private bool playerWon;
    bool isHost;
    Sprite playerSprite;

    public Player(string playerName, CoinType playerFaction, int selectedIndex)
    {
        this.playerName = playerName; this.playerFaction = playerFaction; this.selectedIndex = selectedIndex;
    }
    public Player(string playerName, CoinType playerFaction, int selectedIndex,bool isServer, Sprite playerSprite)
    {
        this.playerName = playerName; this.playerFaction = playerFaction; this.selectedIndex = selectedIndex; isHost = isServer;
        this.playerSprite = playerSprite;
    }
    public void setPlayerState(bool winState)
    {
        playerWon = winState;
    }
}
struct PlayerStruct : INetworkSerializable
{
    public string playerName;
    public bool isHost;
    public CoinType playerFaction;
    public int selectedIndex;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref isHost);
        serializer.SerializeValue(ref playerFaction);
        serializer.SerializeValue(ref selectedIndex);
    }
}
// This is an offline class
// both classes should be synchronized once the players are logged in
// so there should be a network variable handling this stuff
// once both are done, we need to have a network variable controlling turn of each player
// locking out host and client when its other player's turn
// ezeeee
// a perfect place would be after authentication by sending RPCs to handle data synchronization