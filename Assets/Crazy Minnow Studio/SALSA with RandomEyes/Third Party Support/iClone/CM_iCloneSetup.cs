using UnityEngine;

namespace CrazyMinnow.SALSA.iClone
{
    [AddComponentMenu("Crazy Minnow Studio/iClone/SALSA 1-Click iClone Setup")]
    public class CM_iCloneSetup : MonoBehaviour 
    {
		/// <summary>
		/// This initializes Setup when setting up characters at runtime
		/// </summary>
		void Awake()
		{
			Setup();
			Destroy(this);
		}

		/// <summary>
		/// Configures a complete SALSA with RandomEyes enabled iClone character
		/// </summary>
		public void Setup()
        {
            GameObject activeObj; // Selected hierarchy object
            Salsa3D salsa3D; // Salsa3D
            RandomEyes3D reEyes; // RandomEyes3D for eye
            RandomEyes3D reShapes; // RandomEyes3D for custom shapes
            RandomEyes3D[] randomEyes; // All RandomEyes3D compoents
			CM_iCloneSync iCloneSync; // CM_iCloneSync

			activeObj = this.gameObject;

            #region Add and get components
            salsa3D = activeObj.AddComponent<Salsa3D>().GetComponent<Salsa3D>(); // Add/get Salsa3D
            reEyes = activeObj.AddComponent<RandomEyes3D>().GetComponent<RandomEyes3D>(); // Add/get reEyes
            reShapes = reEyes; // Temporarily set the reShapes instance to reEyes so it's not null
            activeObj.AddComponent<RandomEyes3D>(); // Add reShapes
            // Get all RandomEyes compoents so we can distinguish the second reShapes instance
            randomEyes = activeObj.GetComponents<RandomEyes3D>();
            if (randomEyes.Length > 1)
            {
                for (int i = 0; i < randomEyes.Length; i++)
                {
                    // Verify this instance ID does not match the reEyes instance ID
                    if (randomEyes[i].GetInstanceID() != reEyes.GetInstanceID())
                    {
                        // Set the reShapes instance
                        reShapes = randomEyes[i];
                    }
                }
            }
			iCloneSync = activeObj.AddComponent<CM_iCloneSync>().GetComponent<CM_iCloneSync>(); // Add/get CM_iCloneSync
			iCloneSync.Initialize();
            #endregion

            #region Set Salsa3D and RandomEyes3D component parameters
            salsa3D.saySmallTrigger = 0.0005f;
            salsa3D.sayMediumTrigger = 0.0025f;
            salsa3D.sayLargeTrigger = 0.005f;
            salsa3D.SetRangeOfMotion(70f); // Set mouth range of motion
            salsa3D.blendSpeed = 10f; // Set blend speed

            salsa3D.audioSrc = activeObj.GetComponent<AudioSource>(); // Set the salsa3D.audioSrc
            if (salsa3D.audioSrc) salsa3D.audioSrc.playOnAwake = false; // Disable play on wake

            reEyes.SetRangeOfMotion(60f); // Set eye range of motion
			reEyes.SetBlendSpeed(10f); // Set eye blend speed
			reEyes.SetBlinkSpeed(20f); // Set blink speed
            reShapes.useCustomShapesOnly = true; // Set reShapes to custom shapes only
            reShapes.skinnedMeshRenderer = iCloneSync.body; // Set the SkinnedMeshRenderer
            reShapes.AutoLinkCustomShapes(true, salsa3D); // Auto-link custom shapes
            /* Removes all custom shapes from random selection.
             * You should selectively include certain shapes in random selection, 
             * like eyebrows and facial twitches that add natural random movement to the face */
            reShapes.SetCustomShapesAllNotRandom(true);
            // Enable brow and nose shapes for natural facial twitches
            for (int i = 0; i < reShapes.customShapes.Length; i++)
            {
                if (reShapes.customShapes[i].shapeName.ToLower().Contains("brow") ||
                    reShapes.customShapes[i].shapeName.ToLower().Contains("nose"))
                {
                    reShapes.customShapes[i].notRandom = false;
                }
            }
			#endregion

			#region CM_iCloneSync settings
			iCloneSync.salsa3D = salsa3D;
			iCloneSync.randomEyes3D = reEyes;
            #endregion
        }
    }
}