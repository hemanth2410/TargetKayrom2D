using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class HandlePlayerNames : MonoBehaviour
{
    [SerializeField] List<TextMeshProUGUI> m_ValidationTexts;
    [SerializeField] Button m_ButtonToActivate;
    [SerializeField] TextMeshProUGUI m_ButtonText;
    // Start is called before the first frame update
    void Start()
    {
        m_ButtonToActivate.interactable = false;
        m_ButtonText = m_ButtonToActivate.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        m_ButtonToActivate.interactable = !m_ValidationTexts.Any(x => x.text.Length <= 1);
        m_ButtonText.text = m_ValidationTexts.Any(x => x.text.Length <= 1) ? "Waiting for player names" : "Start";
    }
}
