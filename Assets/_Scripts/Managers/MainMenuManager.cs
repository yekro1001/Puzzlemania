using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    // inspector parameters
    [Header("References")]
    [SerializeField]
    private CanvasGroup mainGroup;
    [SerializeField]
    private Image patternImage;
    [SerializeField]
    private Button startButton;
    [SerializeField]
    private Button settingsButton;
    [SerializeField]
    private Button exitButton;
    [SerializeField]
    private CanvasGroup settingsGroup;
    [SerializeField]
    private Transform playerSkinPivot;
    [SerializeField]
    private Button switchSkinButton;
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
    [SerializeField]
    private Button backFromSettingsButton;
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

    private void Start()
    {
        // initialize player prefs
        musicVolume = PlayerPrefsManager.MusicVolume; // todo implement
        playerSkin = PlayerPrefsManager.PlayerSkin;
        patternOffsetSpeed = PlayerPrefsManager.PatternOffsetSpeed;
        patternTransparency = PlayerPrefsManager.PatternTransparency;
        playerSkinRotationSpeed = PlayerPrefsManager.PlayerSkinRotationSpeed;

        // create copy of image material to prevent offsetting every image
        Material imageMaterialCopy = new(patternImage.material);
        patternImage.material = imageMaterialCopy;


        // close all submenus
        settingsGroup.alpha = 0;
        settingsGroup.blocksRaycasts = false;
        playerSkinPivot.gameObject.SetActive(false);
        mainGroup.alpha = 1;
        mainGroup.blocksRaycasts = true;

        // enable active skin
        for (int i = 0; i < playerSkinPivot.childCount; i++)
        {
            playerSkinPivot.GetChild(i).gameObject.SetActive(i == (int)playerSkin);
        }

        // assign button handlers
        startButton.onClick.AddListener(() => { });
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
            // todo change music volume
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
}
