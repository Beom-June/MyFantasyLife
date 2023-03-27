using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Funtion
    /// </summary>
    float HorizentalAxis;
    float VerticalAxis;

    bool playerWalk;                            // 플레이어 걷기 bool 값
    bool playerJump;                            // 플레이어 점프 bool 값
    bool playerDash;                            // 플레이어 회피 bool 값

    bool AttackDown;                            // 플레이어 공격 bool 값
    bool isJump;                                // 플레이어 점프 제어 bool 값
    bool isDash;                                // 플레이어 회피 제어 bool 값
    bool isAttackReady;                         // 플레이어 공격 준비 bool 값

    /// <summary>
    /// Component
    /// </summary>
    Vector3 moveVec;
    Vector3 dashVec;                                  // 회피시 방향이 전환되지 않도록 제한
    Rigidbody playerRigidbody;
    Animator animator;
    GameObject nearObj;
    public Weapon equipWeapon;
    private JoyStick joyStick;

    [Header("PlayerState")]
    public int PlayerHP;
    public int Damage;
    public float Speed = 10f;
    public float AttackDelay;                          // 플레이어 공격 딜레이
    //[SerializeField] VirtualJoyStick virtualJoyStick;


    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        joyStick = GameObject.Find("JoyStickBackGround").GetComponent<JoyStick>();
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
        HorizentalAxis = Input.GetAxisRaw("Horizontal") ;
        VerticalAxis = Input.GetAxisRaw("Vertical");

        // JoyStick Build시 사용
        HorizentalAxis = joyStick.inputHorizontal();
        VerticalAxis = joyStick.inputVertical();

        playerWalk = Input.GetButton("Walk");                       // Left Ctrl
        playerJump = Input.GetButtonDown("Jump");                   // Space Bar
        playerDash = Input.GetButtonDown("Dash");                   // Left Shift
        AttackDown = Input.GetButtonDown("Fire1");                  // Left Mouse

    }


    #region Player 이동 관련
    // 플레이어 이동 함수
    public void PlayerMove()
    {
        moveVec = new Vector3(HorizentalAxis, 0, VerticalAxis).normalized;

        if (isDash)
        {
            // 회피 방향이랑 가는 방향이랑 같게
            moveVec = dashVec;
        }
        if (playerWalk)
        {
            transform.position += moveVec * Speed * 0.3f * Time.deltaTime;
        }
        else
        {
            transform.position += moveVec * Speed * Time.deltaTime;
        }
        //transform.position += moveVec * Speed * (WalkDown ? 0.3f : 1f) * Time.deltaTime;
        animator.SetBool("isMove", moveVec != Vector3.zero);
        animator.SetBool("isWalk", playerWalk);

    }

    // 플레이어 시점 함수
    void PlayerTurn()
    {
        // 이동 방향 카메라 시점
        transform.LookAt(transform.position + moveVec);
    }

    // 플레이어 점프 함수
    void PlayerJump()
    {
        if (playerJump && isJump == false && moveVec == Vector3.zero && isDash == false)
        {
            playerRigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
            animator.SetTrigger("doJump");
            isJump = true;
        }
    }
    // 플레이어 회피 함수
    void PlayerDash()
    {
        //if (playerJump && isJump == false && moveVec != Vector3.zero && isDash == false)
        if (playerDash && isJump == false && moveVec != Vector3.zero && isDash == false)
        {
            dashVec = moveVec;
            Speed *= 2;
            animator.SetTrigger("doDash");
            isDash = true;

            // 회피 빠져나오는 속도
            Invoke("PlayerDashEnd", 0.2f);

            // 딜레이를 넣어야함
        }
    }

    // PlayerDash에서 사용 중
    void PlayerDashEnd()
    {
        // 원래 속도로 돌아오게함
        Speed *= 0.5f;
        isDash = false;
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
        if (equipWeapon == null)
        {
            Debug.Log(equipWeapon);
            return;
        }

        AttackDelay += Time.deltaTime;
        isAttackReady = equipWeapon.attackDelay < AttackDelay;

        // 추후 무기 변경 넣을 꺼면, && isSwap

        if (AttackDown && isAttackReady && !isDash)
        {
            equipWeapon.UseWeapon();
            animator.SetTrigger("doAttack");       // 추후 Animator에서 수정?
            Debug.Log("**");
            //animator.SetInteger("doAttack", 0);
            AttackDelay = 0;                        // 딜레이 초기화
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            isJump = false;
        }
    }
}
