using MatchGame.Utils;
using UnityEngine;
using Zenject;

public class CameraControllerInstaller : MonoInstaller
{
    [SerializeField] private CameraController cam; 
    public override void InstallBindings()
    {
        Container.BindInstance(cam).AsSingle().NonLazy();
    }
}