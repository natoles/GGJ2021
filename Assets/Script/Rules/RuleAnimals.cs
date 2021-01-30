using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleAnimal : Rule
{
    // PUBLIC
    public Animal animal;

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
        return animal.currentEnclosure.GetAnimals();
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
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
