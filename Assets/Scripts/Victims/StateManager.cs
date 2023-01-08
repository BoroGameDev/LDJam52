using UnityEngine;

namespace BoroGameDev.Victims {
    public enum VictimState {
        Patrol,
        Wander,
        Chase,
        Escape,
        Drain
    }

    public class StateManager : MonoBehaviour {
        private VictimState state = VictimState.Patrol;

        public VictimState GetState() {
            return this.state;
        }

        public void SetPatrol() {
            this.state = VictimState.Patrol;
        }
        public void SetChase() {
            this.state = VictimState.Chase;
        }
        public void SetWander() {
            this.state = VictimState.Wander;
        }
        public void SetDrain() {
            this.state = VictimState.Drain;
        }
    }
}
