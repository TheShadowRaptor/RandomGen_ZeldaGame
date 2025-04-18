using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Sword, Bow, Dagger, Staff, Fist }
[CreateAssetMenu(menuName = "Game/Components/Weapon")]
public class WeaponComponent : ScriptableObject
{
    public string weaponName;
    public GameObject prefab;

    public WeaponType weaponType; // Sword, Bow, Wand, etc.
    public DamageType damageType;

    public float attackSpeed;
    public int damageAmount;
    public float criticalChance;

    public bool isTwoHanded;
}
