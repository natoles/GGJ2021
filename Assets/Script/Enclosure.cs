using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enclosure : MonoBehaviour
{
    // PUBLIC
    public List<Animal> animals;


    // PRIVATE
    private bool isFull = false;

    // Start is called before the first frame update
    void Start()
    {
        isFull = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
