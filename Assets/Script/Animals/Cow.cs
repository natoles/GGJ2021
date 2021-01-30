using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : Animal
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        minMoveInterval = 1f;
        maxMoveInterval = 2f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override IEnumerator RageState()
    {
        while (true)
        {
            //Put rage behavior here
            yield return new WaitForSeconds(Random.Range(minMoveInterval, maxMoveInterval));
        }
        
    }
}
