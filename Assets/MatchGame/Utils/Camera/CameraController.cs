using Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Zenject;
using MatchGame.Managers;

namespace MatchGame.Utils
{
    public class CameraController : MonoBehaviour
    {
        [Inject] private GameManager gameManager; 

        [SerializeField] private int fullCameraClip;
        public CinemachineVirtualCamera Cam { get; private set; }

        private void Awake()
        {
            Cam = GetComponent<CinemachineVirtualCamera>();
            Cam.m_Lens.FarClipPlane = 1;    
        }

        private void Start()
        {
            gameManager.refreshEvent += () => Cam.m_Lens.FarClipPlane = 1;
        }

        public async UniTask<bool> SetPreparation()
        {
            for (float i = 1; Cam.m_Lens.FarClipPlane < fullCameraClip; i += Time.deltaTime)
            {
                Cam.m_Lens.FarClipPlane += i;
                await UniTask.Yield();
            }
            return true;
        }
    }
}

