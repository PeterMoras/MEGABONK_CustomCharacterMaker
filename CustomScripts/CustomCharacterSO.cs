using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Audio.Music;
using Assets.Scripts.Inventory__Items__Pickups.Stats;
using Assets.Scripts.Menu.Shop;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "CustomCharacterSO", menuName = "ScriptableObjects/CharacterDataSO")]
public class CustomCharacterSO : ScriptableObject
{
    [Header("Meta")]
    [Tooltip("Who created this custom character")]
    public string author = "Shifty";
    public bool RandomizeUID = false;
    [Tooltip("A Unique ID to identify ths character. MUST BE DIFFERENT FROM ALL OTHER CUSTOM CHARACTERS. If this value is set to lower than 1000, it will be randomized instead")]
    public uint uID = 0;

    [Tooltip("The name of the assetbundle that will store the character. Used to name the built files for clarity")]
    public string assetBundleName = "MyCustomCharacter";

    [Header("Base Fields")]
    [Space]
    public string characterName = "";
    public string characterDescription = "";
    [Space]

    [Tooltip("Hitbox height. Small values can mess up camera tracking")]
    public float colliderHeight = 3.85f; //hitbox size
    [Tooltip("Hitbox width")]
    public float colliderWidth = 0.7f; //hitbox size
    
    [Tooltip("Don't know what this effects")]
    [Range(0,10)]
    public float coolness = 7; //how cool your character is (might affect large head statues?)
    [Tooltip("Don't know what this effects")]
    [Range(0,10)]
    public int difficulty = 1; //no idea what difficulty actually does
    
    [Header("Optional")]
    [Tooltip("Custom footstep sounds. Leave blank for defaults (Fox footsteps)")]    
    public List<AudioClip> audioFootsteps;
    [Tooltip("Custom song for the character. Balnk for most characters")]    
    public MusicTrack themeSong;
    
    [Header("Additional Objects")]
    public Texture2D icon;
    
    [Tooltip("Custom Gameobject that will be rendered as the player. requires Animator at root object")]    
    public GameObject prefab;

    public CustomWeaponSO weapon;
    public CustomPassiveSO passive;
    [Tooltip("Must have at least 1 skin in the list")]
    public List<CustomSkinSO> skins;

    [Header("Stats")]
    [Tooltip("Probably affects impact of shrine upgrades. Based on default character values")]
    public List<StatModifier> statModifiers = new List<StatModifier>()
    {
        new StatModifier() { stat = EStat.JumpHeight, modifyType = EStatModifyType.Flat, modification = 1f },
        new StatModifier() { stat = EStat.Evasion, modifyType = EStatModifyType.Flat, modification = 0.05f },
        new StatModifier() { stat = EStat.PickupRange, modifyType = EStatModifyType.Flat, modification = 45f },
        new StatModifier() { stat = EStat.MoveSpeedMultiplier, modifyType = EStatModifyType.Addition, modification = 0.02f },
    }; //how well specific stats scale. leave empty for default scaling
    
    [Tooltip("No idea is this does anything. Maybe affects what types of items appear? Uses default Fox values")]
    public List<StatCategoryRatio> categoryRatios = new List<StatCategoryRatio>()
    {
        new StatCategoryRatio( category : EStatCategory.Offensive, value : 0),
        new StatCategoryRatio( category : EStatCategory.Defensive, value : 0.2765f),
        new StatCategoryRatio( category : EStatCategory.Movement, value : 0.3056f ),
        new StatCategoryRatio( category : EStatCategory.Utility, value : 1f ),
        new StatCategoryRatio( category : EStatCategory.Difficulty, value : 0f ),
        new StatCategoryRatio( category : EStatCategory.Coolness, value : 0.625f ),
        new StatCategoryRatio( category : EStatCategory.Null, value : 1f ),
    };
    
    
#if UNITY_EDITOR
    public JObject toJSON()
    {
        //var characterPath = AssetDatabase.GetAssetPath(this);
        var iconPath = AssetDatabase.GetAssetPath(icon);
        var prefabPath = AssetDatabase.GetAssetPath(prefab);
        var themesongPath = AssetDatabase.GetAssetPath(themeSong);

        var audioFootstepsPaths = new List<string>();
        foreach (var footstep in audioFootsteps)
        {
            audioFootstepsPaths.Add(AssetDatabase.GetAssetPath(footstep));
        }
        
        // var skinPaths = new List<string>();
        // foreach (var skin in skins)
        // {
        //     skinPaths.Add(AssetDatabase.GetAssetPath(skin));
        // }
        var physicsBones = PhysBoneBaker.PhysBonesToJson(prefab.transform);

        
        return JObject.FromObject(new { 
            //characterPath,
            author, 
            eCharacter = uID,
            assetBundleName,
            characterName,
            characterDescription,
            colliderHeight,
            colliderWidth,
            coolness,
            difficulty,
            themesongPath,
            audioFootstepsPaths,
            prefabPath,
            iconPath,
            statModifiers,
            categoryRatios,
            physicsBones
        });
    }

    public string[] getAssetNameList()
    {
        List<string> list = new List<string>();
        list.Add(AssetDatabase.GetAssetPath(this));
        list.Add(AssetDatabase.GetAssetPath(prefab));
        list.Add(AssetDatabase.GetAssetPath(icon));
        list.Add(AssetDatabase.GetAssetPath(themeSong));
        
        foreach (var footstep in audioFootsteps)
        {
            list.Add(AssetDatabase.GetAssetPath(footstep));
        }
        
        return list.ToArray();
    }
#endif
    



    public void OnValidate()
    {
        if (RandomizeUID)
        {
            RandomizeUID = false;
            uID = (uint)((Random.value) * (int.MaxValue-1001)) + 1000;
        }
        if (uID < 1000)
        {
            uID = (uint)((Random.value) * (int.MaxValue-1001)) + 1000;
        }
    }

    
}


