using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public enum Type { A, B, C, Boss };
    public Type enemyType;

    public int MaxHealth;
    public int CurHealth;
    public int Score;

    public GameManager Manager;
    public Transform Target;
    public GameObject bullet;
    public GameObject[] Coins;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public BoxCollider MeleeArea;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] Meshs;
    public NavMeshAgent Nav;
    public Animator Anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        Meshs = GetComponentsInChildren<MeshRenderer>();
        Nav = GetComponent<NavMeshAgent>();
        Anim = GetComponentInChildren<Animator>();

        if (enemyType != Type.Boss)
        {
            Invoke("ChaseStart", 2);
        }
    }

    void Update()
    {
        if (Nav.enabled && enemyType != Type.Boss)
        {
            Nav.SetDestination(Target.position);
            Nav.isStopped = !isChase;
        }
    }
    void Targeting()
    {
        if (!isDead && enemyType != Type.Boss)
        {
            float targetRadius = 0;
            float targetRange = 0;

            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3f;
                    break;

                case Type.B:
                    targetRadius = 1f;
                    targetRange = 12f;
                    break;

                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        Anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                MeleeArea.enabled = true;

                // 공격이 끝났으니 반대로
                yield return new WaitForSeconds(1f);
                MeleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                // 몬스터 돌진
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                MeleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                MeleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject InstantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = InstantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }
        isChase = true;
        isAttack = false;
        Anim.SetBool("isAttack", false);
    }
    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            CurHealth -= weapon.damage;
            Vector3 ReactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(ReactVec, false));
        }
        else if (other.tag == "Bullet")
        {
            //Bullet bullet = other.GetComponent<Bullet>();
            //CurHealth -= bullet.damage;
            Vector3 ReactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(ReactVec, false));
        }
    }
    public void HitByGrenade(Vector3 ExplosionPos)
    {
        CurHealth -= 100;
        Vector3 ReactVect = transform.position - ExplosionPos;
        StartCoroutine(OnDamage(ReactVect, true));
    }
    IEnumerator OnDamage(Vector3 ReactVec, bool isGrenade)
    {
        foreach (MeshRenderer mesh in Meshs)
        {
            mesh.material.color = Color.red;
        }
        yield return new WaitForSeconds(0.1f);

        if (CurHealth > 0)
        {
            foreach (MeshRenderer mesh in Meshs)
            {
                mesh.material.color = Color.white;
            }
            yield return new WaitForSeconds(0.1f);
        }
        else
        {
            foreach (MeshRenderer mesh in Meshs)
            {
                mesh.material.color = Color.gray;
            }
            gameObject.layer = 14;                                          // 레이어 14 : EnemyDead
            isDead = true;
            isChase = false;
            Nav.enabled = false;

            Anim.SetTrigger("doDie");

            PlayerController player = Target.GetComponent<PlayerController>();
            //player.Score += Score;
            int RanCoin = Random.Range(0, 3);                               // 동전이 3개 이므로
            Instantiate(Coins[RanCoin], transform.position, Quaternion.identity);

            //switch (enemyType)
            //{
            //    case Type.A:
            //        Manager.EnemyCntA--;
            //        break;
            //    case Type.B:
            //        Manager.EnemyCntB--;
            //        break;
            //    case Type.C:
            //        Manager.EnemyCntC--;
            //        break;
            //    case Type.Boss:
            //        Manager.EnemyCntBoss--;
            //        break;
            //}


            if (isGrenade)
            {
                ReactVec = ReactVec.normalized;
                ReactVec += Vector3.up * 3;

                rigid.freezeRotation = false;
                rigid.AddForce(ReactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(ReactVec * 15, ForceMode.Impulse);
                //Destroy(gameObject, 2);
            }
            else
            {
                ReactVec = ReactVec.normalized;
                ReactVec += Vector3.up;
                rigid.AddForce(ReactVec * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 3);
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void ChaseStart()
    {
        isChase = true;
        Anim.SetBool("isWalk", true);
    }
}
