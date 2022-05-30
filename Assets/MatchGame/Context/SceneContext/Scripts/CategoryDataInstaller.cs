using MatchGame.GamePlay.Category;
using UnityEngine;
using Zenject;

public class CategoryDataInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInstance(new CategoryData()).AsSingle().NonLazy();
    }
}