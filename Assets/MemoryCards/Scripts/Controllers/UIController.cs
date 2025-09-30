using TMPro;
using UnityEngine;

namespace MemoryCards.Scripts.Controllers
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text pairsCounter;

        [SerializeField] private GameObject loading;

        public void TurnOffLoading()
        {
            loading.SetActive(false);
        }
        
        public void UpdateCounter(int value)
        {
            pairsCounter.text = "Pairs: " + value;
        }
    }
}