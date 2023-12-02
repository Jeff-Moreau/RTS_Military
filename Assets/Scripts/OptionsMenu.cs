using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    /*[SerializeField] private AudioMixer audioMixer;
    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }*/

    [SerializeField] private AudioMixer mMasterAudio;
    [SerializeField] private Slider mMusicSlider;
    [SerializeField] private Slider mSoundFXSlider;
    [SerializeField] private Slider mMasterSlider;

    public void AdjustMasterVolume()
    {
        float volume = mMasterSlider.value;
        mMasterAudio.SetFloat("MasterAudio", Mathf.Log10(volume) * 20);
    }

    public void AdjustMusicVolume()
    {
        float volume = mMusicSlider.value;
        mMasterAudio.SetFloat("MasterMusic", Mathf.Log10(volume) * 20);
    }

    public void AdjustSoundVolume()
    {
        float volume = mSoundFXSlider.value;
        mMasterAudio.SetFloat("MasterSoundFX", Mathf.Log10(volume) * 20);
    }
}