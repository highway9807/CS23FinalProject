using UnityEngine;

/* the thing in the [] is called an attribute, they have many functions. This
 * one creates a menu shortcut in the unity editor allowing us to create a
 * new instance of an item (then fill in its id, name, point value, and sprite) without
 * ever touching code! To do so:
 *          
 *      right click inside any folder in the Project tab -> create -> Scriptable
 *      Objects -> itemDefinition.
*/
[CreateAssetMenu(fileName = "itemDefinition", 
                 menuName = "Scriptable Objects/itemDefinition")]
public class ItemDefinition : ScriptableObject
{
    public int id;
    public string itemName;
    public int pointValue;
    public Sprite sprite;

}
