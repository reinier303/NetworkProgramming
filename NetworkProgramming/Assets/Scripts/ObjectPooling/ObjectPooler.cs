using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkProgramming
{
    public class ObjectPooler : MonoBehaviour
    {
        //CREATE POOL IN RESOURCES FOLDER TO FUNCTION.
        //Assets/Resources/Pools/CreatePoolHere
        public static ObjectPooler Instance;

        private List<ScriptablePool> Pools = new List<ScriptablePool>();
        public Dictionary<string, Queue<GameObject>> PoolDictionary;
        public Transform PopUpParent;

        private void Awake()
        {
            Instance = this;

            Object[] ScriptablePools = Resources.LoadAll("Pools", typeof(ScriptablePool));
            foreach (ScriptablePool pool in ScriptablePools)
            {
                //Check if pool info is filled.
                if (pool.Amount > 0 && pool.Prefab != null && pool.Tag != null)
                {
                    Pools.Add(pool);
                }
                else
                {
                    Debug.LogWarning("Pool: " + pool.name + " is missing some information. \n Please go back to Resources/Pools and fill in the information correctly");
                }
            }
        }

        // Create pools and put them in empty gameObjects to make sure the hierarchy window is clean.
        private void Start()
        {
            PoolDictionary = new Dictionary<string, Queue<GameObject>>();

            if (Pools.Count < 1)
            {
                return;
            }

            GameObject PoolsContainerObject = new GameObject("Pools");

            foreach (ScriptablePool pool in Pools)
            {

                if (!PoolDictionary.ContainsKey(pool.Tag))
                {
                    GameObject containerObject = new GameObject(pool.Tag + "Pool");
                    containerObject.transform.parent = PoolsContainerObject.transform;

                    Queue<GameObject> objectPool = new Queue<GameObject>();

                    for (int i = 0; i < pool.Amount; i++)
                    {
                        GameObject obj = null;
                        //Set DamagePopUp Pool parent to PopUpParent to make it function
                        if (pool.Tag == "DamagePopUp" || pool.Tag == "CritPopUp")
                        {
                            Destroy(containerObject);
                            if (!PopUpParent)
                            {
                                Debug.LogWarning("PopUp Parent not set up yet, create canvas for popups to be in");
                                continue;
                            }
                            obj = Instantiate(pool.Prefab, PopUpParent);
                        }
                        else
                        {
                            obj = Instantiate(pool.Prefab, containerObject.transform);
                        }
                        if(obj == null)
                        {
                            return;
                        }
                        obj.SetActive(false);
                        objectPool.Enqueue(obj);
                    }
                    PoolDictionary.Add(pool.Tag, objectPool);
                }
            }
        }

        //Spawn an object from the corresponding pool with the given variables
        public GameObject SpawnFromPool(string tag, Vector2 position, Quaternion rotation)
        {
            if (!PoolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            GameObject objectToSpawn = PoolDictionary[tag].Dequeue();

            if (objectToSpawn == null)
            {
                print("object is null");
            }

            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            objectToSpawn.SetActive(false);
            objectToSpawn.SetActive(true);


            PoolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        private void ResetObject(GameObject objectToReset)
        {

        }

    }
}