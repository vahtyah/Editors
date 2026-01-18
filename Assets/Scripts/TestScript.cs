using System;
using UnityEngine;
using System.Collections.Generic;
using VahTyah.Inspector;

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
        [OnValueChanged("OnPlayerNameChanged")]
        public string playerName = "Hero";

        [SerializeField, BoxGroup("Player")]
        [OnValueChanged("OnHealthChanged")]
        private int health = 100;

        [BoxGroup("Player")]
        [OnValueChanged("OnSpeedChanged")]
        public float speed = 5f;

        [BoxGroup("Enemy", "👾 Enemy Settings", 1)]
        [OnValueChanged("OnHealthChanged")]
        public int enemyCount = 10;

        [BoxGroup("Enemy")]
        [AutoRef(RefSource.Parent)]
        public GameObject enemyPrefab;

        [BoxGroup("Audio", "🔊 Audio Settings", 2)]
        public bool enableSound = true;

        [BoxGroup("Audio")]
        [Range(0, 1)]
        public float volume = 0.8f;
        
        [BoxGroup("Audio")]
        public List<string> soundEffects;

        // === AutoRef Examples ===
        
        // Auto-get from same GameObject (default)
        [AutoRef]
        public Transform selfTransform;
        
        // Auto-get Rigidbody from self
        [AutoRef(RefSource.Self)]
        public Rigidbody rb;
        
        // Auto-find in children
        [AutoRef(RefSource.Children)]
        public Collider childCollider;
        
        // Auto-find in children, including inactive
        [AutoRef(RefSource.Children, includeInactive: true)]
        public MeshRenderer childRenderer;
        
        // Auto-find in parent hierarchy
        [AutoRef(RefSource.Parent)]
        public Canvas parentCanvas;
        
        // AutoRef inside a group
        [BoxGroup("Refs", "🔗 References", 3)]
        [AutoRef(RefSource.Self)]
        public Animator animator;
        
        [BoxGroup("Refs")]
        [AutoRef(RefSource.Children)]
        public AudioSource audioSource;

        // === AssetRef Examples ===
        
        // Find first Material in project
        [AssetRef]
        public Material anyMaterial;
        
        // Find Material by name
        [AssetRef("DefaultMaterial")]
        public Material namedMaterial;
        
        // Find ScriptableObject in specific folder
        // [AssetRef("GameConfig", "Assets/Data")]
        // public GameConfig config;

        // === Multiple Attributes Examples ===
        
        // AutoRef + OnValueChanged: Shows both [🔄] and [▶] buttons
        [AutoRef(RefSource.Self)]
        [OnValueChanged("OnRigidbodyChanged")]
        [Required("Tuan")]
        public Rigidbody trackedRb;
        
        // AutoRef + OnValueChanged inside group
        [BoxGroup("Refs")]
        [AutoRef(RefSource.Children)]
        [OnValueChanged("OnAudioSourceChanged")]
        public AudioSource trackedAudio;
        
        // AssetRef + OnValueChanged
        [AssetRef]
        [OnValueChanged("OnMaterialChanged")]
        public Material trackedMaterial;

        // === Required Examples ===
        
        // Required reference - shows warning icon if null
        [Required]
        public Transform requiredTransform;
        
        // Required with custom message
        [Required("Player needs a spawn point!")]
        public Transform spawnPoint;
        
        // Required as error (red icon)
        [Required("Critical: AudioClip is missing!", isError: true)]
        public AudioClip criticalAudio;
        
        // Required string - warns if empty
        [Required]
        public string requiredName;
        
        // Required + AutoRef: Shows warning + refresh button
        [Required]
        [AutoRef(RefSource.Children)]
        public Collider requiredCollider;
        
        // Required inside group
        [BoxGroup("Refs")]
        [Required("Animator is required for animations")]
        public Animator requiredAnimator;

        // === ReadOnly Examples ===
        
        // ReadOnly field - locked by default, click to unlock
        [ReadOnly]
        public int lockedValue = 999;
        
        // ReadOnly string
        [ReadOnly]
        public string lockedName = "Cannot edit by default";
        
        // ReadOnly + Required: Lock icon + warning icon
        [ReadOnly]
        [Required]
        public Transform lockedRequiredRef;
        
        // ReadOnly inside group
        [BoxGroup("Player")]
        [ReadOnly]
        public float maxHealth = 100f;

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

        // === OnValueChanged Callbacks ===
        
        private void OnPlayerNameChanged()
        {
            Debug.Log($"Player name changed to: {playerName}");
        }

        private void OnHealthChanged(int newHealth)
        {
            Debug.Log($"Health changed to: {newHealth}");
        }

        private void OnSpeedChanged()
        {
            Debug.Log($"Speed changed to: {speed}");
        }

        private void OnRigidbodyChanged(Rigidbody rb)
        {
            Debug.Log($"Rigidbody changed to: {(rb != null ? rb.name : "null")}");
        }

        private void OnAudioSourceChanged()
        {
            Debug.Log($"AudioSource changed to: {(trackedAudio != null ? trackedAudio.name : "null")}");
        }

        private void OnMaterialChanged(Material mat)
        {
            Debug.Log($"Material changed to: {(mat != null ? mat.name : "null")}");
        }
    }
}