using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrikerPlacementHelper : MonoBehaviour
{
    public void ConvertToTrigger()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
        // enable some Indication for striker
        GetComponent<StrikerController>().ToggleIndicatorObject(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.GetComponent<Coin>())
        {
            GameController.Instance.SetValidState(false);
        }
        else
        {
            GameController.Instance.SetValidState(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent <Coin>())
        {
            GameController.Instance.SetValidState(true);
        }
    }
}
