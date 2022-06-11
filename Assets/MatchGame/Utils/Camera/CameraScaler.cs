using Cinemachine;
using UnityEngine;

namespace MatchGame.Utils
{
    public class CameraScaler : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cam;
        [SerializeField] private float horizontalFoV;

        private void Awake()
        {
            float halfWidth = Mathf.Tan(0.5f * horizontalFoV * Mathf.Deg2Rad);

            float halfHeight = halfWidth * Screen.height / Screen.width;

            float verticalFoV = 2.0f * Mathf.Atan(halfHeight) * Mathf.Rad2Deg;

            cam.m_Lens.FieldOfView = verticalFoV;
        }
    }
}
