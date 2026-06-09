using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // inspector parameters
    [Header("Groups")]
    [SerializeField]
    private CanvasGroup mainGroup;
    [SerializeField]
    private CanvasGroup startGroup;
    [SerializeField]
    private CanvasGroup settingsGroup;
    [SerializeField]
    private CanvasGroup attributionsGroup;
    [Header("Buttons")]
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button attributionsButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private Button switchSkinButton;
    [SerializeField]
    private Button backFromStartButton;
    [SerializeField]
    private Button backFromSettingsButton;
    [SerializeField]
    private Button backFromAttributionsButton;
    [Header("Sliders")]
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider mouseSensitivitySlider;
    [SerializeField]
    private Slider patternOffsetSpeedXSlider;
    [SerializeField]
    private Slider patternOffsetSpeedYSlider;
    [SerializeField]
    private Slider patternTransparencySlider;
    [SerializeField]
    private Slider playerSkinRotationSpeedSlider;
    [Header("Other References")]
    [SerializeField]
    private AudioClip bgmAudio;
    [SerializeField]
    private Image patternImage;
    [SerializeField]
    private Transform levelsList;
    [SerializeField]
    private Transform playerSkinPivot;
    [Header("Settings")]
    [SerializeField]
    [Range(0, 1)]
    private float musicVolume;
    [SerializeField]
    private PlayerSkin playerSkin;
    [SerializeField]
    [Tooltip("Full shifts per second")]
    private Vector2 patternOffsetSpeed;
    [SerializeField]
    [Range(0, 1)]
    private float patternTransparency;
    [SerializeField]
    [Tooltip("Degrees per second")]
    private float playerSkinRotationSpeed;

    // readonly values
    private static readonly int totalPlayerSkins = Enum.GetValues(typeof(PlayerSkin)).Length;

    // privale vars
    private AudioSource _bgmAudioSource;

    private void Start()
    {
        // reset time scale
        Time.timeScale = 1;

        // load player prefs
        musicVolume = PlayerPrefsManager.MusicVolume;
        playerSkin = PlayerPrefsManager.PlayerSkin;
        patternOffsetSpeed = PlayerPrefsManager.PatternOffsetSpeed;
        patternTransparency = PlayerPrefsManager.PatternTransparency;
        playerSkinRotationSpeed = PlayerPrefsManager.PlayerSkinRotationSpeed;
        int levelsCompleted = PlayerPrefsManager.LevelsCompleted;

        // create copy of image material to prevent offsetting every image
        Material imageMaterialCopy = new(patternImage.material);
        patternImage.material = imageMaterialCopy;

        // close all submenus
        startGroup.alpha = 0;
        startGroup.blocksRaycasts = false;
        settingsGroup.alpha = 0;
        settingsGroup.blocksRaycasts = false;
        playerSkinPivot.gameObject.SetActive(false);
        attributionsGroup.alpha = 0;
        attributionsGroup.blocksRaycasts = false;
        mainGroup.alpha = 1;
        mainGroup.blocksRaycasts = true;

        // unlock levels
        for (int i = 0; i < levelsList.childCount; i++)
        {
            levelsList.GetChild(i).GetComponent<Selectable>().interactable = i <= levelsCompleted;
        }

        // enable active skin
        for (int i = 0; i < playerSkinPivot.childCount; i++)
        {
            playerSkinPivot.GetChild(i).gameObject.SetActive(i == (int)playerSkin);
        }

        // load bgm
        GameObject bgmAudioSource = GameObject.Find("BGM Audio Source");
        if (bgmAudioSource)
        {
            _bgmAudioSource = bgmAudioSource.GetComponent<AudioSource>();
        }
        else
        {
            _bgmAudioSource = new GameObject("BGM Audio Source").AddComponent<AudioSource>();
            _bgmAudioSource.clip = bgmAudio;
            _bgmAudioSource.loop = true;
            _bgmAudioSource.Play();
            DontDestroyOnLoad(_bgmAudioSource);
        }

        // assign button handlers
        startButton.onClick.AddListener(() =>
        {
            mainGroup.alpha = 0;
            mainGroup.blocksRaycasts = false;
            startGroup.alpha = 1;
            startGroup.blocksRaycasts = true;
        });
        backFromStartButton.onClick.AddListener(() =>
        {
            startGroup.alpha = 0;
            startGroup.blocksRaycasts = false;
            mainGroup.alpha = 1;
            mainGroup.blocksRaycasts = true;
        });
        settingsButton.onClick.AddListener(() =>
        {
            mainGroup.alpha = 0;
            mainGroup.blocksRaycasts = false;
            settingsGroup.alpha = 1;
            settingsGroup.blocksRaycasts = true;
            playerSkinPivot.gameObject.SetActive(true);
        });
        backFromSettingsButton.onClick.AddListener(() =>
        {
            settingsGroup.alpha = 0;
            settingsGroup.blocksRaycasts = false;
            playerSkinPivot.gameObject.SetActive(false);
            mainGroup.alpha = 1;
            mainGroup.blocksRaycasts = true;
        });
        attributionsButton.onClick.AddListener(() =>
        {
            mainGroup.alpha = 0;
            mainGroup.blocksRaycasts = false;
            attributionsGroup.alpha = 1;
            attributionsGroup.blocksRaycasts = true;
        });
        backFromAttributionsButton.onClick.AddListener(() =>
        {
            attributionsGroup.alpha = 0;
            attributionsGroup.blocksRaycasts = false;
            mainGroup.alpha = 1;
            mainGroup.blocksRaycasts = true;
        });
        switchSkinButton.onClick.AddListener(() =>
        {
            playerSkinPivot.GetChild((int)playerSkin).gameObject.SetActive(false);
            playerSkin = (PlayerSkin)((int)(playerSkin + 1) % totalPlayerSkins);
            PlayerPrefsManager.PlayerSkin = playerSkin;
            playerSkinPivot.GetChild((int)playerSkin).gameObject.SetActive(true);
        });
        exitButton.onClick.AddListener(() =>
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });

        // assign slider handlers
        musicVolumeSlider.onValueChanged.AddListener((value) =>
        {
            musicVolume = value;
            PlayerPrefsManager.MusicVolume = value;
            _bgmAudioSource.volume = value;
        });
        mouseSensitivitySlider.onValueChanged.AddListener((value) =>
        {
            PlayerPrefsManager.MouseSensitivity = value;
        });
        patternOffsetSpeedXSlider.onValueChanged.AddListener((value) =>
        {
            patternOffsetSpeed.x = value;
            PlayerPrefsManager.PatternOffsetSpeed = patternOffsetSpeed;
        });
        patternOffsetSpeedYSlider.onValueChanged.AddListener((value) =>
        {
            patternOffsetSpeed.y = value;
            PlayerPrefsManager.PatternOffsetSpeed = patternOffsetSpeed;
        });
        patternTransparencySlider.onValueChanged.AddListener((value) =>
        {
            patternTransparency = value;
            PlayerPrefsManager.PatternTransparency = value;
            patternImage.color = new(patternImage.color.r, patternImage.color.g, patternImage.color.b, 1 - value);
        });
        playerSkinRotationSpeedSlider.onValueChanged.AddListener((value) =>
        {
            playerSkinRotationSpeed = value;
            PlayerPrefsManager.PlayerSkinRotationSpeed = value;
        });

        // set slider values
        musicVolumeSlider.value = musicVolume;
        mouseSensitivitySlider.value = PlayerPrefsManager.MouseSensitivity;
        patternOffsetSpeedXSlider.value = patternOffsetSpeed.x;
        patternOffsetSpeedYSlider.value = patternOffsetSpeed.y;
        patternTransparencySlider.value = patternTransparency;
        playerSkinRotationSpeedSlider.value = playerSkinRotationSpeed;
    }

    private void Update()
    {
        // accummulate offset
        patternImage.material.mainTextureOffset += Time.deltaTime * patternOffsetSpeed;

        // rotate player skin pivot
        playerSkinPivot.Rotate(Vector3.up, playerSkinRotationSpeed * Time.deltaTime);
    }

    public static void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    public static void StartLevel(int level)
    {
        PlayerPrefsManager.SelectedLevel = level;
        SceneManager.LoadScene("MainGame");
    }
}
