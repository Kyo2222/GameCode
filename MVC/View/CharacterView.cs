using MVC.Model;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MVC.View
{
    /// <summary>
    /// Attached to the character GameObject, responsible for UI/Visual presentation.
    /// Only listens to Model events and does not directly manipulate Model data.
    /// </summary>
    public class CharacterView : MonoBehaviour
    {
        [Header("UI References (optional)")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private GameObject deadOverlay;

        private CharacterModel _model;

        // ── Initialization (called by Controller) ──────────────────
        public void Initialize(CharacterModel model)
        {
            _model = model;

            // Subscribe to events
            _model.OnHpChanged += HandleHpChanged;
            _model.OnDead += HandleDead;

            // Initial display update
            RefreshAll();
        }

        // ── Event Handling ──────────────────────────────────────
        private void HandleHpChanged(int current, int max)
        {
            if (hpText != null)
                hpText.text = $"{current} / {max}";

            if (hpSlider != null)
                hpSlider.value = (float)current / max;
        }

        private void HandleDead()
        {
            Debug.Log($"[CharacterView] {_model.CharacterName} has died.");
            if (deadOverlay != null)
                deadOverlay.SetActive(true);
        }

        // ── Full Refresh ──────────────────────────────────────
        private void RefreshAll()
        {
            if (nameText != null)
                nameText.text = _model.CharacterName;

            HandleHpChanged(_model.CurrentHp, _model.MaxHp);

            if (deadOverlay != null)
                deadOverlay.SetActive(_model.IsDead);
        }

        private void OnDestroy()
        {
            if (_model == null) return;
            _model.OnHpChanged -= HandleHpChanged;
            _model.OnDead -= HandleDead;
        }
    }
}
