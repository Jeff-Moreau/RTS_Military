using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaynightCycle : MonoBehaviour
{
    [SerializeField] private Light mSun;
    [SerializeField, Range(4.5f, 18.5f)] private float mTimeOfDay;
    [SerializeField] private float mSpeedRotation;

    [Header("SkyColour")]
    [SerializeField] private Gradient mSkyColour;
    [SerializeField] private Gradient mEquantorColour;
    [SerializeField] private Gradient mSunColour;


    private void OnValidate()
    {
        UpdateSunRotation();
        UpdateLight();
    }

    private void UpdateSunRotation()
    {
        float SunRotation = Mathf.Lerp(-90, 270, mTimeOfDay / 20);
        mSun.transform.rotation = Quaternion.Euler(SunRotation, mSun.transform.rotation.y, mSun.transform.rotation.z);

    }

    private void UpdateLight()
    {
        float timeFraction = mTimeOfDay / 20;
        RenderSettings.ambientEquatorColor = mEquantorColour.Evaluate(timeFraction);
        RenderSettings.ambientSkyColor = mSkyColour.Evaluate(timeFraction);
        mSun.color = mSunColour.Evaluate(timeFraction);

    }
    void Update()
    {
        mTimeOfDay += Time.deltaTime * mSpeedRotation;
        if(mTimeOfDay > 20)
        {
            mTimeOfDay = 4;
        }

        UpdateSunRotation();
        UpdateLight();

    }
}
