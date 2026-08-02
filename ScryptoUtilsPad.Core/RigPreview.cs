/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using UnityEngine;

namespace ScryptoUtilsPad.Core
{
	public class RigPreview : MonoBehaviour
	{
		public static ScryptoUtilsPad.Core.RigPreview Instance;

		private static readonly BepInEx.Logging.ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("RigPreview");

		private Transform _holder;

		private GameObject _preview;

		private VRRig _rig;

		private float _spin;

		internal float _nextBake;

		private Transform _pivot;

		private bool _warnedScale;

		public static bool AllowGrab;

		private Transform _grabHand;

		private Vector3 _grabLocalPos;

		private Quaternion _grabLocalRot;

		private bool _prevGrip;

		public static bool Enabled = true;

		public static float PreviewSize = 0.14f;

		public static float Yaw = 217f;

		public static float Pitch = 273f;

		public static float Roll = 323f;

		public static float OffsetX = 0.40f;

		public static float OffsetY = -0.11f;

		public static float OffsetZ = 0.03f;

		public static float SpinSpeed;

		public static void SetEnabled(bool on)
		{
			Enabled = on;
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigPreview", on ? 1 : 0);
			PlayerPrefs.Save();
			ScryptoUtilsPad.Core.RigPreview inst = Instance;
			if ((Object)(object)inst != (Object)null && !on)
			{
				inst.Clear();
			}
		}

		public static void ResetPlacement()
		{
			PreviewSize = 0.14f;
			Yaw = 217f;
			Pitch = 273f;
			Roll = 323f;
			OffsetX = 0.40f;
			OffsetY = -0.11f;
			OffsetZ = 0.03f;
			SpinSpeed = 0f;
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigSize", 14);
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigYaw", 217);
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigPitch", 273);
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigRoll", 323);
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigX", 40);
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigY", -11);
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigZ", 3);
			PlayerPrefs.SetInt("ScryptoUtilsPad.RigSpin", 0);
			PlayerPrefs.Save();
			ScryptoUtilsPad.Core.RigPreview inst = Instance;
			if ((Object)(object)inst != (Object)null)
			{
				inst._nextBake = 0f;
				inst.Bake();
			}
			Log.LogInfo("[RigPreview] Placement reset to defaults.");
		}

		public static void LoadPrefs()
		{
			Enabled = PlayerPrefs.GetInt("ScryptoUtilsPad.RigPreview", 1) == 1;
			PreviewSize = PlayerPrefs.GetInt("ScryptoUtilsPad.RigSize", 14) / 100f;
			Yaw = PlayerPrefs.GetInt("ScryptoUtilsPad.RigYaw", 217);
			Pitch = PlayerPrefs.GetInt("ScryptoUtilsPad.RigPitch", 273);
			Roll = PlayerPrefs.GetInt("ScryptoUtilsPad.RigRoll", 323);
			OffsetX = PlayerPrefs.GetInt("ScryptoUtilsPad.RigX", 40) / 100f;
			OffsetY = PlayerPrefs.GetInt("ScryptoUtilsPad.RigY", -11) / 100f;
			OffsetZ = PlayerPrefs.GetInt("ScryptoUtilsPad.RigZ", 3) / 100f;
			SpinSpeed = PlayerPrefs.GetInt("ScryptoUtilsPad.RigSpin", 0);
		}

		private void Awake()
		{
			Instance = this;
		}

		public void Init(Transform menuRoot)
		{
			if ((Object)(object)menuRoot == (Object)null)
			{
				return;
			}
			Transform secondary = menuRoot.Find("SecondaryMenu");
			if ((Object)(object)secondary == (Object)null)
			{
				return;
			}
			_holder = secondary.Find("PlayerHolder");
			if ((Object)(object)_holder == (Object)null)
			{
				Log.LogWarning("[RigPreview] PlayerHolder not found on SecondaryMenu.");
			}
			else
			{
				Log.LogInfo("[RigPreview] Ready — PlayerHolder found.");
			}
		}

		public void Show(VRRig rig)
		{
			if (!Enabled)
			{
				Clear();
				return;
			}
			if ((Object)(object)rig == (Object)(object)_rig && (Object)(object)_preview != (Object)null)
			{
				return;
			}
			_rig = rig;
			_nextBake = Time.time + 0.5f;
			Bake();
		}

		public void Clear()
		{
			_rig = null;
			if ((Object)(object)_preview != (Object)null)
			{
				Object.Destroy(_preview);
				_preview = null;
			}
		}

