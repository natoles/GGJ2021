using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleTimer : RuleEnclosure
{
    public float totalDuration = 10; //seconds
    private float timeLeft = 0f;


    // Restet timer
    public void Reset()
    {
        timeLeft = totalDuration;
        isFailed = false;
    }

    // Return 
    public float TimeLeft()
    {
        return timeLeft;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        Reset();
    }

    // FixedUpdate is called at a fixed time interval
    protected override void FixedUpdate()
    {
        if (!isFailed)
        {
            timeLeft -= Time.fixedDeltaTime;
            if (timeLeft < 0f){
                isFailed = true;
                timeLeft = 0f;
            }
        }
    }
}
