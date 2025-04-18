using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IKillable 
{
    public void Heal(int amount);
    public void TakeDamage(int damage);
    public bool IsAlive();
}
