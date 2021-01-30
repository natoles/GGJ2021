using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enclosure : MonoBehaviour
{
    // PUBLIC
    public List<Animal> animals;
    public int totalSpace = 15;
    public int currentUsedSpace = 0;
    public bool isExterior = false; // true if this "enclosure" is actually the exterior

    public List<Rule> rules;

    // PRIVATE

    /* ===============================================
    ====================   METHODS   =================
    =============================================== */


    // ==================== RANDOM POINT
    public static Vector2 RandomPointInBounds(Bounds bounds)
    {
        return new Vector2(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y)
            );
    }

    // Returns a random point into the collider
    public Vector2 RandomPoint()
    {
        return RandomPointInBounds(gameObject.GetComponent<BoxCollider2D>().bounds);
    }

    // ==================== GETTTER AND STATE
    public bool IsFull()
    {
       return currentUsedSpace >= totalSpace;
    }

    public bool IsEmpty()
    {
       return currentUsedSpace <= 0;
    }

    // Removes all animals from the enclosure
    public void Empty()
    {
       currentUsedSpace = 0;
       animals.Clear();
       rules.Clear();
    }

    public List<Animal> GetAnimals()
    {
        return animals;
    }

    // ==================== ANIMAL MANAGEMENT

    // Add an animal into the enclosure. Return 0 for success and -1 on fail.
    public int AddAnimal(Animal animal)
    {
        if (IsFull()) return -1;
        if (animals.Contains(animal)) return -1;
        
        animals.Add(animal);
        currentUsedSpace += 1;
        animal.EnterEnclosure(this);

        return 0;
    }

    // Remove an animal into the enclosure. Return 0 for success and -1 on fail.
    public int RemoveAnimal(Animal animal)
    {
        if (IsEmpty()) return -1;
        if (!animals.Contains(animal)) return -1;
        
        animals.Remove(animal);
        currentUsedSpace -= 1;
        animal.LeaveEnclosure();
        return 0;
    }

    // Remove an animal from the current enclosure to the "enclosureTo" one. Return 0 for success and -1 on fail.
    public int TransferAnimal(Animal animal, Enclosure enclosureTo)
    {
        if (enclosureTo.IsFull()
            || !animals.Contains(animal)
            || enclosureTo.animals.Contains(animal)) return -1;

        if (RemoveAnimal(animal) != 0) return -1;
        if (enclosureTo.AddAnimal(animal) != 0) return -1;


        
        return 0;
    }

    // Adding dragged animal that has entered the enclosure collider
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Drag") return;
        Animal animal = collider.gameObject.GetComponent<Animal>();
        
        if (AddAnimal(animal) == 0)
        {
            // Error code
        }
    }

    // Removing dragged animal that has left the enclosure collider
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag != "Drag") return;
        Animal animal = collider.gameObject.GetComponent<Animal>();
        
        if (RemoveAnimal(animal) == 0)
        {
            // Error code
        }
    }


    // ==================== RULE MANAGEMENT










    // ==================== START AND UPDATES

    // Start is called before the first frame update
    void Start()
    {
        Empty();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    
    //}
}
