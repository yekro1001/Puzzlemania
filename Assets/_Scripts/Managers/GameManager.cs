using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Playables;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // inspector parameters
    [Header("Arrays")]
    [SerializeField]
    private PlayerController[] playerSkins;
    [SerializeField]
    private GameObject[] levels;
    [Header("Buttons")]
    [SerializeField]
    private Button pauseButton;
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button returnToMenuButton;
    [SerializeField]
    private Button returnFromWinScreenButton;
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
    private CanvasGroup winGroup;
    [SerializeField]
    private TMP_Text skipCutsceneHint;
    [SerializeField]
    private CanvasGroup blackScreen;
    [SerializeField]
    private TMP_Text theNextDayText;
    [Header("Settings")]
    [SerializeField]
    private float transitionDuration = 1;

    // private vars
    private AudioSource _bgmAudioSource;
    private PlayerController _player;
    private InputActionMap _playerActionMap;
    private InputActionMap _uiActionMap;
    private InputAction _pauseAction;
    private Vector3 _spawnPoint;

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
        int selectedLevel = PlayerPrefsManager.SelectedLevel;

        // close all menus
        pauseGroup.alpha = 0;
        pauseGroup.blocksRaycasts = false;
        winGroup.alpha = 0;
        winGroup.blocksRaycasts = false;
        blackScreen.alpha = 0;
        theNextDayText.alpha = 0;

        // activate selected skin
        for (int i = 0; i < playerSkins.Length; i++)
        {
            playerSkins[i].gameObject.SetActive(i == playerSkin);
        }

        // load seleccted level
        GameObject levelInstance = Instantiate(levels[selectedLevel]);
        levelInstance.name = "Level";

        // set private refs
        _player = playerSkins[playerSkin];
        cameraPivot.AddSource(new()
        {
            sourceTransform = _player.transform,
            weight = 1
        });
        _spawnPoint = _player.transform.position;
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
        returnFromWinScreenButton.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));

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

    public void CompleteLevel()
    {
        int completedLevels = PlayerPrefsManager.LevelsCompleted;
        int selectedLevel = ++PlayerPrefsManager.SelectedLevel;
        if (selectedLevel > completedLevels)
        {
            PlayerPrefsManager.LevelsCompleted = selectedLevel;
        }
        if (selectedLevel >= levels.Length)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            winGroup.alpha = 1;
            winGroup.blocksRaycasts = true;
        }
        else
        {
            StartCoroutine(nameof(TransitionCoroutine));
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

    private IEnumerator TransitionCoroutine()
    {
        _playerActionMap.Disable();
        _uiActionMap.Disable();
        Time.timeScale = 0;
        float halfDuration = transitionDuration * 0.5f;
        float t = 0;
        for (; t < halfDuration; t += Time.unscaledDeltaTime)
        {
            blackScreen.alpha = t / (halfDuration);
            yield return null;
        }
        theNextDayText.alpha = 1;
        for (; t < transitionDuration; t += Time.unscaledDeltaTime)
        {
            blackScreen.alpha = 1 - (t - halfDuration) / (halfDuration);
            yield return null;
        }
        blackScreen.alpha = 0;
        Destroy(GameObject.Find("Level"));
        GameObject levelInstance = Instantiate(levels[PlayerPrefsManager.SelectedLevel]);
        levelInstance.name = "Level";
        _player.transform.position = _spawnPoint;
        _playerActionMap.Enable();
        _uiActionMap.Enable();
        Time.timeScale = 1;
        for (; t < transitionDuration + halfDuration; t += Time.unscaledDeltaTime)
        {
            theNextDayText.alpha = 1 - (t - transitionDuration) / (halfDuration);
            yield return null;
        }
        theNextDayText.alpha = 0;
    }
}
