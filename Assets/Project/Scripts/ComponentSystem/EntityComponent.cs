using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Components/Entity")]
public class EntityComponent : GameComponent
{
    public AreaType spawnArea;
    public DropTableEntry dropPool;

    [Header("Stats")]
    public int health = 5;
    public int damage = 1;
    public int defense = 1;

    [Header("ElementType Defense Attributes")]
    public List<AttakTypeData<ElementType>> elementDefenseModifiers;

    [Header("DamageType Defense Attributes")]
    public List<AttakTypeData<DamageType>> damageDefenseModifiers;

    public int ReceiveDamage(int _damageAmount, ElementType[] _elementTypes, DamageType[] _damageTypes)
    {
        _elementTypes ??= new ElementType[0];
        _damageTypes ??= new DamageType[0];

        // Check for full immunity
        bool allImmune = true;
        foreach (var element in _elementTypes)
        {
            if (CheckDefenseMultiplier(elementDefenseModifiers, element) > 0f)
            {
                allImmune = false;
                break;
            }
        }
        if (allImmune)
        {
            foreach (var damage in _damageTypes)
            {
                if (CheckDefenseMultiplier(damageDefenseModifiers, damage) > 0f)
                {
                    allImmune = false;
                    break;
                }
            }
        }
        if (allImmune) return 0;

        int finalDamage = CheckMultipliers(_damageAmount, _elementTypes, _damageTypes);
        return Mathf.Max(finalDamage, 0);
    }

#if UNITY_EDITOR
    [ContextMenu("Test Damage (Debug Only)")]
    public void TestReceiveDamage()
    {
        int damage = 50;
        ElementType[] elements = new ElementType[] { ElementType.Fire };
        DamageType[] types = new DamageType[] { DamageType.Cut };

        int result = ReceiveDamage(damage, elements, types);
        Debug.Log($"Test Damage Received: {result}");
    }
#endif

    public int CheckMultipliers(int _damageTotal, ElementType[] _elementTypes, DamageType[] _damageTypes)
    {
        float multiplier = 1.0f;

        foreach (var element in _elementTypes)
        {
            multiplier *= CheckDefenseMultiplier(elementDefenseModifiers, element);
        }

        foreach (var damageType in _damageTypes)
        {
            multiplier *= CheckDefenseMultiplier(damageDefenseModifiers, damageType);
        }

        return Mathf.Max(Mathf.RoundToInt(_damageTotal * multiplier), 0);
    }

    private float CheckDefenseMultiplier<T>(List<AttakTypeData<T>> modifiers, T type)
    {
        foreach (var data in modifiers)
        {
            if (EqualityComparer<T>.Default.Equals(data.type, type))
                return data.multiplier;
            
        }
        return 1.0f; // Neutral if no modifier is found
    }
}

[System.Serializable]
public class DropTableEntry
{
    public ConsumableObject dropItem;
    [Range(0f, 1f)] public float dropChance;

    public bool ShouldDrop()
    {
        return Random.value < dropChance;
    }
}
