using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Spawner : MonoBehaviour
{
    [SerializeField] TutorialController tutorialController;
    public float timeToSpawn = 1f; // set by gamemaster
    private float spawnTimeAtstart;
    private GameMaster gameMaster;

    public GameObject asteroid;
    public GameObject asteroid2;
    public GameObject iceBlock;

    private List<GameObject> allSpawners;
    private GameObject prevSpawner = null;
    private int spawnTripleCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        gameMaster = GameMaster.Instance;
        allSpawners = new List<GameObject>();
        foreach (Transform child in transform)
        {
            allSpawners.Add(child.gameObject);
        }

        spawnTimeAtstart = timeToSpawn;
        if (SceneManager.GetActiveScene().name == "GameScene" && tutorialController.IsTutorialCompleted())
        {
            StartSpawners();
        }
        else
        {
            StopSpawners();
        }
    }

    // Update is called once per frame
    void Update()
    {
        PickObstacle();
    }

    private void SpawnObstacle()
    {
        //GameObject gO;
        List<GameObject> spawnersList = new List<GameObject>();
        spawnersList.AddRange(allSpawners);

        if (prevSpawner != null)
        {
            spawnersList.Remove(prevSpawner);
        }

        Transform spawner = spawnersList[Random.Range(0, spawnersList.Count)].transform;
        prevSpawner = spawner.gameObject;

        int random = Random.Range(0, 101);
        if (random <= 20)
        {
            // rare - spawn IceBlock
            GameObject iceblock = ObjectPool.SharedInstance.GetPooledIceBlockObject();
            if (iceblock != null)
            {
                iceblock.transform.position = spawner.position;
                iceblock.transform.rotation = Quaternion.identity;
                iceblock.SetActive(true);
            }
        }
        else
        {
            // default
            //gO = Random.value > 0.5f ? asteroid : asteroid2;
            float randomFloat = Random.value;
            if (randomFloat > 0.5f)
            {
                GameObject asteroid = ObjectPool.SharedInstance.GetPooledAsteroidObject();
                if (asteroid != null)
                {
                    asteroid.transform.position = spawner.position;
                    asteroid.transform.rotation = Quaternion.identity;
                    asteroid.SetActive(true);
                }
            }
            else
            {
                GameObject asteroid2 = ObjectPool.SharedInstance.GetPooledAsteroidTwoObject();
                if (asteroid2 != null)
                {
                    asteroid2.transform.position = spawner.position;
                    asteroid2.transform.rotation = Quaternion.identity;
                    asteroid2.SetActive(true);
                }
            }            
        }

        //Instantiate(gO, spawner.position, Quaternion.identity);
        // reset timer
        spawnTimeAtstart = timeToSpawn;
    }

    void SpawnTripleObs()
    {
        foreach (GameObject s in allSpawners)
        {
            Transform trans = s.transform;
            //Instantiate(iceBlock, trans.position, trans.rotation);
            GameObject iceblock = ObjectPool.SharedInstance.GetPooledIceBlockObject();
            if (iceblock != null)
            {
                iceblock.transform.position = trans.position;
                iceblock.transform.rotation = Quaternion.identity;
                iceblock.SetActive(true);
            }
        }
        spawnTimeAtstart = timeToSpawn;
    }

    private void PickObstacle()
    {
        // countdown timer
        if (spawnTimeAtstart > 0)
        {
            spawnTimeAtstart -= Time.deltaTime;
            return;
        }

        int random = Random.Range(1, 11);
        if (random == 1 && spawnTripleCounter >= 10)
        {
            // rare
            // spawn 3 iceblocks
            SpawnTripleObs();
            spawnTripleCounter = 0;

        }
        else
        {
            SpawnObstacle();
            spawnTripleCounter++;
        }
    }

    public void StopSpawners()
    {
        gameObject.SetActive(false);
    }

    public void StartSpawners()
    {
        gameObject.SetActive(true);
    }
}
