using System.Collections.Generic;
using UnityEngine;

public enum WeaponType { Sword, Bow, Dagger, Staff, Fist }
[CreateAssetMenu(menuName = "Game/Components/Weapon")]
public class WeaponComponent : GameComponent
{
    public List<DamageType> damageTypes = new List<DamageType>();
    public float attackSpeed;
    public int damageAmount;
}
