using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuController : MonoBehaviour
{
    public GameObject AudioSlider;    // Slider để điều chỉnh âm thanh
    public GameObject PlayButton;     // Nút Play
    public GameObject GameplayButton; // Nút Gameplay
    public GameObject QuitButton;     // Nút Thoát
    public GameObject SettingButton;  // Nút Setting
    public GameObject BackButtonSetting;
    public GameObject BackButtonGamePlay;// Nút Back
    public GameObject BackButtonLaw;
    public AudioSource audioSource; // Drag your AudioSource component here  
    public Slider volumeSlider;
    public Slider pitchSlider;
    void Start()
    {
        // Set initial values of sliders  
        volumeSlider.value = audioSource.volume;
        pitchSlider.value = audioSource.pitch;

        // Add listeners to sliders to call the corresponding methods when the value changes  
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        pitchSlider.onValueChanged.AddListener(OnPitchChanged);
    }

    void OnVolumeChanged(float value)
    {
        audioSource.volume = value;
    }

    void OnPitchChanged(float value)
    {
        audioSource.pitch = value;
    }

    public void ShowSettings()
    {
        // Hiển thị giao diện Settings
        AudioSlider.SetActive(true);
        PlayButton.SetActive(false);
        GameplayButton.SetActive(false);
        QuitButton.SetActive(false);
        SettingButton.SetActive(false);
    }

    public void HideSettings()
    {
        // Ẩn giao diện Settings và quay lại menu chính
        AudioSlider.SetActive(false);
        PlayButton.SetActive(true);
        GameplayButton.SetActive(true);
        QuitButton.SetActive(true);
        SettingButton.SetActive(true);
    }

    public void ShowGamePlay()
    {
        BackButtonLaw.SetActive(true);
        PlayButton.SetActive(false);
        GameplayButton.SetActive(false);
        QuitButton.SetActive(false);
        SettingButton.SetActive(false);
    }

    public void HideGamePlay()
    {
        BackButtonLaw.SetActive(false);
        PlayButton.SetActive(true);
        GameplayButton.SetActive(true);
        QuitButton.SetActive(true);
        SettingButton.SetActive(true);
    }

    public void Quit()
    {
            Debug.Log("Quit button clicked!");
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Dừng Play Mode
    #else
        Application.Quit(); // Thoát ứng dụng
    #endif
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("PlayGame");
    }
}

