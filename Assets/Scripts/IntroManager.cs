using UnityEngine;

public class IntroManager : MonoBehaviour
{
    [SerializeField] private GameObject mTheVideo = null;
    [SerializeField] private GameObject mIntroScreen = null;
    [SerializeField] private GameObject mMainMenu = null;

    private void Start()
    {
        Invoke(nameof(ShowIntro), 1.6f);
    }

    public void ShowIntro()
    {
        mTheVideo.SetActive(true);
        Invoke(nameof(GoToMainMenu), 20f);
    }

    public void GoToMainMenu()
    {
        mIntroScreen.SetActive(false);
        mMainMenu.SetActive(true);
    }
}