		private static Transform HandOf(bool right)
		{
			GorillaLocomotion.GTPlayer p = GorillaLocomotion.GTPlayer.Instance;
			if ((Object)(object)p == (Object)null)
			{
				return null;
			}
			return (right ? p.RightHand.controllerTransform : p.LeftHand.controllerTransform);
		}

		private void HandleGrab()
		{
			if (!AllowGrab || (Object)(object)_preview == (Object)null)
			{
				return;
			}
			ControllerInputPoller poller = ControllerInputPoller.instance;
			if ((Object)(object)poller == (Object)null)
			{
				return;
			}
			bool rightGrip = poller.rightGrab;
			bool leftGrip = poller.leftGrab;
			bool grip = rightGrip || leftGrip;

			if (grip && !_prevGrip && (Object)(object)_grabHand == (Object)null)
			{
				Transform hand = HandOf(rightGrip);
				if ((Object)(object)hand != (Object)null)
				{
					float d = Vector3.Distance(hand.position, _preview.transform.position);
					if (d <= 0.18f)
					{
						_grabHand = hand;
						_grabLocalPos = hand.InverseTransformPoint(_preview.transform.position);
						_grabLocalRot = Quaternion.Inverse(hand.rotation) * _preview.transform.rotation;
						Log.LogInfo("[RigPreview] Grabbed — move it, then release to lock.");
					}
				}
			}

			if ((Object)(object)_grabHand != (Object)null)
			{
				if (grip)
				{
					_preview.transform.position = _grabHand.TransformPoint(_grabLocalPos);
					_preview.transform.rotation = _grabHand.rotation * _grabLocalRot;
				}
				else
				{
					_grabHand = null;
					float parentScale = Mathf.Abs(_holder.lossyScale.x);
					if (parentScale < 0.0001f)
					{
						parentScale = 1f;
					}
					Vector3 lp = _preview.transform.localPosition * parentScale * 100f;
					Vector3 le = _preview.transform.localEulerAngles;
					OffsetX = lp.x / 100f;
					OffsetY = lp.y / 100f;
					OffsetZ = lp.z / 100f;
					Pitch = Mathf.Round(le.x);
					Yaw = Mathf.Round(le.y);
					Roll = Mathf.Round(le.z);
					PlayerPrefs.SetInt("ScryptoUtilsPad.RigX", Mathf.RoundToInt(lp.x));
					PlayerPrefs.SetInt("ScryptoUtilsPad.RigY", Mathf.RoundToInt(lp.y));
					PlayerPrefs.SetInt("ScryptoUtilsPad.RigZ", Mathf.RoundToInt(lp.z));
					PlayerPrefs.SetInt("ScryptoUtilsPad.RigPitch", Mathf.RoundToInt(Pitch));
					PlayerPrefs.SetInt("ScryptoUtilsPad.RigYaw", Mathf.RoundToInt(Yaw));
					PlayerPrefs.SetInt("ScryptoUtilsPad.RigRoll", Mathf.RoundToInt(Roll));
					PlayerPrefs.Save();
					Log.LogInfo("[RigPreview] LOCKED >>> RigX = " + Mathf.RoundToInt(lp.x)
						+ " | RigY = " + Mathf.RoundToInt(lp.y)
						+ " | RigZ = " + Mathf.RoundToInt(lp.z)
						+ " | RigPitch = " + Mathf.RoundToInt(Pitch)
						+ " | RigYaw = " + Mathf.RoundToInt(Yaw)
						+ " | RigRoll = " + Mathf.RoundToInt(Roll)
						+ "   (copy these into SUPC_config.txt)");
				}
			}
			_prevGrip = grip;
		}

