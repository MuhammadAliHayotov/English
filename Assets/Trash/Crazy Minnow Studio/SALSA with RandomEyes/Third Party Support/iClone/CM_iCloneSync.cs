using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CrazyMinnow.SALSA;

namespace CrazyMinnow.SALSA.iClone
{
	/// <summary>
	/// This script acts as a proxy between SALSA with RandomEyes and iClone characters,
	/// and allows users to link SALSA with RandomEyes to iClone characters without any model
	/// modifications.
	/// 
    /// Good default inspector values
    /// Salsa3D
    /// 	Trigger values will depend on your recordings
    /// 	Blend Speed: 10
    /// 	Range of Motion: 75
    /// RandomEyes3D
    /// 	Range of Motion: 60
    /// </summary>
	/// 
	/// Crazy Minnow Studio, LLC
	/// CrazyMinnowStudio.com
	/// 
	/// NOTE:While every attempt has been made to ensure the safe content and operation of 
	/// these files, they are provided as-is, without warranty or guarantee of any kind. 
	/// By downloading and using these files you are accepting any and all risks associated 
	/// and release Crazy Minnow Studio, LLC of any and all liability.
	[AddComponentMenu("Crazy Minnow Studio/iClone/CM_iCloneSync")]
	public class CM_iCloneSync : MonoBehaviour 
	{
		public Salsa3D salsa3D; // Salsa3D mouth component
		public RandomEyes3D randomEyes3D; // RandomEyes3D eye componet
		public SkinnedMeshRenderer body; // iClone character body SkinnedMeshRenderer
		public string leftEyeBoneName = "L_Eye"; // Used in search for left eye bone
		public GameObject leftEyeBone; // Left eye bone
		public string rightEyeBoneName = "R_Eye"; // Used in search for right eye bone
		public GameObject rightEyeBone; // Right eye bone
		public string jawBoneName = "JawRoot"; // Use in search for jaw bone
		public GameObject jawBone; // Jaw bone
		[Range(0f, 30f)]
		public float jawRangeOfMotion = 15f; // Total jaw range of motion
		public List<SkinnedMeshRenderer> facialHair = new List<SkinnedMeshRenderer>(); // Facial hair SkinnedMeshRenderer list
		public string beardName = "Beards"; // Used in search for beards
		public string moustacheName = "Moustaches"; // Used in search for moustaches
		public List<CM_ShapeGroup> saySmall = new List<CM_ShapeGroup>(); // saySmall shape group
		public List<CM_ShapeGroup> sayMedium = new List<CM_ShapeGroup>(); // sayMedium shape group
		public List<CM_ShapeGroup> sayLarge = new List<CM_ShapeGroup>(); // sayLarge shape group
		public string[] shapeNames; // Shape name string array for name picker popups
		public bool initialize = true; // Initialize once

		private Transform[] children; // For searching through child objects during initialization
		private float srcShapeWeight = 0f; // Shape weight for whichever shape is currently being managed
		private float eyeSensativity = 500f; // Eye movement reduction from shape value to bone transform value
		private string blinkShape = "Eyes_Blink"; // Blink BlendShape name
		private int blinkIndex = -1; // Blendshape index for the blin
		private float blinkWeight; // Blink weight is applied to the body Blink_Left and Blink_Right BlendShapes
		private float vertical; // Vertical eye bone movement amount
		private float horizontal; // Horizontal eye bone movement amount
		private bool lockShapes; // Used to allow access to shape group shapes when SALSA is not talking
		private Vector3 jawRest; // Rest local eular rotation
		private float jawAmount; // Current movement amount


		/// <summary>
		/// Reset the component to default values
		/// </summary>
		void Reset()
		{
			initialize = true;
			GetSalsa3D();
			GetRandomEyes3D();
			GetBody();
			GetEyeBones();
			GetJawBone();
			GetFacialHair();
			if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
			if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
			if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
			GetShapeNames();
			SetDefaultSmall();
			SetDefaultMedium();
			SetDefaultLarge();
		}

