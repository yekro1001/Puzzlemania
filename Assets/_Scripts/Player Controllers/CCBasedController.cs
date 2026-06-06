using System;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class CCBasedController : MonoBehaviour
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
    [SerializeField]
    [Tooltip("Degrees per pixel")]
    private float lookSensitivity = 0.25f;
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
                    break;
                case CameraType.TopDown:
                    thirdPersonCamera.Priority = 0;
                    topDownCamera.Priority = 1;
                    _moveRotation = Quaternion.LookRotation(new(topDownCamera.transform.up.x, 0, topDownCamera.transform.up.z));
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
    private Quaternion _moveRotation;

    private void OnEnable()
    {
        InputSystem.actions.FindActionMap("Player").FindAction("Camera").performed += SwitchCamera;
    }

    private void OnDisable()
    {
        InputSystem.actions.FindActionMap("Player").FindAction("Camera").performed -= SwitchCamera;
    }

    private void Start()
    {
        _cc = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        // trigger property's set sequence (setup camera for the first time)
        CameraType = cameraType;
    }

    private void Update()
    {
        if (CameraType == CameraType.ThirdPerson)
        {
            // rotate 2rd person camera by rotating its pivot
            Vector2 lookInput = InputSystem.actions.FindActionMap("Player").FindAction("Look").ReadValue<Vector2>();
            _moveRotation = Quaternion.LookRotation(new(thirdPersonCamera.Target.TrackingTarget.forward.x, 0, thirdPersonCamera.Target.TrackingTarget.forward.z));
            thirdPersonCamera.Target.TrackingTarget.Rotate(Vector3.up, lookSensitivity * lookInput.x);
            thirdPersonCamera.Target.TrackingTarget.Rotate(Vector3.right, lookSensitivity * -lookInput.y);
        }
        Vector2 moveIput = InputSystem.actions.FindActionMap("Player").FindAction("Move").ReadValue<Vector2>();
        if (moveIput != Vector2.zero)
        {
            // move character controller in global space
            Vector3 moveVector = new(moveIput.x, 0, moveIput.y);
            _cc.SimpleMove(_moveRotation * (moveSpeed * moveVector));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_moveRotation * moveVector), turnSpeed * Time.deltaTime);
            _animator.SetBool(runningHash, true);
        }
        else
        {
            _animator.SetBool(runningHash, false);
        }
    }

    private void SwitchCamera(InputAction.CallbackContext context)
    {
        // loop between camera types
        CameraType = (CameraType)((int)(CameraType + 1) % totalCameraTypes);
    }
}
