using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleTimer : RuleEnclosure
{
    public float totalDuration = 10; //seconds
    public bool isFinished = false;

    private float startTime = 0f;
    private float timeLeft = 0f;


    // Return 
    public float TimeLeft()
    {
        return timeLeft;
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        isFinished = false;
    }

    // FixedUpdate is called at a fixed time interval
    void FixedUpdate()
    {
        if (!isFinished)
        {
            timeLeft = startTime + totalDuration - Time.time;
            if (timeLeft < 0f){
                isFinished = true;
                timeLeft = 0f;
            }
        }
    }
}
