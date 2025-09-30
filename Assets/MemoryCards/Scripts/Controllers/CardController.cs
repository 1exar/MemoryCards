using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace MemoryCards.Scripts.Controllers
{
    public class CardController : MonoBehaviour
    {
        [SerializeField] private Image frontImage;

        private int _id;
        private Action<CardController> _onClick;

        public void Init(int id, Sprite sprite, Action<CardController> onClick)
        {
            _id = id;
            frontImage.sprite = sprite;
            _onClick = onClick;
        }

        public int Id => _id;

        public void OnCardClick()
        {
            _onClick?.Invoke(this);
        }

        public void Flip(bool faceUp)
        {
            Sequence flipSequence = DOTween.Sequence();

            flipSequence.Append(transform.DOScaleX(0f, 0.15f).SetEase(Ease.InQuad));
    
            flipSequence.AppendCallback(() => frontImage.gameObject.SetActive(faceUp));

            flipSequence.Append(transform.DOScaleX(1.05f, 0.12f).SetEase(Ease.OutQuad));
            flipSequence.Append(transform.DOScaleX(1f, 0.08f).SetEase(Ease.OutQuad));
        }

        public void HideCard()
        {
            gameObject.SetActive(false);
        }
    }
}