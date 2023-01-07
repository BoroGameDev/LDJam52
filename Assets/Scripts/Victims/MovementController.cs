using UnityEngine;
using System.Linq;

namespace BoroGameDev.Victims {
    public class MovementController : MonoBehaviour {
        [SerializeField]
        [Range(0f, 100f)]
        private float Speed;

        [SerializeField]
        [Range(0f, 1000f)]
        private float RotationSpeed;

        [SerializeField]
        [Range(0f, 2f)]
        private float stopDistance;

        public bool reachedDestination;

        [HideInInspector]
        public enum State {
            Patrol,
            Wander,
            Chase,
            Escape,
            Drain
        }

        public State state = State.Patrol;

        private FieldOfView eyes;
        [SerializeField]
        private Vector3 destination;
        private int wanderCount;

        private void Awake() {
            eyes = GetComponent<FieldOfView>();
        }

        public void SetDestination(Vector3 _destination) {
            this.destination = _destination;
        }

        private void Update() {
            switch (state) {
                case State.Patrol:
                    Patrol();
                    break;
                case State.Chase:
                    Chase();
                    break;
                case State.Wander:
                    Wander();
                    break;
                default:
                    break;
            }
        }

        public void Patrol() {
            if(transform.position != destination) {
                MoveToTarget();
            }
        }

        public void Chase() {
            if(transform.position != destination) {
                MoveToTarget();
            }

            if (!eyes.HasVisibleTargets()) {
                destination = GetRoamingPosition();
                this.state = State.Wander;
            }
        }

        public void Wander() {
            if(transform.position != destination) {
                MoveToTarget();
            }

            if (reachedDestination) {
                destination = GetRoamingPosition();
                MoveToTarget();
            }

            if (wanderCount++ == 4) {

            }
        }

        private Vector3 GetRoamingPosition() {
            return transform.position + GetRandomDirection() * Random.Range(1f, 5f);
        }

        private Vector3 GetRandomDirection() {
            return new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
                ).normalized;
        }

        private void MoveToTarget() {
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.z = 0f;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance) {
                reachedDestination = false;
                float angle = Mathf.Atan2(destinationDirection.y, destinationDirection.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                transform.Translate(Vector2.right * Speed * Time.deltaTime);
            } else {
                reachedDestination = true;
            }
        }

        private void LateUpdate() {
            if (eyes.GetVisibleTargets().Count > 0) {
                Vector3 target = eyes.GetVisibleTargets().First<Transform>().position;
                target.z = 0f;
                this.SetDestination(target);
                this.state = State.Chase;
            }
        }
    }
}
