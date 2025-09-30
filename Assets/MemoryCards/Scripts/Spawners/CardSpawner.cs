using System;
using System.Collections.Generic;
using MemoryCards.Scripts.Controllers;
using UnityEngine;
using Zenject;

namespace MemoryCards.Scripts.Spawners
{
    public class CardSpawner
    {
        private readonly CardController _cardPrefab;
        private readonly Transform _gridRoot;
        private readonly DiContainer _container;

        public CardSpawner(CardController cardPrefab, Transform gridRoot, DiContainer container)
        {
            _cardPrefab = cardPrefab;
            _gridRoot = gridRoot;
            _container = container;
        }

        public List<CardController> SpawnCards(List<(int id, Sprite sprite)> deck, Action<CardController> onClick)
        {
            var result = new List<CardController>();

            foreach (var (id, sprite) in deck)
            {
                var card = _container.InstantiatePrefabForComponent<CardController>(_cardPrefab, _gridRoot);
                card.Init(id, sprite, onClick);
                result.Add(card);
            }

            return result;
        }
    }
}