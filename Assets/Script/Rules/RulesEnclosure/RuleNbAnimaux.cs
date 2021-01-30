using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleNbAnimaux : RuleEnclosure
{
    // Mettre à jour "isFailed"
    public bool isAnObjective = false; // true: rule is an objective, false: rule is a limit
    public int animalCount;
    public System.Type type = null;

    /* ===============================================
    ====================   METHODS   =================
    =============================================== */

    
    // ==================== GETTTER AND STATE
    
    private void updateState()
    {
        int count;
        if (type == null) count = enclosure.CountAnimals();
        else count = enclosure.CountAnimals(type);
        
        if (count < animalCount)
        {
            state = 0;
            isFailed = false;
        } else {
            if (isAnObjective)
            {
                state = 3;
            } else {
                state = 2;
                isFailed = true;
            }
        }
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (isDefinitelyFailed) return;
        updateState();
    }
}