        /// <summary>
        /// Initial setup
        /// </summary>
		void Start()
		{
			// Initialize
			GetSalsa3D();
			GetRandomEyes3D();
			GetBody();
			GetEyeBones();
			GetJawBone();
			jawRest = jawBone.transform.localEulerAngles;
			jawAmount = jawRest.z;
			if (facialHair.Count == 0) GetFacialHair();
			if (saySmall == null) saySmall = new List<CM_ShapeGroup>();
			if (sayMedium == null) sayMedium = new List<CM_ShapeGroup>();
			if (sayLarge == null) sayLarge = new List<CM_ShapeGroup>();
			GetShapeNames();
		}

        /// <summary>
        /// Perform the blendshape changes in LateUpdate for mechanim compatibility
        /// </summary>
		void LateUpdate() 
		{
			// Toggle shape lock to provide access to shape group shapes when SALSA is not talking
			if (salsa3D)
			{
				if (salsa3D.sayAmount.saySmall == 0f && salsa3D.sayAmount.sayMedium == 0f && salsa3D.sayAmount.sayLarge == 0f)
				{
					lockShapes = false;
				}
				else
				{
					lockShapes = true;
				}
			}

            // Sync SALSA shapes
			if (salsa3D && body && lockShapes)
			{				
				for (int i=0; i<saySmall.Count; i++)
				{
					body.SetBlendShapeWeight(
						saySmall[i].shapeIndex, ((saySmall[i].percentage/100)*salsa3D.sayAmount.saySmall));
				}
				for (int i=0; i<sayMedium.Count; i++)
				{
					body.SetBlendShapeWeight(
						sayMedium[i].shapeIndex, ((sayMedium[i].percentage/100)*salsa3D.sayAmount.sayMedium));
				}			
				for (int i=0; i<sayLarge.Count; i++)
				{
					body.SetBlendShapeWeight(
						sayLarge[i].shapeIndex, ((sayLarge[i].percentage/100)*salsa3D.sayAmount.sayLarge));
				}

				// Sync jaw bone movement
				switch (salsa3D.sayIndex)
				{
					case 0: // sayRest
						jawAmount = Mathf.Lerp(jawAmount, jawRest.z, Time.deltaTime * salsa3D.blendSpeed);
						break;
					case 1: // saySmall
						jawAmount = Mathf.Lerp(jawAmount, jawRest.z - (jawRangeOfMotion/4), Time.deltaTime * salsa3D.blendSpeed);
						break;
					case 2: // sayMedium
						jawAmount = Mathf.Lerp(jawAmount, jawRest.z - 1, Time.deltaTime * salsa3D.blendSpeed);
						break;
					case 3: // sayLarge
						jawAmount = Mathf.Lerp(jawAmount, jawRest.z - jawRangeOfMotion, Time.deltaTime * salsa3D.blendSpeed);
						break;
				}
				jawBone.transform.localEulerAngles = new Vector3(jawRest.x, jawRest.y, jawAmount);
			}

            // Sync blink, eye movement, and custom shapes
            if (randomEyes3D)
            {
                blinkWeight = randomEyes3D.lookAmount.blink;

                // Apply blink action on eye lids
                if (body)
                {
					if (blinkIndex == -1) blinkIndex = ShapeSearch(body, blinkShape);
					if (blinkIndex != -1) body.SetBlendShapeWeight(blinkIndex, blinkWeight);
                }

                // Apply look amount to bone rotation
                if (leftEyeBone || rightEyeBone)
                {
                    // Apply eye movement weight direction variables
                    if (randomEyes3D.lookAmount.lookUp > 0)
                        vertical = -(randomEyes3D.lookAmount.lookUp / eyeSensativity) * randomEyes3D.rangeOfMotion;
                    if (randomEyes3D.lookAmount.lookDown > 0)
                        vertical = (randomEyes3D.lookAmount.lookDown / eyeSensativity) * randomEyes3D.rangeOfMotion;
                    if (randomEyes3D.lookAmount.lookLeft > 0)
                        horizontal = -(randomEyes3D.lookAmount.lookLeft / eyeSensativity) * randomEyes3D.rangeOfMotion;
                    if (randomEyes3D.lookAmount.lookRight > 0)
                        horizontal = (randomEyes3D.lookAmount.lookRight / eyeSensativity) * randomEyes3D.rangeOfMotion;

                    // Set eye bone rotations
                    if (leftEyeBone) leftEyeBone.transform.localRotation = Quaternion.Euler(vertical, 0, horizontal);
                    if (rightEyeBone) rightEyeBone.transform.localRotation = Quaternion.Euler(vertical, 0, horizontal);
                }

                // Sync custom shape weights to all other SkinnedMeshRenderers
                for (int i = 0; i < body.sharedMesh.blendShapeCount; i++)
                {
                    srcShapeWeight = body.GetBlendShapeWeight(i);

                    if (facialHair.Count > 0)
                    {
                        for (int fh = 0; fh < facialHair.Count; fh++)
                        {
                            if (i < facialHair[fh].sharedMesh.blendShapeCount)
                            {
                                facialHair[fh].SetBlendShapeWeight(i, srcShapeWeight);
                            }
                        }

                    }
                }
            }
		}

