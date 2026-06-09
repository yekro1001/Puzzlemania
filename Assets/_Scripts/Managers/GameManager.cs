using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    // inspector parameters
    [Header("Arrays")]
    [SerializeField]
    private PlayerController[] playerSkins;
    [Header("Buttons")]
    [SerializeField]
    private Button pauseButton;
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button returnToMenuButton;
    [Header("Sliders")]
    [SerializeField]
    private Slider musicVolumeSlider;
    [SerializeField]
    private Slider mouseSensitivitySlider;
    [Header("Other references")]
    [SerializeField]
    private PositionConstraint cameraPivot;
    [SerializeField]
    private CanvasGroup pauseGroup;
    [SerializeField]
    private TMP_Text skipCutsceneHint;

    // private vars
    private AudioSource _bgmAudioSource;
    private PlayerController _player;
    private InputActionMap _playerActionMap;
    private InputActionMap _uiActionMap;
    private InputAction _pauseAction;

    private void OnEnable()
    {
        // get maps
        _playerActionMap = InputSystem.actions.FindActionMap("Player");
        _uiActionMap = InputSystem.actions.FindActionMap("UI");
        _uiActionMap.Enable();

        // get pause action
        _pauseAction = _uiActionMap.FindAction("Pause");
        _pauseAction.performed += TogglePauseMenu;

        // make cutscene skippable
        InputSystem.onAnyButtonPress.CallOnce(control =>
        {
            PlayableDirector director = FindAnyObjectByType<PlayableDirector>();
            director.time = director.playableAsset.duration;
            director.Evaluate();
        });
    }

    private void OnDisable()
    {
        _pauseAction.performed -= TogglePauseMenu;
    }

    private void Start()
    {
        // reset time scale
        Time.timeScale = 1;

        // load player prefs
        float musicVolume = PlayerPrefsManager.MusicVolume;
        float mouseSensitivity = PlayerPrefsManager.MouseSensitivity;
        int playerSkin = (int)PlayerPrefsManager.PlayerSkin;
        int selectedLevel = PlayerPrefsManager.SelectedLevel; // todo use

        // close pause menu
        pauseGroup.alpha = 0;
        pauseGroup.blocksRaycasts = false;

        // activate selected skin
        for (int i = 0; i < playerSkins.Length; i++)
        {
            playerSkins[i].gameObject.SetActive(i == playerSkin);
        }

        // set private refs
        _player = playerSkins[playerSkin];
        cameraPivot.AddSource(new()
        {
            sourceTransform = _player.transform,
            weight = 1
        });
        _bgmAudioSource = GameObject.Find("BGM Audio Source").GetComponent<AudioSource>();

        // assign button handlers
        pauseButton.onClick.AddListener(() =>
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            pauseGroup.alpha = 1;
            pauseGroup.blocksRaycasts = true;
        });
        resumeButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            if (_player.CameraType == CameraType.ThirdPerson)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            pauseGroup.alpha = 0;
            pauseGroup.blocksRaycasts = false;
        });
        returnToMenuButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

        // assign sliders handlers
        musicVolumeSlider.onValueChanged.AddListener((value) =>
        {
            PlayerPrefsManager.MusicVolume = value;
            _bgmAudioSource.volume = value;
        });
        mouseSensitivitySlider.onValueChanged.AddListener((value) =>
        {
            PlayerPrefsManager.MouseSensitivity = value;
            _player.lookSensitivity = value;
        });

        // set slider values
        musicVolumeSlider.value = musicVolume;
        mouseSensitivitySlider.value = mouseSensitivity;
    }

    private void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            pauseGroup.alpha = 1;
            pauseGroup.blocksRaycasts = true;
        }
        else
        {
            Time.timeScale = 1;
            if (_player.CameraType == CameraType.ThirdPerson)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            pauseGroup.alpha = 0;
            pauseGroup.blocksRaycasts = false;
        }
    }

    public void TogglePlayerMap(bool enable)
    {
        if (enable)
        {
            _playerActionMap.Enable();
            skipCutsceneHint.alpha = 0;
        }
        else
        {
            _playerActionMap.Disable();
            skipCutsceneHint.alpha = 1;
        }
    }
}
