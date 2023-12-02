using UnityEngine;

public class BunkerController : MonoBehaviour
{
    private float mCurrentHealth;

    public float GetHealth => mCurrentHealth;

    public void SetHealth(float amount) => mCurrentHealth -= amount;

    private void Start()
    {
        mCurrentHealth = 1000;
    }
}