		/// <summary>
		/// Call this when initializing characters at runtime
		/// </summary>
		public void Initialize()
		{
			Reset();
        }

		/// <summary>
		/// Get the Salsa3D component
		/// </summary>
		public void GetSalsa3D()
		{
			if (!salsa3D) salsa3D = GetComponent<Salsa3D>();
		}

		/// <summary>
		/// Get the RandomEyes3D component
		/// </summary>
		public void GetRandomEyes3D()
		{
			//if (!randomEyes3D) randomEyes3D = GetComponent<RandomEyes3D>();

            RandomEyes3D[] randomEyes = GetComponents<RandomEyes3D>();
            if (randomEyes.Length > 1)
            {
                for (int i = 0; i < randomEyes.Length; i++)
                {
                    // Verify this instance ID does not match the reEyes instance ID
                    if (!randomEyes[i].useCustomShapesOnly)
                    {
                        // Set the reShapes instance
                        randomEyes3D = randomEyes[i];
                    }
                }
            }
		}

		/// <summary>
		/// Find the Body child object SkinnedMeshRenderer
		/// </summary>
		public void GetBody()
		{
			Transform bodyTrans = ChildSearch("Body");
			if (bodyTrans)
			{
				if (!body) body = bodyTrans.GetComponent<SkinnedMeshRenderer>();
			}
		}

		/// <summary>
		/// Find left and right eye bones
		/// </summary>
		public void GetEyeBones()
		{
			Transform leftEyeTrans = ChildSearch(leftEyeBoneName);
			if(leftEyeTrans) 
			{
				if (!leftEyeBone) leftEyeBone = leftEyeTrans.gameObject;
			}
			Transform rightEyeTrans = ChildSearch(rightEyeBoneName);
			if (rightEyeTrans) 
			{
				if (!rightEyeBone) rightEyeBone = rightEyeTrans.gameObject;
			}
		}

		/// <summary>
		/// Find the jaw bone
		/// </summary>
		public void GetJawBone()
		{
			Transform jawTrans = ChildSearch(jawBoneName);
			if (jawTrans)
			{
				if (!jawBone) jawBone = jawTrans.gameObject;
			}
		}

		/// <summary>
		/// Search for beard and moustache SkinnedMeshRenderers
		/// </summary>
		public void GetFacialHair()
		{
			facialHair = new List<SkinnedMeshRenderer>();

			Transform beard = ChildSearch(beardName);
			if (beard) facialHair.Add(beard.GetComponent<SkinnedMeshRenderer>());
			Transform moustache = ChildSearch(moustacheName);
			if (moustache) facialHair.Add(moustache.GetComponent<SkinnedMeshRenderer>());
		}

