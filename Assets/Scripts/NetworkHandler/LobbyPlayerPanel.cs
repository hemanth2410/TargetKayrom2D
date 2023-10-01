using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text m_NameText, m_StatusText;

    public ulong PlayerId { get; private set; }

    public void Init(ulong playerId)
    {
        PlayerId = playerId;
        m_NameText.text = $"Player {PlayerId}";
    }

    public void SetReady()
    {
        m_StatusText.text = "Ready";
        m_StatusText.color = Color.green;
    }
}
