using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class RoomScreen : MonoBehaviour
{
    [SerializeField] private LobbyPlayerPanel m_LobbyPlayerPanelPrefab;
    [SerializeField] private Transform m_PlayerParent;
    [SerializeField] TMP_Text m_WaitingText;
    [SerializeField] GameObject m_StartButton, m_ReadyButton;

    private readonly List<LobbyPlayerPanel> playerPanels = new();
    private bool allReady;
    private bool ready;

    public static event Action StartPressed;

    private void OnEnable()
    {
        foreach (Transform child in m_PlayerParent) Destroy(child.gameObject);
        playerPanels.Clear();

        LobbyOrchestrator.LobbyPlayersUpdated += NetworkLobbyPlayersUpdated;
        MatchMakingService.CurrentLobbyRefreshed += OnCurrentLobbyRefreshed;
        m_StartButton.SetActive(false);
        m_ReadyButton.SetActive(false);
        ready = false;
    }

    private void OnDisable()
    {
        LobbyOrchestrator.LobbyPlayersUpdated -= NetworkLobbyPlayersUpdated;
        MatchMakingService.CurrentLobbyRefreshed -= OnCurrentLobbyRefreshed;
    }
    public static event Action LobbyLeft;
    public void OnLeaveLobby()
    {
        LobbyLeft?.Invoke();
    }
    private void NetworkLobbyPlayersUpdated(Dictionary<ulong, bool> players)
    {
        var allActivePlayerIds = players.Keys;

        // Remove all inactive panels
        var toDestroy = playerPanels.Where(p => !allActivePlayerIds.Contains(p.PlayerId)).ToList();
        foreach (var panel in toDestroy)
        {
            playerPanels.Remove(panel);
            Destroy(panel.gameObject);
        }

        foreach (var player in players)
        {
            var currentPanel = playerPanels.FirstOrDefault(p => p.PlayerId == player.Key);
            if (currentPanel != null)
            {
                if (player.Value) currentPanel.SetReady();
            }
            else
            {
                var panel = Instantiate(m_LobbyPlayerPanelPrefab, m_PlayerParent);
                panel.Init(player.Key);
                playerPanels.Add(panel);
            }
        }

        m_StartButton.SetActive(NetworkManager.Singleton.IsHost && players.All(p => p.Value));
        m_ReadyButton.SetActive(!ready);
    }

    private void OnCurrentLobbyRefreshed(Lobby lobby)
    {
        m_WaitingText.text = $"Waiting on players... {lobby.Players.Count}/{lobby.MaxPlayers}";
    }
    public void OnReadyClicked()
    {
        m_ReadyButton.SetActive(false);
        ready = true;
    }

    public void OnStartClicked()
    {
        StartPressed?.Invoke();
    }
}
