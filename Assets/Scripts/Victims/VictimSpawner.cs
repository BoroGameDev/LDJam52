using BoroGameDev.Utilities;
using System.Collections;
using UnityEngine;

namespace BoroGameDev.Victims {
    public class VictimSpawner : MonoBehaviour {
        [SerializeField]
        private GameObject VictimPrefab;

        [SerializeField]
        private float Delay = 10f;

        [SerializeField]
        private Waypoint StartingWaypoint;

        void Start() {
            StartCoroutine("SpawnLoop");
        }

        IEnumerator SpawnLoop() {
            while (true) {
                SpawnEnemy();
                yield return new WaitForSeconds(Delay);
            }
        }

        private void SpawnEnemy() {
            GameObject go = Instantiate(VictimPrefab, transform.position, Quaternion.identity, transform);
            WaypointNavigator nav = go.GetComponent<WaypointNavigator>();
            nav.Init(StartingWaypoint);
        }
    }
}
