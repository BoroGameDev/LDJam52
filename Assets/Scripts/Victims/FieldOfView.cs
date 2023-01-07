using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoroGameDev.Victims {
    public class FieldOfView : MonoBehaviour {
        [SerializeField]
        private float ViewRadius;

        [SerializeField]
        [Range(0, 360f)]
        private float ViewAngle;

        [SerializeField]
        private LayerMask TargetMask;
        [SerializeField]
        private LayerMask ObstacleMask;

        private List<Transform> visibleTargets = new List<Transform>();

        [Header("View Mesh Rendering")]
        [SerializeField]
        [Range(0, 1f)]
        private float MeshResolution;
        [SerializeField]
        private MeshFilter viewMeshFilter;
        [SerializeField]
        private int EdgeResolveIterations;
        [SerializeField]
        private float EdgeDistanceThreshold;

        private Mesh viewMesh;

        private void Start() {
            viewMesh = new Mesh();
            viewMesh.name = "View Mesh";
            viewMeshFilter.mesh = viewMesh;

            StartCoroutine("FindTargetsWithDelay", 0.2f);
        }

        private void LateUpdate() {
            DrawFieldOfView();
        }

        IEnumerator FindTargetsWithDelay(float delay) {
            while(true) {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        void DrawFieldOfView() {
            int stepCount = Mathf.RoundToInt(ViewAngle * MeshResolution);
            float stepAngleSize = ViewAngle / stepCount;
            List<Vector2> viewPoints = new List<Vector2>();
            ViewCastInfo oldCastInfo = new ViewCastInfo();

            for (int i = 0; i <= stepCount; i++) {
                float angle = transform.eulerAngles.z + ViewAngle * 0.5f - stepAngleSize * i;
                ViewCastInfo castInfo = ViewCast(angle);

                if (i > 0) {
                    bool edgeDistanceExceeded = Mathf.Abs(oldCastInfo.distance - castInfo.distance) > EdgeDistanceThreshold;
                    if (oldCastInfo.hit != castInfo.hit || (oldCastInfo.hit && castInfo.hit && edgeDistanceExceeded)) {
                        EdgeInfo edge = FindEdge(oldCastInfo, castInfo);
                        if (edge.pointA != Vector2.zero) {
                            viewPoints.Add(edge.pointA);
                        }
                        if (edge.pointB != Vector2.zero) {
                            viewPoints.Add(edge.pointB);
                        }
                    }
                }

                viewPoints.Add(castInfo.point);
                oldCastInfo = castInfo;
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector2.zero;
            for (int i = 0; i < vertexCount - 1; i++) {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2) {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector2 minPoint = Vector2.zero;
            Vector2 maxPoint = Vector2.zero;

            for (int i = 0; i < EdgeResolveIterations; i++) {
                float angle = (minAngle + maxAngle) * 0.5f;
                ViewCastInfo newViewCast = ViewCast(angle);

                bool edgeDistanceExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > EdgeDistanceThreshold;
                if (newViewCast.hit == minViewCast.hit) {
                    minAngle = angle;
                    minPoint = newViewCast.point;
                } else {
                    maxAngle = angle;
                    maxPoint = newViewCast.point;
                }
            }

            return new EdgeInfo(minPoint, maxPoint);
        }

        ViewCastInfo ViewCast(float globalAngle) {
            Vector3 direction = DirectionFromAngle(globalAngle, true);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, ViewRadius, ObstacleMask);
            if (hit) {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            } else {
                return new ViewCastInfo(false, transform.position + direction * ViewRadius, ViewRadius, globalAngle);
            }
        }

        void FindVisibleTargets() {
            visibleTargets.Clear();
            Collider2D[] targets = Physics2D.OverlapCircleAll((Vector2)transform.position, ViewRadius, TargetMask);
            
            for (int i = 0; i < targets.Length; i++) {
                Transform target = targets[i].transform;
                Vector2 directionToTarget = (target.position - transform.position).normalized;
                if (Vector2.Angle(transform.right, directionToTarget) < ViewAngle * 0.5f) {
                    float distanceToTarget = Vector2.Distance(transform.position, target.position);
                    if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, ObstacleMask)) {
                        visibleTargets.Add(target);
                    }
                }
            }
        }

        public List<Transform> GetVisibleTargets() {
            return visibleTargets;
        }

        public bool HasVisibleTargets() {
            return visibleTargets.Count > 0;
        }
        
        public float GetViewRadius() {
            return ViewRadius;
        }

        public float GetViewAngle() {
            return ViewAngle;
        }
        
        public Vector3 DirectionFromAngle(float angleDegrees, bool angleIsGlobal) {
            if (!angleIsGlobal) {
                angleDegrees += transform.eulerAngles.z;
            }
            return new Vector3(Mathf.Cos(angleDegrees * Mathf.Deg2Rad), Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0f);
        }

        public struct ViewCastInfo {
            public bool hit;
            public Vector2 point;
            public float distance;
            public float angle;

            public ViewCastInfo(bool _hit, Vector2 _point, float _distance, float _angle) {
                hit = _hit;
                point = _point;
                distance = _distance;
                angle = _angle;
            }
        }

        public struct EdgeInfo {
            public Vector2 pointA;
            public Vector2 pointB;

            public EdgeInfo(Vector2 _pointA, Vector2 _pointB) {
                pointA = _pointA;
                pointB = _pointB;
            }
        }
    }
}
