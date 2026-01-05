using UnityEngine;
using System;
using TMPro;
using System.Collections;

public class Asteroid : MonoBehaviour
{
    [SerializeField] bool isBreakable;
    public int health = 0;
    private int healthAtStart;

    [Header("Ice Block Ore Settings")]
    [SerializeField] bool hasOre;
    public GameObject iceBlockExplosionObj;
    private Ore _ore;
    public Material rare;
    public Material uncommon;

    private GameMaster gameMaster;
    [HideInInspector]
    public float speed;
    public float fadeOutPointsSpeed = 0.4f;
    private GameObject model;
    private GameObject modelContainer;
    private GameObject textGameObject;
    private float rotateSpeed = -60f;
    private bool hasAddedPoints;
    private bool isBeingDestroyed = false;
    private Vector3 camPos;
    private TextMeshPro textMeshProComponent;

    [Header("Audio")]
    private AudioSource audioSource;

    [Range(0f, 1f)]
    [SerializeField] float explosionVol = 1f;
    [Range(0f, 2f)]
    [SerializeField] float minPitch;
    [Range(0f, 2f)]
    [SerializeField] float maxPitch;

    void Start()
    {
        gameMaster = GameMaster.Instance;
        healthAtStart = health;
        speed = gameMaster.globalObstacleSpeed;
        gameMaster.obstaclesInPlay.Add(gameObject);
        camPos = gameMaster.camPos;
     
        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>(true);
        if (transforms.Length > 1)
        {
            foreach (Transform t in transforms)
            {
                //Debug.Log("Loop: " + t.gameObject.name);
                if (t.gameObject.name == "Model")
                {
                    model = t.gameObject;
                    float randomRotate = UnityEngine.Random.Range(0f, 180f);
                    model.transform.eulerAngles = new Vector3(randomRotate, randomRotate, randomRotate);
                    //Debug.Log("set rotation values for " + model.name + model.transform.eulerAngles);
                }
                else if (t.gameObject.name == "Model Container")
                {
                    modelContainer = t.gameObject;
                }
                else if (t.gameObject.tag == "Text")
                {
                    textGameObject = t.gameObject;
                    textMeshProComponent = textGameObject.GetComponent<TextMeshPro>();
                }
            }
        }

        if (hasOre)
        {
            _ore = new Ore();
            SetOreRarity();
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = explosionVol;
        }

        if (isBreakable)
        {
            // setIsbreakable in Collision layer
            model.GetComponent<AsteroidCollisions>().isBreakable = isBreakable;
        }
    }

    private void SetOreRarity()
    {
        int random = UnityEngine.Random.Range(0, 101);
        if (random <= 10)
        {
            _ore.type = 3;
            model.GetComponent<Renderer>().material = rare;
        }
        else if(random <= 40)
        {
            _ore.type = 2;
            model.GetComponent<Renderer>().material = uncommon;
        }
        else
        {
            _ore.type = 1;
            // leave material at default.
        }
        textMeshProComponent.text = "+" + _ore.points.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameMaster.isAlive)
        {
            // move
            transform.Translate(0, 0, -1 * speed * Time.deltaTime);
            Rotate();
        }

        if (isBreakable && health <= 0)
        {
            // update points
            if (hasOre && !hasAddedPoints)
            {
                gameMaster.AddPoints(_ore.points);
                hasAddedPoints = true;
            }
            DestroyIceblock();
        }
    }

    private void DestroyIceblock()
    {
        if(!isBeingDestroyed)
        {
            isBeingDestroyed = true;
            audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            //Debug.Log("Pitch is "+ audioSource.pitch);
            audioSource.PlayOneShot(audioSource.clip);
            
            // reveal points and hide model object
            modelContainer.SetActive(false);
            textGameObject.SetActive(true);
            speed = 0.20f * speed;
            StartCoroutine(FadePointsText());
            if (hasOre && iceBlockExplosionObj != null)
            {
                GameObject newObj = Instantiate(iceBlockExplosionObj, gameObject.transform.position, Quaternion.identity) as GameObject;
                var explosion = newObj.GetComponent<IceBlockExplosion>();
                explosion.SetParticleMaterials(_ore.type);
            }

            StartCoroutine(DestroyedIceBlockReset());
        } 
    }

    IEnumerator DestroyedIceBlockReset()
    {
        // reset to be ready to instantiated/active again
        yield return new WaitForSeconds(0.7f);
        gameObject.SetActive(false);
        modelContainer.SetActive(true);
        textGameObject.SetActive(false);
        textMeshProComponent.alpha = 1;
        speed = gameMaster.globalObstacleSpeed;
        isBeingDestroyed = false;
        health = healthAtStart;
        hasAddedPoints = false;
        gameMaster.obstaclesInPlay.Remove(gameObject);
    }

    IEnumerator FadePointsText()
    {
        while (textMeshProComponent.color.a > 0.0f)
        {
            textMeshProComponent.alpha = textMeshProComponent.alpha - (Time.deltaTime * fadeOutPointsSpeed);
            yield return null;
        }
    }


    private void OnEnable()
    {
        
        if (gameMaster != null)
        {
            speed = gameMaster.globalObstacleSpeed;
        } 
    }

    private void Rotate()
    {
        //var currRotation = transform.rotation;
        modelContainer.transform.Rotate(rotateSpeed * Time.deltaTime, rotateSpeed * Time.deltaTime, 0f);
    }
}
