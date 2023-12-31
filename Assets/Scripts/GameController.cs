using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private RuleEvaluator ruleEvaluator;
    private GameManager gameManager;

    bool validStrikerPlacement;
    static GameController instance;
    PostShotRuleEvaluator _evaluator;
    public static GameController Instance { get
        {
            if(instance == null)
            {
                instance = FindAnyObjectByType<GameController>();
            }
            return instance;
        } }
    
    public RuleEvaluator RuleEvaluator { get { return ruleEvaluator; } }
    public GameManager GameManager { get {  return gameManager; } }
    public bool ValidStrikerPlacement { get { return validStrikerPlacement; } }
    public event Action<CoinType, int> OnFactionScored;
    public event Action ResetPhysicsCoins;
    public event Action GameOverEvent;
    public event Action PlacementCompleteSuccess;
    public PostShotRuleEvaluator Evaluator { get { return _evaluator; } }
    public void RegisterGameManager(GameManager manager)
    {
        gameManager = manager;
    }
    public void RegisterRuleEvaluator(RuleEvaluator Evaluator)
    {
        ruleEvaluator = Evaluator;
    }
    public void InvokeScoreEvent(CoinType faction, int value)
    {
        OnFactionScored?.Invoke(faction, value);
    }
    public void InvokeResetCoinPhysics()
    {
        ResetPhysicsCoins?.Invoke();
    }

   

    internal void RegisterPostShotEvaluator(PostShotRuleEvaluator postShotRuleEvaluator)
    {
        _evaluator = postShotRuleEvaluator;
    }
    public void InvokeGameOverEvent()
    {
        GameOverEvent?.Invoke();
    }

    public void InvokePlacementComplete()
    {
        PlacementCompleteSuccess?.Invoke();
    }
    public void SetValidState(bool value)
    {
        validStrikerPlacement = value;
    }
}
