using UnityEngine;

[CreateAssetMenu(fileName = "CameraData", menuName = "ScriptableObjects/CameraData")]
public class CameraData : ScriptableObject
{
    // INSPECTOR VARIABLES
    [Header("Camera Data for Zoom")]
    [SerializeField] private float mTopEdgeBuffer;
    [SerializeField] private float mEdgeScrollSpeed;
    [SerializeField] private float mBottomEdgeBuffer;
    [SerializeField] private float mMiniMapMaxDistance;
    [SerializeField] private float mMiniMapMinDistance;
    [SerializeField] private float mLeftRighEdgeBuffer;
    [SerializeField, Range(0.1f, 10)] private float mZoomSpeed;
    [SerializeField, Range(15, 50)] private float mMinZoomDistance;
    [SerializeField, Range(15, 120)] private float mMaxZoomDistance;
    [SerializeField, Range(0.1f, 10)] private float mMiniMapZoomSpeed;

    // GETTERS
    public float GetZoomSpeed => mZoomSpeed;
    public float GetTopEdgeBuffer => mTopEdgeBuffer;
    public float GetMaxZoomDistance => mMaxZoomDistance;
    public float GetMinZoomDistance => mMinZoomDistance;
    public float GetEdgeScrollSpeed => mEdgeScrollSpeed;
    public float GetBottomEdgeBuffer => mBottomEdgeBuffer;
    public float GetMiniMapZoomSpeed => mMiniMapZoomSpeed;
    public float GetMiniMapMaxDistance => mMiniMapMaxDistance;
    public float GetMiniMapMinDistance => mMiniMapMinDistance;
    public float GetLeftRightEdgeBuffer => mLeftRighEdgeBuffer;
}
