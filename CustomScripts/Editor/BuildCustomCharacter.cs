using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

public class CreateAssetBundles
{
    public static readonly string VERSION = "1.3.0";
    public static readonly string CUSTOM_CHARACTER_KEY = "character";
    public static readonly string CUSTOM_SOLO_SKIN_KEY = "soloSkin";
    public static readonly string CUSTOM_SOLO_WEAPON_KEY = "soloWeapon";
    
    
    [MenuItem("Build/Build Custom Assets")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = "Assets/AssetBundles";
        string characterABDirectory = Path.Combine(assetBundleDirectory, "Characters");
        string skinABDirectory = Path.Combine(assetBundleDirectory, "Skins");
        if (!Directory.Exists(assetBundleDirectory))
            Directory.CreateDirectory(assetBundleDirectory);
        if (!Directory.Exists(characterABDirectory))
            Directory.CreateDirectory(characterABDirectory);
        if (!Directory.Exists(skinABDirectory))
            Directory.CreateDirectory(skinABDirectory);
        
        
        //Create Custom Characters
        var characterAssetBundles = GenerateAssetBundleBuildsForCustomCharacters(characterABDirectory);
        BuildAssetBundles(characterABDirectory, characterAssetBundles);

        //Create Custom Skins
        var skinAssetBundles = GenerateAssetBundleBuildsForSkins(skinABDirectory);
        BuildAssetBundles(skinABDirectory, skinAssetBundles);

    }

    

    private static void BuildAssetBundles(string assetBundleDirectory, List<AssetBundleBuild> assetBundles)
    {
        Debug.Log($"Building asset bundles {assetBundles.Count}");
        BuildAssetBundlesParameters buildInput = new()
        {
            outputPath = assetBundleDirectory,
            options = BuildAssetBundleOptions.None,
            bundleDefinitions = assetBundles.ToArray()
        };
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(buildInput);
        
        // Look at the results
        if (manifest != null)
        {
            foreach (var bundleName in manifest.GetAllAssetBundles())
            {
                string projectRelativePath = buildInput.outputPath + "/" + bundleName;
                Debug.Log($"Successfully created AssetBundle {bundleName} at {projectRelativePath}");
                //Debug.Log($"Size of AssetBundle {projectRelativePath} is {new FileInfo(projectRelativePath).Length}");
            }
        }
        else
        {
            Debug.Log("Build failed, see Console and Editor log for details");
        }
    }
    
    private static List<AssetBundleBuild> GenerateAssetBundleBuildsForSkins(string outputPath)
    {
        List<AssetBundleBuild> assetBundles = new List<AssetBundleBuild>();

        var customSkins = AssetDatabase.FindAssets("t:CustomSoloSkinSO");
        Debug.Log($"Custom Solo skins found: {customSkins.Length}");
        foreach (var asset in customSkins)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var skin = AssetDatabase.LoadAssetAtPath<CustomSoloSkinSO>(path);
            var assetBundleName = skin.assetBundleName;
            Debug.Log($"Building Custom Skin: {skin.skinName}, assetBundleName: {assetBundleName}");
            AssetBundleBuild build = new();
            HashSet<string> assetPathList = new HashSet<string>();
            build.assetBundleName = assetBundleName;

            JObject customSkinJSON = new JObject();
            customSkinJSON.Add("version",VERSION);
            
            customSkinJSON.Add(CUSTOM_SOLO_SKIN_KEY, skin.toJSON());
            assetPathList.UnionWith(skin.getAssetNameList());

            build.assetNames = assetPathList.ToArray();
            assetBundles.Add(build);
            
            var filePath = Path.Combine(outputPath, assetBundleName.ToLower() + ".custom.json");
            using (var fs = File.Create(filePath))
            using (var writer = new StreamWriter(fs))
                writer.Write(customSkinJSON.ToString());
        }
        return assetBundles;
    }

    static List<AssetBundleBuild> GenerateAssetBundleBuildsForCustomCharacters(string outputPath)
    {
        List<AssetBundleBuild> assetBundles = new List<AssetBundleBuild>();
        
        var customCharacter = AssetDatabase.FindAssets("t:CustomCharacterSO");
        foreach (var asset in customCharacter)
        {
            var path = AssetDatabase.GUIDToAssetPath(asset);
            var character = AssetDatabase.LoadAssetAtPath<CustomCharacterSO>(path);
            var passive = character.passive;
            var weapon = character.weapon;
            var skins = character.skins;
            var assetBundleName = character.assetBundleName;
            
            Debug.Log($"Building Custom Character: {character.name}, assetBundleName: {assetBundleName}");
            AssetBundleBuild build = new AssetBundleBuild();
            HashSet<string> assetPathList = new HashSet<string>();
            build.assetBundleName = assetBundleName;
            
            JObject customCharacterJSON = new JObject();
            customCharacterJSON.Add("version",VERSION);
            customCharacterJSON.Add(CUSTOM_CHARACTER_KEY, character.toJSON());
            assetPathList.UnionWith(character.getAssetNameList());
            customCharacterJSON.Add("passive", passive.toJSON());
            assetPathList.UnionWith(passive.getAssetNameList());
            customCharacterJSON.Add("weapon", weapon.toJSON());
            assetPathList.UnionWith(weapon.getAssetNameList());

            JArray skinJSON = new JArray();
            foreach (var skin in skins)
            {
                skinJSON.Add(skin.toJSON());
                assetPathList.UnionWith(skin.getAssetNameList());
            }
            customCharacterJSON.Add("skins", skinJSON);

            foreach (var assetPath in assetPathList)
            {
                Debug.Log(assetPath);
            }
            build.assetNames = assetPathList.ToArray();
            assetBundles.Add(build);
            
            //create json file with variables and references to assets
            var filePath = Path.Combine(outputPath, assetBundleName.ToLower() + ".custom.json");
            using (var fs = File.Create(filePath))
                using (var writer = new StreamWriter(fs))
                    writer.Write(customCharacterJSON.ToString());
            
            //File.WriteAllText(filePath, );
        }

        return assetBundles;

    }
    
    
    
    
}