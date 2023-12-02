using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mTimer = null;
    private float mTimeIncrease = 1;
    private float mTimeElapsed;

    void Update()
    {
        if (mTimeElapsed >= 0)
        {
            var totalTime = mTimeElapsed + mTimeIncrease * Time.time;

            var minutes = Mathf.FloorToInt(totalTime / 60);
            var seconds = Mathf.FloorToInt(totalTime % 60);

            mTimer.text = string.Format("{0:00} : {1:00}", minutes, seconds); 
        }
    }
}
