using UnityEngine;

[CreateAssetMenu(fileName = "CollectableObject", menuName = "NewObjects/Misc")]
public class CollectableData : ScriptableObject
{
    public enum AreaTypes
    {
        darkForest,
        frozenLakes,
        infestedVillage,
        volcanicMountain
    }

    public enum RarityTypes
    {
        common,
        uncommon,
        rare,
        epic,
        legendary
    }

    [Header("DataSettings")]
    public string collectableName;
    public bool isActive;
    public GameObject prefab;
    public AreaTypes areaType;
    public RarityTypes rarityType;

    public void Initialize(CollectableData collectable) 
    {
        collectableName = collectable.collectableName;
        prefab = collectable.prefab;
        areaType = collectable.areaType;
        rarityType = collectable.rarityType;
    }
}
