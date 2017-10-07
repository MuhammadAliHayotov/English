using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CrazyMinnow.SALSA.iClone
{
    [CustomEditor(typeof(CM_iCloneSetup))]
    public class CM_iCloneSetupEditor : Editor
    {
        private CM_iCloneSetup iCloneSetup; // CM_iCloneSetup reference

		public void OnEnable()
        {
			// Get reference
			iCloneSetup = target as CM_iCloneSetup;

			// Run Setup
			iCloneSetup.Setup();

            // Remove setup component
            DestroyImmediate(iCloneSetup);
        }
    }
}