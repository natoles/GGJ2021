using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : Animal
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        type = "Mouse";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
