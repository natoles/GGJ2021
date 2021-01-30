using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Animal
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        moveInterval = 2f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override IEnumerator RageState()
    {
        while(true)
        {
            Debug.Log("end of path :" + reachedEndOfPath);
            if (reachedEndOfPath)
            {
                Debug.Log(inEnclosure);
                if (inEnclosure)
                {
                    MoveTo(currentEnclosure.RandomPoint());
                    Debug.Log(currentEnclosure.RandomPoint());
                }
                else
                {
                    //Outside
                }
            }
              

            yield return new WaitForSeconds(moveInterval);
        }
        
    }
}
