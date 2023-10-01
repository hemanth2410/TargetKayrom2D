using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using System;

public class Lobbyroom : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_LobbyNameText, m_PLayerCountText;
    public Lobby Lobby { get;private set; }
    public static event Action<Lobby> LobbySelected;
    
    public void Init(Lobby lobby)
    {
        UpdateDetails(lobby);
    }

    public void UpdateDetails(Lobby lobby)
    {
        Lobby = lobby;
        m_LobbyNameText.text = lobby.Name;
        m_PLayerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
        int GetValue(string key)
        {
            return int.Parse(lobby.Data[key].Value);
        }
    }
    public void Clicked()
    {
        LobbySelected?.Invoke(Lobby);
    }
}
