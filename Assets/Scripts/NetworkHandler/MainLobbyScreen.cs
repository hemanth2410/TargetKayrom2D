using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainLobbyScreen : MonoBehaviour
{
    [SerializeField] private Lobbyroom m_LobbyRoomPrefab;
    [SerializeField] private Transform m_LobbyParent;
    [SerializeField] private GameObject m_NoLobbiesText;
    [SerializeField] private float m_LobbyRefreshRate;

    private readonly List<Lobbyroom> currentLobbySpawns = new();
    private float nextRefreshTime;

    private void Update()
    {
        if (Time.time >= nextRefreshTime) FetchLobbies();
    }

    private void OnEnable()
    {
        foreach (Transform child in m_LobbyParent) Destroy(child.gameObject);
        currentLobbySpawns.Clear();
    }

    private async void FetchLobbies()
    {
        try
        {
            nextRefreshTime = Time.time + m_LobbyRefreshRate;
            var allLobbies = await MatchMakingService.GatherLobbies();

            var lobbyIds = allLobbies.Where(l => l.HostId != AuthenticationService.PlayerID).Select(l => l.Id);
            var notActive = currentLobbySpawns.Where(l => !lobbyIds.Contains(l.Lobby.Id)).ToList();
            foreach(var lobby in allLobbies)
            {
                var current = currentLobbySpawns.FirstOrDefault(p => p.Lobby.Id == lobby.Id);
                if (current != null)
                {
                    current.UpdateDetails(lobby);
                }
                else
                {
                    var panel = Instantiate(m_LobbyRoomPrefab, m_LobbyParent);
                    panel.Init(lobby);
                    currentLobbySpawns.Add(panel);
                }
            }
            m_NoLobbiesText.SetActive(!currentLobbySpawns.Any());
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }
}
