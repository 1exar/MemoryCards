using TMPro;
using UnityEngine;

namespace MemoryCards.Scripts.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text pairsCounter;

        public void UpdateCounter(int value)
        {
            pairsCounter.text = "Pairs: " + value;
        }
    }
}