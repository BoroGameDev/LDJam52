using UnityEngine;
using UnityEngine.UI;
using BoroGameDev.Utilities;

namespace BoroGameDev.Victims {
    public class HealthController : MonoBehaviour {
        [SerializeField]
        [Range(0, 200)]
        private int MaxHealth = 100;
        private float Health;

        [SerializeField]
        private GameObject HealthCanvas;

        [SerializeField]
        private Image HealthBarFill;

        private Animator anim;

        private void Awake() {
            this.Health = MaxHealth;
            this.anim = GetComponent<Animator>();
        }

        public float GetHealth() {
            return Health;
        }
        public int GetMaxHealth() {
            return MaxHealth;
        }

        public void DrainHealth(float damage) {
            if (this.Health == 0) { return;  }

            HealthCanvas.SetActive(true);
            this.Health = Mathf.Max(this.Health - damage, 0);

            HealthBarFill.fillAmount = this.Health / this.MaxHealth;

            if (this.Health == 0) {
                anim.SetTrigger("Die");
            }
        }

        private void Die() {
            Destroy(gameObject);
            GameEvents.Instance.VictimDied();
        }
    }
}
