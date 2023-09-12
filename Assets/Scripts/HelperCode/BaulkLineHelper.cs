using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaulkLineHelper : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Coin>())
        {
            other.GetComponent<Coin>().UpdateBaulkLine(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Coin>())
        {
            other.GetComponent<Coin>().UpdateBaulkLine(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Coin>())
        {
            collision.GetComponent<Coin>().UpdateBaulkLine(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponent<Coin>())
        {
            collision.GetComponent<Coin>().UpdateBaulkLine(false);
        }
    }
}
