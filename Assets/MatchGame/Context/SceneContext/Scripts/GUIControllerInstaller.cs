using MatchGame.GUI;
using UnityEngine;
using Zenject;

public class GUIControllerInstaller : MonoInstaller
{
    [SerializeField] private GUIController gUIController;

    public override void InstallBindings()
    {
        Container.BindInstance(gUIController).AsSingle().NonLazy();
    }
}