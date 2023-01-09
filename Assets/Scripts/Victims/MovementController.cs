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

        [SerializeField]
        private Transform EyesTransform;

        [SerializeField]
        private LayerMask ObstacleMask;

        [SerializeField]
        private float obstacleDetectionRadius = 3f;

        [Header("Sprites")]
        [SerializeField]
        private Sprite UpSprite;
        [SerializeField]
        private Sprite DownSprite;
        [SerializeField]
        private Sprite LeftSprite;
        [SerializeField]
        private Sprite RightSprite;
        [SerializeField]
        private bool showGizmos = false;

        private Vector2[] directions = {
            new Vector2(0, 1).normalized,
            new Vector2(1, 1).normalized,
            new Vector2(1, 0).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(0, -1).normalized,
            new Vector2(-1, -1).normalized,
            new Vector2(-1, 0).normalized,
            new Vector2(-1, 1).normalized
        };

        public bool reachedDestination;

        private FieldOfView eyes;
        private Vector3 destination;
        private int wanderCount = 0;
        private StateManager stateManager;
        private Animator anim;
        private HealthController health;
        private SpriteRenderer spriteRenderer;
        private float[] interestGizmo;
        Collider2D[] obstacles;

        private void Awake() {
            eyes = GetComponentInChildren<FieldOfView>();
            stateManager = GetComponent<StateManager>();
            anim = GetComponent<Animator>();
            health = GetComponent<HealthController>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetDestination(Vector3 _destination) {
            this.destination = _destination;
        }

        private void Update() {
            obstacles = Physics2D.OverlapCircleAll(transform.position, obstacleDetectionRadius, ObstacleMask);

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
            return transform.position + GetRandomDirection() * Random.Range(1f, 2f);
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
            float destinationDistance = destinationDirection.magnitude;

            destinationDirection = GetMoveDirection();

            if (destinationDistance >= stopDistance) {
                reachedDestination = false;
                float angle = Mathf.Atan2(destinationDirection.y, destinationDirection.x) * Mathf.Rad2Deg - 90;
                Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                EyesTransform.rotation = Quaternion.RotateTowards(EyesTransform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

                if (Mathf.Abs(destinationDirection.x) > Mathf.Abs(destinationDirection.y)) {
                    if (destinationDirection.x > 0) {
                        spriteRenderer.sprite = RightSprite;
                    } else {
                        spriteRenderer.sprite = LeftSprite;
                    }
                } else {
                    if (destinationDirection.y < 0) {
                        spriteRenderer.sprite = DownSprite;
                    } else {
                        spriteRenderer.sprite = UpSprite;
                    }
                }

                transform.Translate(destinationDirection.normalized * Speed * Time.deltaTime);
            } else {
                reachedDestination = true;
            }
        }

        private Vector3 GetMoveDirection() {
            float[] danger = new float[8];
            float[] interest = new float[8];

            Vector3 destinationDirection = destination - transform.position;
            destinationDirection.z = 0f;

            (danger, interest) = GetSeekSteering(destinationDirection, danger, interest);
            (danger, interest) = GetAvoidanceSteering(destinationDirection, danger, interest);

            for (int i = 0; i < directions.Length; i++) {
                interest[i] = Mathf.Clamp01(interest[i] - danger[i]);
            }

            interestGizmo = interest;

            Vector2 outputDirection = Vector2.zero;
            for (int i = 0; i < directions.Length; i++) {
                outputDirection += directions[i] * interest[i];
            }

            outputDirection.Normalize();

            return outputDirection;
        }

        private (float[] danger, float[] interest) GetSeekSteering(Vector3 destinationDirection, float[] danger, float[] interest) {
            if (destination == null) {
                return (danger, interest);
            }

            for (int i = 0; i < directions.Length; i++) {
                float result = Vector2.Dot(destinationDirection.normalized, directions[i]);

                interest[i] = Mathf.Clamp01(Mathf.Max(interest[i], result));
            }

            return (danger, interest);
        }

        private (float[] danger, float[] interest) GetAvoidanceSteering(Vector3 destinationDirection, float[] danger, float[] interest) {
            if (destination == null) {
                return (danger, interest);
            }

            foreach(Collider2D obstacleCollider in obstacles) {
                Vector2 directionToObstacle = obstacleCollider.ClosestPoint(transform.position) - (Vector2)transform.position;
                float distanceToObstacle = directionToObstacle.magnitude;

                float weight = distanceToObstacle <= 1f ? 1 : (obstacleDetectionRadius - distanceToObstacle) / obstacleDetectionRadius;

                for (int i = 0; i < directions.Length; i++) {
                    float result = Vector2.Dot(directionToObstacle.normalized, directions[i]) * weight;
                    danger[i] = Mathf.Clamp01(Mathf.Max(danger[i], result));
                }
            }

            return (danger, interest);
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

        private void OnDrawGizmos() {
            if (!showGizmos || !Application.isPlaying) { return; }

            Gizmos.color = Color.yellow;

            for (int i = 0; i < interestGizmo.Length; i++) {
                Gizmos.DrawRay(transform.position, directions[i].normalized * interestGizmo[i] * 3f);
            }
        }
    }
}
