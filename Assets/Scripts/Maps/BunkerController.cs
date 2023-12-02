using UnityEngine;

public class BunkerController : MonoBehaviour
{
    private float mCurrentHealth;

    public float GetHealth => mCurrentHealth;

    private void Start()
    {
        mCurrentHealth = 100;
    }
}
