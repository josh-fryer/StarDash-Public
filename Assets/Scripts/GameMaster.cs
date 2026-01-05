using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class GameMaster : MonoBehaviour
{
    public List<GameObject> obstaclesInPlay;
    public GameObject playerObj;
    [SerializeField] UIManager uIManager;
    [SerializeField] TutorialController tutorialController;
    [SerializeField] Spawner spawner;
    [SerializeField] float distanceSpeedMultiplyer = 3f;
    [SerializeField] float addPointsLoopInSeconds = 0.00001f;
    public float globalObstacleSpeed = 12;

    public bool isAlive = true;
    [HideInInspector] public Vector3 camPos;

    [Header("Target for Distance to hit before incresing difficulty")]
    public int distanceTarget = 250;
    public int increaseTargetBy = 150;

    // Start speeds and spawn time
    [Header("Start and Max spawn times and speeds")]
    public float minObstacleSpeed = 12f;
    public float maxTimeToSpawn = 1f;

    public float maxObstacleSpeed = 32f;
    public float minTimeToSpawn = 0.35f;
    private float newTimeToSpawn;

    private int points = 0; // score in ui
    private int distance = 0;
    [HideInInspector]
    public int deaths = 0;
    private float time = 0;

    private static GameMaster _instance;
    private bool coroutineRunning = false;
    private SaveLoad saveLoad;
    private Ad_Manager _AdManager;
    private ObjectPool _ObjectPool;
    private List<GameObject> pooledActiveObstacles;
    [SerializeField] int deathsBeforeAd = 3;

    [Header("Dev Tools")]
    [Tooltip("Should be off/false for release")]
    public bool maxSpeedFromStart_DevTool = false;

    // Singleton pattern
    public static GameMaster Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameMaster");
                go.AddComponent<GameMaster>();
            }
            return _instance;
        }
    }


    void Awake()
    {
        _instance = this;
        camPos = GameObject.FindGameObjectWithTag("MainCamera").transform.position;
        obstaclesInPlay = new List<GameObject>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        uIManager.SetScoreText(points);
        saveLoad = FindObjectOfType<SaveLoad>();
        _AdManager = FindObjectOfType<Ad_Manager>();
        _ObjectPool = FindObjectOfType<ObjectPool>();
        pooledActiveObstacles = new List<GameObject>();

        if (maxSpeedFromStart_DevTool)
        {
            // max hardest settings
            spawner.timeToSpawn = minTimeToSpawn;
            globalObstacleSpeed = maxObstacleSpeed;
        }
        else
        {
            // deafault start settings
            spawner.timeToSpawn = maxTimeToSpawn;

        }
        globalObstacleSpeed = minObstacleSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            Timer();
            DifficultyProgression();
        }
    }

    void Timer()
    {
        time += Time.deltaTime * distanceSpeedMultiplyer;
        distance = Mathf.FloorToInt(time);
        uIManager.SetDistanceText(distance);
    }

    void DifficultyProgression()
    {
        if (maxSpeedFromStart_DevTool || globalObstacleSpeed >= maxObstacleSpeed)
        {
            return;
        }

        if (distance >= distanceTarget && !coroutineRunning)
        {
            coroutineRunning = true;
            StartCoroutine(IncreaseDifficulty());

        }
    }

    IEnumerator IncreaseDifficulty()
    {
        // pause spawning to let asteroids with old speeds go
        spawner.StopSpawners();
        GameObject lastAsteroid = new GameObject();
        lastAsteroid.transform.position = new Vector3(0, 0, -50f);
        AddActiveObstaclesToList();

        // find last asteroid
        foreach (GameObject i in pooledActiveObstacles)
        {
            if (i.transform.position.z > lastAsteroid.transform.position.z)
            {
                lastAsteroid = i;
            }
        }

        uIManager.ShowHideSpeedUpPanel();
        yield return new WaitUntil(() => !lastAsteroid.activeInHierarchy || lastAsteroid.transform.position.z <= -3);
        // increase diffulcuilty
        newTimeToSpawn = spawner.timeToSpawn - 0.175f;
        globalObstacleSpeed += 5;
        if (newTimeToSpawn < minTimeToSpawn)
        {
            newTimeToSpawn = minTimeToSpawn;
        }

        if (globalObstacleSpeed > maxObstacleSpeed)
        {
            globalObstacleSpeed = maxObstacleSpeed;
        }

        spawner.timeToSpawn = newTimeToSpawn;
        // increase speed

        distanceTarget += increaseTargetBy;
        spawner.StartSpawners();
        coroutineRunning = false;
    }

    public void OnDeath()
    {
        // stop spawners, timer and obstacles
        isAlive = false; // also stops asteroids moving

        deaths++;
        Debug.Log("Deaths = " + deaths);

        spawner.StopSpawners();

        // slow motion for a few seconds
        StartCoroutine(DeathSlowMo());
    }

    IEnumerator DeathSlowMo()
    {
        float slowMoSpeed = 0.3f;
        float defaultFixedDeltaTime = 0.02f;
        Time.timeScale = slowMoSpeed;
        Time.fixedDeltaTime = defaultFixedDeltaTime * slowMoSpeed;
        yield return new WaitForSecondsRealtime(2.5f);
        // stop and show score screen. restart scene?
        // reset time scale
        Time.timeScale = 1f;
        Time.fixedDeltaTime = defaultFixedDeltaTime;
        Debug.Log("Finished slow mo");
        // switch UI & show score
        // save highscore
        int total = GetPoints() + GetDistance();
        bool isNewHighScore = saveLoad.SetHighScore(total);
        uIManager.ShowHideDeathUI(isNewHighScore);
    }

    public int GetPoints()
    {
        return points;
    }

    public void AddPoints(int addPoints)
    {
        //points += addPoints;
        StartCoroutine(AddPointByPoint(addPoints)); 
    }

    IEnumerator AddPointByPoint(int addPoints)
    {
        for (int i = 0; i < addPoints; i++)
        {
            points++;
            uIManager.SetScoreText(points);
            yield return new WaitForSeconds(addPointsLoopInSeconds);
        }      
    }

    public int GetDistance()
    {
        return distance;
    }


    // activated by play again UI button
    public void PlayAgain()
    {
        Debug.Log("Play Again!");
        // ShowAd
        if (deaths > deathsBeforeAd)
        {
            // reset deaths counter
            saveLoad.SetDeathsCount(0);
            deaths = 0;

            // 1 true
            if (PlayerPrefs.GetInt("AdsRemoved", 0) == 0)
            {
                _AdManager.ShowRewardedInterstitialAd();
            }
            else
            {
                saveLoad.LoadScene(1);
                
            }
        }
        else
        {
            saveLoad.LoadScene(1);
        }
    }

    public void Continue()
    {
        Debug.Log("Clicked Continue()");
        _AdManager.ShowRewardedInterstitialAd(); // will load scene after ad
    }

    // call here by ad manager after reward earned
    public void AdRewardEarnedContinueGame()
    {
        //clear asteroids
        AddActiveObstaclesToList();
        pooledActiveObstacles.ForEach(o => o.SetActive(false));

        //disable game over ui
        uIManager.ShowHideDeathUI();

        // spawn back player ship
        //Instantiate(player, new Vector3(0,0,0), Quaternion.identity);
        playerObj.GetComponent<Player>().SetAliveAgain();
        isAlive = true;
        spawner.StartSpawners();
    }

    private void AddActiveObstaclesToList()
    {
        pooledActiveObstacles.AddRange(_ObjectPool.pooledAsteroidObjects.FindAll(x => x.activeInHierarchy));
        pooledActiveObstacles.AddRange(_ObjectPool.pooledAsteroidTwoObjects.FindAll(x => x.activeInHierarchy));
        pooledActiveObstacles.AddRange(_ObjectPool.pooledIceBlockObjects.FindAll(x => x.activeInHierarchy));
    }
}