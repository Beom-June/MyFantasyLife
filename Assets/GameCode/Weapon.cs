using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    meleeAttack,                                    // 근접 공격
    rangeAttack                                     // 원거리 공격
};
public class Weapon : MonoBehaviour
{
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private int _damage;                                  // 무기 데미지
    [SerializeField] private float _attackDelay;                           // 공격 딜레이
    [SerializeField] private BoxCollider _attackArea;                      // 공격 범위
    [SerializeField] private TrailRenderer _trailRenderer;                 // 공격 효과

    #region Property
    public int Damage
    {
        get { return _damage; }
    }

    public float AttackDelay
    {
        get { return _attackDelay; }
    }
    #endregion

    public void UseWeapon()
    {
        if (_weaponType == WeaponType.meleeAttack)
        {
            StopCoroutine(Swing());
            StartCoroutine(Swing());
        }
    }

    // 근접 공격 코루틴
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        _attackArea.enabled = true;
        _trailRenderer.enabled = true;

        yield return new WaitForSeconds(0.3f);
        _attackArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        _trailRenderer.enabled = false;
    }

    // 원거리 공격 코루틴
}
