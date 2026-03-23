using MVC.Base;
using MVC.Model;
using MVC.View;
using UnityEngine;

namespace MVC.Controller
{
    /// <summary>
    /// Attached to the character GameObject, responsible for:
    ///   1. Assembling Model + View
    ///   2. Handling input / external calls
    ///   3. Converting "intent" into Model operations
    /// </summary>
    public class CharacterController : MonoBehaviour
    {
        [Header("Initial Character Settings")]
        [SerializeField] private CharacterData characterData;

        // ── Internal References ──────────────────────────────────────
        private CharacterModel _model;
        private CharacterView _view;
        private Rigidbody _rb;

        // ── Unity Lifecycle ────────────────────────────────
        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();

            if (characterData == null)
                characterData = new CharacterData();

            _model = new CharacterModel(characterData);

            if (GameContext.Instance != null)
                GameContext.Instance.RegisterCharacter(_model);
            else
                Debug.LogWarning("[CharacterController] GameContext not initialized. Make sure GameContext GameObject exists in the scene.");

            _view = GetComponent<CharacterView>();
            if (_view != null)
                _view.Initialize(_model);
            else
                Debug.LogWarning("[CharacterController] CharacterView not found. UI will not be displayed.");
        }

        private void Update()
        {
            HandleMovementInput();
            HandleDebugInput();
        }

        // ── Movement Input ──────────────────────────────────────
        private void HandleMovementInput()
        {
            if (_model.IsDead) return;

            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 dir = new Vector3(h, 0f, v).normalized;

            if (_rb != null)
            {
                _rb.MovePosition(
                    _rb.position + dir * _model.MoveSpeed * Time.deltaTime
                );
            }
            else
            {
                transform.Translate(dir * _model.MoveSpeed * Time.deltaTime, Space.World);
            }

            if (dir != Vector3.zero)
                transform.forward = dir;
        }

        /// <summary>Testing: Keyboard Shortcuts</summary>
        private void HandleDebugInput()
        {
            // T: Take 10 Damage
            if (Input.GetKeyDown(KeyCode.T))
                TakeDamage(10);

            // H: Heal 20
            if (Input.GetKeyDown(KeyCode.H))
                Heal(20);

            // R: Revive
            if (Input.GetKeyDown(KeyCode.R))
                Revive();
        }

        // ── Public API (for external systems like traps, skills) ──────
        public void TakeDamage(int amount) => _model.TakeDamage(amount);
        public void Heal(int amount) => _model.Heal(amount);
        public void Revive() => _model.Revive();

        public bool IsDead => _model?.IsDead ?? false;
        public int CurrentHp => _model?.CurrentHp ?? 0;

        private void OnDestroy()
        {
            if (GameContext.Instance != null)
                GameContext.Instance.UnregisterCharacter();
        }
    }
}
