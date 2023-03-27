using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Animator animator;
    public GameObject Monster;                  // 추후 태그로 찾는 방식으로 수정
    public int Damage;                          // 몬스터에게 주는 Damage

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(Monster == null)
        {
            Idle_Anim();
        }
        else if (Monster != null)
        {
            Attack_Anim();
        }
    }

    public void SetDamage()
    {
        Monster.GetComponent<MonsterScript>().GetDamage(Damage);
    }

    // Player Idle
    void Idle_Anim()
    {
        // "" 안에 Animator 이름 받아옴
        animator.SetInteger("doAttack", 0);
    }

    // Player Attack
    void Attack_Anim()
    {
        animator.SetInteger("doAttack", 1);
    }
}
