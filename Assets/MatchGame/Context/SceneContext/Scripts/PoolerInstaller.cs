using MatchGame.Managers;
using UnityEngine;
using Zenject;

public class PoolerInstaller : MonoInstaller
{
    [SerializeField] private Pooler pooler;
    public override void InstallBindings()
    {
        Container.BindInstance(pooler).AsSingle().NonLazy();
    }
}
