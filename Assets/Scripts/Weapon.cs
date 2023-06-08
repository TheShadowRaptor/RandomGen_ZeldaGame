using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Create Weapon")]
[System.Serializable]
public class Weapon : ScriptableObject
{
    public string name;

    public List<TypesOfDamage.DamageType> damageTypes = new List<TypesOfDamage.DamageType>();
    public List<int> damage = new List<int>();

}
