using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enclosure : MonoBehaviour
{
    // PUBLIC
    public List<Animal> animals;
    public int totalSpace = 10;
    public int currentUsedSpace = 0;

    // PRIVATE


    // METHODS

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
        return RandomPointInBounds(gameObject.GetComponent<Collider>().bounds);
    }

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
    }

    // Add an animal into the enclosure. Return 0 for success and -1 on fail.
    public int AddAnimal(Animal animal)
    {
        if (IsFull()) return -1;
        
        animals.Add(animal);
        currentUsedSpace += 1;

        return 0;
    }

    // Remove an animal into the enclosure. Return 0 for success and -1 on fail.
    public int RemoveAnimal(Animal animal)
    {
        if (IsEmpty()) return -1;
        
        animals.Remove(animal);
        currentUsedSpace -= 1;

        return 0;
    }

    // Remove an animal into the enclosure. Return 0 for success and -1 on fail.
    public int TransferAnimal(Animal animal, Enclosure enclosure)
    {
        if (enclosure.IsFull()) return -1;
        if (!animals.Contains(animal)) return -1;

        if (enclosure.AddAnimal(animal) != 0) return -1;
        if (RemoveAnimal(animal) != 0) return -1;
        
        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        Empty();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
