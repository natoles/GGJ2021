using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleWolf : RuleAnimal
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
            if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Dog"))){
                animal.Enrage();
            } else if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Sheep"))) {
                animal.ChaseFight(FindNeighborTypeOf(StringToClass.TypeFromString("Sheep")));
            } else if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Pig"))) {
                animal.ChaseFight(FindNeighborTypeOf(StringToClass.TypeFromString("Pig")));
            } else {
                animal.Calm();
            }
        }
    }
}
