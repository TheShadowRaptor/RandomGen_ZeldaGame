using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Components/Entity")]
public class EntityComponent : ScriptableObject
{
    public string entityName;
    public GameObject prefab;

    public EntityType entityType; // Enemy, Player, Object, Terrain, etc.
    public AreaType spawnArea;

    public List<DamageType> weaknesses;
    public List<DropTableEntry> dropTable;

    public Rarity rarity;
}