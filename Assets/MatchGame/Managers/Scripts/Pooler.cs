using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MatchGame.Managers
{
    public class Pooler : MonoBehaviour
    {
        [Inject] private DiContainer container;

        public event Action<GameObject> capacityChangedEvent;

        [SerializeField] private List<Poolable> poolObjects;

        public Dictionary<GameObject, Queue<GameObject>> PoolDict { get => poolDict; }
        private Dictionary<GameObject, Queue<GameObject>> poolDict;

        private void Awake()
        {
            poolDict = new Dictionary<GameObject, Queue<GameObject>>();
            Spawn(poolObjects);
        }

        private void Spawn(List<Poolable> poolObjects)
        {
            foreach (Poolable poolObject in poolObjects)
            {
                Queue<GameObject> pool = new Queue<GameObject>();
                for (int i = 0; i < poolObject.ObjectCount; i++)
                {
                    GameObject go = container.InstantiatePrefab(poolObject.gameObject, transform.position, Quaternion.identity,null);
                    go.SetActive(false);
                    pool.Enqueue(go);
                }
                poolDict.Add(poolObject.gameObject, pool);
            }
        }

        //private void Spawn(Poolable poolObject)
        //{
        //    Queue<GameObject> pool = new Queue<GameObject>();
        //    for (int i = 0; i < poolObject.ObjectCount; i++)
        //    {
        //        GameObject go = Instantiate(poolObject.gameObject, transform.position, Quaternion.identity, transform);
        //        go.SetActive(false);
        //        pool.Enqueue(go);
        //    }
        //    poolDict.Add(poolObject.gameObject, pool);
        //}

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
            //poolableObject.SetActive(true);
            pool.Enqueue(poolableObject);
            return poolableObject;
        }

        private GameObject AddPoolObject(GameObject prefab)
        {
            GameObject go = container.InstantiatePrefab(prefab.gameObject, transform.position, Quaternion.identity, null);
            go.SetActive(false);
            capacityChangedEvent?.Invoke(prefab);
            return go;
        }
    }
}
