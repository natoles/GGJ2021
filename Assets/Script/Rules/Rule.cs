using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule : MonoBehaviour
{
    // PUBLIC
    public string ruleName;
    public string ruleDescription;
    public bool isDefinitelyFailed = false; // Rule have been failed for too much time
    public bool isFailed = false; // Rule is failed but there is still some time to repair it
    public bool failedTimerStarted = false; // Failed timer started
    public float timeBeforeTrigger; // Time during which rule isn't considered failed yet
    public float timeLeftBeforeTrigger;


    // PRIVATE


        /* ===============================================
    ====================   METHODS   =================
    =============================================== */

    
    // ==================== GETTTER AND STATE

    public bool IsFailed()
    {
        return isDefinitelyFailed;
    }

    
    // ==================== TIMER BEFORE RULE FAILED

    public void startFailedTimer()
    {
        failedTimerStarted = true;
        timeLeftBeforeTrigger = timeBeforeTrigger;
    }

    public void cancelFailedTimer()
    {
        failedTimerStarted = false;
        timeLeftBeforeTrigger = timeBeforeTrigger;
    }

    public void updateFailedTimer()
    {
        if (failedTimerStarted)
        {
            timeLeftBeforeTrigger -= Time.deltaTime;
            Debug.Log("Time left: "+timeLeftBeforeTrigger.ToString());
            if (timeLeftBeforeTrigger <= 0)
            {
                isDefinitelyFailed = true;
            }
        }
    }

    // ==================== UI DISPLAY

    

    // ==================== START AND UPDATES

    // Start is called before the first frame update
    protected virtual void Start()
    {
        isDefinitelyFailed = false;
        isFailed = false;
        failedTimerStarted = false;
        timeLeftBeforeTrigger = timeBeforeTrigger;
    }

    
    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isDefinitelyFailed)
        {
            if (isFailed && !failedTimerStarted)
            {
                startFailedTimer();
                Debug.Log("Timer started");
            }
            if (!isFailed && failedTimerStarted)
            {
                Debug.Log("Timer canceled");
            }
        }
        if (isDefinitelyFailed){
            Debug.Log("FAILED!!");
        }
    }

    // FixedUpdate is called at a fixed time interval
    protected virtual void FixedUpdate()
    {
    }
}
