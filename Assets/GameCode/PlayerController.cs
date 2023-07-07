using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Funtion
    /// </summary>
    float _horizentalAxis;
    float _verticalAxis;

    bool _playerWalk;                            // 플레이어 걷기 bool 값
    bool _playerJump;                            // 플레이어 점프 bool 값
    bool _playerDash;                            // 플레이어 회피 bool 값

    bool _AttackDown;                            // 플레이어 공격 bool 값
    bool _isJump;                                // 플레이어 점프 제어 bool 값
    bool _isDash;                                // 플레이어 회피 제어 bool 값
    bool _isAttackReady;                         // 플레이어 공격 준비 bool 값

    /// <summary>
    /// Component
    /// </summary>
    Vector3 _moveVec;
    Vector3 _dashVec;                                  // 회피시 방향이 전환되지 않도록 제한
    Rigidbody _playerRigidbody;
    Animator _animator;
    GameObject nearObj;
    [SerializeField] private Weapon _equipWeapon;
    private JoyStick _joyStick;

    [Header("PlayerState")]
    [SerializeField] private int _playerHP;
    [SerializeField] private int _damage;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _attackDelay;                          // 플레이어 공격 딜레이
    //[SerializeField] VirtualJoyStick virtualJoyStick;


    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _playerRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        _joyStick = GameObject.Find("BackGround_JoyStick").GetComponent<JoyStick>();
    }
    void Update()
    {
        PlayerInput();

        PlayerMove();
        PlayerTurn();
        PlayerJump();
        PlayerDash();
        PlayerAttack();
        Swap();

    }

    // 키 입력 함수
    void PlayerInput()
    {
        _horizentalAxis = Input.GetAxisRaw("Horizontal");
        _verticalAxis = Input.GetAxisRaw("Vertical");

        // JoyStick Build시 사용
        _horizentalAxis = _joyStick.inputHorizontal();
        _verticalAxis = _joyStick.inputVertical();

        _playerWalk = Input.GetKeyDown(KeyCode.LeftControl);
        _playerJump = Input.GetKeyDown(KeyCode.Space);
        _playerDash = Input.GetKeyDown(KeyCode.LeftShift);
        _AttackDown = Input.GetButtonDown("Fire1");                  // Left Mouse

    }


    #region Player 이동 관련
    // 플레이어 이동 함수
    public void PlayerMove()
    {
        _moveVec = new Vector3(_horizentalAxis, 0, _verticalAxis).normalized;

        if (_isDash)
        {
            // 회피 방향이랑 가는 방향이랑 같게
            _moveVec = _dashVec;
        }
        if (_playerWalk)
        {
            //transform.position += _moveVec * _speed * 0.3f * Time.deltaTime;
            _playerRigidbody.velocity = _moveVec * _speed * 0.3f;
        }
        else
        {
            //transform.position += _moveVec * _speed * Time.deltaTime;
            _playerRigidbody.velocity = _moveVec * _speed;
        }
        //transform.position += moveVec * Speed * (WalkDown ? 0.3f : 1f) * Time.deltaTime;
        _animator.SetBool("isMove", _moveVec != Vector3.zero);
        _animator.SetBool("isWalk", _playerWalk);

    }

    // 플레이어 시점 함수
    void PlayerTurn()
    {
        // 이동 방향 카메라 시점
        transform.LookAt(transform.position + _moveVec);
    }

    // 플레이어 점프 함수
    void PlayerJump()
    {
        if (_playerJump && _isJump == false && _moveVec == Vector3.zero && _isDash == false)
        {
            _playerRigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
            _animator.SetTrigger("doJump");
            _isJump = true;
        }
    }
    // 플레이어 회피 함수
    void PlayerDash()
    {
        //if (playerJump && isJump == false && moveVec != Vector3.zero && isDash == false)
        if (_playerDash && _isJump == false && _moveVec != Vector3.zero && _isDash == false)
        {
            _dashVec = _moveVec;
            _speed *= 2;
            _animator.SetTrigger("doDash");
            _isDash = true;

            // 회피 빠져나오는 속도
            Invoke("PlayerDashEnd", 0.2f);

            // 딜레이를 넣어야함
        }
    }

    // PlayerDash에서 사용 중
    void PlayerDashEnd()
    {
        // 원래 속도로 돌아오게함
        _speed *= 0.5f;
        _isDash = false;
    }
    #endregion


    // 무기 변경
    void Swap()
    {

        //if (equipWeapon != null)
        //    //equipWeapon.gameObject.SetActive(false);
    }

    // 플레이어 공격
    void PlayerAttack()
    {
        //장비한 무기가 없으면
        if (_equipWeapon == null)
        {
            Debug.Log(_equipWeapon);
            return;
        }

        _attackDelay += Time.deltaTime;
        _isAttackReady = _equipWeapon.AttackDelay < _attackDelay;

        // 추후 무기 변경 넣을 꺼면, && isSwap

        if (_AttackDown && _isAttackReady && !_isDash)
        {
            _equipWeapon.UseWeapon();
            _animator.SetTrigger("doAttack");       // 추후 Animator에서 수정?
            Debug.Log("**");
            //animator.SetInteger("doAttack", 0);
            _attackDelay = 0;                        // 딜레이 초기화
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _isDash = false;
        }
    }
}
