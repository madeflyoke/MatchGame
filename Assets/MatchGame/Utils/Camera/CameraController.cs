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
        private CinemachineVirtualCamera cam;

        private void Awake()
        {
            cam = GetComponent<CinemachineVirtualCamera>();
            cam.m_Lens.FarClipPlane = 1;          
        }

        private void Start()
        {
            gameManager.refreshEvent += () => cam.m_Lens.FarClipPlane = 1;
        }

        public async UniTask<bool> SetPreparation()
        {
            for (float i = 1; cam.m_Lens.FarClipPlane < fullCameraClip; i += 0.001f)
            {
                cam.m_Lens.FarClipPlane += i;
                await UniTask.DelayFrame(2);
            }
            return true;
        }
    }
}

