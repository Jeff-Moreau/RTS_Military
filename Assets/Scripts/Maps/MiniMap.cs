using UnityEngine;

public class MiniMap : MonoBehaviour
{
    [SerializeField] private Camera mMinimapCamera = null;
    [SerializeField] private CameraData mData = null;

    private void Start()
    {
        mMinimapCamera.orthographicSize = 20;
    }

    private void Update()
    {
        if (mMinimapCamera.orthographicSize < mData.GetMiniMapMinDistance)
        {
            mMinimapCamera.orthographicSize = mData.GetMiniMapMinDistance;
        }
        else if (mMinimapCamera.orthographicSize > mData.GetMiniMapMaxDistance)
        {
            mMinimapCamera.orthographicSize = mData.GetMiniMapMaxDistance;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            mMinimapCamera.orthographicSize -= mData.GetMiniMapZoomSpeed;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            mMinimapCamera.orthographicSize += mData.GetMiniMapZoomSpeed;
        }
    }
}
