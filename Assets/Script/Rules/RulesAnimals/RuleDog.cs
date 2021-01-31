using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleDog : RuleAnimal
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
            if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Wolf"))) {
                animal.Chase(FindNeighborTypeOf(StringToClass.TypeFromString("Wolf")));
            } else if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Cat"))){
                animal.Enrage();
            } else {
                animal.Calm();
            }
        }
    }
}
