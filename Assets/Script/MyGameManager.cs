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
    - Level manager -> prends un tableau en entrée
    - Référence à chacun des animaux/enclos/règles
    - Barre de stress
    - Condition de 
    */
    
    // PUBLIC
    public Transform bullPrefab;
    public Transform catPrefab;
    public Transform cowPrefab;
    public Transform dogPrefab;
    public Transform mousePrefab;

    public Camera gameCamera;


    public int objectiveAnimalInEnclosure = 0;
    public int currentAnimalInEnclosure = 0;

    // PRIVATE
    private float levelTimeStart;

    public List<SpawnInfo> spawnList;
    public List<Animal> aliveAnimalList;

    // TODO: Ajouter les animaux morts à spawnList avec t+5s
    private void OnStartComputeObjective()
    {
        objectiveAnimalInEnclosure = 0;
        foreach (SpawnInfo spawnInfo in spawnList)
        {
            if (SelectAnimalPrefab(spawnInfo.type) == null){
                Debug.Log(spawnInfo.type + " is not a valid animal! Use Bull, Cat, Cow, Mouse");
                spawnList.Remove(spawnInfo);
            } else {
                objectiveAnimalInEnclosure += spawnInfo.quantity;
            }
        }
    }

    public Transform SelectAnimalPrefab(string animal)
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

    public int SpawnAnimal(SpawnInfo spawnInfo)
    {
        Transform prefab;
        for (int i = 0; i < spawnInfo.quantity; i++)
        {
            // Selecting prefab
            prefab = SelectAnimalPrefab(spawnInfo.type);
            if (prefab == null){
                Debug.Log(spawnInfo.type + " is not a valid animal! Use Bull, Cat, Cow, Mouse");
                return -1;
            }

            // Getting Camera dimension
            //Screen.width, Screen.height;
            //Camera
            //TODO: automate this
            int xmin = -300;
            int xmax = 300;
            int ymin = -300;
            int ymax = 300;

            // Selecting random point for spawn
            bool onXAxis = Random.value > 0.5;
            int x,y;
            if (onXAxis)
            {
                x = Random.Range(xmin, xmax);
                y = Random.value > 0.5 ?  ymin - 5: ymax + 5;
            } else {
                x = Random.value > 0.5 ?  xmin - 5: xmax + 5;
                y = Random.Range(ymin, ymax);
            }

            // Spawning Animal
            Instantiate(prefab, new Vector2(x, y), Quaternion.identity);
        }

        return 0;
    }

    public void UpdateSpawnAnimals()
    {
        float currentTime = Time.time - levelTimeStart;
        List<SpawnInfo> toSpawn = spawnList.FindAll(x => x.time - currentTime < 0);

        foreach (SpawnInfo spawnInfo in spawnList)
        {
            SpawnAnimal(spawnInfo);
            spawnList.Remove(spawnInfo);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        levelTimeStart = Time.time;
        OnStartComputeObjective();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpawnAnimals();
    }
}
