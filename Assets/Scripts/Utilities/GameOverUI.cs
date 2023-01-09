using UnityEngine;
using TMPro;

namespace BoroGameDev.Utilities {
    public class GameOverUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI VictimsKilled;
        [SerializeField]
        private TextMeshProUGUI HowDied;

        private void Start() {
            VictimsKilled.text = $"You killed {GameManager.Instance.GetVictimCount()} victims.";
            HowDied.text = GameManager.Instance.GetLoseReason();
        }

        public void ToTitle() {
            GameManager.Instance.ToTitle();
        }
    }
}
