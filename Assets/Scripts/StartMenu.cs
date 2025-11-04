using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class StartMenu : MonoBehaviour
{
    [SerializeField] GameObject optionsPanel;
    [SerializeField] AudioMixer mixer; // im Inspector zuweisen (z.B. "Master")

    public void PlayGame() {
        SceneManager.LoadScene("SampleScene"); // Zielszene anpassen
    }

    public void OpenOptions(bool open) {
        if (optionsPanel) optionsPanel.SetActive(open);
    }

    public void QuitGame() {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void SetVolume(float value) {
        // Slider 0..1 → dB
        float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        mixer.SetFloat("MasterVol", dB); // Exposed Param im Mixer
    }
}
