using MatchGame.GamePlay.Category;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace MatchGame.GamePlay.Player
{
    public class PlayerVisualChanger : MonoBehaviour
    {
        [Inject] private PlayerController playerController;
        [Inject] private CategoryData categoryData;

        [Serializable]
        public struct VisualSize
        {
            public CategoryType type;
            public float scale;
        }

        [SerializeField] private List<VisualSize> sizes;
        [SerializeField] private float rotationSpeed;
        private Dictionary<CategoryType, List<GameObject>> visuals;
        private GameObject currentTypeObj;
        private CancellationTokenSource cancellationTokenSource;

        private void Update()
        {
            if (currentTypeObj != null)
            {
                transform.Rotate(
                    Vector3.up * Time.deltaTime * rotationSpeed+
                    Vector3.right * Time.deltaTime * rotationSpeed+
                    Vector3.forward * Time.deltaTime * rotationSpeed,
                    Space.Self);
            }
        }

        //private async void Rotate()
        //{
        //    float prevTime = Time.time;
        //    while (prevTime+)
        //    {
        //        transform.rotation = Quaternion.Slerp(transform.rotation, transform.rotation *
        //            Quaternion.Euler(new Vector3(UnityEngine.Random.Range(-180f, 180f),
        //            UnityEngine.Random.Range(-180f, 180f),
        //            UnityEngine.Random.Range(-180f, 180f))),
        //            Time.deltaTime * rotationSpeed);
        //        await UniTask.Yield(cancellationTokenSource.Token);
        //    }
        //}

        private void Awake()
        {
            cancellationTokenSource = new CancellationTokenSource();
            visuals = new Dictionary<CategoryType, List<GameObject>>();
            foreach (var playerCategory in categoryData.PlayerCategories)
            {
                List<GameObject> categoryList=new List<GameObject>();
                foreach (var item in playerCategory.visuals)
                {
                    GameObject categoryObject = Instantiate(item, transform, false);
                    categoryObject.SetActive(false);
                    float size = sizes.Where(x => x.type == playerCategory.type).Select(x=>x.scale).FirstOrDefault();
                    categoryObject.transform.localScale *= size == 0 ? 1 : size;
                    categoryList.Add(categoryObject);
                }
                visuals.Add(playerCategory.type,categoryList);
            }
        }

        private void OnEnable()
        {
            playerController.playerCategoryChangedEvent += ChangeVisual;
        }
        private void OnDisable()
        {
            playerController.playerCategoryChangedEvent += ChangeVisual;
        }

        private void ChangeVisual(CategoryType type)
        {
            if (currentTypeObj!=null) currentTypeObj.SetActive(false);
            GameObject prevObj = currentTypeObj;
            currentTypeObj = visuals[type][UnityEngine.Random.Range(0, visuals[type].Count)];
            if (prevObj==currentTypeObj)
            {
                Debug.Log("Identical Visuals");
                ChangeVisual(type);
                return;
            }
            currentTypeObj.transform.position = transform.position;
            currentTypeObj.SetActive(true);
        }
    }
}

