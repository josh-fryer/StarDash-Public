using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool SharedInstance;
    public List<GameObject> pooledAsteroidObjects;
    public List<GameObject> pooledAsteroidTwoObjects;
    public List<GameObject> pooledIceBlockObjects;
    public List<GameObject> pooledLaserObjects;

    public GameObject Asteroid_objectToPool;
    public GameObject Asteroid2_objectToPool;
    public GameObject IceBlock_objectToPool;
    public GameObject Lasers_objectToPool;

    public int amountToPool_Asteroid;
    public int amountToPool_Asteroid2;
    public int amountToPool_IceBlock;
    public int amountToPool_lasers;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        pooledAsteroidObjects = new List<GameObject>();
        pooledAsteroidTwoObjects = new List<GameObject>();
        pooledIceBlockObjects = new List<GameObject>();
        pooledLaserObjects = new List<GameObject>();
        GameObject tmp;

        // ASTEROID 1
        for (int i = 0; i < amountToPool_Asteroid; i++)
        {
            tmp = Instantiate(Asteroid_objectToPool);
            tmp.SetActive(false);
            pooledAsteroidObjects.Add(tmp);
        }

        // ASTEROID 2
        for (int i = 0; i < amountToPool_Asteroid2; i++)
        {
            tmp = Instantiate(Asteroid2_objectToPool);
            tmp.SetActive(false);
            pooledAsteroidTwoObjects.Add(tmp);
        }

        // ICEBLOCK
        for (int i = 0; i < amountToPool_IceBlock; i++)
        {
            tmp = Instantiate(IceBlock_objectToPool);
            tmp.SetActive(false);
            pooledIceBlockObjects.Add(tmp);
        }

        // LASERS
        for (int i = 0; i < amountToPool_lasers; i++)
        {
            tmp = Instantiate(Lasers_objectToPool);
            tmp.SetActive(false);
            pooledLaserObjects.Add(tmp);
        }
    }

    public GameObject GetPooledAsteroidObject()
    {
        for (int i = 0; i < amountToPool_Asteroid; i++)
        {
            if (!pooledAsteroidObjects[i].activeInHierarchy)
            {
                return pooledAsteroidObjects[i];
            }
        }
        // ran out of objects in pool. add more
        amountToPool_Asteroid++;
        GameObject tmp;
        tmp = Instantiate(Asteroid_objectToPool);
        tmp.SetActive(false);
        pooledAsteroidObjects.Add(tmp);
        return tmp;
    }

    public GameObject GetPooledAsteroidTwoObject()
    {
        for (int i = 0; i < amountToPool_Asteroid2; i++)
        {
            if (!pooledAsteroidTwoObjects[i].activeInHierarchy)
            {
                return pooledAsteroidTwoObjects[i];
            }
        }
        // ran out of objects in pool. add more
        amountToPool_Asteroid2++;
        GameObject tmp;
        tmp = Instantiate(Asteroid2_objectToPool);
        tmp.SetActive(false);
        pooledAsteroidTwoObjects.Add(tmp);
        return tmp;
    }

    public GameObject GetPooledIceBlockObject()
    {
        for (int i = 0; i < amountToPool_IceBlock; i++)
        {
            if (!pooledIceBlockObjects[i].activeInHierarchy)
            {
                return pooledIceBlockObjects[i];
            }
        }
        // ran out of objects in pool. add more
        amountToPool_IceBlock++;
        GameObject tmp;
        tmp = Instantiate(IceBlock_objectToPool);
        tmp.SetActive(false);
        pooledIceBlockObjects.Add(tmp);
        return tmp;
    }

    public GameObject GetPooledLaserObject()
    {
        for (int i = 0; i < amountToPool_lasers; i++)
        {
            if (!pooledLaserObjects[i].activeInHierarchy)
            {
                return pooledLaserObjects[i];
            }
        }
        // ran out of objects in pool. add more
        amountToPool_lasers++;
        GameObject tmp;
        tmp = Instantiate(Lasers_objectToPool);
        tmp.SetActive(false);
        pooledLaserObjects.Add(tmp);
        return tmp;
    }
}
