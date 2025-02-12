using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SpawnInfo {
    public string type;
    public int quantity;
    public float time;
};

[System.Serializable]
public class EnclosureInfo {
    public Enclosure enclosure;
    public int maxCapacity;
    public float time;
};

[System.Serializable]
public class EnclosureExteriorLimits {
    public int maxCapacity;
    public float time;
};

public class MyGameManager : MonoBehaviour
{
    // PUBLIC
    public Transform bullPrefab;
    public Transform catPrefab;
    public Transform cowPrefab;
    public Transform dogPrefab;
    public Transform mousePrefab;
    public Transform pigPrefab;
    public Transform sheepPrefab;
    public Transform wolfPrefab;

    public Camera gameCamera;
    public ShakeBehavior cameraShake;

    public GameObject tilemapObstacles;
    public GameObject bus;

    public string LevelToLoad;

    public AudioSource victorySound;

    public Transform p1;
    public Transform p2;
    public Transform p3;
    public Transform p4;
    public Enclosure exteriorEnclusure;

    public Text rightLimit, leftLimit, bottomLimit, exteriorLimit;

    public int objectiveAnimalInEnclosure = 0;
    public int currentAnimalInEnclosure = 0;
    public float progression;

    public ProgressBar progressBar;

    bool oneVictory = true;
    public const bool checkIfCalm = true;

    // PRIVATE
    private float levelTimeStart;

    public List<SpawnInfo> spawnList;
    public List<EnclosureInfo> enclosureCapacityList;
    public List<Animal> animalsInEnclosure;
    public List<Enclosure> enclosureList;
    public List<EnclosureExteriorLimits> enclosureExteriorLimits;

