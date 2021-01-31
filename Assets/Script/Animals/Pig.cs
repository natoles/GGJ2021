using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : Animal
{

    public void ThrowMud()
    {
        
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        type = "Pig";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
