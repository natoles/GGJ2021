using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cat : Animal
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        minMoveInterval = 3f;
        maxMoveInterval = 7f;
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
