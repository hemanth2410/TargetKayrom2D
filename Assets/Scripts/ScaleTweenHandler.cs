using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTweenHandler : MonoBehaviour
{
    [SerializeField] float m_IntialScale;
    [SerializeField] float m_MinScale;
    [SerializeField] float m_AnimationTime;
    bool doScale;
    float internalTimer;
    Vector3 targetScale;
    // Update is called once per frame
    void Update()
    {
        if (!doScale)
            return;
        internalTimer -= Time.deltaTime;
        transform.localScale = Vector3.Lerp(targetScale, transform.localScale, internalTimer);
        if (internalTimer <= 0)
            doScale = false;
    }
    // This method will be called to help players determine whose turn it is
    // This method will be called upon determining whose turn it is.
    public void DOScale(bool expand)
    {
        doScale = true;
        internalTimer = m_AnimationTime;
        float _targetScale = expand ? m_IntialScale : m_MinScale;
        targetScale = new Vector3(_targetScale, _targetScale, 1);
    }
}
