using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleNbAnimaux : RuleEnclosure
{
    // Mettre à jour "isFailed"
    bool isAnObjective = false; // true: rule is an objective, false: rule is a limit
    int animalCount;

    /* ===============================================
    ====================   METHODS   =================
    =============================================== */

    
    // ==================== GETTTER AND STATE
    
    //public void limitReached


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //animalCount = enclosure.
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
