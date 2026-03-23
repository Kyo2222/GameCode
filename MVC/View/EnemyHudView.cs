using MVC.Base;
using MVC.Model;
using UnityEngine;
using TMPro;

namespace MVC.View
{
    /// <summary>
    /// Cross-object query example: Directly retrieves CharacterModel from GameContext.
    /// No direct reference or string keys required.
    /// </summary>
    public class EnemyHudView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI targetHpText;

        private CharacterModel _target;

        private void Start()
        {
            _target = GameContext.Instance?.Character;

            if (_target == null)
            {
                Debug.LogWarning("[EnemyHudView] GameContext.Character not yet registered.");
                return;
            }

            _target.OnHpChanged += OnTargetHpChanged;
            OnTargetHpChanged(_target.CurrentHp, _target.MaxHp);
        }

        private void OnTargetHpChanged(int current, int max)
        {
            if (targetHpText != null)
                targetHpText.text = $"{_target.CharacterName} HP: {current}/{max}";
        }

        private void OnDestroy()
        {
            if (_target != null)
                _target.OnHpChanged -= OnTargetHpChanged;
        }
    }
}
