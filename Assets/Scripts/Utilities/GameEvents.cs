using System;

namespace BoroGameDev.Utilities {

	public class GameEvents {
		#region Singleton
		private static GameEvents instance = null;
		private static readonly object padlock = new object();

		GameEvents() { }

		public static GameEvents Instance {
			get {
				lock (padlock) {
					if (instance == null) {
						instance = new GameEvents();
					}
					return instance;
				}
			}
		}
		#endregion

		/**
		 * Game Event Example:
		 *		public event Action<type> onEventHappened;
		 *		public void EventHappened(type param) {
		 *			if (onEventHappened != null) {
		 *				onEventHappened(param);
		 *			}
		 *		}
		 *
		 * To trigger an event:
		 *		GameEvents.Instance.EventHappened(param);
		 *
		 * To subscribe to an event:
		 *		void Awake() {
		 *			GameEvents.Instance.onEventHappened += this.EventListener;
		 *		}
		 *	Unsubscribe to avoid memory leaks:
		 *		void OnDestroy() {
		 *			GameEvents.Instance.onEventHappened -= this.EventListener;
		 *		}
		 *		void EventListener(type param) {...}
		 **/

		public event Action<SceneIndexes> onSceneLoaded;
		public void SceneLoaded(SceneIndexes _index) {
			if (onSceneLoaded != null) {
				onSceneLoaded(_index);
			}
		}

		public event Action onVictimDied;
		public void VictimDied() {
			if (onVictimDied != null) {
				onVictimDied();
            }
        }

		public event Action<string> onYouLose;
		public void YouLose(string reason) {
			if (onYouLose != null) {
				onYouLose(reason);
            }
        }
    }
}
