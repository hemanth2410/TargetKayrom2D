using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class TouchInput : MonoBehaviour
{
    [SerializeField] LayerMask m_DetectionMask;
    [SerializeField] LayerMask m_StrikerDetectionMask;
    [SerializeField] GameObject m_DebugObject;
    [SerializeField] GameObject m_VirtualRoot;
    [SerializeField] float m_Sensitivity = 0.5f;
    [SerializeField] UnityEvent m_EventToExecuteOnStriker;
    [SerializeField] CoinToPlaceObject m_CointoPlaceObject;
    Vector3 dirPre;
    Vector3 dirPost;
    Vector3 currentMousePosition;
    Vector3 PreviousMousePosition;
    Camera mainCamera;
    bool handleInput;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!handleInput)
            return;
        if (m_CointoPlaceObject.Value != null)
            return;
        if(Input.GetMouseButtonDown(0))
        {
            PreviousMousePosition = Vector3.zero;
        }
        if(Input.GetMouseButton(0))
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
            RaycastHit2D _hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, m_DetectionMask);
            if (_hit.collider != null)
            {
                currentMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                dirPre = PreviousMousePosition - m_VirtualRoot.transform.position;
                dirPost = currentMousePosition - m_VirtualRoot.transform.position;
                dirPre.z = 0;
                dirPost.z = 0;
                Debug.DrawRay(m_VirtualRoot.transform.position, Vector3.Cross(dirPre, dirPost) * 10 , Color.blue);
                PreviousMousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

                m_VirtualRoot.transform.Rotate(Vector3.Cross(dirPre, dirPost) * m_Sensitivity * Time.deltaTime);
            }
            RaycastHit2D _StrikerCheck = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, m_StrikerDetectionMask);
            if(_StrikerCheck.collider != null && _StrikerCheck.collider.tag == Constants.Tag_Striker && GameController.Instance.ValidStrikerPlacement)
            {
                // we have detected striker 
                // Message Game Manager to unlock striker.
                m_EventToExecuteOnStriker.Invoke();
            }
        }
    }

    public void HandleInput(bool value)
    {
        handleInput = value;
    }
}
