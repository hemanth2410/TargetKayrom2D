using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

#pragma warning disable CS4014

/// <summary>
///     Lobby orchestrator. I put as much UI logic within the three sub screens,
///     but the transport and RPC logic remains here. It's possible we could pull
/// </summary>
public class LobbyOrchestrator : NetworkBehaviour
{
    [SerializeField] private MainLobbyScreen _mainLobbyScreen;
    [SerializeField] private CreateLobbyScreen _createScreen;
    [SerializeField] private RoomScreen _roomScreen;
    int HostSelectedIndex;
    private void Start()
    {
        _mainLobbyScreen.gameObject.SetActive(true);
        _createScreen.gameObject.SetActive(false);
        _roomScreen.gameObject.SetActive(false);

        CreateLobbyScreen.LobbyCreated += CreateLobby;
        Lobbyroom.LobbySelected += OnLobbySelected;
        RoomScreen.LobbyLeft += OnLobbyLeft;
        RoomScreen.StartPressed += OnGameStart;

        NetworkObject.DestroyWithScene = true;
    }

    #region Main Lobby
    private async void OnLobbySelected(Lobby lobby)
    {
        using (new Load("Joining Lobby..."))
        {
            try
            {
                await MatchMakingService.JoinLobbyWithAllocation(lobby.Id);

                _mainLobbyScreen.gameObject.SetActive(false);
                _roomScreen.gameObject.SetActive(true);

                while (!NetworkManager.Singleton.StartClient())
                    await Task.Yield();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                CanvasUtilities.Instance.ShowError("Failed joining lobby");
            }
        }
    }



    #endregion

    #region Create

    private async void CreateLobby(LobbyData data)
    {
        using (new Load("Creating Lobby..."))
        {
            try
            {
                await MatchMakingService.CreateLobbyWithAllocation(data);

                _createScreen.gameObject.SetActive(false);
                _roomScreen.gameObject.SetActive(true);

                // Starting the host immediately will keep the relay server alive
                NetworkManager.Singleton.StartHost();
                // we can update persistant player data here
                HostSelectedIndex = UnityEngine.Random.Range(0, 8);
                Player player1 = new Player("Anonymous burrito Host", CoinType.Faction1, HostSelectedIndex);
                PersistantPlayerData.Instance.RegisterPlayer1(player1);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                CanvasUtilities.Instance.ShowError("Failed creating lobby");
            }
        }
    }

    #endregion

    #region Room

    private readonly Dictionary<ulong, bool> _playersInLobby = new();
    public static event Action<Dictionary<ulong, bool>> LobbyPlayersUpdated;
    private float _nextLobbyUpdate;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            _playersInLobby.Add(NetworkManager.Singleton.LocalClientId, false);
            UpdateInterface();
        }

        // Client uses this in case host destroys the lobby
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;


    }

    private void OnClientConnectedCallback(ulong playerId)
    {
        if (!IsServer) return;

        // Add locally
        if (!_playersInLobby.ContainsKey(playerId)) _playersInLobby.Add(playerId, false);

        PropagateToClients();
        SyncPlayerData();
        UpdateInterface();
    }

    private void PropagateToClients()
    {
        foreach (var player in _playersInLobby) UpdatePlayerClientRpc(player.Key, player.Value);
    }

    private void SyncPlayerData()
    {
        PlayerStruct ServerHost = new PlayerStruct
        {
            playerName = "Anonymous Buttito Host",
            playerFaction = CoinType.Faction1,
            selectedIndex = HostSelectedIndex
        };
        foreach (var player in _playersInLobby) SyncPlayersClientRpc(ServerHost);
    }
    [ClientRpc]
    private void SyncPlayersClientRpc(PlayerStruct host)
    {
        if (IsServer) return;
        Player player1 = new Player(host.playerName ,host.playerFaction, host.selectedIndex);
        PersistantPlayerData.Instance.RegisterPlayer1(player1);

        if(PersistantPlayerData.Instance.Player1 == null)
        {
            Debug.LogError("Player 1 is null");
        }
        if(PersistantPlayerData.Instance.Player2 == null)
        {
            Debug.LogError("PLayer 2 is null");
        }
    }
    [ClientRpc]
    private void UpdatePlayerClientRpc(ulong clientId, bool isReady)
    {
        if (IsServer) return;

        if (!_playersInLobby.ContainsKey(clientId)) _playersInLobby.Add(clientId, isReady);
        else _playersInLobby[clientId] = isReady;
        UpdateInterface();
    }

    private void OnClientDisconnectCallback(ulong playerId)
    {
        if (IsServer)
        {
            // Handle locally
            if (_playersInLobby.ContainsKey(playerId)) _playersInLobby.Remove(playerId);

            // Propagate all clients
            RemovePlayerClientRpc(playerId);

            UpdateInterface();
        }
        else
        {
            // This happens when the host disconnects the lobby
            _roomScreen.gameObject.SetActive(false);
            _mainLobbyScreen.gameObject.SetActive(true);
            OnLobbyLeft();
        }
    }

    [ClientRpc]
    private void RemovePlayerClientRpc(ulong clientId)
    {
        if (IsServer) return;

        if (_playersInLobby.ContainsKey(clientId)) _playersInLobby.Remove(clientId);
        UpdateInterface();
    }

    public void OnReadyClicked()
    {
        SetReadyServerRpc(NetworkManager.Singleton.LocalClientId);
        if (IsHost) return;
        PlayerStruct player = new PlayerStruct
        {
            playerName = "Anonymous Burrito Client",
            playerFaction = CoinType.Faction2,
            selectedIndex = UnityEngine.Random.Range(0, 8)
        };
        RegisterPlayerServerRpc(player);
        Player player2 = new Player(player.playerName, player.playerFaction, player.selectedIndex);
        PersistantPlayerData.Instance.RegisterPlayer2(player2);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyServerRpc(ulong playerId)
    {
        _playersInLobby[playerId] = true;
        PropagateToClients();
        UpdateInterface();
    }
    [ServerRpc(RequireOwnership = false)]
    private void RegisterPlayerServerRpc(PlayerStruct  player)
    {
        Player player2 = new Player(player.playerName, player.playerFaction, player.selectedIndex);
        PersistantPlayerData.Instance.RegisterPlayer2(player2);
        Debug.Log("Phew Sending off player info to the server");
        if (PersistantPlayerData.Instance.Player1 == null)
        {
            Debug.LogError("Player 1 is null");
        }
        if (PersistantPlayerData.Instance.Player2 == null)
        {
            Debug.LogError("Player 2 is null");
        }
    }

    private void UpdateInterface()
    {
        LobbyPlayersUpdated?.Invoke(_playersInLobby);
    }

    private async void OnLobbyLeft()
    {
        using (new Load("Leaving Lobby..."))
        {
            _playersInLobby.Clear();
            NetworkManager.Singleton.Shutdown();
            await MatchMakingService.LeaveLobby();
        }
    }

    public override void OnDestroy()
    {

        base.OnDestroy();
        CreateLobbyScreen.LobbyCreated -= CreateLobby;
        Lobbyroom.LobbySelected -= OnLobbySelected;
        RoomScreen.LobbyLeft -= OnLobbyLeft;
        RoomScreen.StartPressed -= OnGameStart;

        // We only care about this during lobby
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
        }

    }

    private async void OnGameStart()
    {
        using (new Load("Starting the game..."))
        {
            await MatchMakingService.LockLobby();
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }

    #endregion
}
