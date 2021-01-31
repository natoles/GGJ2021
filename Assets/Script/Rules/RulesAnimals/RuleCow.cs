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
        if (animal.IsInEnclosure()){
            int nBull = CountNeighborTypeOf(StringToClass.TypeFromString("Bull"));
            if (nBull == 1){
                animal.Calm();
            } else if (nBull == 2){
                animal.Run();
            } else {
                animal.Enrage();
            }
        }
    }
}
