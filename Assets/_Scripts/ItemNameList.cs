using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script whose functionality is only to hold lists with names for objects and enemies
/// </summary>
public class ItemNameList : MonoBehaviour {

    public List<List<string>> AllItemNamesList = new List<List<string>>();
    public List<List<string>> InteractableObjectList = new List<List<string>>();
    
    public List<string> ObjectsToOpenList = new List<string>() {"CHEST", "HATCH", "TRAPDOOR", "EXIT", "DOOR"};

    public List<string> LightNameList = new List<string>() {"LIGHT", "FLAME", "FIRE", "TORCH"};
    public List<string> WallNameList = new List<string>() {"WALL", "PUSHABLEWALL"};
    public List<string> WeaponNameList = new List<string>() {"WEAPON", "SWORD"};
    public List<string> ChestNameList = new List<string>() {"CHEST"};
    public List<string> ButtonNameList = new List<string>() {"BUTTON"};
    public List<string> HatchNameList = new List<string>() {"HATCH", "TRAPDOOR", "EXIT"};
    public List<string> SkeletonNameList = new List<string>() {"ENEMY", "FOE", "ADVERSARY", "OPPOSER", "HOSTILE", "BODY", "SKELETON", "MONSTER"};
    public List<string> TrollNameList = new List<string>() {"ENEMY", "FOE", "ADVERSARY", "OPPOSER", "HOSTILE", "BODY", "TROLL", "MONSTER"};
	public List<string> StoneNameList = new List<string>() {"STONE", "TABLET", "ROCK", "PIECE"};
    public List<string> DoorNameList = new List<string>() {"DOOR", "GATE"};

    void Awake() {
        AllItemNamesList.Add(LightNameList);
        AllItemNamesList.Add(WallNameList);
        AllItemNamesList.Add(WeaponNameList);
        AllItemNamesList.Add(ChestNameList);
        AllItemNamesList.Add(ButtonNameList);
        AllItemNamesList.Add(HatchNameList);
        AllItemNamesList.Add(SkeletonNameList);
        AllItemNamesList.Add(TrollNameList);
		AllItemNamesList.Add(StoneNameList);
        AllItemNamesList.Add(DoorNameList);

        InteractableObjectList.Add(LightNameList);
        InteractableObjectList.Add(WallNameList);
        InteractableObjectList.Add(WeaponNameList);
        InteractableObjectList.Add(ChestNameList);
        InteractableObjectList.Add(ButtonNameList);
        InteractableObjectList.Add(HatchNameList);
    }
}
