using MatchGame.GamePlay.Category;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MatchGame.Managers;

namespace MatchGame.GamePlay.Player
{
    public class PlayerVisualChanger : MonoBehaviour, IPausable
    {
        [Inject] private CategoryData categoryData;

        [Serializable]
        public struct VisualSize
        {
            public CategoryType type;
            public float scale;
        }

        [SerializeField] private List<VisualSize> sizes;
        [SerializeField] private float rotationSpeed;

        public bool IsPaused { get; set; }     
        private Dictionary<CategoryType, List<GameObject>> visuals;
        private GameObject currentTypeObj;

        private void Awake()
        {
            SetupVisuals();
            transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            if (IsPaused==false&&currentTypeObj != null)
            {
                transform.Rotate(
                    Vector3.up * Time.deltaTime * rotationSpeed+
                    Vector3.right * Time.deltaTime * rotationSpeed+
                    Vector3.forward * Time.deltaTime * rotationSpeed,
                    Space.Self);
            }
        }

        private void SetupVisuals()
        {
            visuals = new Dictionary<CategoryType, List<GameObject>>();
            foreach (var playerCategory in categoryData.PlayerCategories)
            {
                List<GameObject> categoryList = new List<GameObject>();
                foreach (var item in playerCategory.visuals)
                {
                    GameObject categoryObject = Instantiate(item, transform, false);
                    categoryObject.SetActive(false);
                    float size = sizes.Where(x => x.type == playerCategory.type).Select(x => x.scale).FirstOrDefault();
                    categoryObject.transform.localScale *= size == 0 ? 1 : size;
                    categoryList.Add(categoryObject);
                }
                visuals.Add(playerCategory.type, categoryList);
            }
        }

        public void ChangeVisual(CategoryType type)
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

        public void Pause(bool isPaused)
        {
            IsPaused = isPaused;
        }

        public void Refresh()
        {
            transform.localScale = Vector3.zero;
        }

        public async UniTask<bool> SetPreparation()
        {
            await UniTask.WhenAny(transform.DOScale(Vector3.one, 0.1f).AsyncWaitForCompletion().AsUniTask().ContinueWith(() =>
            transform.DOPunchScale(Vector3.one * 0.4f, 0.3f).AsyncWaitForCompletion()).AsUniTask());
            return true;
        }
    }
}

