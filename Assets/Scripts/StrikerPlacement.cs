using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerPlacement : MonoBehaviour
{
    [SerializeField] CoinToPlaceObject coinToPlace;
    [SerializeField] Vector3 m_OffsetFromCenter;
    [SerializeField] GameObject m_StrikerObject;
    [SerializeField] LayerMask m_DetectionMask;
    [SerializeField] bool placeStriker = true;
    [SerializeField] Vector3 m_center;
    [SerializeField] float m_MaxDistance;
    Camera mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        if (m_StrikerObject == null)
        {
            m_StrikerObject = GameObject.FindGameObjectWithTag(Constants.Tag_Striker);
        }
        if (coinToPlace.Value != null)
            return;
        if (placeStriker)
            m_StrikerObject.transform.position = m_OffsetFromCenter;
    }

    private void FixedUpdate()
    {
        if(coinToPlace.Value != null && coinToPlace.Value.GetComponent<Coin>().CoinType != CoinType.Striker)
        {
            // we lock the striker and button here
            GameController.Instance.SetValidState(false);
            RaycastHit2D _hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, m_DetectionMask);
            if(_hit.collider != null && Input.GetMouseButtonUp(0) && Vector3.Distance(m_center,_hit.point) < m_MaxDistance)
            {
                coinToPlace.Value.SetActive(true);
                coinToPlace.Value.transform.position = _hit.point;
                coinToPlace.Value = null;
                GameController.Instance.InvokePlacementComplete();
            }
            if (_hit.collider != null && Input.GetMouseButtonUp(0) && Vector3.Distance(m_center, _hit.point) < m_MaxDistance && coinToPlace.Value != null)
            {
                coinToPlace.Value.SetActive(true);
                // add some game juice.
            }
        }
    }

    public void ToggleStrikerPlacement(bool value)
    {
        placeStriker = value;
    }
    
}
