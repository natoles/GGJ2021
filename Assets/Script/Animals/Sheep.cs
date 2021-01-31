using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : Animal
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        type = "Sheep";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
