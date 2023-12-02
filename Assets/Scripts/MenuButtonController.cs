using UnityEngine;

public class MenuButtonController : MonoBehaviour
{
    [SerializeField] private int mIndex;
    [SerializeField] bool mKeyDown;
    [SerializeField] int mMaxIndex;
    [SerializeField] GameObject mThisScreen;
    [SerializeField] GameObject mManagers;
    [SerializeField] GameObject mTerrain;
    [SerializeField] GameObject mHUD;
    [SerializeField] GameObject mObjectPools;

    private void Update()
    {
        if(Input.GetAxis ("Vertical") !=0)
        {
            if(!mKeyDown)
            {
                if(Input.GetAxis ("Vertical") < 0)
                {
                    if(mIndex < mMaxIndex)
                    {
                        mIndex++;
                    }
                    else
                    {
                        mIndex = 0;
                    }
                   
                }
                else if(Input.GetAxis ("Vertical") > 0)
                {
                    if (mIndex > 0)
                    {
                        mIndex--;
                    }
                    else
                    {
                        mIndex = mMaxIndex;
                    }
                }
                mKeyDown = true;
            }
        }
        else
        {
            mKeyDown = false;
        }
    }

    public void PlayGame()
    {
        mThisScreen.SetActive(false);
        mObjectPools.SetActive(true);
        mManagers.SetActive(true);
        mTerrain.SetActive(true);
        mHUD.SetActive(true);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
