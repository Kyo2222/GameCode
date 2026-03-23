using System;
using UnityEngine;

namespace MVC.Model
{
    [Serializable]
    public class HealerData
    {
        public string healerName = "Healer";
        [Range(0f, 1f)] public float triggerThreshold = 0.45f; // Cast when health is below this ratio
        [Range(0f, 1f)] public float healPercent = 0.20f;      // Heal % of target's max HP
        public float cooldown = 3f;                             // Cooldown in seconds
    }

    public class HealerModel
    {
        public string HealerName { get; private set; }
        public float TriggerThreshold { get; private set; }
        public float HealPercent { get; private set; }
        public float Cooldown { get; private set; }

        // Cooldown state
        private float _lastHealTime = float.MinValue;

        public bool IsReady(float currentTime) => currentTime - _lastHealTime >= Cooldown;

        public void RecordHeal(float currentTime) => _lastHealTime = currentTime;

        public HealerModel(HealerData data)
        {
            HealerName = data.healerName;
            TriggerThreshold = data.triggerThreshold;
            HealPercent = data.healPercent;
            Cooldown = data.cooldown;
        }
    }
}
