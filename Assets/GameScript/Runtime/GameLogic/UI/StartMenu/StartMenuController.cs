using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public sealed class StartMenuController : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject settings;
    [SerializeField] private UnityEngine.UI.Button startButton;
    [SerializeField] private UnityEngine.UI.Slider volume;
    [SerializeField] private UnityEngine.UI.Toggle fullscreen;

    public void Configure(GameObject menuRoot, GameObject settingsRoot,
        UnityEngine.UI.Button firstButton, UnityEngine.UI.Slider volumeSlider,
        UnityEngine.UI.Toggle fullscreenToggle)
    {
        menu = menuRoot;
        settings = settingsRoot;
        startButton = firstButton;
        volume = volumeSlider;
        fullscreen = fullscreenToggle;
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    private void Update()
    {
        if (settings.activeSelf && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseSettings();
    }

    public void StartGame()
    {
        startButton.interactable = false;
        SceneManager.LoadSceneAsync("TrainingGround");
    }

    public void OpenSettings()
    {
        volume.SetValueWithoutNotify(AudioListener.volume);
        fullscreen.SetIsOnWithoutNotify(Screen.fullScreen);
        menu.SetActive(false);
        settings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(volume.gameObject);
    }

    public void CloseSettings()
    {
        settings.SetActive(false);
        menu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(startButton.gameObject);
    }

    public void SetVolume(float value) => AudioListener.volume = value;
    public void SetFullscreen(bool value) => Screen.fullScreen = value;

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
