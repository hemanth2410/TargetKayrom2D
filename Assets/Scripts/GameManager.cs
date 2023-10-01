using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;
using Unity.Netcode.Components;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public float StrikeForceMultiplier = 1f;
    public LineRenderer ShotRenderer;

    public int MaxSimulatedFrames = 60;
    [Space]
    List<GameObject> carromCoins;
    List<Vector3> preShotPos = new List<Vector3>();                   //save pre shot positions of coins
    List<Rigidbody2D> coinRigs = new List<Rigidbody2D>();                 // convert Rigidbody to RB2D
    public Transform[] StaticPhysicsObjects;
    public LayerMask ignoreLayers;
    [SerializeField] TextMeshProUGUI m_StatusText;
    [SerializeField] UnityEvent m_EventToExecuteOnWin;
    [SerializeField] ParticleSystem m_PowSystem;
    [SerializeField] bool m_LockRaycast = true;
    [SerializeField] CoinToPlaceObject m_CoinToPlace;
    [SerializeField] TouchInput m_TouchInput;
    public Vector3 StrikerForceDirection { get { return strikerForceDirection; } }
    [Space]
    [Header("Debug Only")]
    [SerializeField] NewFactionHolder factionHolder1;
    [SerializeField] NewFactionHolder factionHolder2;
    [SerializeField] Sprite[] m_DefaultAvatars;
    //Get private references to striker and other coins
    GameObject striker;
    Rigidbody2D strikerRig;
    Transform strikerTransfrom;
    GameObject ghostStriker;
    GameObject queen;
    GameObject coinToPlace;

    bool touchIsDragging;
    Vector2 dragStartPos;
    Vector2 dragEndPos;

    Scene simulationScene;
    PhysicsScene2D physicsSimulationScene;


    List<GameObject> ghostCoins = new List<GameObject>();
    List<Vector3> ghostsPreSimPos = new List<Vector3>();
    Vector2 strikerForceDirection;

    bool panelsSwitched = false;
    bool isShotPlaying;
    bool opponentPlacesCoin;
    PostShotRuleEvaluator ruleEvaluator;
    List<GameObject> puckedCoins = new List<GameObject>();
    List<GameObject> puckedGhosts = new List<GameObject>();
    CoinType currentFaction = CoinType.Faction1;
    NetworkMessenger networkMessenger;
    bool strikerReset;

    // Game juice
    PostRuleExecution postRuleExecution;
    ScaleTweenHandler player1Scalehandler;
    ScaleTweenHandler player2ScaleHandler;
    StrikerPlacement strikerPlacement;
    // Start is called before the first frame update
    void Start()
    {
        networkMessenger = GetComponent<NetworkMessenger>();
        GameController.Instance.RegisterGameManager(this);
        Application.targetFrameRate = 60;
        striker = GameObject.FindGameObjectWithTag(Constants.Tag_Striker);
        strikerRig = striker.GetComponent<Rigidbody2D>();
        strikerTransfrom = striker.transform;
        GameController.Instance.PlacementCompleteSuccess += carromManPlaced;
        queen = GameObject.FindGameObjectWithTag(Constants.Tag_Queen);
        strikerPlacement = GetComponent<StrikerPlacement>();
        simulationScene = SceneManager.CreateScene("SimulatedBoard", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        physicsSimulationScene = simulationScene.GetPhysicsScene2D();


        //Find and add all coins on board into the list
        carromCoins = new List<GameObject>
        {
            striker
        };
        striker.GetComponent<StrikerPlacementHelper>().ConvertToTrigger();
        var faction1coins = GameObject.FindGameObjectsWithTag(Constants.Tag_Faction1);
        var faction2coins = GameObject.FindGameObjectsWithTag(Constants.Tag_Faction2);

        carromCoins.AddRange(faction1coins);
        carromCoins.AddRange(faction2coins);

        carromCoins.Add(queen);

        foreach (Transform GO in StaticPhysicsObjects)
        {
            var go = Instantiate(GO.gameObject, GO.position, GO.rotation);
            SceneManager.MoveGameObjectToScene(go, simulationScene);

            try
            {
                go.GetComponent<SpriteRenderer>().enabled = false;
            }
            catch (Exception e)
            {
                continue;
            }
        }



        //Create ghost objects for physics simulation, add rigidbodies to collection
        for (int i = 0; i < carromCoins.Count; i++)
            {
                var coinGO = carromCoins[i];
                coinRigs.Add(coinGO.GetComponent<Rigidbody2D>());

                var ghostGameObject = Instantiate(coinGO, coinGO.transform.position, coinGO.transform.rotation);
                ghostGameObject.GetComponent<Coin>().enabled = false;
                //Destroy(ghostGameObject.GetComponent<NetworkTransform>());
                Destroy(ghostGameObject.GetComponent<NetworkRigidbody2D>());
                Destroy(ghostGameObject.GetComponent<NetworkTransform>());
                Destroy(ghostGameObject.GetComponent<NetworkObject>());
                if (ghostGameObject.tag == Constants.Tag_Striker)
                {
                    ghostStriker = ghostGameObject;
                    ghostStriker.GetComponent<StrikerController>().ToggleIndicatorObject(false);
                    Destroy(ghostStriker.GetComponent<StrikerController>());
                }
                ghostGameObject.GetComponent<SpriteRenderer>().enabled = false;
                SceneManager.MoveGameObjectToScene(ghostGameObject, simulationScene);
                ghostCoins.Add(ghostGameObject);
                ghostsPreSimPos.Add(ghostGameObject.transform.position);

            }


        ruleEvaluator = GetComponent<PostShotRuleEvaluator>();
        postRuleExecution = GetComponent<PostRuleExecution>();
        ruleEvaluator.SetFaction(CoinType.Faction1);
        // break the code here and initialise the player data
        //InitializeGameWithDefaultPlayer();
        initializeFactions();
        player1Scalehandler = factionHolder1.GetComponent<ScaleTweenHandler>();
        player2ScaleHandler = factionHolder2.GetComponent<ScaleTweenHandler>();
        m_StatusText.text = currentFaction == PersistantPlayerData.Instance.Player1.PlayerFaction ? PersistantPlayerData.Instance.Player1.PlayerName + " Plays" : PersistantPlayerData.Instance.Player2.PlayerName + " Plays";
        GameController.Instance.GameOverEvent += Instance_GameOverEvent;
        m_LockRaycast = true;
        scalePlayerPanels();
    }

    private void initializeFactions()
    {
        factionHolder1.InitializeFaction(PersistantPlayerData.Instance.Player1.PlayerName, m_DefaultAvatars[PersistantPlayerData.Instance.Player1.SelectedIndex], CoinType.Faction1);
        factionHolder2.InitializeFaction(PersistantPlayerData.Instance.Player2.PlayerName, m_DefaultAvatars[PersistantPlayerData.Instance.Player2.SelectedIndex], CoinType.Faction2);
    }

    void scalePlayerPanels()
    {
        //player1Scalehandler.DOScale(currentFaction == CoinType.Faction1);
        //player2ScaleHandler.DOScale(currentFaction == CoinType.Faction2);
        factionHolder1.gameObject.SetActive(currentFaction == CoinType.Faction1);
        factionHolder2.gameObject.SetActive(currentFaction == CoinType.Faction2);
        m_TouchInput.HandleInput(true);
        striker.GetComponent<StrikerPlacementHelper>().ConvertToTrigger();
        Debug.Log("Setting place striker to true");
        strikerPlacement.ToggleStrikerPlacement(true);
    }

    //void InitializeGameWithDefaultPlayer()
    //{
    //    PersistantPlayerData.Instance.RegisterPlayer1(new Player("John Doe", CoinType.Faction1, 0));
    //    PersistantPlayerData.Instance.RegisterPlayer2(new Player("Jane Doe", CoinType.Faction2, 0));
    //    // populate faction holders
    //    factionHolder1.InitializeFactionWithDefault(new Player("John Doe", CoinType.Faction1, 0));
    //    factionHolder2.InitializeFactionWithDefault(new Player("Jane Doe", CoinType.Faction2, 0));
    //}

    private void Instance_GameOverEvent()
    {
        // wait for 3 seconds;
        StartCoroutine(waitForSceneLoad());
    }

    IEnumerator waitForSceneLoad()
    {
        yield return new WaitForSeconds(3);
        m_EventToExecuteOnWin.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        if(!strikerReset)
        {
            if (ghostStriker.GetComponent<Rigidbody2D>().isKinematic)
            {
                ghostStriker.GetComponent<Rigidbody2D>().isKinematic = false;
                strikerReset = true;
            }
        }
        


        if (m_LockRaycast)
            return;

        Vector3 currentMousePos = Input.mousePosition;
        RaycastHit hit;
        var clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(clickRay, out hit, 50f/*, ignoreLayers*/);

        //Get viewport point to calculate for camera delta
        var clickViewport = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);


        if (Input.GetMouseButton(0) && !touchIsDragging)
        {

            if (hit2D.collider.gameObject == striker && !isShotPlaying)
            {
                dragStartPos = strikerTransfrom.position;
                //dragStartPos.y = 0;
                touchIsDragging = true;
                ShotRenderer.enabled = true;

            }
            Physics2D.simulationMode = SimulationMode2D.Script;
        }

        if (touchIsDragging)
        {
            var hitPoint = hit2D.point;
            Debug.DrawLine(strikerTransfrom.position, hitPoint);
            strikerForceDirection = dragStartPos - hitPoint;
            float magnitude = Vector2.Distance(hitPoint, dragStartPos);

            for (int i = 0; i < carromCoins.Count; i++)
            {
                ghostCoins[i].transform.position = carromCoins[i].transform.position;
            }
            //GameController.Instance.RuleEvaluator.EvaluateRules();

            ghostStriker.GetComponent<CircleCollider2D>().isTrigger = !GameController.Instance.ValidStrikerPlacement;
            ghostStriker.transform.position = striker.transform.position;
            ghostStriker.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            ghostStriker.GetComponent<Rigidbody2D>().AddForce((strikerForceDirection.normalized) * StrikeForceMultiplier * magnitude, ForceMode2D.Impulse);
            ShotRenderer.positionCount = MaxSimulatedFrames;

            for (int i = 0; i < MaxSimulatedFrames; i++)
            {
                physicsSimulationScene.Simulate(Time.fixedDeltaTime);
                Vector3 _position = ghostStriker.transform.position;
                _position.z = 0;
                ShotRenderer.SetPosition(i, _position);

            }
            Vector3 _firstPosition = striker.transform.position;
            _firstPosition.z = 0;
            ShotRenderer.SetPosition(0, _firstPosition);

            //Strike the striker when we release
            if (Input.GetMouseButtonUp(0))
            {
                Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
                dragEndPos = hit2D.point;
                touchIsDragging = false;
                isShotPlaying = true;

                ShotRenderer.positionCount = 0;

                striker.GetComponent<StrikerController>().ToggleIndicatorObject(false);
                strikerTransfrom.GetComponent<Rigidbody2D>().AddForce((strikerForceDirection.normalized) * StrikeForceMultiplier * magnitude, ForceMode2D.Impulse);
                StartCoroutine("checkforShotEnd");
                ShotRenderer.enabled = false;
                for (int i = 0; i < carromCoins.Count; i++)
                {
                    preShotPos.Add(carromCoins[i].transform.position);
                }
                m_PowSystem.transform.position = striker.transform.position + new Vector3(0, 0, -5);
                m_PowSystem.Play();
                m_LockRaycast = true;
            }


            dragStartPos.y = hitPoint.y = strikerTransfrom.position.y;
            ShotRenderer.transform.position = dragStartPos;
            var shotDir = dragStartPos - hitPoint;
            //ShotRenderer.SetPosition(0, strikerTransfrom.position);
            //ShotRenderer.SetPosition(1, strikerTransfrom.position + (shotDir * StrikeForceMultiplier));

        }

    }
    IEnumerator checkforShotEnd()
    {
        yield return new WaitForSeconds(0.5f);

        while (isShotPlaying)
        {
            if (coinRigs.All(x => (x.velocity.magnitude <= 0.01f || !x.gameObject.activeSelf)))
            {
                evaluateShot();
                Debug.Log("Shot complete. Evaluating...");
                isShotPlaying = false;
            }
            yield return new WaitForFixedUpdate();
        }

    }

    void evaluateShot()
    {
        var eval = ruleEvaluator.EvaluateRules();
        // item 1 = score
        // item 2 = place carromman
        // item 3 = extra turn
        // item 4 = Queen reached goal
        // item 5 = resetToPast

        //1,1 == Score and retain turn,\n 1,0 == score, lose turn,\n 0,1 == reset and ose turn,\n 0,0 == no score, lose turn
        Debug.Log(eval);
        //No collision reported Or striker collided with Queen and nothing happened
        if (!eval.Item1 && !eval.Item2 && !eval.Item3 && !eval.Item4 && !eval.Item5)
        {
            // switch factions
            toggleFaction();
            postRuleExecution.ExecutePostScoreEventNoScore();
        }
        //Striker collided with opponent 
        if (!eval.Item1 && !eval.Item2 && !eval.Item3 && !eval.Item4 && eval.Item5)
        {
            // trigger the board to reset to past and switch faction
            revertCoinPositions();
            revertBoard();
            toggleFaction();
            postRuleExecution.ExecutePostScoreEventNoScore();
        }
        if (eval.Item1 && !eval.Item2 && eval.Item3 && !eval.Item4 && !eval.Item5)
        {
            // carromman of same faction went into the goal withoug striker going through it
            processScore();
            preShotPos.Clear();
            postRuleExecution.ExecutePostScoreEvent();
            // this should retain turn like this.
        }
        //Queen reached goalPost but score is not maximum
        if (!eval.Item1 && !eval.Item2 && !eval.Item3 && eval.Item4 && eval.Item5)
        {
            // reset the board and change faction
            revertCoinPositions();
            revertBoard();
            toggleFaction();
            postRuleExecution.ExecutePostScoreEventNoScore();
        }
        // coin is already in the baulk line when collided with striker or carromman and striker reaches the goal at the same time
        if (!eval.Item1 && eval.Item2 && !eval.Item3 && !eval.Item4 && !eval.Item5)
        {
            // give the opponent an ability to place the carrom according to his advantage
            opponentPlacesCoin = true;
            toggleFaction();
            postRuleExecution.ExecutePostScoreEventNoScore();
        }
        // queen reached the goal post with maximum score.
        if (!eval.Item1 && eval.Item2 && !eval.Item3 && eval.Item4 && !eval.Item5)
        {
            // time to trigger win condition.
            // GameController.Instance.InvokeGameOverEvent();
            // do nothing, Rule Evaluator is handling this case
        }
        //m_CoinToPlace.Value = eval.Item6;
        // Update this on network immediately.
        m_CoinToPlace.Value = eval.Item6;
        puckedCoins.Clear();
        puckedGhosts.Clear();
        

    }

    void placeCoinToPlace()
    {
        if (m_CoinToPlace.Value == null)
        {
            m_StatusText.text = currentFaction == PersistantPlayerData.Instance.Player1.PlayerFaction ? PersistantPlayerData.Instance.Player1.PlayerName + " Plays" : PersistantPlayerData.Instance.Player2.PlayerName + " Plays";
        }
        if (m_CoinToPlace.Value != null && m_CoinToPlace.Value.GetComponent<Coin>().CoinType != CoinType.Striker)
        {
            m_StatusText.text = currentFaction == PersistantPlayerData.Instance.Player1.PlayerFaction ? PersistantPlayerData.Instance.Player1.PlayerName + " Places the carromman" : PersistantPlayerData.Instance.Player2.PlayerName + " Places the carromman";
        }

    }

    public void SetCoinToPlaceServer(GameObject value)
    {
        m_CoinToPlace.Value = value;
    }
    void carromManPlaced()
    {
        GameController.Instance.SetValidState(true);
        m_StatusText.text = currentFaction == PersistantPlayerData.Instance.Player1.PlayerFaction ? PersistantPlayerData.Instance.Player1.PlayerName + " Plays" : PersistantPlayerData.Instance.Player2.PlayerName + " Plays";
    }
    void toggleFaction()
    {
        switch (currentFaction)
        {
            case CoinType.Faction1:
                currentFaction = CoinType.Faction2;
                SendTurnChangeOnServer(currentFaction);
                break;
            case CoinType.Faction2:
                currentFaction = CoinType.Faction1;
                SendTurnChangeOnServer(currentFaction);
                break;
        }
    }

    public void SetCurrentFaction(CoinType currentFaction)
    {
        Debug.Log("Setting current faction");
        this.currentFaction = currentFaction;
        scalePlayerPanels();
        placeCoinToPlace();
    }

    public void UpdateClientToPlace(GameObject value)
    {
        m_CoinToPlace.Value = value;
    }

    void SendTurnChangeOnServer(CoinType targetFaction)
    {
        networkMessenger.SendTurnChangeToserver(targetFaction);
    }
    void revertCoinPositions()
    {
        foreach (GameObject coin in puckedCoins)
        {
            coin.SetActive(true);
        }
        foreach (GameObject ghost in puckedGhosts)
        {
            ghost.SetActive(true);
        }
    }
    private void revertBoard()
    {

        for (int i = 0; i < carromCoins.Count; i++)
        {
            carromCoins[i].transform.position = preShotPos[i];
            if (carromCoins[i].GetComponent<RedCoinHelper>())
            {
                carromCoins[i].GetComponent<RedCoinHelper>().MakeWait();
            }
        }

    }

    private void processScore()
    {
        foreach (GameObject coin in puckedCoins)
        {
            var i = carromCoins.IndexOf(coin);
            //GameController.Instance.InvokeScoreEvent(coin.GetComponent<Coin>().CoinType, 1);
            // sendthe RPC to the server
            networkMessenger.RegisterScoreUpdatedEvent(coin.GetComponent<Coin>().CoinType, 1);

            coinRigs.RemoveAt(i);
            carromCoins.Remove(coin);
            Destroy(coin);
        }
        foreach (GameObject ghost in puckedGhosts)
        {
            ghostCoins.Remove(ghost);
            Destroy(ghost);
        }

    }

    //Called from Goal post to indicate which faction coin has been pucked
    public void CoinPucked(GameObject coin)
    {
        if (!isShotPlaying) return;


        var index = carromCoins.IndexOf(coin);
        coinRigs[index].velocity = Vector3.zero;
        coin.SetActive(false);
        puckedCoins.Add(coin);
        //carromCoins.Remove(coin);
        //Destroy(coin);

        var ghost = ghostCoins[index];
        ghost.SetActive(false);
        puckedGhosts.Add(ghost);

        //ghostCoins.Remove(ghost);
        //Destroy(ghost);
    }

    public void UnlockRaycast()
    {
        m_LockRaycast = false;
    }
    private void OnDestroy()
    {
        SceneManager.UnloadSceneAsync(simulationScene);
    }
}
