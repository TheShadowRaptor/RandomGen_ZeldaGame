using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EntityType { Enemy, Player, Destructible, NPC, Environment }
public enum AreaType { Forest, Lake, Mountain, Desert, All }
public enum Rarity { Common, Rare, Epic, Legendary }
public enum DamageType { Pierce, Cut, Blunt, Magic}
public enum ElementType { Fire, Water, Earth, Wind }

public class GameComponent : ScriptableObject
{
    [SerializeField] protected new string name;
    [SerializeField] protected GameObject prefab;
}

[System.Serializable]
public class AttakTypeData<T>
{
    public T type;
    public float multiplier = 1.0f;
}