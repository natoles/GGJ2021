using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulePig : RuleAnimal
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
            if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Wolf"))){
                animal.Enrage();
            } else if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Pig"))){
                if (animal.GetType() == StringToClass.TypeFromString("Pig")){
                    ((Pig) animal).ThrowMud();
                }
            }
        }
    }
}
