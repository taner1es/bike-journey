using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchinAreaTrigger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(gameObject.name == collision.gameObject.name)
        {
            Destroy(collision.gameObject);
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
