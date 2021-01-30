using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public class SpawnInfo {
    public string type;
    public int quantity;
    public float time;
};

public class MyGameManager : MonoBehaviour
{
    // TODO:
    /*
    - Gestion du spawn des animaux
    - Level manager -> prends un tableau en entrée
    - Référence à chacun des animaux/enclos/règles
    - Boucle de jeu
    - Barre de stress
    */
    
    // PUBLIC
    public Transform bullPrefab;
    public Transform catPrefab;
    public Transform cowPrefab;
    public Transform dogPrefab;
    public Transform mousePrefab;

    // PRIVATE
    private float levelTimeStart;

    public List<SpawnInfo> spawnList;
    public List<Animal> aliveAnimalList;

    // TODO: Ajouter les animaux morts à spawnList avec t+5s

    public Transform selectAnimalPrefab(string animal)
    {
        switch(animal)
        {
            case "Bull":
                return bullPrefab;
            case "Cat":
                return catPrefab;
            case "Cow":
                return cowPrefab;
            case "Dog":
                return dogPrefab;
            case "Mouse":
                return mousePrefab;
        }
        
        return null;
    }

    public void spawnAnimal(SpawnInfo spawnInfo)
    {
        Transform prefab;
        for (int i = 0; i < spawnInfo.quantity; i++)
        {
            // Selecting prefab
            prefab = selectAnimalPrefab(spawnInfo.type);
            Assert.IsNotNull(prefab, spawnInfo.type + " is not a valid animal! Use Bull, Cat, Cow, Mouse");

            // Spawning Animal
            

        }
    }

    public void updateSpawnAnimals()
    {
        float currentTime = Time.time - levelTimeStart;
        List<SpawnInfo> toSpawn = spawnList.FindAll(x => x.time - currentTime < 0);

        for (int i = 0; i < toSpawn.Count; i++)
        {
            SpawnInfo spawnInfo = toSpawn[i];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        levelTimeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        updateSpawnAnimals();
    }
}
