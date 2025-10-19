using System.Collections;
using System.Collections.Generic;
using Assets.Scripts._Data;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomSoloSkinSO", menuName = "ScriptableObjects/CustomSoloSkinSO")]
public class CustomSoloSkinSO : ScriptableObject
{
    [Header("Meta")]
    public string author = "Shifty";
    public string assetBundleName = "TestBundle";

    [Header("Data")] 
    [Tooltip("Character to create a custom skin for")]
    public ECharacter targetCharacter;
    [Tooltip("An override value that you can use to target custom made characters with custom skins. 0 is disabled. The value should be the eCharacter value found in the custom.json file for that character.")]
    public uint targetCustomCharacterOverride = 0;
    public string skinName;
    public string description;
    public Texture2D icon;
    public List<Material> materials = new List<Material>();
    public GameObject prefab;

    public ESkinType skinType = ESkinType.Kills;
    
#if UNITY_EDITOR
    public JObject toJSON()
    {
        //var skinPath = AssetDatabase.GetAssetPath(this);
        var iconPath = AssetDatabase.GetAssetPath(icon);
        var materialPaths = new List<string>();
        var prefabPath = AssetDatabase.GetAssetPath(prefab);
        uint eCharacter = (uint)targetCharacter;
        if (targetCustomCharacterOverride != 0)
            eCharacter = targetCustomCharacterOverride;
        
        foreach (var mat in materials)
        {
            var matPath = AssetDatabase.GetAssetPath(mat);
            materialPaths.Add(AssetDatabase.GetAssetPath(mat));
        }
        //get prefab phys bones
        var physicsBones = PhysBoneBaker.PhysBonesToJson(prefab.transform);
        
        
        return JObject.FromObject(new
        {
            //skinPath,
            author,
            skinName,
            description, 
            eCharacter,
            iconPath,
            prefabPath,
            materialPaths,
            physicsBones
        });
    }
    public string[] getAssetNameList()
    {
        List<string> list = new List<string>();
        list.Add(AssetDatabase.GetAssetPath(this));
        list.Add(AssetDatabase.GetAssetPath(icon));
        list.Add(AssetDatabase.GetAssetPath(prefab));
        foreach (var mat in materials)
        {
            list.Add(AssetDatabase.GetAssetPath(mat));
        }
        
        return list.ToArray();
    }
#endif
    
}
