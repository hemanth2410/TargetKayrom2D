using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//[RequireComponent(typeof(Rigidbody))]
public class StrikerController : MonoBehaviour
{

    //striker shooting is controlled by game manager

    [SerializeField] GameObject m_IndicatorObject;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }
    public void ConvertToPhysicalCollider()
    {
        GetComponent<CircleCollider2D>().isTrigger = false;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    var coin = collision.gameObject.GetComponent<Coin>();

    //    if (coin != null)
    //    {
    //        var report = new ShotReport(this.gameObject, coin.gameObject, Time.time, coin.IsInBaulkLine);
    //        if(gameObject.scene.name != "SimulatedBoard")
    //        {
    //            GameController.Instance.Evaluator.AppendShotReport(this.gameObject, report);
    //        }
    //    }
    //}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var coin = collision.gameObject.GetComponent<Coin>();
        if(coin != null)
        {
            var report = new ShotReport(gameObject, coin.gameObject, Time.time, coin.IsInBaulkLine);
            if(gameObject.scene.name != "SimulatedBoard")
            {
                GameController.Instance.Evaluator.AppendShotReport(gameObject,report);
            }
        }
    }
    public void ToggleIndicatorObject(bool value)
    {
        m_IndicatorObject.SetActive(value);
    }
    //private void OnCollisionExit(Collision other)
    //{
        
    //}
}