        /// <summary>
        /// Find a child by name that ends with the search string. 
        /// This should compensates for BlendShape name prefixes variations.
        /// </summary>
        /// <param name="endsWith"></param>
        /// <returns></returns>
		public Transform ChildSearch(string endsWith)
		{
			Transform trans = null;

			children = transform.gameObject.GetComponentsInChildren<Transform>();

			for (int i=0; i<children.Length; i++)
			{
                if (children[i].name.EndsWith(endsWith)) trans = children[i];
			}

			return trans;
		}	

		/// <summary>
        /// Find a shape by name, that contains the search string.
		/// </summary>
		/// <param name="skndMshRndr"></param>
		/// <param name="endsWith"></param>
		/// <returns></returns>
		public int ShapeSearch(SkinnedMeshRenderer skndMshRndr, string contains)
		{
			int index = -1;
			if (skndMshRndr)
			{
				for (int i=0; i<skndMshRndr.sharedMesh.blendShapeCount; i++)
				{
                    if (skndMshRndr.sharedMesh.GetBlendShapeName(i).Contains(contains))
					{
						index = i;
						break;
					}
				}
			}
			return index;
		}

		/// <summary>
		/// Populate the shapeName popup list
		/// </summary>
		public int GetShapeNames()
		{
			int nameCount = 0;

			if (body)
			{
				shapeNames = new string[body.sharedMesh.blendShapeCount];
				for (int i=0; i<body.sharedMesh.blendShapeCount; i++)
				{
					shapeNames[i] = body.sharedMesh.GetBlendShapeName(i);
					if (shapeNames[i] != "") nameCount++;
				}
			}

			return nameCount;
		}

        /// <summary>
        /// Set the default saySmall shape group
        /// </summary>
        public void SetDefaultSmall()
        {
            int index = -1;
            string name = "";

            saySmall = new List<CM_ShapeGroup>();

            index = ShapeSearch(body, "Affricate");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                saySmall.Add(new CM_ShapeGroup(index, name, 40f));
            }
		}
        /// <summary>
        /// Set the default sayMedium shape group
        /// </summary>
        public void SetDefaultMedium()
        {
            int index = -1; ;
            string name = "";

            sayMedium = new List<CM_ShapeGroup>();

            index = ShapeSearch(body, "Explosive");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayMedium.Add(new CM_ShapeGroup(index, name, 100f));
            }

			index = ShapeSearch(body, "Wide");
			if (index != -1)
			{
				name = body.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 100f));
			}

			index = ShapeSearch(body, "Lip_Raise_Top");
			if (index != -1)
			{
				name = body.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 30f));
			}

			index = ShapeSearch(body, "Lips_Tuck");
			if (index != -1)
			{
				name = body.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 20f));
			}

			index = ShapeSearch(body, "Lips_Open");
			if (index != -1)
			{
				name = body.sharedMesh.GetBlendShapeName(index);
				sayMedium.Add(new CM_ShapeGroup(index, name, 60f));
			}
		}
        /// <summary>
        /// Set the default sayLarge shape group
        /// </summary>
        public void SetDefaultLarge()
        {
            int index = -1; ;
            string name = "";

            sayLarge = new List<CM_ShapeGroup>();

			index = ShapeSearch(body, "Lips_Drop");
			if (index != -1)
			{
				name = body.sharedMesh.GetBlendShapeName(index);
				sayLarge.Add(new CM_ShapeGroup(index, name, 30f));
			}

			index = ShapeSearch(body, "Lips_Puckered_Open");
            if (index != -1)
            {
                name = body.sharedMesh.GetBlendShapeName(index);
                sayLarge.Add(new CM_ShapeGroup(index, name, 90f));
            }
        }
	}

	/// <summary>
	/// Shape index and percentage class for SALSA/iClone shape groups
	/// </summary>
	[System.Serializable]
    public class CM_ShapeGroup
    {
        public int shapeIndex;
        public string shapeName;
        public float percentage;

        public CM_ShapeGroup()
        {
            this.shapeIndex = 0;
            this.shapeName = "";
            this.percentage = 100f;
        }

        public CM_ShapeGroup(int shapeIndex, string shapeName, float percentage)
        {
            this.shapeIndex = shapeIndex;
            this.shapeName = shapeName;
            this.percentage = percentage;
        }
    }
}
