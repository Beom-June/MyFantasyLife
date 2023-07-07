using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Type { A, B, C, Boss };
public class Monster : MonoBehaviour
{
    [SerializeField]private Type _enemyType;

    [SerializeField] private int _maxHealth;
    [SerializeField] private int _curHealth;
    [SerializeField] private int _score;

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private Transform _target;
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject[] _coins;
    [SerializeField] private bool _isChase;
    [SerializeField] private bool _isAttack;
    [SerializeField] private bool _isDead;
    [SerializeField] private BoxCollider _attackArea;

    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] private MeshRenderer[] Meshs;
    [SerializeField] private NavMeshAgent _navMesh;
    [SerializeField] private Animator _animator;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _boxCollider = GetComponent<BoxCollider>();
        Meshs = GetComponentsInChildren<MeshRenderer>();
        _navMesh = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        if (_enemyType != Type.Boss)
        {
            Invoke("ChaseStart", 2);
        }
    }

    void Update()
    {
        if (_navMesh.enabled && _enemyType != Type.Boss)
        {
            _navMesh.SetDestination(_target.position);
            _navMesh.isStopped = !_isChase;
        }
    }
    void Targeting()
    {
        if (!_isDead && _enemyType != Type.Boss)
        {
            float _targetRadius = 0;
            float _targetRange = 0;

            switch (_enemyType)
            {
                case Type.A:
                    _targetRadius = 1.5f;
                    _targetRange = 3f;
                    break;

                case Type.B:
                    _targetRadius = 1f;
                    _targetRange = 12f;
                    break;

                case Type.C:
                    _targetRadius = 0.5f;
                    _targetRange = 25f;
                    break;
            }

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, _targetRadius, transform.forward, _targetRange, LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !_isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }
    IEnumerator Attack()
    {
        _isChase = false;
        _isAttack = true;
        _animator.SetBool("isAttack", true);

        switch (_enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                _attackArea.enabled = true;

                // 공격이 끝났으니 반대로
                yield return new WaitForSeconds(1f);
                _attackArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;

            case Type.B:
                // 몬스터 돌진
                yield return new WaitForSeconds(0.1f);
                _rigidBody.AddForce(transform.forward * 20, ForceMode.Impulse);
                _attackArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                _rigidBody.velocity = Vector3.zero;
                _attackArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;

            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject InstantBullet = Instantiate(_bullet, transform.position, transform.rotation);
                Rigidbody rigidBullet = InstantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);
                break;
        }
        _isChase = true;
        _isAttack = false;
        _animator.SetBool("isAttack", false);
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
            Weapon _weapon = other.GetComponent<Weapon>();
            _curHealth -= _weapon.Damage;
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
        _curHealth -= 100;
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

        if (_curHealth > 0)
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
            _isDead = true;
            _isChase = false;
            _navMesh.enabled = false;

            _animator.SetTrigger("doDie");

            PlayerController player = _target.GetComponent<PlayerController>();
            //player.Score += Score;
            int RanCoin = Random.Range(0, 3);                               // 동전이 3개 이므로
            Instantiate(_coins[RanCoin], transform.position, Quaternion.identity);

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

                _rigidBody.freezeRotation = false;
                _rigidBody.AddForce(ReactVec * 5, ForceMode.Impulse);
                _rigidBody.AddTorque(ReactVec * 15, ForceMode.Impulse);
                //Destroy(gameObject, 2);
            }
            else
            {
                ReactVec = ReactVec.normalized;
                ReactVec += Vector3.up;
                _rigidBody.AddForce(ReactVec * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 3);
        }
    }

    void FreezeVelocity()
    {
        if (_isChase)
        {
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }
    }
    void ChaseStart()
    {
        _isChase = true;
        _animator.SetBool("isWalk", true);
    }
}
