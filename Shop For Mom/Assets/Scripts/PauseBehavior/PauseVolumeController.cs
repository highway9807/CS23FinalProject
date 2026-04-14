using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class PauseVolumeController : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private string parameterName = "MasterVolume";
    private const string PrefKey = "MasterVolume01";
    private void Start()
    {
        float saved = PlayerPrefs.GetFloat(PrefKey, 1f);
        volumeSlider.value = saved;
        ApplyVolume(saved);
        volumeSlider.onValueChanged.AddListener(OnSliderChanged);
    }
    private void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(OnSliderChanged);
    }
    private void OnSliderChanged(float value01)
    {
        ApplyVolume(value01);
        PlayerPrefs.SetFloat(PrefKey, value01);
    }
    private void ApplyVolume(float value01)
    {
        // Convert 0..1 slider to decibels (avoid log(0))
        float dB = Mathf.Log10(Mathf.Max(value01, 0.0001f)) * 20f;
        mixer.SetFloat(parameterName, dB);
    }
}