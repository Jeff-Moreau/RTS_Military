using UnityEngine;

public class SpawnpointTaken : MonoBehaviour
{
    private bool mIsTaken;

    public bool GetIsTaken => mIsTaken;
    public void SetIsTaken(bool taken) => mIsTaken = taken;

    private void Start()
    {
        mIsTaken = false;
    }
}
