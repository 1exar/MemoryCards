using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using MemoryCards.Scripts.Configs;
using MemoryCards.Scripts.Services;
using MemoryCards.Scripts.Spawners;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MemoryCards.Scripts.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GridLayoutGroup gridLayoutGroup;
        
        private List<CardController> _cards = new List<CardController>();
        private CardController _firstCard, _secondCard;
        private int _pairsCollected;
        
        private UIController _ui;
        private ImageService _imageService;
        private CardSpawner _spawner;
        private GameConfig _config;
        
        private bool _isCheckingPair;
        
        [Inject]
        public void Construct(UIController ui, CardSpawner spawner, ImageService imageService, GameConfig config)
        {
            _ui = ui;
            _spawner = spawner;
            _imageService = imageService;
            _config = config;
        }

        private async Task StartGame()
        {
            _isCheckingPair = true;
            _pairsCollected = 0;
            _ui.UpdateCounter(_pairsCollected);
            
            _imageService.OnSpritesLoaded += HandleSpritesDownload;

            var sprites = await _imageService.LoadSpritesAsync();
            
            var deck = new List<(int, Sprite)>();
            
            for (int i = 0; i < sprites.Count; i++)
            {
                deck.Add((i, sprites[i]));
                deck.Add((i, sprites[i]));
            }

            Shuffle(deck);

            _cards = _spawner.SpawnCards(deck, OnCardClicked);
            
            await Task.Delay(_config.ShowDuration);
            
            if(gridLayoutGroup)
                gridLayoutGroup.enabled = false;
            
            foreach (var card in _cards) card.Flip(false);
            
            await Task.Delay(100);
            
            _isCheckingPair = false;
            
        }

        private async void Start()
        {
            await StartGame();
        }

        private void HandleSpritesDownload()
        {
            _ui.TurnOffLoading();
        }
        
        private async void OnCardClicked(CardController card)
        {
            if (_isCheckingPair) return;

            card.Flip(true);

            if (_firstCard == null)
            {
                _firstCard = card;
                return;
            }

            if (_secondCard == null && card != _firstCard)
            {
                _secondCard = card;
                _isCheckingPair = true;

                if (_firstCard.Id == _secondCard.Id)
                {
                    await Task.Delay(_config.FlipDelay);
                    _firstCard.HideCard();
                    _secondCard.HideCard();

                    _pairsCollected++;
                    _ui.UpdateCounter(_pairsCollected);

                    if (_pairsCollected == _cards.Count / 2)
                    {
                        await Task.Delay(2000);
                        RestartGame();

                        return;
                    }
                }
                else
                {
                    await Task.Delay(_config.FlipDelay);
                    _firstCard.Flip(false);
                    _secondCard.Flip(false);
                }

                _firstCard = null;
                _secondCard = null;
                _isCheckingPair = false;
            }
        }

        private async void RestartGame()
        {
            Debug.Log("Restart");
            
            _firstCard = null;
            _secondCard = null;
            
            _isCheckingPair = true;
            
            if(gridLayoutGroup)
                gridLayoutGroup.enabled = true;
            
            _pairsCollected = 0;
            _ui.UpdateCounter(_pairsCollected);

            foreach (var card in _cards)
            {
                card.gameObject.SetActive(true);
            }
            
            Shuffle(_cards);
            
            Transform parent = _cards.Count > 0 ? _cards[0].transform.parent : null;
            
            if (parent != null)
            {
                for (int i = 0; i < _cards.Count; i++)
                {
                    _cards[i].transform.SetSiblingIndex(i);
                }
            }

            foreach (var card in _cards)
            {
                card.Flip(true);
            }

            await Task.Delay(_config.ShowDuration);

            if(gridLayoutGroup)
                gridLayoutGroup.enabled = false;
            
            foreach (var card in _cards)
            {
                card.Flip(false);
            }
            
            await Task.Delay(100);
            
            _isCheckingPair = false;
        }

        private void Shuffle<T>(List<T> list)
        {
            var rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        private void OnDestroy()
        {
            _imageService.OnSpritesLoaded -= HandleSpritesDownload;
            DOTween.KillAll();
        }
    }
    
}