		internal void Bake()
		{
			if ((Object)(object)_holder == (Object)null || (Object)(object)_rig == (Object)null)
			{
				return;
			}
			SkinnedMeshRenderer skin = _rig.mainSkin;
			if ((Object)(object)skin == (Object)null)
			{
				return;
			}
			if ((Object)(object)_preview != (Object)null)
			{
				Object.Destroy(_preview);
				_preview = null;
			}

			Mesh baked = new Mesh();
			skin.BakeMesh(baked, true);

			if (!_holder.gameObject.activeSelf)
			{
				_holder.gameObject.SetActive(true);
			}
			_preview = new GameObject("SUPCRigPreview");
			_preview.layer = _holder.gameObject.layer;
			_preview.transform.SetParent(_holder, false);
			_preview.transform.localPosition = Vector3.zero;
			_preview.transform.localRotation = Quaternion.identity;

			GameObject pivot = new GameObject("Model");
			pivot.layer = _preview.layer;
			pivot.transform.SetParent(_preview.transform, false);
			_pivot = pivot.transform;
			MeshFilter mf = pivot.AddComponent<MeshFilter>();
			mf.sharedMesh = baked;
			MeshRenderer mr = pivot.AddComponent<MeshRenderer>();
			Material[] src = skin.sharedMaterials;
			Material[] copies = new Material[(src != null) ? src.Length : 0];
			for (int mi = 0; mi < copies.Length; mi++)
			{
				if ((Object)(object)src[mi] == (Object)null)
				{
					continue;
				}
				copies[mi] = new Material(src[mi]);
			}
			mr.sharedMaterials = copies;
			try
			{
				MaterialPropertyBlock mpb = new MaterialPropertyBlock();
				skin.GetPropertyBlock(mpb);
				mr.SetPropertyBlock(mpb);
			}
			catch
			{
			}
			Color pc = _rig.playerColor;
			for (int ci = 0; ci < copies.Length; ci++)
			{
				Material m = copies[ci];
				if ((Object)(object)m == (Object)null)
				{
					continue;
				}
				if (m.HasProperty("_BaseColor"))
				{
					m.SetColor("_BaseColor", pc);
				}
				if (m.HasProperty("_Color"))
				{
					m.SetColor("_Color", pc);
				}
			}
			mr.shadowCastingMode = (UnityEngine.Rendering.ShadowCastingMode)0;
			mr.receiveShadows = false;

			Bounds b = baked.bounds;
			float size = Mathf.Max(b.size.x, Mathf.Max(b.size.y, b.size.z));
			float parentScale = Mathf.Abs(_holder.lossyScale.x);
			if (parentScale < 0.0001f)
			{
				parentScale = 1f;
			}
			float scale = ((size > 0.0001f) ? (PreviewSize / (size * parentScale)) : 1f);
			_preview.transform.localScale = Vector3.one * scale;
			_preview.transform.localPosition = new Vector3(OffsetX, OffsetY, OffsetZ) / parentScale;
			_preview.transform.localRotation = Quaternion.Euler(Pitch, Yaw, Roll);
			_pivot.localPosition = -b.center;
			{
				NetPlayer who = _rig.creator;
				Log.LogInfo("[RigPreview] Built for " + ((who != null) ? who.SanitizedNickName : "?") + " - verts=" + baked.vertexCount + " scale=" + scale.ToString("0.000")
					+ " meshSize=" + b.size.ToString("0.00") + " meshCenter=" + b.center.ToString("0.00")
					+ " | holder active=" + _holder.gameObject.activeInHierarchy
					+ " layer=" + _holder.gameObject.layer
					+ " holderLossyScale=" + _holder.lossyScale.ToString("0.000")
					+ " holderWorldPos=" + _holder.position.ToString("0.00")
					+ " | worldSize=" + (size * scale * parentScale).ToString("0.000")
					+ " previewWorldPos=" + _preview.transform.position.ToString("0.00")
					+ " mats=" + ((mr.sharedMaterials != null) ? mr.sharedMaterials.Length : 0)
					+ " mat0=" + ((mr.sharedMaterial != null) ? ((Object)mr.sharedMaterial).name : "null")
					+ " shader=" + (((Object)(object)mr.sharedMaterial != (Object)null && (Object)(object)mr.sharedMaterial.shader != (Object)null) ? ((Object)mr.sharedMaterial.shader).name : "null"));
			}
		}

		private void Update()
		{
			if ((Object)(object)_preview == (Object)null || (Object)(object)_holder == (Object)null)
			{
				return;
			}
			if ((Object)(object)_rig == (Object)null)
			{
				Clear();
				return;
			}
			if (_holder.lossyScale.x < 0.0001f && !_warnedScale)
			{
				_warnedScale = true;
				Log.LogWarning("[RigPreview] PlayerHolder lossyScale is ~0 — preview cannot be seen. Holder is collapsed.");
			}
			if (SpinSpeed > 0.01f)
			{
				_spin += Time.deltaTime * SpinSpeed;
			}
			else
			{
				_spin = 0f;
			}
			if ((Object)(object)_grabHand == (Object)null)
			{
				_preview.transform.localRotation = Quaternion.Euler(Pitch, Yaw + _spin, Roll);
			}
			HandleGrab();
			if (Time.time >= _nextBake && (Object)(object)_grabHand == (Object)null)
			{
				_nextBake = Time.time + 0.5f;
				float keep = _spin;
				Bake();
				_spin = keep;
			}
		}
	}
}
