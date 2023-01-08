using BoroGameDev.Utilities;
using BoroGameDev.Victims;
using System.Linq;
using UnityEngine;

namespace BoroGameDev.Player {
    public class DrainController : MonoBehaviour {
        [SerializeField]
        private LayerMask HarvestCheckLayer;

        private FieldOfView eyes;
        private bool canDrain = false;
        private Rigidbody2D body;
        private MovementController move;

        private void Awake() {
            eyes = GetComponentInChildren<FieldOfView>();
            body = GetComponent<Rigidbody2D>();
            move = GetComponent<MovementController>();
        }

        private void Update() {
            canDrain = eyes.HasVisibleTargets();

            if (!canDrain) { return; }

            if (Input.GetButtonDown("Jump")) {
                HarvestVictim();
            }
        }

        void HarvestVictim() {
            if (eyes.GetVisibleTargets().Count > 0) {
                Transform target = eyes.GetVisibleTargets().First<Transform>();
                StateManager victim = target.GetComponent<StateManager>();
                Vector3 pos = target.position;
                pos.z = 0f;

                Dash(pos);
                victim.SetDrain();
            }
        }

        void Dash(Vector3 position) {
            move.SetCanMove(false);
            Vector2 dashDirection = position - transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, eyes.GetViewRadius(), HarvestCheckLayer);
            if (hit.collider != null) {
                position = hit.point;
            }

            print(position);
            body.MovePosition(position);
        }

    }
}
