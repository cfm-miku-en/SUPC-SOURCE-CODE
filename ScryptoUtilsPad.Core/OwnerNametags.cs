using System;
using System.Collections.Generic;
using BepInEx.Logging;
namespace ScryptoUtilsPad.Core
{
	public static class OwnerNametags
	{
		
		private static readonly ManualLogSource Log = Logger.CreateLogSource("OwnerNametags");

		private const string OwnerTagText = "Owner";

		private static readonly Color32 OwnerTagColor = new Color32((byte)255, (byte)196, (byte)32, (byte)255);

		private static readonly Color32 OwnerTagOutline = new Color32((byte)70, (byte)45, (byte)0, (byte)255);

		private static readonly Dictionary<VRRig, TextMeshPro> _ownerTags = new Dictionary<VRRig, TextMeshPro>();

		private static readonly List<VRRig> _stale = new List<VRRig>();

		private static float _scanTimer;

		public static bool IsOwner(string userId)
		{
			string label;
			return ScryptoUtilsPad.Core.OwnerData.TryGetLabel(userId, out label);
		}

		public static string LabelFor(string userId)
		{
			string label;
			if (ScryptoUtilsPad.Core.OwnerData.TryGetLabel(userId, out label))
			{
				return label;
			}
			return OwnerTagText;
		}

		public static void Reset()
		{
			ClearTags();
		}

		public static void RefreshFont()
		{
			TMP_FontAsset font = ScryptoUtilsPad.Core.NametagFontSettings.CurrentFont;
			if ((UnityEngine.Object)(object)font == (UnityEngine.Object)null)
			{
				return;
			}
			foreach (KeyValuePair<VRRig, TextMeshPro> pair in _ownerTags)
			{
				if ((UnityEngine.Object)(object)pair.Value != (UnityEngine.Object)null)
				{
					((TMP_Text)pair.Value).font = font;
				}
			}
		}

		public static void Tick(float deltaTime)
		{
			_scanTimer -= deltaTime;
			if (_scanTimer <= 0f)
			{
				_scanTimer = 2f;
				ScanOwnerRigs();
			}
			UpdateOwnerTags();
		}

		private static void ScanOwnerRigs()
		{
			try
			{
				VRRig[] rigs = System.Linq.Enumerable.ToArray<VRRig>(VRRigCache.ActiveRigs);
				for (int i = 0; i < rigs.Length; i++)
				{
					VRRig rig = rigs[i];
					if ((UnityEngine.Object)(object)rig == (UnityEngine.Object)null || _ownerTags.ContainsKey(rig))
					{
						continue;
					}
					try
					{
						if (rig.isLocal)
						{
							continue;
						}
					}
					catch
					{
						continue;
					}
					NetPlayer creator = rig.creator;
					string ownerLabel;
					if (ScryptoUtilsPad.Core.OwnerData.TryGetLabel((creator != null) ? creator.UserId : null, out ownerLabel))
					{
						TextMeshPro newTag = CreateOwnerTag();
						if ((UnityEngine.Object)(object)newTag != (UnityEngine.Object)null && !string.IsNullOrEmpty(ownerLabel))
						{
							((TMP_Text)newTag).text = ownerLabel;
						}
						_ownerTags[rig] = newTag;
					}
				}
			}
			catch (Exception ex)
			{
				Log.LogWarning("[OwnerNametags] ScanOwnerRigs error: " + ex.Message);
			}
		}

		private static void UpdateOwnerTags()
		{
			if (_ownerTags.Count == 0)
			{
				return;
			}
			Camera main = Camera.main;
			Transform camera = ((main != null) ? ((Component)main).transform : null);
			_stale.Clear();
			foreach (KeyValuePair<VRRig, TextMeshPro> pair in _ownerTags)
			{
				VRRig rig = pair.Key;
				TextMeshPro tag = pair.Value;
				if ((UnityEngine.Object)(object)rig == (UnityEngine.Object)null || (UnityEngine.Object)(object)tag == (UnityEngine.Object)null)
				{
					_stale.Add(rig);
					continue;
				}
				try
				{
					NetPlayer creator = rig.creator;
					if (!IsOwner((creator != null) ? creator.UserId : null))
					{
						_stale.Add(rig);
						continue;
					}
					bool visible = ((Component)rig).gameObject.activeInHierarchy;
					((Component)tag).gameObject.SetActive(visible);
					if (!visible)
					{
						continue;
					}
					GameObject headMesh = rig.headMesh;
					Transform head = ((headMesh != null) ? headMesh.transform : null) ?? ((Component)rig).transform;
					float scale = ((rig.scaleFactor > 0f) ? rig.scaleFactor : 1f);
					float height = (ScryptoUtilsPad.Core.NametagManager.Enabled ? 0.62f : 0.48f) * scale;
					tag.transform.position = head.position + Vector3.up * height;
					FaceCamera(tag, camera);
				}
				catch (Exception ex)
				{
					Log.LogWarning("[OwnerNametags] UpdateOwnerTags error on rig: " + ex.Message);
				}
			}
			for (int j = 0; j < _stale.Count; j++)
			{
				DestroyTag(_stale[j]);
			}
			_stale.Clear();
		}

		private static TextMeshPro CreateOwnerTag()
		{
			GameObject val = new GameObject("ScryptoOwnerTag");
			TextMeshPro val2 = val.AddComponent<TextMeshPro>();
			((TMP_Text)val2).fontSize = 1f;
			((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
			((TMP_Text)val2).text = OwnerTagText;
			((TMP_Text)val2).color = OwnerTagColor;
			TMP_FontAsset font = ScryptoUtilsPad.Core.NametagFontSettings.CurrentFont;
			if ((UnityEngine.Object)(object)font != (UnityEngine.Object)null)
			{
				((TMP_Text)val2).font = font;
			}
			try
			{
				((TMP_Text)val2).fontStyle = (FontStyles)1;
				((TMP_Text)val2).outlineWidth = 0.25f;
				((TMP_Text)val2).outlineColor = OwnerTagOutline;
			}
			catch
			{
			}
			return val2;
		}

		private static void DestroyTag(VRRig rig)
		{
			if ((object)rig == null)
			{
				return;
			}
			TextMeshPro tag;
			if (_ownerTags.TryGetValue(rig, out tag))
			{
				if ((UnityEngine.Object)(object)tag != (UnityEngine.Object)null)
				{
					UnityEngine.Object.Destroy((UnityEngine.Object)(object)((Component)tag).gameObject);
				}
				_ownerTags.Remove(rig);
			}
		}

		private static void ClearTags()
		{
			foreach (KeyValuePair<VRRig, TextMeshPro> pair in _ownerTags)
			{
				if ((UnityEngine.Object)(object)pair.Value != (UnityEngine.Object)null)
				{
					UnityEngine.Object.Destroy((UnityEngine.Object)(object)((Component)pair.Value).gameObject);
				}
			}
			_ownerTags.Clear();
		}

		private static void FaceCamera(TextMeshPro text, Transform camera)
		{
			if ((UnityEngine.Object)(object)camera == (UnityEngine.Object)null)
			{
				return;
			}
			Vector3 dir = text.transform.position - camera.position;
			if (dir.sqrMagnitude > 0.0001f)
			{
				text.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
			}
		}
	}

	public class OwnerNametagsDriver : MonoBehaviour
	{
		private void Update()
		{
			ScryptoUtilsPad.Core.OwnerNametags.Tick(Time.deltaTime);
		}
	}
}
