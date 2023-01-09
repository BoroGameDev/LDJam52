using BoroGameDev.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace BoroGameDev.Player {
    public class PlayerController : MonoBehaviour {
        [SerializeField]
        [Range(0, 200)]
        private int MaxHealth = 100;
        private float Health;

        [SerializeField]
        private Image HealthBarFill;

        private void Awake() {
            this.Health = MaxHealth;
        }

        private void FixedUpdate() {
            this.DrainHealth(0.1f);
            HealthBarFill.fillAmount = this.Health / this.MaxHealth;
        }

        public float GetHealth() {
            return Health;
        }
        public int GetMaxHealth() {
            return MaxHealth;
        }

        public void GainHealth(float amount) {
            if (this.Health == this.MaxHealth) { return;  }

            this.Health = Mathf.Min(this.Health + amount, this.MaxHealth);
        }

        public void DrainHealth(float damage) {
            if (this.Health == 0) { return;  }

            this.Health = Mathf.Max(this.Health - damage, 0);

            if (this.Health == 0) {
                GameEvents.Instance.YouLose("Life force ran ou.");
            }
        }
    }
}