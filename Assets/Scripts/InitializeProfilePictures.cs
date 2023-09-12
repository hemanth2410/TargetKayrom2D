using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InitializeProfilePictures : MonoBehaviour
{
    [SerializeField] GameObject m_ProfilePictureAvatarPrefab;
    [SerializeField] Sprite[] m_DefaultAvatarSprites;
    [SerializeField] Image m_ProfileDisplayPicture;
    [SerializeField] Transform m_AvatarPicturesContainer;
    [SerializeField] TextMeshProUGUI m_ProfileNameText;
    [SerializeField] Button m_createAccountButton;
    List<GameObject> instantiatedButtons = new List<GameObject>();

    int selectedIndex;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_DefaultAvatarSprites.Length; i++)
        {
            var k = Instantiate(m_ProfilePictureAvatarPrefab, m_AvatarPicturesContainer);
            k.GetComponent<ProfileAvatar>().SetAvatarInfo(m_DefaultAvatarSprites[i], i);
            instantiatedButtons.Add(k);
        }
    }
    private void Update()
    {
        m_createAccountButton.interactable = (m_ProfileNameText.text.Length > 1);
    }
    public void PerformDelelect()
    {
        for (int i = 0; i < instantiatedButtons.Count; i++)
        {
            instantiatedButtons[i].GetComponent<ProfileAvatar>().Deselect();
        }
    }
    public void SetSelectedAvatar(int  index)
    {
        selectedIndex = index;
        m_ProfileDisplayPicture.sprite = m_DefaultAvatarSprites[index];
    }
    public void UpdatePlayerName()
    {
        GetComponentInParent<AuthenticationHandler>().UpdatePlayerName(m_ProfileNameText.text);
    }
   
}
