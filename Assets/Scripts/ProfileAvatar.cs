using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
public class ProfileAvatar : MonoBehaviour
{
    [SerializeField] Image m_ProfilePictureImage;
    [SerializeField] GameObject m_HighlightImage;
    int index;
    bool iSOnline;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        Deselect();
    }
    public void SetAvatarInfo(Sprite sprite, int _index)
    {
        m_ProfilePictureImage.sprite = sprite;
        index = _index;
        Button _button = GetComponent<Button>();
        _button.onClick.AddListener(() => PerformSelection());
    }
    /// <summary>
    /// this will be depricated and this is for testing purpose only.
    /// </summary>
    /// <param name="sprite"></param>
    /// <param name="_index"></param>
    /// <param name="offlineMode"></param>
    public void SetAvatarInfo(Sprite sprite, int _index, bool offlineMode)
    {
        m_ProfilePictureImage.sprite = sprite;
        index = _index;
        iSOnline = !offlineMode;
        Button _button = GetComponent<Button>();
        _button.onClick.AddListener(() => PerformSelectionOnPlayer());
    }
    public void Deselect()
    {
        m_HighlightImage.SetActive(false);
        if (iSOnline)
            GetComponentInParent<InitializeProfilePictures>().SetSelectedAvatar(0);
        else
            GetComponentInParent<PlayerAvaterHandlerOffline>().SetSelectedIndex(0);
    }
    void PerformSelection()
    {
        GetComponentInParent<InitializeProfilePictures>().PerformDelelect();
        GetComponentInParent<InitializeProfilePictures>().SetSelectedAvatar(index);
        m_HighlightImage.SetActive(true);
    }
    void PerformSelectionOnPlayer()
    {
        // determine which player has pressed the button and change the avatar accordingly
        GetComponentInParent<PlayerAvaterHandlerOffline>().PerformDelelect();
        GetComponentInParent<PlayerAvaterHandlerOffline>().SetPlayerAvatar(index);
        m_HighlightImage.SetActive(true);
    }
}
