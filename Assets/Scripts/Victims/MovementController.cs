using BoroGameDev.Player;
using BoroGameDev.Utilities;
using System.Linq;
using UnityEngine;

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

        private FieldOfView eyes;
        [SerializeField]
        private Vector3 destination;
        private int wanderCount = 0;
        private StateManager stateManager;
        private Animator anim;
        private HealthController health;

        private void Awake() {
            eyes = GetComponent<FieldOfView>();
            stateManager = GetComponent<StateManager>();
            anim = GetComponent<Animator>();
            health = GetComponent<HealthController>();
        }

        public void SetDestination(Vector3 _destination) {
            this.destination = _destination;
        }

        private void Update() {
            switch (stateManager.GetState()) {
                case VictimState.Patrol:
                    Patrol();
                    break;
                case VictimState.Chase:
                    Chase();
                    break;
                case VictimState.Wander:
                    Wander();
                    break;
                case VictimState.Drain:
                    Drain();
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
                this.stateManager.SetWander();
            }
        }

        public void Wander() {
            if(transform.position != destination) {
                MoveToTarget();
            }

            if (reachedDestination) {
                ++wanderCount;
                destination = GetRoamingPosition();
                MoveToTarget();
            }

            if (wanderCount == 4) {
                wanderCount = 0;
                this.stateManager.SetPatrol();
            }
        }

        public void Drain() {
            anim.SetBool("Walking", false);
            health.DrainHealth(0.25f);
            return;
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
            anim.SetBool("Walking", true);
            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.z = 0f;

            float destinationDistance = destinationDirection.magnitude;

            if (destinationDistance >= stopDistance) {
                reachedDestination = false;
                float angle = Mathf.Atan2(destinationDirection.y, destinationDirection.x) * Mathf.Rad2Deg - 90;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
                transform.Translate(Vector2.up * Speed * Time.deltaTime);
            } else {
                reachedDestination = true;
            }
        }

        private void LateUpdate() {
            if (eyes.GetVisibleTargets().Count > 0) {
                Transform target = eyes.GetVisibleTargets().First<Transform>();
                PlayerController _player = target.GetComponent<PlayerController>();
                Vector3 pos = target.position;
                pos.z = 0f;

                this.SetDestination(pos);
                this.stateManager.SetChase();
            }
        }
    }
}
