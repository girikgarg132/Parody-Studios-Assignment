using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] InputActionReference _lookAction;
    [SerializeField] float _rotationSpeed = 1.5f;
    [SerializeField] float _minVerticalAngle = -30f;
    [SerializeField] float _maxVerticalAngle = 60f;
    [SerializeField] float _distance = 5f;
    [SerializeField] float _height = 2f;

    private Vector2 _lookInput;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void OnEnable()
    {
        _lookAction.action.Enable();
        _lookAction.action.performed += OnLookPerformed;
    }

    void OnDisable()
    {
        _lookAction.action.Disable();
        _lookAction.action.performed -= OnLookPerformed;
    }

    void OnLookPerformed(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
    }

    void LateUpdate()
    {
        if (!_player)
            return;

        Vector3 playerPos = _player.position + _player.parent.up * _height;
        transform.position = playerPos - Quaternion.Euler(_lookInput.y, _lookInput.x, 0) * _player.parent.forward * _distance;
        transform.LookAt(_player);
    }
}