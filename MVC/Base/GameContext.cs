using MVC.Model;
using UnityEngine;

namespace MVC.Base
{
    /// <summary>
    /// Global data entry point, attached to the unique GameContext GameObject in the scene.
    /// All Model references are centralized here.
    ///
    /// Usage: GameContext.Instance.Character.CurrentHp
    /// </summary>
    public class GameContext : MonoBehaviour
    {
        // ── Singleton ──────────────────────────────────────
        public static GameContext Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── Model References ───────────────────────────────
        // Add a property here for every new type of Model added.

        public CharacterModel Character { get; private set; }

        // Future expansion examples (commented out):
        // public InventoryModel Inventory { get; private set; }
        // public QuestModel Quest { get; private set; }

        // ── Register methods for Controllers ──────────
        public void RegisterCharacter(CharacterModel model)
        {
            Character = model;
            Debug.Log($"[GameContext] CharacterModel registered: {model.CharacterName}");
        }

        public void UnregisterCharacter()
        {
            Character = null;
        }
    }
}
