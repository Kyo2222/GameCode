using System;
using UnityEngine;

namespace MVC.Model
{
    [Serializable]
    public class CharacterData
    {
        public string characterName = "Hero";
        public int maxHp = 100;
        public float moveSpeed = 5f;
    }

    public class CharacterModel
    {
        // ── Read-only Initial Data ──────────────────────────────────
        public string CharacterName { get; private set; }
        public int MaxHp { get; private set; }
        public float MoveSpeed { get; private set; }

        // ── Dynamic State ──────────────────────────────────────
        private int _currentHp;
        public int CurrentHp
        {
            get => _currentHp;
            private set
            {
                _currentHp = Mathf.Clamp(value, 0, MaxHp);
                OnHpChanged?.Invoke(_currentHp, MaxHp);
                if (_currentHp <= 0) OnDead?.Invoke();
            }
        }

        private bool _isDead;
        public bool IsDead => _isDead;

        // ── Events ─────────────────────────────────────────
        public event Action<int, int> OnHpChanged;
        public event Action OnDead;

        // ── Initialization ────────────────────────────────────────
        public CharacterModel(CharacterData data)
        {
            CharacterName = data.characterName;
            MaxHp = data.maxHp;
            MoveSpeed = data.moveSpeed;
            _currentHp = MaxHp;
            _isDead = false;
        }

        // ── Public Methods (called by Controller) ────────────────
        public void TakeDamage(int amount)
        {
            if (_isDead) return;
            CurrentHp -= amount;
            if (_currentHp <= 0) _isDead = true;
        }

        public void Heal(int amount)
        {
            if (_isDead) return;
            CurrentHp += amount;
        }

        public void Revive()
        {
            _isDead = false;
            CurrentHp = MaxHp;
        }
    }
}
