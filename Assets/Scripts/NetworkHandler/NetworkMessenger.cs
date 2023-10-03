using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Text;
using DG.Tweening;
using Unity.Collections;

public class NetworkMessenger : NetworkBehaviour
{
    Dictionary<ulong, NetworkClient> connectedClients = new();
    GameManager gameManager;
    [SerializeField] CoinType requestedFaction;
    [SerializeField] NetworkObject[] ObjectsToChangeOwnership;
    [SerializeField] bool IsOwnedByLocalClient;
    [SerializeField] bool IsOwnedByServer;
    // Start is called before the first frame update
    void Start()
    {
        requestedFaction = CoinType.Striker;
        gameManager = GetComponent<GameManager>();
        foreach(var client in NetworkManager.Singleton.ConnectedClients)
        {
            connectedClients[client.Key] = client.Value;
            Debug.Log("Client Key : " +  client.Key + " Connected client value : " +client.Value.ClientId);
        }
    }

    public void GameOverEvent(CoinType winningFaction)
    {
        processWinEventServerRpc(winningFaction);
    }
    [ServerRpc]
    void processWinEventServerRpc(CoinType winningFaction)
    {
        processScoreOnClinent(winningFaction);
    }

    void processScoreOnClinent(CoinType winningFaction)
    {
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            setWinnginClientRpc(winningFaction);
        }
    }

    [ClientRpc]
    void setWinnginClientRpc(CoinType winningFaction)
    {
        if (PersistantPlayerData.Instance.Player1.PlayerFaction == winningFaction)
        {
            PersistantPlayerData.Instance.Player1.setPlayerState(true);
        }
        if (PersistantPlayerData.Instance.Player2.PlayerFaction == winningFaction)
        {
            PersistantPlayerData.Instance.Player2.setPlayerState(true);
        }
        if(IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("WinScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public void RegisterScoreUpdatedEvent(CoinType faction, int value, string gameobjectName)
    {
        FixedString128Bytes _name = gameobjectName;
        ProcessScoresServerRpc(faction, value, _name);
    }

    [ServerRpc(RequireOwnership =false)]
    void ProcessScoresServerRpc(CoinType faction, int value, FixedString128Bytes gameobjectName)
    {
        PropagateScoreChangesToClients(faction, value, gameobjectName);
    }

    void PropagateScoreChangesToClients(CoinType faction, int value, FixedString128Bytes name)
    {
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            UpdateScoreOnClientRpc(faction, value, name);
        }
    }
    [ClientRpc]
    void UpdateScoreOnClientRpc(CoinType faction, int value, FixedString128Bytes name)
    {
        GameController.Instance.InvokeScoreEvent(faction, value);
        var k = GameObject.Find(name.ToString());
        Destroy(k.GetComponent<Coin>());
        // Remove the ghost coin corresponding to this coin
    }


    public void SendTurnChangeToserver(CoinType targetFaction)
    {
        ulong localClientId = 0;
        if(IsClient && !IsHost)
        {
            localClientId = 0;
        }
        if(IsHost)
        {
            localClientId = 1;
        }
        Debug.Log("Target client ID : " + NetworkManager.Singleton.LocalClientId);
        updateTurnOnServerRpc(targetFaction);
    }

    void propagateChangesToClients(CoinType targetFaction)
    {
        if (!IsServer) return;
        foreach(var client in NetworkManager.Singleton.ConnectedClients)
        {
            Debug.Log("Propagating changes to clients");
            updateTurnOnClientRpc(targetFaction);
        }
    }


    [ClientRpc]
    void updateTurnOnClientRpc(CoinType targetFaction)
    {
        // Request Game manager to switch the Turn
        gameManager.SetCurrentFaction(targetFaction);
        //striker.GetComponent<ClientNetworkTransform>().SetserverAuthority(targetFaction == CoinType.Faction1 ? true : false);
    }

    [ServerRpc(RequireOwnership = false)]
    void updateTurnOnServerRpc(CoinType targetFaction, ServerRpcParams serverRpcParams = default)
    {
        // Request Switch turn on game manager. and propogate changes to clients.
        // server receives the message, process it and send to client
        requestedFaction = targetFaction;
        var clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log(clientId + " is the sender");
        if(targetFaction == CoinType.Faction2)
        {
            changeOwnership(false);
        }
        if(targetFaction == CoinType.Faction1)
        {
            changeOwnership(true);
        }
        propagateChangesToClients(requestedFaction);
    }

    void changeOwnership(bool value)
    {
        for (int i = 0; i < ObjectsToChangeOwnership.Length; i++)
        {
            if(value)
            {
                ObjectsToChangeOwnership[i].RemoveOwnership();
            }
            else
            {
                ObjectsToChangeOwnership[i].ChangeOwnership(1);
            }
        }
    }

    public void CoinToPlaceRpcMessage(CoinType targetFaction, string gameobjectName)
    {
        FixedString128Bytes _name = gameobjectName;
        RegisterContToPlaceOnServerRpc(targetFaction, _name);
    }

    [ServerRpc(RequireOwnership = false)]
    void RegisterContToPlaceOnServerRpc(CoinType targetFaction, FixedString128Bytes targetGameObjectName)
    {
        // Trigger change of ownership
        if (targetFaction == CoinType.Faction2)
        {
            changeOwnership(false);
        }
        if (targetFaction == CoinType.Faction1)
        {
            changeOwnership(true);
        }
        updateClientsOnCoinToPlace(targetFaction, targetGameObjectName);
    }

    void updateClientsOnCoinToPlace(CoinType targetFaction, FixedString128Bytes targetGameObjectName)
    {
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            updateCoinToPlaceOnClientRpc(targetFaction, targetGameObjectName);
        }
    }

    [ClientRpc]
    void updateCoinToPlaceOnClientRpc(CoinType targetFaction, FixedString128Bytes targetGameObjectName)
    {
        // Now Invoke a public method on GameManager and update the server requesting to switch message on the client side too.
        gameManager.SetCoinToPlaceMessage(targetFaction, targetGameObjectName.ToString());
    }

    public void UpdateCoinsOnServer(List<string> ghostNames, List<string> puckedCoinNames)
    {
        FixedString128Bytes[] _ghostNames = new FixedString128Bytes[ghostNames.Count];
        FixedString128Bytes[] _puckedCoinNames = new FixedString128Bytes[puckedCoinNames.Count];
        for (int i = 0; i < _ghostNames.Length; i++)
        {
            _ghostNames[i] = ghostNames[i];
        }
        for(int j = 0; j <  _puckedCoinNames.Length; j ++)
        {
            _puckedCoinNames[j] = puckedCoinNames[j];
        }
         // Invoke an RPC
         updateCoinsServerRpc(_ghostNames, _puckedCoinNames);
    }
    [ServerRpc(RequireOwnership =false)]
    void updateCoinsServerRpc(FixedString128Bytes[] ghosts, FixedString128Bytes[] pucked)
    {
        propogateCoinChanges(ghosts, pucked);
    }

    void propogateCoinChanges(FixedString128Bytes[] ghosts, FixedString128Bytes[] pucked)
    {
        for(int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            reflectCoinChangesClientRpc(ghosts, pucked);
        }
    }
    [ClientRpc]
    void reflectCoinChangesClientRpc(FixedString128Bytes[] ghosts, FixedString128Bytes[] pucked)
    {
        List<string> _ghostNames = new List<string>();
        List<string> _PuckedNames = new List<string>();
        for (int i = 0; i < ghosts.Length; i++)
        {
            _ghostNames.Add(ghosts[i].ToString());
            _PuckedNames.Add(pucked[i].ToString());
        }
        gameManager.DestroyGhostCoin(_ghostNames);
        gameManager.DestroyPuckedCoins(_PuckedNames);
    }
}


