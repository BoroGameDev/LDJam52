using UnityEngine;

namespace BoroGameDev.Player {
    public class MovementController : MonoBehaviour {
        [SerializeField]
        [Range(0f, 10f)]
        private float MoveSpeed;

        [SerializeField]
        [Range(0f, 10f)]
        private float RotationSpeed;

        [SerializeField]
        private Transform Flashlight;

        [Header("Sprites")]
        [SerializeField]
        private Sprite UpSprite;
        [SerializeField]
        private Sprite DownSprite;
        [SerializeField]
        private Sprite LeftSprite;
        [SerializeField]
        private Sprite RightSprite;

        private Vector2 moveInput;
        private Rigidbody2D body;
        private SpriteRenderer spriteRenderer;
        private Vector2 velocity;
        private Animator anim;

        private void Start() {
            body = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            anim = GetComponent<Animator>();
        }

        private void Update() {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            velocity = moveInput.normalized * MoveSpeed;

            float angle = Mathf.Atan2(-velocity.x, velocity.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            Flashlight.transform.rotation = Quaternion.RotateTowards(Flashlight.transform.rotation, targetRotation, RotationSpeed);
            if (Mathf.Abs(moveInput.x) > 0.1f) {
                if (moveInput.x > 0) {
                    spriteRenderer.sprite = RightSprite;
                } else {
                    spriteRenderer.sprite = LeftSprite;
                }
            } else {
                if (moveInput.y < 0) {
                    spriteRenderer.sprite = DownSprite;
                } else {
                    spriteRenderer.sprite = UpSprite;
                }
            }

            anim.SetBool("walking", false);
            if (moveInput.magnitude > 0.1f) {
                anim.SetBool("walking", true);
            }
        }

        private void FixedUpdate() {
            body.MovePosition(body.position + velocity * Time.deltaTime);
        }
    }
}
