using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleTimer : RuleEnclosure
{
    public float totalDuration = 10; //seconds
    public bool isFinished = false;
    private float timeLeft = 0f;


    // Restet timer
    public void Reset()
    {
        timeLeft = totalDuration;
        isFinished = false;
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
        if (!isFinished)
        {
            timeLeft -= Time.fixedDeltaTime;
            if (timeLeft < 0f){
                isFinished = true;
                timeLeft = 0f;
            }
        }
    }
}
