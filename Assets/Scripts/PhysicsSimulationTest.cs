using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhysicsSimulationTest : MonoBehaviour
{
    [SerializeField] GameObject m_ActiveObject;
    [SerializeField] LineRenderer m_LineRenderer;
    GameObject clone;
    bool simulate;
    Scene simulationScene;
    PhysicsScene2D physicsSimulationScene;
    List<GameObject> coins = new List<GameObject>();
    List<GameObject> SimulatedCoins = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        m_ActiveObject.GetComponent<Rigidbody2D>().simulated = false;
        m_ActiveObject.GetComponent<CircleCollider2D>().enabled = false;
        
        simulate = true;
        simulationScene = SceneManager.CreateScene("SimulatedBoard", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        physicsSimulationScene = simulationScene.GetPhysicsScene2D();
        coins = GameObject.FindGameObjectsWithTag(Constants.Tag_Faction1).ToList();

        for (int i = 0; i < coins.Count; i++)
        {
            var ghost = Instantiate(coins[i], coins[i].transform.position, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(ghost, simulationScene);
            SimulatedCoins.Add(ghost);
        }
        createSimulation();
        // create ghosts

    }

    // Update is called once per frame
    void Update()
    {
        if (simulate)
            visualizePath();
        if(Input.GetKeyDown(KeyCode.F) && simulate)
        {
            stopSimulation();
            simulate = false;
        }
    }

    void createSimulation()
    {
        clone = Instantiate(m_ActiveObject, m_ActiveObject.transform.position, Quaternion.identity);
        clone.GetComponent<Rigidbody2D>().simulated = true;
        clone.GetComponent<CircleCollider2D>().enabled = true;
        SceneManager.MoveGameObjectToScene(clone, simulationScene);
        Physics2D.simulationMode = SimulationMode2D.Script;
    }
    void visualizePath()
    {
        // reset position here and simulate again
        //clone.GetComponent<Rigidbody2D>().simulated = false;
        clone.transform.position = m_ActiveObject.transform.position;
        //clone.GetComponent<Rigidbody2D>().simulated = true;

        List<Vector3> points = new List<Vector3>();
        int simulationSteps = 1000;
        for (int i = 0; i < simulationSteps; i++)
        {
            physicsSimulationScene.Simulate(Time.fixedDeltaTime);
            points.Add(clone.transform.position);
        }
        m_LineRenderer.positionCount = points.Count;
        m_LineRenderer.SetPositions(points.ToArray());
    }
    void stopSimulation()
    {
        m_ActiveObject.GetComponent<Rigidbody2D>().simulated = true;
        m_ActiveObject.GetComponent<CircleCollider2D>().enabled = true;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Destroy(clone);
    }
}
