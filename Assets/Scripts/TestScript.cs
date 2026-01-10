using System;
using UnityEngine;
using System.Collections.Generic;

namespace VahTyah
{
    public enum TestEnum
    {
        OptionA,
        OptionB,
        OptionC
    }
    [Serializable]
    public class Dog
    {
        public string name;
    }
    public class TestScript : MonoBehaviour
    {
        [BoxGroup("Player", "⚔️ Player Settings", 0)]
        public string playerName = "Hero";

        [SerializeField, BoxGroup("Player")]
        private int health = 100;

        [BoxGroup("Player")]
        public float speed = 5f;

        [BoxGroup("Enemy", "👾 Enemy Settings", 1)]
        public int enemyCount = 10;

        [BoxGroup("Enemy")]
        public GameObject enemyPrefab;

        [BoxGroup("Audio", "🔊 Audio Settings", 2)]
        public bool enableSound = true;

        [BoxGroup("Audio")]
        [Range(0, 1)]
        public float volume = 0.8f;
        
        [BoxGroup("Audio")]
        public List<string> soundEffects;

        // Ungrouped properties
        public string ungroupedField = "I'm free!";
        public int anotherField = 42;
        
        [Button()]
        private void ButtonInPlayerGroup(List<int> a, GameObject key, TestEnum option, Dog dog)
        {
            Debug.Log("Button in Player Group clicked!");
        }
        
        [Button]
        private void ButtonOutsideGroup()
        {
            Debug.Log("Button outside group clicked!");
        }
    }
}