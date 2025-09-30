using UnityEngine;
using Zenject;
using MemoryCards.Scripts.Controllers;
using MemoryCards.Scripts.Services;
using MemoryCards.Scripts.Spawners;

namespace MemoryCards.Scripts.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Prefabs & References")]
        public CardController cardPrefab;
        public Transform gridRoot;
        public GameController gameController;
        public UIController uiController;

        public override void InstallBindings()
        {
            Container.Bind<ImageService>().AsSingle();

            Container.Bind<CardSpawner>().AsSingle()
                .WithArguments(cardPrefab, gridRoot, Container);

            Container.Bind<GameController>().FromInstance(gameController).AsSingle();
            
            Container.Bind<UIController>().FromInstance(uiController).AsSingle();
        }
    }
}