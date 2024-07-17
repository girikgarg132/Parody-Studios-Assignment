using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private List<GameObject> _cubes;
    [SerializeField] private Animator _animator;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _hologram;
    [SerializeField] private Transform _mesh;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _gravity;
    [SerializeField] private float _groundDistance;
    [SerializeField] private float _turnTime;
    

    private PlayerInputActions _inputActions;
    private Vector2 _movementInput;
    private float _turnVelocity;
    private bool _jumpInput;
    private bool _isGrounded;

    private void Start()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Player.Jump.performed += OnJump;
        _inputActions.Player.GravityManipulation.performed += OnGravityChange;
        _inputActions.Player.GravityConfirm.performed += OnGravityChangeConfirmed;
        _inputActions.Player.GravityDeny.performed += OnCancelHologram;
    }

    private void OnEnable()
    {
        if (_inputActions != null)
        {
            _inputActions.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if (_inputActions != null)
        {
            _inputActions.Player.Disable();
        }
    }

    private void FixedUpdate()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        if (_movementInput != Vector2.zero && _isGrounded)
        { 
            Vector3 direction = (transform.forward * _movementInput.y + transform.right * _movementInput.x).normalized;
            _rigidbody.MovePosition(_rigidbody.position + direction * (_speed * Time.deltaTime));
            Vector3 rotationDirection = new Vector3(_movementInput.x, 0f, _movementInput.y).normalized;
            
            float targetAngle = Mathf.Atan2(rotationDirection.x, rotationDirection.z) * Mathf.Rad2Deg;
                
            _mesh.localRotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
        
        _animator.SetBool("Falling", !_isGrounded);
        _animator.SetBool("Running", _movementInput != Vector2.zero && _isGrounded);
        _animator.SetBool("Jumping", _jumpInput && _isGrounded);

        if (_jumpInput && _isGrounded)
        {
            _rigidbody.AddForce(transform.up * _jumpPower, ForceMode.Impulse);
            _jumpInput = false;
        }
        
        _rigidbody.AddForce(-transform.up * _gravity, ForceMode.Acceleration);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Boundry"))
        {
            FindObjectOfType<UI>().GameOver(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Collectible")) return;
        _cubes.Remove(other.gameObject);
        Destroy(other.gameObject);
        if (_cubes.Count == 0)
        {
            FindObjectOfType<UI>().GameOver(true);
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jumpInput = true;
        }
    }

    void OnGravityChange(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (input == Vector2.zero) return;

        _hologram.gameObject.SetActive(true);
        _hologram.localRotation = Quaternion.Euler(new Vector3(input.y, 0f, input.x) * 90f);
    }

    void OnGravityChangeConfirmed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!_hologram.gameObject.activeInHierarchy) return;
        _hologram.gameObject.SetActive(false);
        _rigidbody.rotation = _hologram.rotation;
    }

    void OnCancelHologram(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        _hologram.gameObject.SetActive(false);
    }
}