    private void OnStartComputeObjective()
    {
        objectiveAnimalInEnclosure = 0;
        foreach (SpawnInfo spawnInfo in spawnList)
        {
            if (SelectAnimalPrefab(spawnInfo.type) == null) {
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
        switch (animal)
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
            case "Pig":
                return pigPrefab;
            case "Sheep":
                return sheepPrefab;
            case "Wolf":
                return wolfPrefab;
        }

        return null;
    }

    public Enclosure GetRandomOtherEnclosure(Enclosure enclosure)
    {
        int r = Mathf.FloorToInt(Random.value * 3);
        while (enclosureList[r] == enclosure)
        {
            r = Mathf.FloorToInt(Random.value * 3);
        }

        return enclosureList[r];
    }

    public Enclosure GetLessPopulatedOtherEnclosure(Enclosure enclosure)
    {
        Enclosure enclosureMin = enclosureList[0] == enclosure ? enclosureList[1] : enclosureList[0];
        Enclosure enclosureTmp;
        int i;
        for (i = 1; i < enclosureList.Count; i++)
        {
            enclosureTmp = enclosureList[i];
            if (enclosureTmp.CountAnimals() < enclosureMin.CountAnimals())
            {
                enclosureMin = enclosureTmp;
            }
        }

        return enclosureMin;
    }

    public int SpawnAnimal(SpawnInfo spawnInfo)
    {
        Transform prefab;
        for (int i = 0; i < spawnInfo.quantity; i++)
        {
            // Selecting prefab
            prefab = SelectAnimalPrefab(spawnInfo.type);
            if (prefab == null) {
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
            exteriorEnclusure.currentUsedSpace += 1;
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

    public void UpdateEnclosureCapacity()
    {
        float currentTime = Time.time - levelTimeStart;
        List<EnclosureInfo> toModify = enclosureCapacityList.FindAll(x => x.time - currentTime < 0);

        foreach (EnclosureInfo enclosureInfo in toModify)
        {
            Enclosure enc = enclosureInfo.enclosure;
            enc.totalSpace = enclosureInfo.maxCapacity;
        }
    }

    public void UpdateEnclosureExteriorLimit()
    {
        float currentTime = Time.time - levelTimeStart;
        List<EnclosureExteriorLimits> limits = enclosureExteriorLimits.FindAll(x => x.time - currentTime < 0);

        foreach (EnclosureExteriorLimits enclosureExteriorLimits in limits)
        {
            exteriorEnclusure.totalSpace = enclosureExteriorLimits.maxCapacity;
        }
    }


    // ==================== VICTORY
    public float ComputeProgressionPercent()
    {
        int animalsInEnclosure = 0;
        foreach (Enclosure enclosure in enclosureList)
        {
            // Check how many animals are calm
            List<Animal> enclosureAnimals = enclosure.GetAnimals();
            foreach (Animal a in enclosureAnimals)
            {
                if (checkIfCalm) {
                    if (a.IsCalm() || a.IsFollow()) animalsInEnclosure += 1;
                } else {
                    animalsInEnclosure += 1;
                }
            }
        }

        // Error division by 0
        if (objectiveAnimalInEnclosure == 0)
            return 0;

        return ((float)animalsInEnclosure) / ((float)objectiveAnimalInEnclosure);
    }

    public void UpdateProgressBar()
    {
        if (!oneVictory) progressBar.CurrentValue = 1f;
        else if (progressBar == null) return;
        else progressBar.CurrentValue = progression;
    }

    private bool IsVictoryConditionsFulfilled()
    {
        if ((1f - progression) >= 1e-4 || !oneVictory) return false;

        foreach (Enclosure enclosure in enclosureList)
        {
            if (enclosure.IsOverFull()) return false;
        }

        return true;
    }

    public void Victory()
    {
        UpdateAnimalsReference();
        StartCoroutine(VictoryCoroutine());
        tilemapObstacles.GetComponent<Collider2D>().enabled = false;
        Debug.Log(AstarPath.active);
        AstarPath.active.Scan();
        victorySound.Play();
        foreach (Animal animal in animalsInEnclosure)
        {
            animal.StopCoroutine(animal.movementsHandlingCoroutine);

            MovementProperties mProperties = new MovementProperties();

            mProperties.moveSpeed = 40000f;
            mProperties.topSpeed = 40f;
            mProperties.minMoveInterval = 0f;
            mProperties.maxMoveInterval = 0f;
            mProperties.linearDrag = 5f;

            bus.GetComponent<BusScript>().firstPhase = true;
            bus.GetComponent<BusScript>().leave = true;

            animal.isVictory = true;
            animal.ComputeMovement(GameObject.Find("BusStopPos").transform.position, mProperties);
            animal.MoveTo(GameObject.Find("BusStopPos").transform.position);
            animal.Calm();
            animal.StopAllCoroutines();
        }
    }

    public IEnumerator VictoryCoroutine()
    {
        yield return new WaitForSeconds(8f);
        SceneManager.LoadScene(LevelToLoad);
    }


    
    // ==================== DEFEAT

    // Return true if deafeat, false else
    public bool IsDefeatConditionsFulfilled()
    {

        if (exteriorEnclusure.currentUsedSpace > exteriorEnclusure.totalSpace) return true;

        return false;
    }

    
    public void Defeat()
    {
        SceneManager.LoadScene(LevelToLoad);
    }


    // ==================== START AND UPDATES

    // Start is called before the first frame update
    void Start()
    {
        levelTimeStart = Time.time;
        OnStartComputeObjective();
        exteriorLimit.text = ""; // Remove me and uncomment in "Update" to enable exteriorLimit text
        victorySound = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    void Update()
    {
        UpdateSpawnAnimals();
        UpdateEnclosureCapacity();
        UpdateEnclosureExteriorLimit();
        progression = ComputeProgressionPercent();
        UpdateProgressBar();

        //if (IsDefeatConditionsFulfilled()){
        //    Defeat();
        //}
        if (IsVictoryConditionsFulfilled()){
            oneVictory = false;
            Victory();
        }

        if (bottomLimit == null) return;

        //Limit text
        bool launchShake = false;
        int current, max;
        current = enclosureList[0].currentUsedSpace;
        max = enclosureList[0].totalSpace;
        bottomLimit.text = current.ToString() + "/" + max.ToString();
        if (current > max)
        {
            bottomLimit.color = Color.red;
            launchShake = true;
        }
        else
        {
            bottomLimit.color = Color.gray;
        }

        current = enclosureList[1].currentUsedSpace;
        max = enclosureList[1].totalSpace;
        leftLimit.text = current.ToString() + "/" + max.ToString();
        if (current > max)
        {
            leftLimit.color = Color.red;
            launchShake = true;
        }
        else
        {
            leftLimit.color = Color.gray;
        }

        current = enclosureList[2].currentUsedSpace;
        max = enclosureList[2].totalSpace;
        rightLimit.text = current.ToString() + "/" + max.ToString();
        if (current > max)
        {
            rightLimit.color = Color.red;
            launchShake = true;
        }
        else
        {
            rightLimit.color = Color.gray;
        }

        if (launchShake) cameraShake.TriggerShake();
        else cameraShake.StopShake();

        current = exteriorEnclusure.currentUsedSpace;
        max = exteriorEnclusure.totalSpace;
        //exteriorLimit.text = "!! " + current.ToString() + "/" + max.ToString() + " !!";
    }
}
