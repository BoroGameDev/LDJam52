using UnityEngine;

namespace BoroGameDev.Victims {
    public class MovementController : MonoBehaviour {
        [SerializeField]
        [Range(0f, 100f)]
        private float Speed;

        private void Update() {
            transform.Rotate(new Vector3(0f, 0f, Speed * Time.deltaTime));
        }
    }
}
