# Megabonk Custom Character Maker
These files are used to simplify the process of making custom characters in Megabonk.
They are used in conjunction with my Megabonk Custom Character Loader plugin (https://github.com/PeterMoras/MEGABONK_CustomCharacterLoader) in order to fully add custom characters.


## Installation
1. Ensure you have Unity 2023.2.20f1 downloaded, as this is Megabonk's Unity version, and is needed specifically to generate valid AssetBundles.
2. Create a new Unity project with the 2023.2.20f1 version, make it obvious that this is for Megabonk mods.
	Use the 3D built-in render pipeline, though it doesn't matter what setup you start with.
3. Extract or move all files into the Assets directory of the new Unity project.
	After compiling finishes, clear the console and you should still see a few errors mentioning 'Newtonsoft'.
4. Install Newtonsoft.JSON by opening the Unity Package Manager (Window > Package Manager), click the + icon in the top left, and add package by name.
	the package name is: 'com.unity.nuget.newtonsoft-json', and version doesn't matter
	
If you have followed these instructions, you should now have a Unity project with no Errors. You can ignore the 999+ warnings.
I would **HIGHLY** recommend immediately building your assets with the existing example assets (Section found below) to ensure that everything was set up correctly, and that you can see the character/skins in Megabonk.



## Creating a Character
Everything you need to help create a character can be found in the EXPORT folder for use as reference. 
PLEASE NOTE: The example characters are NOT BALANCED and are mainly a sanity check to make sure you can properly add a character to the game.
The SourceCode and CustomScripts folders only contain code to help facilitate the creation of custom characters, and SHOULD NOT BE MODIFIED.

Here is a step by step guide on the required steps of creating a character.
1. Find/Create a model with 5 animations: Idle, run, jump, fall, grind.
2. Import that model into this unity project.
3. For all imported models (assuming .fbx file) under import otions, Model - Set Read/Write to true, Materials - Extract Materials to ensure the materials can be stored in the assetbundle.
4. Animations.
	- Create an animator by copying the customCharacter Animator Controller, then replace the animations with your own.
		- The current animation controller is just an estimation of how animations actually work, and make show the incorrect animation at some points. If anyone finds a more reasonable looking animation graph, please let me know.
	- Make sure all your animations besides jump are set to 'Loop Time' (and likely loop pose too)
5. Scriptable Objects (SO folder)
	- Each custom character is defined by the CharacterDataSO object, and needs its own copy
	- Make sure to click the 'Randomize UID' at least once
	- Described in more detail in the Features section



### Features
- Custom Armatures. Any armature should work for the player model, as long as the root object has the Animator component.
- Custom Weapons. Weapons are defined by the 'Custom Weapon SO' scriptable object + 'Weapon Attack' script + 'Projectile X' script (like Projectile Explode or Projectile Mines).
	- 'Custom Weapon SO' defines the base stats and upgrade options for the weapon. In other words, all the math.
		- Upgrade Options determine what options can appear when you upgrade a weapon. If flat damage is all that ocurred, you would only see weapon damage upgrades when leveling.
		- 
	- The 'Weapon Attack' script is used primary to define the spawned objects. 
		- Prefab projectile is the 'attack' that spawns and is multiplied by # of projectiles
		- Muzzle is the effect the player makes when shooting the projectile, like a puff of smoke or bullet casing
		- Hit is the effect when the attack hits an opponent.
	- The Stats section defines all required stats, though additional base stats can also be added below
- Custom Passives. Not really. Currently only works with the predefined passives under different names.
- Custom Shaders/Materials. 
  - You can use the original MegabonkShader on your Materials, which will shade your character in the Megabonk Style in-game without throwing errors.
    - Make sure to place your texture on the _MainTex field at the bottom of the material property list
    - The shader will not look correct in the Unity Editor, but should do so in-game.
  - Alternatively, you can use your own custom shader + Material (like the 'Standard' material), and it will show up in that rendering style in game.
- Jiggle Bones.
  - Add the JiggleRigBuilder component to your character, and it will add bones to the targeted Root Transform and all children.
  - properties can be changed with the JiggleSettings, of which there are already a few presets (they have not been tested)
  - Required fields: Root Transform, Jiggle Settings.
  - Level of Detail does nothing
  - The 'MyTestCharacter' uses a JiggleRig for scarf physics, that can be used as an example.
  - Jiggle code courtesy of naelstrof via https://github.com/naelstrof/JigglePhysics
    - I'm using an old and modified version from the BepinExCompatible branch with fewer features for compatability.



## Building Your Assets
- Once you are happy with your custom assets, you can build them by finding the Build option (Build > Build Custom Assets) on the menu bar at the top.
- After a few seconds of compilation, you should see a new 'AssetBundles' folder created with your bundled character files inside.
- The files will be named after your CharacterDataSO object's 'Asset Bundle Name' field in all lowercase, of which you should see 3. One with no extension, once with a .custom.json extension, and one with .manifest. (There will also be files named after the folder you are in, but you can ignore those)
- ~~Copy the one with no extension and .json extension into your BepInEx/plugins/CustomCharacters folder. if the CustomCharacters folder does not exist, create it.~~ (This still works but the *.custom.json method below is recommended)
	- as of version 1.0.1+ you just make sure the json file ends with the extension '.custom.json' and it can be found anywhere in the plugin folder.
- When you start the game, you should see your character loaded as an option in the character select, and a new custom skin for Fox.





## Issues/In Progress
If you have any issues with creating your character, or find certain options don't exist, or want more features, please greate a github issue with the request.
If there are any issues involving the loading of your custom character, please put the issue on my other repository, linekd at the top.
- Currently, the base materials will throw errors in the log because they don't have the correct shader properties. This shouldn't affect performance but still needs to be fixed.