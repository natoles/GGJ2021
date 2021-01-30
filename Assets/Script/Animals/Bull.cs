using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bull : Animal
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
    }
    protected override IEnumerator RageState()
    {
        Debug.Log("bull cor");
        yield return new WaitForSeconds(0.5f);
    }
}
