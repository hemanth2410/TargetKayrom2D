using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class InvertedCircleCollider : MonoBehaviour
{
    [SerializeField] float m_Radius;
    [SerializeField][Range(20, 200)] int m_NumberOfPoints;

    private void Start()
    {
        
    }

    public void generateCollider()
    {
        EdgeCollider2D edge = gameObject.AddComponent<EdgeCollider2D>();
        Vector2[] points = new Vector2[m_NumberOfPoints + 1];
        for (int i = 0; i < m_NumberOfPoints; i++)
        {
            float _angle = 2 * Mathf.PI * i / m_NumberOfPoints;
            float x = m_Radius * Mathf.Cos(_angle);
            float y = m_Radius * Mathf.Sin(_angle);
            points[i]= new Vector2(x, y);
        }
        points[m_NumberOfPoints] = points[0];
        edge.points = points;
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(InvertedCircleCollider))]
public class InvertedCircleColliderEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InvertedCircleCollider _target = (InvertedCircleCollider)target;
        if(GUILayout.Button("Generate collider"))
        {
            _target.generateCollider();
        }
       
    }
}

#endif