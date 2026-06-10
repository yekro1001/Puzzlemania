using System;
using UnityEngine;
using UnityEngine.AI;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class PlayerController : MonoBehaviour
{
    // inspector parameters
    [Header("References")]
    [SerializeField]
    private CinemachineCamera thirdPersonCamera;
    [SerializeField]
    private CinemachineCamera topDownCamera;
    [Header("Movement settings")]
    [SerializeField]
    [Tooltip("Meters per second")]
    private float moveSpeed = 3;
    [Tooltip("Degrees per pixel")]
    public float lookSensitivity = 0.25f;
    [SerializeField]
    [Tooltip("Degrees per second")]
    private float turnSpeed = 360;
    [SerializeField]
    private CameraType cameraType;

    // properties
    public CameraType CameraType
    {
        get => cameraType;
        set
        {
            switch (value)
            {
                // switch camera and global move rotation
                case CameraType.ThirdPerson:
                    topDownCamera.Priority = 0;
                    thirdPersonCamera.Priority = 1;
                    _moveRotation = Quaternion.LookRotation(new(thirdPersonCamera.Target.TrackingTarget.forward.x, 0, thirdPersonCamera.Target.TrackingTarget.forward.z));
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case CameraType.TopDown:
                    thirdPersonCamera.Priority = 0;
                    topDownCamera.Priority = 1;
                    _moveRotation = Quaternion.LookRotation(new(topDownCamera.transform.up.x, 0, topDownCamera.transform.up.z));
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }
            cameraType = value;
        }
    }

    // readonly values
    private static readonly int runningHash = Animator.StringToHash("running");
    private static readonly int totalCameraTypes = Enum.GetValues(typeof(CameraType)).Length;

    // private vars
    private CharacterController _cc;
    private Animator _animator;
    private NavMeshAgent _agent;
    private Quaternion _moveRotation;
    private InputActionMap _playerActionMap;
    private InputAction _lookAction;
    private InputAction _moveAction;
    private InputAction _cameraAction;
    private Vector3 _spawnPoint;

    private void OnEnable()
    {
        // get actions
        _playerActionMap = InputSystem.actions.FindActionMap("Player");
        _lookAction = _playerActionMap.FindAction("Look");
        _moveAction = _playerActionMap.FindAction("Move");
        _cameraAction = _playerActionMap.FindAction("Camera");
        _cameraAction.performed += SwitchCamera;
    }

    private void OnDisable()
    {
        _cameraAction.performed -= SwitchCamera;
    }

    private void Start()
    {
        // set private refs
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.enabled = false;
        _spawnPoint = transform.position;

        // trigger property's set sequence (setup camera for the first time)
        CameraType = cameraType;
    }

    private void Update()
    {
        if (CameraType == CameraType.ThirdPerson && Time.timeScale > 0)
        {
            // rotate 2rd person camera by rotating its pivot
            Vector2 lookInput = _lookAction.ReadValue<Vector2>();
            _moveRotation = Quaternion.LookRotation(new(thirdPersonCamera.Target.TrackingTarget.forward.x, 0, thirdPersonCamera.Target.TrackingTarget.forward.z));
            thirdPersonCamera.Target.TrackingTarget.Rotate(Vector3.up, lookSensitivity * lookInput.x);
            thirdPersonCamera.Target.TrackingTarget.Rotate(Vector3.right, lookSensitivity * -lookInput.y);
        }
        Vector2 moveIput = _moveAction.ReadValue<Vector2>();
        if (moveIput != Vector2.zero)
        {
            // move character controller in global space
            Vector3 moveVector = new(moveIput.x, 0, moveIput.y);
            _cc.SimpleMove(_moveRotation * (moveSpeed * moveVector));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_moveRotation * moveVector), turnSpeed * Time.deltaTime);
            _animator.SetBool(runningHash, true);
        }
        else if (!_agent.enabled)
        {
            _animator.SetBool(runningHash, false);
        }
    }

    public void GetAttacked()
    {
        StartCoroutine(nameof(GoBackCoroutine));
    }

    private void SwitchCamera(InputAction.CallbackContext context)
    {
        // loop between camera types
        CameraType = (CameraType)((int)(CameraType + 1) % totalCameraTypes);
    }

    private IEnumerator GoBackCoroutine()
    {
        _agent.enabled = true;
        _playerActionMap.Disable();
        _agent.SetDestination(_spawnPoint);
        _animator.SetBool(runningHash, true);
        yield return new WaitUntil(() => !_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance);
        _agent.enabled = false;
        _playerActionMap.Enable();
        _animator.SetBool(runningHash, false);
    }
}
