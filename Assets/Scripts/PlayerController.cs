using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private CharacterController _controller;
    private Animator _animator;

    private InputAction _moveAction;
    public Vector2 _moveValue;
    private InputAction _jumpAction;

    [SerializeField] float _moveSpeed = 5;

    [SerializeField] Transform _sensorPosition;
    [SerializeField] float _sensorRadius;
    [SerializeField] LayerMask _groundMask;

    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] Vector3 _playerGravity;
    private float _jumpForce = 4;

    public Transform cameraMain;

    public float tiltAngle = 5;
    public float smooth = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _jumpAction = InputSystem.actions["Jump"];
    }

    // Update is called once per frame
    void Update()
    {
        _moveValue = _moveAction.ReadValue<Vector2>();

        if(_jumpAction.WasPressedThisFrame() && IsGrounded())
        {
            Jump();
            Debug.Log("Estas Saltando");
        }
        
        Movement();

        Gravity();
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_moveValue.x, 0, _moveValue.y);

        _animator.SetFloat("Horizontal", _moveValue.x);
        _animator.SetFloat("Vertical", _moveValue.y);

        Vector3 cameraDirection = Quaternion.Euler(0, cameraMain.eulerAngles.y, 0) * direction;
        
        //transform.rotation = Quaternion.Euler(0, cameraDirection.y, 0);

        if(direction != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg * smooth;
            Quaternion.Euler(0, cameraDirection.y * targetAngle, 0);
            _controller.Move(cameraDirection * _moveSpeed * Time.deltaTime);
        }    
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundMask);
    }

    void Jump()
    {
        _animator.SetBool("IsJumping", true);
        _playerGravity.y = Mathf.Sqrt(_jumpForce * -2 * _gravity);
        _controller.Move(_playerGravity * Time.deltaTime);
        
    }

    void Gravity()
    {
        if(!IsGrounded())
        {
            
            _playerGravity.y += _gravity * Time.deltaTime;

            _controller.Move(_playerGravity * Time.deltaTime);
        }
        else if(IsGrounded())
        {
            _animator.SetBool("IsJumping", false);
            _playerGravity.y = -2;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_sensorPosition.position, _sensorRadius);
    }
}
