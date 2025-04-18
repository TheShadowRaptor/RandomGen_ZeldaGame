using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EntityType { Enemy, Player, Destructible, NPC, Environment }
public enum AreaType { Forest, Lake, Mountain, Desert, All }
public enum Rarity { Common, Rare, Epic, Legendary }
public enum DamageType { Fire, Ice, Physical, Poison, Electric }

[CreateAssetMenu(menuName = "Game/Components")]
public class GameComponent : ScriptableObject
{
    public string entityName;
    public Sprite icon;
    public List<GameComponent> components;

    public T GetComponent<T>() where T : GameComponent
    {
        return components.OfType<T>().FirstOrDefault();
    }

    public int CalculateDamage(EntityComponent target, WeaponComponent weapon)
    {
        int baseDamage = weapon.damageAmount;

        // Bonus damage vs weaknesses
        if (target.weaknesses.Contains(weapon.damageType))
        {
            baseDamage = Mathf.RoundToInt(baseDamage * 1.5f); // +50% vs weakness
        }

        // Random crit
        if (UnityEngine.Random.value < weapon.criticalChance)
        {
            baseDamage = Mathf.RoundToInt(baseDamage * 2f);
        }

        return baseDamage;
    }
}

[System.Serializable]
public class DropTableEntry
{
    public GameComponent dropItem;
    [Range(0, 1f)] public float dropChance;
}