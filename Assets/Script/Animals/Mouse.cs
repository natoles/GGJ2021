using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : Animal
{
    public const float timeBeforeEnteringEnclosure = 5f; // seconds

    private float startEnterEnclosureTimer = -1;
    private float enterEnclosureTimeLeft = 0; // timer

    public override void Flee()
    {
        if (exteriorEnclosure != null)
        {
            Animal currentAnimal = gameObject.GetComponent<Animal>();
            currentEnclosure.RemoveAnimal(currentAnimal);
            exteriorEnclosure.AddAnimal(currentAnimal);
            rb.velocity = Vector2.zero;
            transform.position = currentEnclosure.RandomPoint();
            reachedEndOfPath = true;
            path = null;
        } else {
            // Should not happen but just in case
            enterEnclosure();
        }
    }

    private void resetMouseTimer()
    {
        startEnterEnclosureTimer = -1;
        enterEnclosureTimeLeft = timeBeforeEnteringEnclosure;
    }

    // Oui c'est Flee(), et alors ? (Ne pas modifier car Flee fonctionne avec Run())
    public void enterEnclosure()
    {
        Animal currentAnimal = gameObject.GetComponent<Animal>();
        currentEnclosure.RemoveAnimal(currentAnimal);
        (gameManager.GetLessPopulatedOtherEnclosure(currentEnclosure)).AddAnimal(currentAnimal);
        rb.velocity = Vector2.zero;
        transform.position = currentEnclosure.RandomPoint();
        reachedEndOfPath = true;
        path = null;
    }

    public void updateEnterEnclosure()
    {
        if (!IsInEnclosure())
        {
            if (startEnterEnclosureTimer < 1e-5){
                startEnterEnclosureTimer = Time.time;
                enterEnclosureTimeLeft = timeBeforeEnteringEnclosure;
            } else {
                enterEnclosureTimeLeft -= Time.deltaTime;
                if (enterEnclosureTimeLeft < 0){
                    resetMouseTimer();
                    enterEnclosure();
                }
            }
        }
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        resetMouseTimer();

        type = "Mouse";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        updateEnterEnclosure();
    }
}
