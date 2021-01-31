using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusScript : MonoBehaviour
{
    Rigidbody2D rb;
    public Transform pStop;
    public bool leave = false;
    public bool firstPhase;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.AddForce(new Vector2(-5000f, 0));
        firstPhase = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= pStop.position.x && firstPhase)
        {
            firstPhase = false;
            rb.velocity = Vector2.zero;
        }

        if(leave)
        {
            leave = false;
            rb.AddForce(new Vector2(-5000f, 0));
        }
    }
}
