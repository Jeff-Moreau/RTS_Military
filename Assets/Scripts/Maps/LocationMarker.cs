using UnityEngine;

public class LocationMarker : MonoBehaviour
{
    private float mStayTime;
    private float mStartTime;

    private void Start()
    {
        mStayTime = 1;
        mStartTime = 0;
    }

    private void Update()
    {
        mStartTime += Time.deltaTime;

        if (mStartTime > mStayTime)
        {
            mStartTime = 0;
            gameObject.SetActive(false);
        }
    }
}
