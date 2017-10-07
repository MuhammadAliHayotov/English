----------------------
Version 1.0 beta 1
----------------------
CM_iCloneSync is designed to be used in conjunction with SALSA with RandomEyes, and iClone models, as outlined in the workflow created by:

Crazy Minnow Studio, LLC
CrazyMinnowStudio.com

The workflow is documented at the following URL, along with a downloadable zip file that contains the supporting files.

http://crazyminnowstudio.com/posts/1-click-lipsync-with-iClone/


Package Contents
----------------
Crazy Minnow Studio/SALSA with RandomEyes/Third Party Support/
	iClone
		Editor
			CM_iCloneSyncEditor.cs
				Custom inspector for CM_iCloneSync.cs
			CM_iCloneSetupEditor.cs
				Custom inspector for CM_iCloneSetup.cs
		CM_iCloneSync.cs
			Helper script to apply Salsa and RandomEyes BlendShape data to iClone BlendShapes and eye bones.
		CM_iCloneSetup.cs
			SALSA 1-click iClone setup script for iClone characters.
		ReadMe.txt
			This readme file.			


Installation Instructions
-------------------------
1. Install SALSA with RandomEyes into your project.
	Select [Window] -> [Asset Store]
	Once the Asset Store window opens, select the download icon, and download and import [SALSA with RandomEyes].

2. Import the SALSA with RandomEyes iClone support package.
	Select [Assets] -> [Import Package] -> [Custom Package...]
	Browse to the [SALSA_3rdPartySupport_iClone.unitypackage] file and [Open].


Usage Instructions
------------------
1. Add an iClone character, that contains BlendShapes, to your scene.

2. Select the character root, then select:
	[Component] -> [Crazy Minnow Studio] -> [iClone] -> [SALSA 1-Click iClone Setup]
	This will add and configure all necessary component for a complete SALSA with RandomEyes setup.


What [SALSA 1-Click iClone Setup] does
------------------------------------
1. It adds the following components:
	[Component] -> [Crazy Minnow Studio] -> [Salsa3D] (for lip sync)
	[Component] -> [Crazy Minnow Studio] -> [RandomEyes3D] (for eyes)
	[Component] -> [Crazy Minnow Studio] -> [RandomEyes3D] (for custom shapes)
	[Component] -> [Crazy Minnow Studio] -> [iClone] -> [CM_iCloneSync] (for syncing SALSA with RandomEyes to your iClone character)

2. On the Salsa3D component, it leaves the SkinnedMeshRenderer empty, and sets the SALSA [Range of Motion] to 70. (Set this to your preference)

3. On the RandomEyes3D componet for eyes, it leaves the SkinnedMeshRenderer empty, and sets the [Range of Motion] to 60. (Set this to your preference)

4. On the RandomEyes3D component for custom shapes, it attempts to find and link the main SkinnedMeshRenderer with BlendShapes. If you've exported a multi resolution character, delete the resolutions you're not using and create a prefab with only the resolution you wish to use.

5. On the RandomEyes3D component for custom shapes, it checks [Use Custom Shapes Only], Auto-Link's all custom shapes, and removes most of them from random selection. 
	It selectively includes small shapes, like eyebrow and facial twitches, in random selection to add natural random facial movement.

6. On the CM_iCloneSync.cs component it attempts to link the following:
	Salsa3D
	RandomEyes3D (for eyes)
	The main SkinnedMeshRenderer with BlendShapes.
	The Left and Right eye bones.
	The Jaw bone.