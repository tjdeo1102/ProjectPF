using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LSY_ItemManager;

[CreateAssetMenu(fileName = "Shop Object Data", menuName = "Scriptable Object/Shop Object")]
public class LSY_ShopObject : ScriptableObject
{
    [Header("아이템 데이터")]
    [SerializeField] public string itemName;
    [SerializeField] public float itemPrice;
    [SerializeField] public string itemExplain;
    [SerializeField] public Sprite itemSprite;
}
