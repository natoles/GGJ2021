using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule : MonoBehaviour
{
    // PUBLIC
    public string ruleName;
    public string ruleDescription;
    public int state; // 0: Neutral, 1: On Failed Timer, 2: Failed, 3 Success
    public bool isDefinitelyFailed = false; // Rule have been failed for too much time
    public bool isFailed = false; // Rule is failed but there is still some time to repair it
    public bool failedTimerStarted = false; // Failed timer started
    public float timeMarginBeforeFail; // Time during which rule isn't considered failed yet


    // PRIVATE
    private float _timeLeftBeforeFail;


    /* ===============================================
    ====================   METHODS   =================
    =============================================== */

    
    // ==================== GETTTER AND STATE

    public bool IsFailed()
    {
        return isDefinitelyFailed;
    }

    public float timeLeftBeforeFail()
    {
        return _timeLeftBeforeFail;
    }
    
    public int GetState()
    {
        return state;
    }

    // ==================== TIMER BEFORE RULE FAILED

    public void startFailedTimer()
    {
        failedTimerStarted = true;
        _timeLeftBeforeFail = timeMarginBeforeFail;
        state = 1;
    }

    public void cancelFailedTimer()
    {
        failedTimerStarted = false;
        _timeLeftBeforeFail = timeMarginBeforeFail;
        state = 0;
    }

    public void updateFailedTimer()
    {
        if (failedTimerStarted)
        {
            _timeLeftBeforeFail -= Time.deltaTime;
            Debug.Log("Time left: "+_timeLeftBeforeFail.ToString());
            if (_timeLeftBeforeFail <= 0)
            {
                isDefinitelyFailed = true;
                state = 2;
            }
        }
    }

    // ==================== UI DISPLAY

    

    // ==================== START AND UPDATES

    // Start is called before the first frame update
    protected virtual void Start()
    {
        state = 0;
        isDefinitelyFailed = false;
        isFailed = false;
        failedTimerStarted = false;
        _timeLeftBeforeFail = timeMarginBeforeFail;
    }

    
    // Update is called once per frame
    protected virtual void Update()
    {
        // Update UI etc..

        // Stops here if definitely failed
        if (isDefinitelyFailed) return;

        // Timer before definitely failed
        if (isFailed && !failedTimerStarted)
        {
            startFailedTimer();
        }
        if (!isFailed && failedTimerStarted)
        {
            cancelFailedTimer();
        }
        if (failedTimerStarted)
        {
            updateFailedTimer();
        }
    }

    // FixedUpdate is called at a fixed time interval
    protected virtual void FixedUpdate()
    {
    }
}
