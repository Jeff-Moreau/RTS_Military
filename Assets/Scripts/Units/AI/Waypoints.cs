using UnityEngine;

public class Waypoints : MonoBehaviour
{
    private const string WAYPOINT = "AIEnemyWaypoint ";

    [SerializeField] private float mWaypointSphereSize = 1.0f;
    [SerializeField] private Color mWaypointColor = Color.red;

    private Transform[] mWaypoints = null;

    private void OnDrawGizmos()
    {
        mWaypoints = GetComponentsInChildren<Transform>();
        var lastWaypoint = mWaypoints[mWaypoints.Length - 1].position;

        for (int i = 1; i < mWaypoints.Length; i++)
        {
            Gizmos.color = mWaypointColor;
            Gizmos.DrawSphere(mWaypoints[i].position, mWaypointSphereSize);
            Gizmos.DrawLine(lastWaypoint, mWaypoints[i].position);
            lastWaypoint = mWaypoints[i].position;

            mWaypoints[i].name = WAYPOINT + i.ToString();
        }
    }
}
