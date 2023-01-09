using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace BoroGameDev.Utilities {

	public enum SceneIndexes {
		MANAGER = 0,
		TITLE_SCREEN = 1,
		TEST_LEVEL = 2,
		GAME_OVER = 3
	}

	public class GameManager : MonoBehaviour {
		#region Singleton
		public static GameManager Instance { get; private set; }

        void Awake() {
			if (Instance == null) {
				Instance = this;
				DontDestroyOnLoad(gameObject);
			} else {
				DestroyImmediate(gameObject);
				return;
			}
		}
		#endregion

		private SceneIndexes currentScene;
		public SceneIndexes CurrentScene { get { return currentScene; } }

		public GameObject Player { get; private set; }

		[SerializeField] private GameObject LoadingScreen;
		[SerializeField] private ProgressBar m_ProgressBar;

		private int VictimCount = 0;

		List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
		float TotalProgress;

		string LoseReason = "";

		#region Unity Methods
		private void Start() {
			SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
			currentScene = SceneIndexes.TITLE_SCREEN;
			LoadingScreen.SetActive(false);
			GameEvents.Instance.onYouLose += GameOver;
			//currentScene = SceneIndexes.TEST_LEVEL;
		}

        private void OnDestroy() {
			GameEvents.Instance.onYouLose -= GameOver;
        }
        #endregion

        #region Custom Methods
		private void GameOver(string reason) {
			LoseReason = reason;
			LoadingScreen.SetActive(true);

			scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TEST_LEVEL));
			scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GAME_OVER, LoadSceneMode.Additive));

			StartCoroutine(GetSceneLoadProgress());

			currentScene = SceneIndexes.GAME_OVER;
			GameEvents.Instance.SceneLoaded(SceneIndexes.GAME_OVER);
			LoadingScreen.SetActive(false);
        }

        public void AddVictim() {
			VictimCount++;
        }
        public void SetVictimCount(int num) {
			VictimCount = num;
        }
        public int GetVictimCount() {
			return VictimCount;
        }
		public string GetLoseReason() {
			return LoseReason;
        }

		public void SetPlayer(GameObject _player) {
			Player = _player;
		}

		public void LoadGameScene() {
			LoadingScreen.SetActive(true);

			scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN));
			scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.TEST_LEVEL, LoadSceneMode.Additive));

			StartCoroutine(GetSceneLoadProgress());

			currentScene = SceneIndexes.TEST_LEVEL;
			GameEvents.Instance.SceneLoaded(SceneIndexes.TEST_LEVEL);
			LoadingScreen.SetActive(false);
		}

		public IEnumerator GetSceneLoadProgress() {
			for (int i = 0; i < scenesLoading.Count; i++) {
				while (!scenesLoading[i].isDone) {
					TotalProgress = 0;

					foreach (AsyncOperation operation in scenesLoading) {
						TotalProgress += operation.progress;
					}

					TotalProgress = (TotalProgress / scenesLoading.Count) * 100f;

					m_ProgressBar.Current = Mathf.RoundToInt(TotalProgress);
					yield return new WaitForSeconds(0.5f);
				}
			}

		}

		public void ToTitle() {
			LoadingScreen.SetActive(true);

			scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.GAME_OVER));
			scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive));

			StartCoroutine(GetSceneLoadProgress());
			currentScene = SceneIndexes.TITLE_SCREEN;
			GameEvents.Instance.SceneLoaded(SceneIndexes.TITLE_SCREEN);
		}

		public void QuitGame() {
			Application.Quit();
		}
		#endregion

	}

}
