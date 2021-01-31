using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightScript : MonoBehaviour
{

    public float lifeSpan = 3f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DestroyRoutine()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }
}
