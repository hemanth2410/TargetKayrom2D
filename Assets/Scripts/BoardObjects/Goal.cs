using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    List<GameObject> coins = new List<GameObject>();

    GameManager gameManager;
    PostShotRuleEvaluator ruleEvaluator;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag(Constants.Tag_GameManager).GetComponent<GameManager>();  
        ruleEvaluator = gameManager.GetComponent<PostShotRuleEvaluator>();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Coin>() != null && !coins.Contains(other.gameObject))
        {
            coins.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(coins.Contains(other.gameObject))
        {
            
            var coinScript = other.GetComponent<Coin>();
            if(coinScript.CoinType == CoinType.Queen && other.GetComponent<RedCoinHelper>().WaitingForExit)
            {
                // coin exited the goal for the first time. Dont destroy. && dont generate collidion event
                other.GetComponent<RedCoinHelper>().EnableNormalMode();
                coins.Remove(other.gameObject);
                return;
            }

            coins.Remove(other.gameObject);
            // we need a smart way to remove coins through rule manager
            ruleEvaluator.AppendShotReport(this.gameObject, new ShotReport(this.gameObject, other.gameObject, Time.time, coinScript.IsInBaulkLine));

            if(other.GetComponent<Coin>().CoinType != CoinType.Striker)
            {
                gameManager.CoinPucked(other.gameObject);
            }
                
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Coin>()!= null && !coins.Contains(collision.gameObject))
        {
            coins.Add(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(coins.Contains(collision.gameObject))
        {
            var coinScript = collision.GetComponent<Coin>();
            if(coinScript.CoinType == CoinType.Queen && collision.GetComponent<RedCoinHelper>().WaitingForExit)
            {
                collision.GetComponent<RedCoinHelper>().EnableNormalMode();
                coins.Remove(collision.gameObject);
                return;
            }
            coins.Remove(collision.gameObject);
            ruleEvaluator.AppendShotReport(gameObject, new ShotReport(gameObject, collision.gameObject, Time.time, coinScript.IsInBaulkLine));
            if(collision.GetComponent<Coin>().CoinType != CoinType.Striker)
            {
                gameManager.CoinPucked(collision.gameObject);
            }
        }
    }
}
