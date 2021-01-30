using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEnclosure : Rule
{

    public Enclosure enclosure;


    /* ===============================================
    ====================   METHODS   =================
    =============================================== */

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        enclosure = gameObject.GetComponent<Enclosure>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
