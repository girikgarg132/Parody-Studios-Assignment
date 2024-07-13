using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _gravity;
    [SerializeField] private float _groundDistance;
    
    private PlayerInputActions _inputActions;
    private Vector3 _velocity;
    private Vector2 _movementInput;
    private bool _jumpInput;
    private bool _isGrounded;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Move.performed += OnMove;
        _inputActions.Player.Move.canceled += OnMove;
        _inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _inputActions.Player.Move.performed -= OnMove;
        _inputActions.Player.Move.canceled -= OnMove;
        _inputActions.Player.Jump.performed -= OnJump;
        _inputActions.Player.Disable();
    }

    private void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        Vector3 move = new Vector3(_movementInput.x, 0, _movementInput.y);
        _controller.Move(move * (_speed * Time.deltaTime));

        if (_jumpInput && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            _animator.SetBool("Jumping", true);
            _jumpInput = false;
        }
        else
        {
            _animator.SetBool("Jumping", false);
        }
        
        if (!_isGrounded)
        {
            _animator.SetBool("Falling", true);
        }
        else
        {
            _animator.SetBool("Falling", false);
        }

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = -context.ReadValue<Vector2>();
        if (_movementInput == Vector2.zero)
        {
            _animator.SetBool("Running", false);
        }
        else
        {
            _animator.SetBool("Running", true);
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jumpInput = true;
        }
    }
}
