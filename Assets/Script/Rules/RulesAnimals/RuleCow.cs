using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleCow : RuleAnimal
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
        if (animal.inEnclosure){
            if (IsAnyNeighborTypeOf(StringToClass.TypeFromString("Bull")))
            {
                animal.Calm();
            }
            else if(IsAnyNeighborTypeOf(StringToClass.TypeFromString("Mouse")))
            {
                animal.Run();
            }
            else
            {
                animal.Enrage();
            }
        }
    }
}
