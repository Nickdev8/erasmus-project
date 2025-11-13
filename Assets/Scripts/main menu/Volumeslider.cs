using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volumeslider : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;

    public void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            Loadvolume();
        }
        else
        {
            setmusicvolume();
        }
    }

    public void setmusicvolume()
    {
        float volume = volumeSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    private void Loadvolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        setmusicvolume();

    }
}
