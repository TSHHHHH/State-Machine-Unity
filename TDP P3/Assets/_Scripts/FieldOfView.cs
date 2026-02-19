using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("References")]
    public EnemyStats enemyStats;

    public float viewRadius;

    [Range(0, 360)]
    public float viewAngle;

    public float closeViewRadius;
    public float closeViewPuffSize;

    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public LayerMask coverMask;

    [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
    [HideInInspector] public List<Transform> visibleCovers = new List<Transform>();

    [Header("FOV Mesh Settings")]
    public float meshResolution;
    public MeshFilter viewMeshFilterCone;
    private Mesh viewMesh;

    public float closeViewMeshResolution;
    public MeshFilter viewMeshFilterCloseView;
    private Mesh viewMeshCloseView;

    public int edgeResolveIterations;
    public float edgeDstThreshold;

    [Header("Debug Settings")]
    public bool isDebugMode = false;

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float dst, float angle)
        {
            this.hit = hit;
            this.point = point;
            this.dst = dst;
            this.angle = angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 pointA, Vector3 pointB)
        {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    private void Start()
    {
        viewMesh = new Mesh
        {
            name = "View Mesh Cone"
        };
        viewMeshFilterCone.mesh = viewMesh;

        viewMeshCloseView = new Mesh
        {
            name = "View Mesh Close View"
        };
        viewMeshFilterCloseView.mesh = viewMeshCloseView;

        StartCoroutine(FindTargetWithDelay(0.2f));
    }

    private IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            // FindVisibleCovers();
        }
    }

    private void LateUpdate()
    {
        DrawFOV();
        DrawFOVCloseView();
    }

    private void FindVisibleTargets()
    {
        // Clear the list of visible targets
        visibleTargets.Clear();

        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, enemyStats.viewDistance, enemyStats.targetMask);

        foreach (Collider2D target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;

            // Check if the target is within the view angle or within the close view radius
            if (Vector3.Angle(transform.up, dirToTarget) < enemyStats.viewAngle / 2 || IsWithinCloseView(targetTransform))
            {
                // Check if the target is within the view radius
                float dstToTarget = Vector3.Distance(transform.position, targetTransform.position);

                // Check if the target is not obstructed by any obstacles
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, enemyStats.obstacleMask))
                {
                    visibleTargets.Add(targetTransform);
                }
            }
        }
    }

    private void FindVisibleCovers()
    {
        // Clear the list of visible covers
        visibleCovers.Clear();

        Collider2D[] coversInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, coverMask);

        foreach (Collider2D cover in coversInViewRadius)
        {
            Transform coverTransform = cover.transform;
            Vector3 dirToCover = (coverTransform.position - transform.position).normalized;

            // Check if the cover is within the view angle or within the close view radius
            if (Vector3.Angle(transform.up, dirToCover) < viewAngle / 2 || IsWithinCloseView(coverTransform))
            {
                // Check if the cover is within the view radius
                float dstToCover = Vector3.Distance(transform.position, coverTransform.position);

                // Check if the cover is not obstructed by any obstacles
                if (!Physics2D.Raycast(transform.position, dirToCover, dstToCover, obstacleMask))
                {
                    visibleCovers.Add(coverTransform);
                }
            }
        }
    }

    private bool IsWithinCloseView(Transform targetTransform)
    {
        // Compute the center of the close view radius
        Vector3 closeViewCenter = transform.position + transform.up * enemyStats.closeViewDistance;

        // Check if the target is within the close view radius
        float dstToTarget = Vector3.Distance(closeViewCenter, targetTransform.position);

        return dstToTarget <= enemyStats.closeViewDistance + enemyStats.closeViewPuffSize;
    }

    private void DrawFOV()
    {
        if (!isDebugMode)
            return;

        int rayCount = Mathf.RoundToInt(enemyStats.viewAngle * meshResolution);
        float angleStep = enemyStats.viewAngle / rayCount;

        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.z - enemyStats.viewAngle / 2 + angleStep * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        // Compute the number of vertices required to draw the mesh
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; ++i)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // Debug draw the view points
        foreach (var vp in viewPoints)
        {
            Debug.DrawLine(transform.position, vp, Color.black);
        }

        // Update the mesh
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    private void DrawFOVCloseView()
    {
        if (!isDebugMode)
            return;

        //float angleStep = 360f / closeViewMeshResolution;
        //int rayCount = Mathf.RoundToInt(closeViewMeshResolution);

        // Since the close view mesh is a circle, we can use the circumference formula to compute the number of rays
        int rayCount = Mathf.RoundToInt(360f * closeViewMeshResolution);
        float angleStep = 360f / rayCount;

        Vector2 agentPos = transform.position;
        Vector2 closeViewCenter = agentPos + (Vector2)transform.up * enemyStats.closeViewDistance;

        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i < rayCount; ++i)
        {
            float angle = i * angleStep;
            ViewCastInfo newViewCast = ViewCastCloseView(angle, closeViewCenter);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdgeCloseView(oldViewCast, newViewCast, closeViewCenter);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        // Compute the number of vertices required to draw the mesh
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; ++i)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // Debug draw the view points
        foreach (var vp in viewPoints)
        {
            Debug.DrawLine(transform.position, vp, Color.cyan);
        }

        // Update the mesh
        viewMeshCloseView.Clear();
        viewMeshCloseView.vertices = vertices;
        viewMeshCloseView.triangles = triangles;
        viewMeshCloseView.RecalculateNormals();
    }

    private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = minViewCast.point;
        Vector3 maxPoint = maxViewCast.point;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > 0.1f;

            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private EdgeInfo FindEdgeCloseView(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, Vector2 closeViewCenter)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = minViewCast.point;
        Vector3 maxPoint = maxViewCast.point;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCastCloseView(angle, closeViewCenter);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > 0.1f;

            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    private ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, enemyStats.viewDistance, enemyStats.obstacleMask);
        if (hit.collider != null)
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * enemyStats.viewDistance, enemyStats.viewDistance, globalAngle);
        }
    }

    private ViewCastInfo ViewCastCloseView(float globalAngle, Vector2 closeViewCenter)
    {
        Vector2 dir = DirFromAngle(globalAngle, false);
        Vector2 endPos = closeViewCenter + dir * (enemyStats.closeViewDistance + enemyStats.closeViewPuffSize);

        Vector2 agentPos = transform.position;

        Vector2 agentToEndPosDir = (endPos - agentPos).normalized;
        float distanceToEndPos = Vector2.Distance(agentPos, endPos);

        RaycastHit2D hit = Physics2D.Raycast(agentPos, agentToEndPosDir, distanceToEndPos, enemyStats.obstacleMask);
        if (hit.collider != null)
        {
            //Debug.DrawLine(agentPos, hit.point, Color.cyan);

            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            //Debug.DrawLine(agentPos, endPos, Color.cyan);

            return new ViewCastInfo(false, endPos, distanceToEndPos, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }

        return new Vector3(-Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0);
    }

    public bool IsTargetInFieldOfView(Vector3 transformPosition)
    {
        return visibleTargets.Exists(t => t.position == transformPosition);
    }

    //public bool IsPostInFieldOfView(Vector3 transformPosition)
    //{
    //  return visibleCovers.Exists(t => t.position == transformPosition);
    //}
}