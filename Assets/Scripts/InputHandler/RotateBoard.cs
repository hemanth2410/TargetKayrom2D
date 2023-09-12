using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateBoard : MonoBehaviour
{
    [SerializeField] StarterAssetsInputs m_Inputs;
    [SerializeField] Transform m_TransformToRotate;
    [SerializeField] float m_RotationSpeed;
    [SerializeField] bool m_UseSliderAsInput;
    [SerializeField] Slider m_RotationSlider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!m_UseSliderAsInput)
        {
            m_TransformToRotate.Rotate(new Vector3(0, 0, m_RotationSpeed * Time.deltaTime * m_Inputs.move.x));
        }
        else
        {
            m_TransformToRotate.rotation = Quaternion.Euler(0,0,m_RotationSlider.value);
        }
    }

   
    
}
