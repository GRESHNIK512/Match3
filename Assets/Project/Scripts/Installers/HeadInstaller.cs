using UnityEngine;
using Zenject;

public class HeadInstaller : MonoInstaller
{
    [SerializeField] GameSettings _gameSettings;
  
    public override void InstallBindings()
    {
        Container.Bind<GameSettings>().FromInstance(_gameSettings).AsSingle().NonLazy();

    }
}