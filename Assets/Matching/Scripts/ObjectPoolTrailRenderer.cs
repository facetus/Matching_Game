using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This is the main class that handles the Object Pool Design pattern in our game;
 * The main idea is that we don't want many gameobjects being instantiated during run time
 * as it will be too expensive to our CPU;
 * Instead, we want to instantiate them before the script actually needs them as inactive,
 * so once we need them we just activate them instead of re-instantiated them;
 * After that we just deactivate them again until we need them again;
 * 
 * How to create new Object Pool:
 * Create a new ObjectPool*NameOfPrefab* script and attach it to a new empty gameobject;
 * Fill up the serialized fields with the prefab that you want to spawn, the pool size and the parent object(important if its in UI to be inside the canvas);
 * 
 * Use the GameObject *prefabName* = ObjectPool*prefabName.SharedInstance.GetPooledObject(); method to get the prefab gameobject from the other script;
 * set up its position if needed (if its in the UI)
 * 
 * When you want to destroy the object, deactivate it by using the ObjectPool*prefabName*.SharedInstance.ReturnPooledObject(*prefabName*); method;
 * 
 * For actual use of this script see the MatchingGame_TextScript.cs script;
 **/

public class ObjectPoolTrailRenderer : MonoBehaviour
{
    //We need to have only 1 active Instance at the time, this follows the Singleton pattern design;
    public static ObjectPoolTrailRenderer SharedInstance;

    //The reason we a QUEUE instead of a List is because the time complexity is much better with the queue as its FirstIn-FirstOut;
    //Using a list we would iterate through the list each time we want to add/remove an element (which is O(n) time complexity), meanwhile
    //with the queue is only O(1) as it jsut activates/deactivates the first;
    public Queue<GameObject> pooledObjects;

    //The prefab we want to instantiate;
    public GameObject objectToPool;

    //How many pooledObjects we want to create (usually keep this somewhere between 20-50, depending on the game);
    public int poolSize;

    //Where we want to spawn our pooledObjects;
    public GameObject parentOfPooledObjects;

    private void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        //We create a new empty Queue;
        pooledObjects = new Queue<GameObject>();

        //This tmp value will be the prefab to populate our Queue
        GameObject tmp;

        for(int i=0; i<poolSize; i++)
        {
            //We populate the Queue with our prefab;
            tmp = Instantiate(objectToPool, parentOfPooledObjects.transform);

            //Important to set it as inactive;
            tmp.SetActive(false);

            //Enqueue adds an element to the Queue;
            pooledObjects.Enqueue(tmp);

        }
    }

    //This is the function we call from other scripts to get our pooledObject;
    // We simply get the first gameobject that is inactive from our Queue and activate it;
    public GameObject GetPooledObject()
    {
        if(pooledObjects.Count > 0)
        {
            //Removes an item from the Queue
            GameObject obj = pooledObjects.Dequeue();

            if (obj != null)
            {

                //This is the gameobject that we activate in our scene;
                obj.SetActive(true);
                return obj;
            }
        }
        return null;
    }

    public void ReturnPooledObject(GameObject obj)
    {
        obj.SetActive(false);

        //Endqueue adds the item to the end of our queue;
        pooledObjects.Enqueue(obj);
    }

}
