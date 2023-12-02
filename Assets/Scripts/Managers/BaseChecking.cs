using UnityEngine;

public class BaseChecking : MonoBehaviour
{
    [SerializeField] private GameObject mRubble = null;
    [SerializeField] private ParticleSystem mExplosion = null;
    [SerializeField] private BunkerController mBunker = null;

    private bool mBlownUp;

    private void Start()
    {
        mBlownUp = false;
    }

    private void Update()
    {
        
        if (mBunker.GetHealth <= 0 && !mBlownUp)
        {
            mBunker.gameObject.SetActive(false);
            Instantiate(mExplosion);
            mExplosion.gameObject.SetActive(true);
            Instantiate(mRubble);
            mRubble.SetActive(true);
            mBlownUp = true;
        }
    }
}
