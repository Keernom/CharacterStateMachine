using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateMachine : MonoBehaviour
{
    public StateMachine MovementSM;
    public StandingState Standing; 
    public RunningState Running;
    public JumpingState Jumping; 
    public FallingState Falling;
    public ClimbingState Climbing;
    public DashState Dashing;
    public WallJumpState WallJumping;
    public DamagedState Damaged;

    [HideInInspector] public float MoveInput;
    [SerializeField] public Vector2 Velocity;           // Hide in inspector

    Transform _playerTransform;
    BoxCollider2D _playerCollider;
    Collider2D _ignorePlatform;

    Vector2 _UpDownOverlapSize;
    Vector2 _LeftRightOverlapSize;

    private float _takenDamage = 0;
    float _currentJumpIncreasingTime;
    float _currentInvincibilityTime;
    int _dir = 1;

    [Header("Layers Setting")]
    [SerializeField] LayerMask _obstacleLayer;
    [SerializeField] LayerMask _platformLayer;
    [SerializeField] LayerMask _damagableLayer;

    [Header("Global Params")]
    [SerializeField] float _speed;
    [SerializeField] float _mass;
    [SerializeField] int _dashCount;
    [SerializeField] float _dashPower;
    [SerializeField] float _defaultDashTime;

    [Header("Damaged Params")]
    [SerializeField] Vector2 _damagedKnockback;
    [SerializeField] float _damagedStateDuration = 1f;
    [SerializeField] float _invincibilityTime = .5f;

    [Header("Wall Interaction")]
    [SerializeField] Vector2 _wallJumpPower;
    [SerializeField] float _wallJumpTime;
    [SerializeField] float _climbingGravity;
    [SerializeField] float _climbingDownSpeed;

    [Header("Ground State Params")]
    [SerializeField] float _walkAcceleration;
    [SerializeField] float _groundDeceleration;

    [Header("Air State Params")]
    [SerializeField] float _jumpHeight;
    [SerializeField] float _jumpCount;
    [SerializeField] float _airAcceleration;
    [SerializeField] float _jumpIncreasingTime = 1;

    [Header("Debug")]
    [SerializeField] float _jumpsRemain;
    [SerializeField] float _dashRemain;

    #region Properties

    public Transform PlayerTransform { get { return _playerTransform; } }
    public BoxCollider2D PlayerCollider { get { return _playerCollider; } }

    public LayerMask ObstacleLayer { get { return _obstacleLayer; } }
    public LayerMask PlatformLayer { get { return _platformLayer; } }
    public LayerMask DamagableLayer { get { return _damagableLayer; } }

    public int Direction { get { return _dir; } }
    public float Speed { get { return _speed; } }
    public float Mass { get { return _mass; } }
    public float DashPower { get { return _dashPower; } }
    public float DashTime { get { return _defaultDashTime; } }
    public float WalkAcceleration { get { return _walkAcceleration; } }
    public float GroundDeceleration { get { return _groundDeceleration; } }
    public float JumpHeight { get { return _jumpHeight; } }
    public float JumpCount { get { return _jumpCount; } }
    public float AirAcceleration { get { return _airAcceleration; } }
    public float JumpIncreasingTime { get { return _jumpIncreasingTime; } }
    public Vector2 WallJumpPower { get { return _wallJumpPower; } }
    public float WallJUmpTime { get { return _wallJumpTime; } }
    public float ClimbingGravity { get { return _climbingGravity; } }
    public float ClimbingDownSpeed { get { return _climbingDownSpeed; } }
    public float DamagedStateDuration { get { return _damagedStateDuration; } }
    public Vector2 DamagedKnockback { get { return _damagedKnockback; } }
    public float InvincibilityTime { get { return _invincibilityTime; } }

    public float TakenDamage 
    { 
        get { return _takenDamage; } 
        set { _takenDamage = value; }
    }

    public Collider2D IgnorePlatform 
    { 
        get { return _ignorePlatform; }
        set { _ignorePlatform = value; }
    }

    public float CurrentJumpIncreasingTime 
    { 
        get { return _currentJumpIncreasingTime; }
        set { _currentJumpIncreasingTime = value; }
    }

    public float CurrentInvincibilityTime
    {
        get { return _currentInvincibilityTime; }
        set { _currentInvincibilityTime = value; }
    }

    public float JumpsRemain 
    { 
        get { return _jumpsRemain; }
        set { _jumpsRemain = value; }
    }

    public float DashRemain
    {
        get { return _dashRemain; }
        set { _dashRemain = value; }
    }

    public bool IsAired
    {
        get { return MovementSM.CurrentState == Jumping || MovementSM.CurrentState == Falling; }
    }

    public bool IsGrounded
    {
        get { return MovementSM.CurrentState == Running || MovementSM.CurrentState == Standing; }
    }

    #endregion

    private void Start()
    {
        _playerTransform = GetComponent<Transform>();
        _playerCollider = GetComponent<BoxCollider2D>();

        _UpDownOverlapSize = new Vector2(_playerCollider.size.x * 0.75f, _playerCollider.size.y / 25);
        _LeftRightOverlapSize = new Vector2(_playerCollider.size.x / 25, _playerCollider.size.y * 0.75f);

        MovementSM = new StateMachine();
        Standing = new StandingState(this, MovementSM);
        Running = new RunningState(this, MovementSM);
        Jumping = new JumpingState(this, MovementSM);
        Falling = new FallingState(this, MovementSM);
        Climbing = new ClimbingState(this, MovementSM);
        Dashing = new DashState(this, MovementSM);
        WallJumping = new WallJumpState(this, MovementSM);
        Damaged = new DamagedState(this, MovementSM);

        MovementSM.Initialize(Standing);
        _jumpsRemain = _jumpCount;
        _dashRemain = _dashCount;

        _currentInvincibilityTime = float.MaxValue;
    }

    private void Update()
    {
        MovementSM.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        if (MovementSM.CurrentState != Damaged)
        {
            MoveInput = Input.GetAxisRaw("Horizontal");
            SetDirection();
        }

        if (Velocity.x > 0 != (_dir == 1) && MovementSM.CurrentState != WallJumping)
            Velocity.x = 0;

        MovementSM.CurrentState.StateUpdate();
    }

    private void SetDirection()
    {
        if (MoveInput > 0)
            _dir = 1;
        else if (MoveInput < 0)
            _dir = -1;
    }

    public void DebugMessage(object message)
    {
        print(message);
    }

    public void MovePlayer(Vector2 velocity)
    {
        transform.Translate(velocity * Time.deltaTime);
    }

    public void RestoreJumps()
    {
        _jumpsRemain = _jumpCount;
    }

    public void RestoreDash()
    {
        _dashRemain = _dashCount;
    }

    #region MovementTriggers

    public bool DownSideCheck(LayerMask layer)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y - _playerCollider.size.y / 2);
        return Physics2D.OverlapBoxAll(position, _UpDownOverlapSize, 0, layer).Length > 0;
    }

    public bool UpSideCheck(LayerMask layer)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y + _playerCollider.size.y / 2);
        return Physics2D.OverlapBoxAll(position, _UpDownOverlapSize, 0, layer).Length > 0;
    }

    public bool LeftSideCheck(LayerMask layer)
    {
        Vector2 position = new Vector2(transform.position.x - _playerCollider.size.x/2, transform.position.y);
        return Physics2D.OverlapBoxAll(position, _LeftRightOverlapSize, 0, layer).Length > 0;
    }

    public bool RightSideCheck(LayerMask layer)
    {
        Vector2 position = new Vector2(transform.position.x + _playerCollider.size.x/2, transform.position.y);
        return Physics2D.OverlapBoxAll(position, _LeftRightOverlapSize, 0, layer).Length > 0;
    }

    public bool IsBumpRightWall(LayerMask layer)
    {
        return (Velocity.x > 0 || MoveInput > 0) && RightSideCheck(layer);
    }

    public bool IsBumpLeftWall(LayerMask layer)
    {
        return (Velocity.x < 0 || MoveInput < 0) && LeftSideCheck(layer);
    }

    public bool IsBumpUpWall(LayerMask layer)
    {
        return Velocity.y > 0 && UpSideCheck(layer);
    }

    public bool IsBumpDownWall(LayerMask layer)
    {
        return Velocity.y < 0 && DownSideCheck(layer);
    }

    #endregion

    private void OnDrawGizmos()
    {
        //Downside overlap drawing
        Vector2 downPosition = new Vector2(transform.position.x, transform.position.y - _playerCollider.size.y / 2);
        Gizmos.DrawCube(downPosition, _UpDownOverlapSize);

        //Upside overlap drawing
        Vector2 upPosition = new Vector2(transform.position.x, transform.position.y + _playerCollider.size.y / 2);
        Gizmos.DrawCube(upPosition, _UpDownOverlapSize);

        //Leftside overlap drawing
        Vector2 leftPosition = new Vector2(transform.position.x - _playerCollider.size.x/2, transform.position.y);
        Gizmos.DrawCube(leftPosition, _LeftRightOverlapSize);

        //Rightside overlap drawing
        Vector2 rightPosition = new Vector2(transform.position.x + _playerCollider.size.x/2, transform.position.y);
        Gizmos.DrawCube(rightPosition, _LeftRightOverlapSize);
    }
}
