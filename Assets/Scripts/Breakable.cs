using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakableData", menuName = "BreakableObjects/Create BreakableObject")]
public class Breakable : ScriptableObject
{
    public string name;
    public int health;
    public TypesOfDamage.DamageType[] resist;
    public TypesOfDamage.DamageType[] immune;
}
