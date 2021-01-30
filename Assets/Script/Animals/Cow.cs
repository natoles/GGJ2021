using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Animal
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        moveInterval = 8f;
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
            Debug.Log(reachedEndOfPath);


            if (reachedEndOfPath)
            {
                target.position *= -1; //TEMPORARY
                MoveTo(target.position);
            }

            yield return new WaitForSeconds(moveInterval);
        }
        
    }
}
