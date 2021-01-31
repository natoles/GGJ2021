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

    public GameObject tilemapObstacles;

    public Transform p1;
    public Transform p2;
    public Transform p3;
    public Transform p4;

    public int objectiveAnimalInEnclosure = 0;
    public int currentAnimalInEnclosure = 0;
    public int nbRulesInGame, nbRulesFailed;
    public float progression;

    public ProgressBar progressBar;

    public bool victoryTest;

    // PRIVATE
    private float levelTimeStart;

    public List<SpawnInfo> spawnList;
    public List<Animal> animalsInEnclosure;
    public List<Enclosure> enclosureList;

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

    private void UpdateAnimalsReference()
    {
        animalsInEnclosure.Clear();
        foreach (Enclosure enclosure in enclosureList)
        {
            foreach (Animal animal in enclosure.animals)
            {
                animalsInEnclosure.Add(animal);
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

            // Selecting spawning point
            float r = Random.value;
            float x, y;
            if (r < 0.25)
            {  
                x = Random.Range(p1.transform.position.x, p2.transform.position.x);
                y = p1.transform.position.y;
            } 
            else if (r < 0.5)
            {
                x = Random.Range(p3.transform.position.x, p4.transform.position.x);
                y = p3.transform.position.y;
            }
            else if (r < 0.75)
            {
                x = p1.transform.position.x;
                y = Random.Range(p1.transform.position.y, p4.transform.position.y);
            }
            else
            {
                x = p2.transform.position.x;
                y = Random.Range(p2.transform.position.y, p3.transform.position.y);
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

        foreach (SpawnInfo spawnInfo in toSpawn)
        {
            SpawnAnimal(spawnInfo);
            spawnList.Remove(spawnInfo);
        }
    }

    public float ComputeProgressionPercent()
    {
        
        int animalsInEnclosure = 0;
        foreach(Enclosure enclosure in enclosureList)
        {
            animalsInEnclosure += enclosure.CountAnimals();
            Debug.Log(animalsInEnclosure.ToString());
        }

        if ((objectiveAnimalInEnclosure + nbRulesInGame) == 0)
            return 0;
        return ((float) (animalsInEnclosure + nbRulesInGame - nbRulesFailed))
                / ((float) (objectiveAnimalInEnclosure + nbRulesInGame));
    }

    public void updateProgressBar()
    {
        progressBar.CurrentValue = progression;
    }


    // ==================== VICTORY
    public void Victory()
    { 
        UpdateAnimalsReference();
        tilemapObstacles.GetComponent<Collider2D>().enabled = false;
        Debug.Log(AstarPath.active);
        AstarPath.active.Scan();
        foreach (Animal animal in animalsInEnclosure)
        {
            animal.StopCoroutine(animal.movementsHandlingCoroutine);

            MovementProperties mProperties = new MovementProperties();

            mProperties.moveSpeed = 50000f;
            mProperties.topSpeed = 50f;
            mProperties.minMoveInterval = 0f;
            mProperties.maxMoveInterval = 0f;
            mProperties.linearDrag = 6f;

            animal.ComputeMovement(GameObject.Find("BusStopPos").transform.position, mProperties);
            animal.MoveTo(GameObject.Find("BusStopPos").transform.position);
        }
    }


    // ==================== START AND UPDATES

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
        progression = ComputeProgressionPercent();
        updateProgressBar();
        if ((1f - progression) < 1e-4 || victoryTest){
            Victory();
        }
    }
}
