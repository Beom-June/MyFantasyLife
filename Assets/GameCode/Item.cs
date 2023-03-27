using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Coin,
    Heart,
    Weapon,
    ETC
}

public class Item : MonoBehaviour
{
    public ItemType itemType;
    public int value;

    void Update()
    {
        ItemRotate(30);
    }
    void Start()
    {
        
    }

    // 아이템 회전 함수
    void ItemRotate(int speed)
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }

}
