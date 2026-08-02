namespace ScryptoUtilsPad.Core
{
	public class NametagManager : MonoBehaviour
	{
		public static ScryptoUtilsPad.Core.NametagManager Instance;

		private readonly System.Collections.Generic.Dictionary<VRRig, TextMeshPro> _tags = new System.Collections.Generic.Dictionary<VRRig, TextMeshPro>();

		private float _scanTimer = 2f;

		private static readonly ManualLogSource Log = Logger.CreateLogSource("NametagManager");

		public static void Refresh()
		{
			ScryptoUtilsPad.Core.NametagManager inst = Instance;
			if ((Object)(object)inst == (Object)null)
			{
				return;
			}
			foreach (TextMeshPro tag in inst._tags.Values)
			{
				if ((Object)(object)tag != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)tag).gameObject);
				}
			}
			inst._tags.Clear();
		}

		public static bool Enabled
		{
			get
			{
				return PlayerPrefs.GetInt("ScryptoUtilsPad.Nametags", 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("ScryptoUtilsPad.Nametags", value ? 1 : 0);
			}
		}

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			_scanTimer -= Time.deltaTime;
			if (_scanTimer <= 0f)
			{
				_scanTimer = 2f;
				ScanRigs();
			}
			UpdateTags();
		}

		private void ScanRigs()
		{
			System.Collections.Generic.List<VRRig> list = new System.Collections.Generic.List<VRRig>();
			System.Collections.Generic.Dictionary<VRRig, TextMeshPro>.KeyCollection.Enumerator enumerator = _tags.Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VRRig current = enumerator.Current;
					if ((Object)(object)current == (Object)null)
					{
						list.Add(current);
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator).Dispose();
			}
			System.Collections.Generic.List<VRRig>.Enumerator enumerator2 = list.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					VRRig current2 = enumerator2.Current;
					if ((Object)(object)_tags[current2] != (Object)null)
					{
						Object.Destroy((Object)(object)((Component)_tags[current2]).gameObject);
					}
					_tags.Remove(current2);
				}
			}
			finally
			{
				((System.IDisposable)enumerator2).Dispose();
			}
			try
			{
				VRRig[] array = System.Linq.Enumerable.ToArray<VRRig>(VRRigCache.ActiveRigs);
				int num = 0;
				while (num < array.Length)
				{
					VRRig val = array[num];
					if (!((Object)(object)val == (Object)null) && !_tags.ContainsKey(val))
					{
						try
						{
							if (val.isLocal)
							{
								goto IL_0125;
							}
						}
						catch
						{
							goto IL_0125;
						}
						_tags[val] = CreateTag();
					}
					goto IL_0125;
					IL_0125:
					num++;
				}
			}
			catch (System.Exception ex)
			{
				Log.LogWarning((object)string.Concat("[NametagManager] ScanRigs error: ", ex.Message));
			}
		}

		private static TextMeshPro CreateTag()
		{
			GameObject val = new GameObject("ScryptoNametag");
			TextMeshPro val2 = val.AddComponent<TextMeshPro>();
			((TMP_Text)val2).fontSize = 1.2f;
			((TMP_Text)val2).alignment = (TextAlignmentOptions)514;
			TMP_FontAsset font = ScryptoUtilsPad.Core.NametagFontSettings.CurrentFont;
			if ((Object)(object)font != (Object)null)
			{
				((TMP_Text)val2).font = font;
			}
			try
			{
				((TMP_Text)val2).outlineWidth = 0.25f;
			}
			catch
			{
			}
			return val2;
		}

		public void RefreshFont()
		{
			TMP_FontAsset font = ScryptoUtilsPad.Core.NametagFontSettings.CurrentFont;
			if ((Object)(object)font == (Object)null)
			{
				return;
			}
			System.Collections.Generic.Dictionary<VRRig, TextMeshPro>.ValueCollection.Enumerator enumerator = _tags.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					TextMeshPro tag = enumerator.Current;
					if ((Object)(object)tag != (Object)null)
					{
						((TMP_Text)tag).font = font;
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator).Dispose();
			}
		}

		private void UpdateTags()
		{
			Camera main = Camera.main;
			Transform val = ((main != null) ? ((Component)main).transform : null);
			System.Collections.Generic.Dictionary<VRRig, TextMeshPro>.Enumerator enumerator = _tags.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VRRig key;
					TextMeshPro value;
					enumerator.Current.Deconstruct(out key, out value);
					VRRig val2 = key;
					TextMeshPro val3 = value;
					if ((Object)(object)val2 == (Object)null || (Object)(object)val3 == (Object)null)
					{
						continue;
					}
					try
					{
						NetPlayer creator = val2.creator;
						bool flag = Enabled && ((Component)val2).gameObject.activeInHierarchy;
						((Component)val3).gameObject.SetActive(flag);
						if (flag)
						{
							GameObject headMesh = val2.headMesh;
							Transform val4 = ((headMesh != null) ? headMesh.transform : null) ?? ((Component)val2).transform;
							float num = ((val2.scaleFactor > 0f) ? val2.scaleFactor : 1f);
							val3.transform.position = val4.position + Vector3.up * (0.48f * num);
							FaceCamera(val3, val);
							((TMP_Text)val3).text = (((creator != null) ? creator.SanitizedNickName : null) ?? string.Empty) + ScryptoUtilsPad.Core.NametagExtras.BuildSuffix(val2);
							try
							{
								((TMP_Text)val3).color = Color.white;
								((TMP_Text)val3).outlineColor = (Color32)val2.playerColor;
							}
							catch
							{
							}
						}
					}
					catch (System.Exception ex)
					{
						Log.LogWarning((object)string.Concat("[NametagManager] UpdateTags error on rig: ", ex.Message));
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator).Dispose();
			}
		}

		private static void FaceCamera(TextMeshPro text, Transform camera)
		{
			if ((Object)(object)camera == (Object)null)
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
}
