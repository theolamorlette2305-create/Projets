using UnityEngine;

public class Mouvement : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private Rigidbody _rb;

    [Header("Paramètres de Mouvement")]
    [SerializeField] private float _speed = 5;
    [SerializeField] private float _turnSpeed = 360;

    [Header("Paramètres de Sprint")]
    [SerializeField] private float _sprintMultiplier = 2f;

    [Header("Paramètres de Saut")]
    [SerializeField] private float _jumpForce = 7f;
    private bool _isGrounded = true;

    private Vector3 _input;
    private bool _jumpRequested = false;

    private void Update()
    {
        GatherInput();
        Look();
        GatherJumpInput();
    }

    private void FixedUpdate()
    {
        Move();
        Jump();
    }

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;
        var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        float currentSpeed = _speed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= _sprintMultiplier;
        }
        _rb.MovePosition(transform.position + transform.forward * _input.normalized.magnitude * currentSpeed * Time.fixedDeltaTime);
    }

    private void GatherJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _jumpRequested = true;
        }
    }

    private void Jump()
    {
        if (_jumpRequested)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _isGrounded = false;
            _jumpRequested = false; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }
}

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}