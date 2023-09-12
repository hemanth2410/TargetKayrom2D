using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(SphereCollider))]
public class Coin : MonoBehaviour
{
    #region EditorVariables
    [SerializeField] CoinType m_coinType;
    [SerializeField, Tooltip("Set value as -1 for striker")] int m_coinValue;
    #endregion

    #region PublicProperties
    public CoinType CoinType { get { return m_coinType; } }
    public int CoinValue { get { return m_coinValue; } }
    public bool IsInBaulkLine { get { return isInBaulkLine; } }
    #endregion

    private SpriteRenderer spriteRenderer;
    bool isInBaulkLine;
    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out spriteRenderer))
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        else
        {
            Debug.LogError("Mesh renderer is missng on the coin");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeMaterial(Material  material)
    {
        spriteRenderer.material = material;
    }

    public void UpdateBaulkLine(bool _isBaulkLine)
    {
        isInBaulkLine = _isBaulkLine;
    }
    //private void OnCollisionEnter(Collision collision)
    //{
        
    //    if (collision.gameObject.GetComponent<StrikerController>() != null)
    //    {
    //        ShotReport report = new ShotReport(this.gameObject, collision.gameObject, Time.time, isInBaulkLine);
    //        if(gameObject.scene.name != "SimulatedBoard")
    //        {
    //            GameController.Instance.Evaluator.AppendShotReport(this.gameObject, report);
    //        }
            
    //    }
    //    else
    //    {
    //        ShotReport report = new ShotReport(this.gameObject, collision.gameObject, Time.time, false);
    //        if(gameObject.scene.name != "SimulatedBoard")
    //        {
    //            GameController.Instance.Evaluator.AppendShotReport(this.gameObject, report);
    //        }
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<StrikerController>() != null)
        {
            ShotReport report = new ShotReport(this.gameObject, collision.gameObject, Time.time, isInBaulkLine);
            if(gameObject.scene.name != "SimulatedBoard")
            {
                GameController.Instance.Evaluator.AppendShotReport(gameObject,report);
            }
        }
        else
        {
            ShotReport report = new ShotReport(this.gameObject, collision.gameObject, Time.time, false);
            if (gameObject.scene.name != "SimulatedBoard")
            {
                GameController.Instance.Evaluator.AppendShotReport(gameObject, report);
            }
        }
    }
}


