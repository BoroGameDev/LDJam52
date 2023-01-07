using UnityEngine;

namespace BoroGameDev.Player {
    public class MovementController : MonoBehaviour {
        [SerializeField]
        [Range(0f, 10f)]
        private float MoveSpeed;

        private Rigidbody2D body;
        private Vector2 velocity;

        private void Start() {
            body = GetComponent<Rigidbody2D>();
        }

        private void Update() {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z));
            Vector3 dir = mousePos - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            velocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * MoveSpeed;
        }

        private void FixedUpdate() {
            body.MovePosition(body.position + velocity * Time.deltaTime);
        }
    }
}
