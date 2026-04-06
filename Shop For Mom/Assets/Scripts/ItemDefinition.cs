using UnityEngine;

public class ItemDefinition : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public int maxStackSize = 1;
}
