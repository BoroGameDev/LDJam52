using BoroGameDev.Utilities;
using BoroGameDev.Victims;
using System.Linq;
using UnityEngine;

namespace BoroGameDev.Player {
    public class DrainController : MonoBehaviour {
        [SerializeField]
        private LayerMask HarvestCheckLayer;

        [SerializeField]
        private GameObject CallToAction;

        private FieldOfView eyes;
        private bool canDrain = false;
        private bool draining = false;
        private Rigidbody2D body;
        private MovementController move;
        private StateManager Victim;
        private PlayerController player;

        private void Awake() {
            eyes = GetComponentInChildren<FieldOfView>();
            body = GetComponent<Rigidbody2D>();
            move = GetComponent<MovementController>();
            player = GetComponent<PlayerController>();
            GameEvents.Instance.onVictimDied += VictimDied;
        }

        private void OnDestroy() {
            GameEvents.Instance.onVictimDied -= VictimDied;
        }

        public bool IsDraining() {
            return this.draining;
        }

        private void Update() {
            canDrain = eyes.HasVisibleTargets();

            CallToAction.SetActive(false);

            if (!canDrain) { return; }

            CallToAction.SetActive(true);

            if (Input.GetButtonDown("Jump")) {
                if (!draining) {
                    HarvestVictim();
                } else {
                    draining = false;
                    move.SetCanMove(true);
                    Victim.SetWander();
                    Victim = null;
                }
            }
        }

        public void FixedUpdate() {
            if (this.draining) {
                player.GainHealth(0.25f);
            }
        }

        public void VictimDied() {
            this.draining = false;
            move.SetCanMove(true);
        }

        void HarvestVictim() {
            if (eyes.GetVisibleTargets().Count > 0) {
                Transform target = eyes.GetVisibleTargets().First<Transform>();
                Victim = target.GetComponent<StateManager>();
                Vector3 pos = target.position;
                pos.z = 0f;

                Dash(pos);
                Victim.SetDrain();
            }
        }

        void Dash(Vector3 position) {
            draining = true;
            move.SetCanMove(false);
            Vector2 dashDirection = position - transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, eyes.GetViewRadius(), HarvestCheckLayer);
            if (hit.collider != null) {
                position = hit.point;
            }

            body.MovePosition(position);
        }

    }
}
