using MVC.Base;
using MVC.Model;
using UnityEngine;

namespace MVC.Controller
{
    /// <summary>
    /// Attached to the healer GameObject.
    /// Subscribes to HP events on GameContext.Instance.Character
    /// and automatically heals when HP drops below the threshold and cooldown is ready.
    /// </summary>
    public class HealerController : MonoBehaviour
    {
        [SerializeField] private HealerData healerData;

        private HealerModel _model;
        private CharacterModel _target; // retrieved from GameContext

        private void Start()
        {
            if (healerData == null)
                healerData = new HealerData();

            _model = new HealerModel(healerData);

            // Get the player Model from the global context
            _target = GameContext.Instance?.Character;

            if (_target == null)
            {
                Debug.LogWarning("[HealerController] GameContext.Character not found. Make sure CharacterController has already Awake'd.");
                return;
            }

            // Subscribe to HP change events
            _target.OnHpChanged += OnTargetHpChanged;

            // Perform an immediate check on start in case the target is already low on HP
            OnTargetHpChanged(_target.CurrentHp, _target.MaxHp);

            Debug.Log($"[HealerController] {_model.HealerName} started monitoring {_target.CharacterName} (threshold: {_model.TriggerThreshold * 100}%)");
        }

        private void OnTargetHpChanged(int current, int max)
        {
            if (_target.IsDead) return;
            float ratio = (float)current / max;

            if (ratio < _model.TriggerThreshold && _model.IsReady(Time.time))
            {
                int healAmount = Mathf.RoundToInt(max * _model.HealPercent);
                _target.Heal(healAmount);
                _model.RecordHeal(Time.time);

                Debug.Log($"[HealerController] {_model.HealerName} cast heal! Restored {healAmount} HP ({_model.HealPercent * 100}%)");
            }
        }

        private void OnDestroy()
        {
            if (_target != null)
                _target.OnHpChanged -= OnTargetHpChanged;
        }
    }
}
