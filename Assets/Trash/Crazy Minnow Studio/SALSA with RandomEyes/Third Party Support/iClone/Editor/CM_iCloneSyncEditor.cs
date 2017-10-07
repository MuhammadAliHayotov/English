using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.iClone
{
	/// <summary>
	/// This is the custom inspector for CM_iCloneSync, a script that acts as a proxy between 
	/// SALSA with RandomEyes and Mixamo iClone characters, and allows users to link SALSA with 
	/// RandomEyes to iClone characters without any model modifications.
	/// 
	/// Crazy Minnow Studio, LLC
	/// CrazyMinnowStudio.com
	/// 
	/// NOTE:While every attempt has been made to ensure the safe content and operation of 
	/// these files, they are provided as-is, without warranty or guarantee of any kind. 
	/// By downloading and using these files you are accepting any and all risks associated 
	/// and release Crazy Minnow Studio, LLC of any and all liability.
	[CustomEditor(typeof(CM_iCloneSync))]
	public class CM_iCloneSyncEditor : Editor 
	{
		private CM_iCloneSync iCloneSync; // CM_iCloneSync reference
		private bool dirtySmall; // SaySmall dirty inspector status
		private bool dirtyMedium; // SayMedum dirty inspector status
		private bool dirtyLarge; // SayLarge dirty inspector status
		private bool dirtyHair; // Facial hair dirty inspector status

		private float width = 0f; // Inspector width
		private float addWidth = 10f; // Percentage
		private float deleteWidth = 10f; // Percentage
		private float shapeNameWidth = 60f; // Percentage
		private float percentageWidth = 30f; // Percentage

		public void OnEnable()
		{
			// Get reference
			iCloneSync = target as CM_iCloneSync;

			// Initialize
			if (iCloneSync.initialize)
			{
				iCloneSync.GetSalsa3D();
				iCloneSync.GetRandomEyes3D();
				iCloneSync.GetBody();
				iCloneSync.GetEyeBones();
                if (iCloneSync.facialHair.Count == 0) iCloneSync.GetFacialHair();
				if (iCloneSync.saySmall == null) iCloneSync.saySmall = new List<CM_ShapeGroup>();
				if (iCloneSync.sayMedium == null) iCloneSync.sayMedium = new List<CM_ShapeGroup>();
				if (iCloneSync.sayLarge == null) iCloneSync.sayLarge = new List<CM_ShapeGroup>();
				iCloneSync.GetShapeNames();
				iCloneSync.SetDefaultSmall();
				iCloneSync.SetDefaultMedium();
				iCloneSync.SetDefaultLarge();
				iCloneSync.initialize = false;
			}
		}

		public override void OnInspectorGUI()
		{
			// Minus 45 width for the scroll bar
			width = Screen.width - 50f;

			// Set dirty flags
			dirtySmall = false; 
			dirtyMedium = false;
			dirtyLarge = false;
			dirtyHair = false;

			// Keep trying to get the shapeNames until I've got them
			if (iCloneSync.GetShapeNames() == 0) iCloneSync.GetShapeNames();

			// Make sure the CM_ShapeGroups are initialized
			if (iCloneSync.saySmall == null) iCloneSync.saySmall = new System.Collections.Generic.List<CM_ShapeGroup>();
			if (iCloneSync.sayMedium == null) iCloneSync.sayMedium = new System.Collections.Generic.List<CM_ShapeGroup>();
			if (iCloneSync.sayLarge == null) iCloneSync.sayLarge = new System.Collections.Generic.List<CM_ShapeGroup>();

			GUILayout.Space(10);
			EditorGUILayout.BeginVertical(new GUILayoutOption[] {GUILayout.Width(width)});
			iCloneSync.salsa3D = EditorGUILayout.ObjectField(
				"Salsa3D", iCloneSync.salsa3D, typeof(Salsa3D), true) as Salsa3D;
			iCloneSync.randomEyes3D = EditorGUILayout.ObjectField(
				"RandomEyes3D", iCloneSync.randomEyes3D, typeof(RandomEyes3D), true) as RandomEyes3D;
			iCloneSync.body = EditorGUILayout.ObjectField(
				"Body", iCloneSync.body, typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
			iCloneSync.leftEyeBone = EditorGUILayout.ObjectField(
				"Left Eye Bone", iCloneSync.leftEyeBone, typeof(GameObject), true) as GameObject;
			iCloneSync.rightEyeBone = EditorGUILayout.ObjectField(
				"Right Eye Bone", iCloneSync.rightEyeBone, typeof(GameObject), true) as GameObject;
			iCloneSync.jawBone = EditorGUILayout.ObjectField(
				"Jaw Bone", iCloneSync.jawBone, typeof(GameObject), true) as GameObject;
			iCloneSync.jawRangeOfMotion = EditorGUILayout.Slider(
				"Jaw Range of Motion", iCloneSync.jawRangeOfMotion, 0f, 30f);
			EditorGUILayout.EndVertical();


			GUILayout.Space(20);


			EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
			EditorGUILayout.LabelField("Facial Hair (Beards, Moustaches, etc.)");
			if (GUILayout.Button("+", new GUILayoutOption[] {GUILayout.Width((addWidth/100)*width)}))
			{
				iCloneSync.facialHair.Add(new SkinnedMeshRenderer());
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.BeginVertical();
			if (iCloneSync.facialHair.Count > 0)
			{
				for (int i=0; i<iCloneSync.facialHair.Count; i++)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(width)});
					if (GUILayout.Button(
						new GUIContent("X", "Remove this SkinnedMeshRenderer from the list"), 
						new GUILayoutOption[] {GUILayout.Width((addWidth/100)*width)}))
					{
						iCloneSync.facialHair.RemoveAt(i);
						dirtyHair = true;
					}
					if (!dirtyHair) iCloneSync.facialHair[i] = EditorGUILayout.ObjectField(
						iCloneSync.facialHair[i], typeof(SkinnedMeshRenderer), true) as SkinnedMeshRenderer;
					GUILayout.EndHorizontal();
				}
			}
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();


			GUILayout.Space(10);
			GUILayout.Box("", new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)}); // Horizontal rule
			GUILayout.Space(10);


			if (iCloneSync.body)
			{
				EditorGUILayout.LabelField("SALSA shape groups");
				GUILayout.Space(10);

				EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
				EditorGUILayout.LabelField("SaySmall Shapes");
				if (GUILayout.Button("+", new GUILayoutOption[] {GUILayout.Width((addWidth/100)*width)}))
				{
					iCloneSync.saySmall.Add(new CM_ShapeGroup());
					iCloneSync.initialize = false;
				}
				EditorGUILayout.EndHorizontal();
				if (iCloneSync.saySmall.Count > 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
					EditorGUILayout.LabelField(
						new GUIContent("Delete", "Remove shape"), 
						GUILayout.Width((deleteWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("ShapeName", "BlendShape - (shapeIndex)"), 
						GUILayout.Width((shapeNameWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("Percentage", "The percentage of total range of motion for this shape"), 
						GUILayout.Width((percentageWidth/100)*width));
					GUILayout.EndHorizontal();

					for (int i=0; i<iCloneSync.saySmall.Count; i++)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(width)});
						if (GUILayout.Button(
							new GUIContent("X", "Remove this shape from the list (index:" + iCloneSync.saySmall[i].shapeIndex + ")"), 
							GUILayout.Width((deleteWidth/100)*width)))
						{
							iCloneSync.saySmall.RemoveAt(i);
							dirtySmall = true;
							break;
						}
						if (!dirtySmall)
						{
							iCloneSync.saySmall[i].shapeIndex = EditorGUILayout.Popup(
								iCloneSync.saySmall[i].shapeIndex, iCloneSync.shapeNames, 
								GUILayout.Width((shapeNameWidth/100)*width));
							iCloneSync.saySmall[i].shapeName = 
								iCloneSync.body.sharedMesh.GetBlendShapeName(iCloneSync.saySmall[i].shapeIndex);
							iCloneSync.saySmall[i].percentage = EditorGUILayout.Slider(
								iCloneSync.saySmall[i].percentage, 0f, 100f, 
								GUILayout.Width((percentageWidth/100)*width));
							iCloneSync.initialize = false;
						}
						GUILayout.EndHorizontal();
					}
				}

				GUILayout.Space(10);
				
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
				EditorGUILayout.LabelField("SayMedium Shapes");
				if (GUILayout.Button("+", new GUILayoutOption[] {GUILayout.Width((addWidth/100)*width)}))
				{
					iCloneSync.sayMedium.Add(new CM_ShapeGroup());
					iCloneSync.initialize = false;
				}
				EditorGUILayout.EndHorizontal();
				if (iCloneSync.sayMedium.Count > 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
					EditorGUILayout.LabelField(
						new GUIContent("Delete", "Remove shape"), 
						GUILayout.Width((deleteWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("ShapeName", "BlendShape - (shapeIndex)"), 
						GUILayout.Width((shapeNameWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("Percentage", "The percentage of total range of motion for this shape"), 
						GUILayout.Width((percentageWidth/100)*width));
					GUILayout.EndHorizontal();
					
					for (int i=0; i<iCloneSync.sayMedium.Count; i++)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(width)});
						if (GUILayout.Button(
							new GUIContent("X", "Remove this shape from the list (index:" + iCloneSync.sayMedium[i].shapeIndex + ")"), 
							GUILayout.Width((deleteWidth/100)*width)))
						{
							iCloneSync.sayMedium.RemoveAt(i);
							dirtyMedium = true;
							break;
						}
						if (!dirtyMedium)
						{
							iCloneSync.sayMedium[i].shapeIndex = EditorGUILayout.Popup(
								iCloneSync.sayMedium[i].shapeIndex, iCloneSync.shapeNames, 
								GUILayout.Width((shapeNameWidth/100)*width));
							iCloneSync.sayMedium[i].shapeName = 
								iCloneSync.body.sharedMesh.GetBlendShapeName(iCloneSync.sayMedium[i].shapeIndex);
							iCloneSync.sayMedium[i].percentage = EditorGUILayout.Slider(
								iCloneSync.sayMedium[i].percentage, 0f, 100f, 
								GUILayout.Width((percentageWidth/100)*width));
							iCloneSync.initialize = false;
						}
						GUILayout.EndHorizontal();
					}
				}
				
				GUILayout.Space(10);
				
				EditorGUILayout.BeginHorizontal(new GUILayoutOption[] {GUILayout.Width(width)});
				EditorGUILayout.LabelField("SayLarge Shapes");
				if (GUILayout.Button("+", new GUILayoutOption[] {GUILayout.Width((addWidth/100)*width)}))
				{
					iCloneSync.sayLarge.Add(new CM_ShapeGroup());
					iCloneSync.initialize = false;
				}
				EditorGUILayout.EndHorizontal();
				if (iCloneSync.sayLarge.Count > 0)
				{
					GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(true), GUILayout.Height(1)});
					EditorGUILayout.LabelField(
						new GUIContent("Delete", "Remove shape"), 
						GUILayout.Width((deleteWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("ShapeName", "BlendShape - (shapeIndex)"), 
						GUILayout.Width((shapeNameWidth/100)*width));
					EditorGUILayout.LabelField(
						new GUIContent("Percentage", "The percentage of total range of motion for this shape"), 
						GUILayout.Width((percentageWidth/100)*width));
					GUILayout.EndHorizontal();
					
					for (int i=0; i<iCloneSync.sayLarge.Count; i++)
					{
						GUILayout.BeginHorizontal(new GUILayoutOption[]{GUILayout.ExpandWidth(false), GUILayout.Width(width)});
						if (GUILayout.Button(
							new GUIContent("X", "Remove this shape from the list (index:" + iCloneSync.sayLarge[i].shapeIndex + ")"), 
							GUILayout.Width((deleteWidth/100)*width)))
						{
							iCloneSync.sayLarge.RemoveAt(i);
							dirtyLarge = true;
							break;
						}
						if (!dirtyLarge)
						{
							iCloneSync.sayLarge[i].shapeIndex = EditorGUILayout.Popup(
								iCloneSync.sayLarge[i].shapeIndex, iCloneSync.shapeNames, 
								GUILayout.Width((shapeNameWidth/100)*width));
							iCloneSync.sayLarge[i].shapeName = iCloneSync.body.sharedMesh.GetBlendShapeName(iCloneSync.sayLarge[i].shapeIndex);
							iCloneSync.sayLarge[i].percentage = EditorGUILayout.Slider(
								iCloneSync.sayLarge[i].percentage, 0f, 100f, 
								GUILayout.Width((percentageWidth/100)*width));
							iCloneSync.initialize = false;
						}
						GUILayout.EndHorizontal();
					}
				}
			}
		}
	}
}
