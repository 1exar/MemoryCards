using UnityEngine;

namespace MemoryCards.Scripts.Configs
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "MemoryCards/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("JSON Settings")]
        [SerializeField] private string jsonUrl;

        [Header("Gameplay Timers (miliseconds)")]
        [SerializeField] private int initialShowDuration;
        [SerializeField] private int flipDelay;
        
        public string JsonUrl => jsonUrl;
        public int ShowDuration => initialShowDuration;
        public int FlipDelay => flipDelay;
    }
}