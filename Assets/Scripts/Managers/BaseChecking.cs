using UnityEngine;

public class BaseChecking : MonoBehaviour
{
    [SerializeField] private GameObject mRubble = null;
    [SerializeField] private ParticleSystem mExplosion = null;
    [SerializeField] private BunkerController mBunker = null;

    private void Update()
    {
        if (mBunker.GetHealth <= 0)
        {
            mBunker.gameObject.SetActive(false);
            mExplosion.gameObject.SetActive(true);
            mRubble.SetActive(true);
        }
    }
}
