using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerData : MonoBehaviour
{
    static NetworkPlayerData instance;

    public static NetworkPlayerData Instance
    {
        get { if(instance == null)
            {
                instance = FindAnyObjectByType<NetworkPlayerData>();
            }
            return instance;
        }
    }

    bool isHost;
    Sprite playerProfile;
    string playerName;

    public bool IsHost { get { return isHost; } }
    public string Name { get { return playerName; } }
    public Sprite PlayerProfile { get { return playerProfile; } }

    public void LoadPlayerData(bool isHost, string playerName, Sprite playerProfile)
    {
        this.isHost = isHost;
        this.playerName = playerName;
        this.playerProfile = playerProfile;
    }
}
