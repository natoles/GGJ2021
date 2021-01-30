using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleBull : RuleAnimal
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
        if (animal.IsInEnclosure()){
            if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Bull"))){
                animal.Enrage();
            } else {
                animal.Calm();
            }
        }
    }
}
