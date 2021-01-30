using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleCat : RuleAnimal
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Mouse"))){
            animal.Run();
        } else if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Mouse"))) {
            animal.Chase(FindNeighborTypeOf(StringToClass.TypeFromString("Mouse")));
        } else {
            animal.Calm();
        }
    }
}
