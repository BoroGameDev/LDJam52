using UnityEngine;

namespace BoroGameDev.Victims {
    public class WaypointNavigator : MonoBehaviour {
        MovementController movement;
        StateManager state;
        public Waypoint currentWaypoint;

        private int direction = 0;

        void Awake() {
            movement = GetComponent<MovementController>();
            state = GetComponent<StateManager>();
        }

        private void Start() {
            movement.SetDestination(currentWaypoint.GetPosition());
        }

        void Update() {
            if (state.GetState() != VictimState.Patrol) { return; }

            if (movement.reachedDestination) {
                bool shouldBranch = false;

                if(currentWaypoint.branches != null && currentWaypoint.branches.Count > 0) {
                    shouldBranch = Random.Range(0f, 1f) <= currentWaypoint.branchRatio;
                }

                if (shouldBranch) {
                    currentWaypoint = currentWaypoint.branches[Random.Range(0, currentWaypoint.branches.Count - 1)];
                } else {
                    if (direction == 0) {
                        if (currentWaypoint.nextWaypoint != null) {
                            currentWaypoint = currentWaypoint.nextWaypoint;
                        } else {
                            currentWaypoint = currentWaypoint.previousWaypoint;
                            direction = 1;
                        }
                    } else if (direction == 1) {
                        if (currentWaypoint.previousWaypoint != null) {
                            currentWaypoint = currentWaypoint.previousWaypoint;
                        } else {
                            currentWaypoint = currentWaypoint.nextWaypoint;
                            direction = 0;
                        }
                    }
                }

                movement.SetDestination(currentWaypoint.GetPosition());
            }
        }
    }
}
