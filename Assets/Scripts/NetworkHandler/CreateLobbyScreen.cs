using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreateLobbyScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_nameInput, m_MaxPlayers;

    private void Start()
    {
        m_MaxPlayers.text = "2";
    }
    public static event Action<LobbyData> LobbyCreated;
    public void OnCreateClicked()
    {
        var lobbyData = new LobbyData
        {
            Name = m_nameInput.text,
            MaxPlayers = 2
        };
        LobbyCreated?.Invoke(lobbyData);
    }
}
public struct LobbyData
{
    public string Name;
    public int MaxPlayers;
}