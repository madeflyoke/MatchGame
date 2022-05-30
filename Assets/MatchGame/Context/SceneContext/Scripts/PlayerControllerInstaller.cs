using MatchGame.GamePlay.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerControllerInstaller : MonoInstaller
{
    [SerializeField] private PlayerController playerController;
    public override void InstallBindings()
    {
        Container.BindInstance(playerController).AsSingle().NonLazy();
    }
}
