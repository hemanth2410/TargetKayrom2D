using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class NetworkMessenger : NetworkBehaviour
{
    Dictionary<ulong, NetworkClient> connectedClients = new();
    GameManager gameManager;
    [SerializeField] CoinType requestedFaction;
    [SerializeField] NetworkObject board;

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
            board.ChangeOwnership(1);
        }
        if(targetFaction == CoinType.Faction1)
        {
            board.RemoveOwnership();
        }
        propagateChangesToClients(requestedFaction);
    }
}
