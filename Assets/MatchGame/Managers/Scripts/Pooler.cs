using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchGame.Managers
{
    public class Pooler : MonoBehaviour
    {
        private Dictionary<GameObject, Queue<GameObject>> poolDict;

        private void Awake()
        {
            poolDict = new Dictionary<GameObject, Queue<GameObject>>();
        }

        public void Spawn(List<Poolable> poolObjects)
        {
            foreach (Poolable poolObject in poolObjects)
            {
                Queue<GameObject> pool = new Queue<GameObject>();
                for (int i = 0; i < poolObject.ObjectCount; i++)
                {
                    GameObject go = Instantiate(poolObject.gameObject, transform.position, Quaternion.identity, transform);
                    go.SetActive(false);
                    pool.Enqueue(go);
                }
                poolDict.Add(poolObject.gameObject, pool);
            }
        }

        public void Spawn(Poolable poolObject)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < poolObject.ObjectCount; i++)
            {
                GameObject go = Instantiate(poolObject.gameObject, transform.position, Quaternion.identity, transform);
                go.SetActive(false);
                pool.Enqueue(go);
            }
            poolDict.Add(poolObject.gameObject, pool);
        }

        public GameObject GetObjectFromPool(GameObject prefab, Vector3 pos)
        {
            if (poolDict.ContainsKey(prefab) == false)
            {
                throw new Exception(prefab + " is not key of dictionary");
            }
            var pool = poolDict[prefab];
            GameObject poolableObject = pool.Count > 0 && !pool.Peek().activeInHierarchy ? pool.Dequeue()
                                                                         : AddPoolObject(prefab);

            poolableObject.transform.position = pos;
            poolableObject.SetActive(true);
            pool.Enqueue(poolableObject);
            return poolableObject;
        }

        public Queue<GameObject> GetAllObjectsFromPool(GameObject prefab)
        {
            
        }

        private GameObject AddPoolObject(GameObject prefab)
        {
            GameObject go = Instantiate(prefab.gameObject, transform.position, Quaternion.identity, transform);
            go.SetActive(false);
            return go;
        }
    }
}
