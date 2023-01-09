using UnityEngine;

namespace BoroGameDev.Utilities {
    public class TitleSceneManager : MonoBehaviour {

        public void StartGame() {
            GameManager.Instance.LoadGameScene();
        }

        public void QuitGame() {
            GameManager.Instance.QuitGame();
        }
    }
}
