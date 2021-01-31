using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleAnimal : Rule
{
    // PUBLIC
    public Animal animal;
    protected List<Animal> neighbors;

    // PRIVATE

    /* ===============================================
    ====================   METHODS   =================
    =============================================== */

    
    // ==================== GETTTER AND STATE
    public Enclosure GetEnclosure()
    {
        return animal.currentEnclosure;
    }

    public List<Animal> GetNeighborAnimals()
    {
        neighbors.Clear();
        if (!animal.IsInEnclosure()) return neighbors;

        // Get enclosure animals and remove the current one
        List<Animal> enclosureAnimals = animal.currentEnclosure.GetAnimals();
        foreach (Animal animalEnclosure in enclosureAnimals)
        {
            if (animalEnclosure != animal) neighbors.Add(animalEnclosure);
        }

        return neighbors;
    }

    // ==================== SEARCH TOOLS INTO ENCLOSURE

    // Returns a random animal of the given type in the same enclosure if exists, else return null
    public Animal FindNeighborTypeOf(System.Type type)
    {
        List<Animal> neighbors = GetNeighborAnimals();
        List<Animal> tmp = neighbors.FindAll(x => x.GetType() == type);
        if (tmp.Count <= 0) return null; // NO NEIGHBHOR OF THAT TYPE
        
        return tmp[(int) Random.Range(0, tmp.Count-0.1f)];
    }

    public int CountNeighborTypeOf(System.Type type)
    {
        List<Animal> neighbors = GetNeighborAnimals();
        List<Animal> tmp = neighbors.FindAll(x => x.GetType() == type);
        return tmp.Count;
    }

    // Checks if an animal of the given type exists in the same enclosure
    public bool IsAnyNeighborTypeOf(System.Type type)
    {
        return FindNeighborTypeOf(type) != null;
    }

    // ==================== START AND UPDATES

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        animal = gameObject.GetComponent<Animal>();
        neighbors = new List<Animal>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
