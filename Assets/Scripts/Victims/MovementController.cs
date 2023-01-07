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

        [SerializeField]
        private Vector3 destination;

        public bool reachedDestination;
        private FieldOfView eyes;

        private void Awake() {
            eyes = GetComponent<FieldOfView>();
        }

        public void SetDestination(Vector3 _destination) {
            this.destination = _destination;
        }

        private void Update() {
            if(transform.position != destination) {
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

        }

        private void LateUpdate() {
            if (eyes.GetVisibleTargets().Count > 0) {
                Vector3 target = eyes.GetVisibleTargets().First<Transform>().position;
                target.z = 0f;
                this.SetDestination(target);
            }
        }
    }
}
