namespace ScryptoUtilsPad.Core
{
	public class ModsPage : MonoBehaviour
	{
		public static ScryptoUtilsPad.Core.ModsPage Instance;

		private Transform _pageTr;

		private TMP_Text _nameText;

		private TMP_Text _modsText;

		private bool _showingUnknown;

		private TMP_Text _viewUnknownLabel;

		private static readonly ManualLogSource Log = Logger.CreateLogSource("ModsPage");

		private void Awake()
		{
			Instance = this;
		}

		public void Init(Transform menuRoot)
		{
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			_pageTr = menuRoot.Find("ModsPage");
			if ((Object)(object)_pageTr == (Object)null)
			{
				Log.LogWarning((object)"[ModsPage] ModsPage not found in menu hierarchy");
				return;
			}
			Transform obj = _pageTr.Find("NameText");
			_nameText = ((obj != null) ? ((Component)obj).GetComponent<TMP_Text>() : null);
			Transform obj2 = _pageTr.Find("ModsText");
			_modsText = ((obj2 != null) ? ((Component)obj2).GetComponent<TMP_Text>() : null);
			if ((Object)(object)_modsText != (Object)null)
			{
				_modsText.enableAutoSizing = true;
				_modsText.fontSizeMin = 0f;
				_modsText.fontSizeMax = 3.2f;
				_modsText.fontSize = 2.3f;
				((Graphic)_modsText).color = Color.white;
				_modsText.fontStyle = (FontStyles)0;
				_modsText.textWrappingMode = (TextWrappingModes)1;
				_modsText.overflowMode = (TextOverflowModes)0;
				_modsText.alignment = (TextAlignmentOptions)514;
				_modsText.characterSpacing = 0f;
				_modsText.wordSpacing = 0f;
				_modsText.lineSpacing = 0f;
				_modsText.paragraphSpacing = 0f;
				_modsText.characterWidthAdjustment = 0f;
				_modsText.text = "No user selected";
			}
			ScryptoUtilsPad.Core.SUPCData.FetchAsync((MonoBehaviour)(object)this);
			CreateViewUnknownButton();
		}

		private void CreateViewUnknownButton()
		{
			if ((Object)(object)_pageTr == (Object)null || (Object)(object)_modsText == (Object)null)
			{
				return;
			}
			GameObject btn = Object.Instantiate<GameObject>(((Component)_modsText).gameObject, _pageTr);
			((Object)btn).name = "ViewUnknownButton";
			btn.transform.localPosition = ((Component)_modsText).transform.localPosition + new Vector3(0f, -4.5f, 0f);
			btn.transform.localRotation = ((Component)_modsText).transform.localRotation;
			btn.transform.localScale = ((Component)_modsText).transform.localScale;

			TMP_Text label = btn.GetComponent<TMP_Text>();
			if ((Object)(object)label != (Object)null)
			{
				label.enableAutoSizing = false;
				label.fontSize = 2.6f;
				label.text = "View Unknown";
				((Graphic)label).color = Color.white;
				label.alignment = (TextAlignmentOptions)514;
				_viewUnknownLabel = label;
			}

			BoxCollider col = btn.GetComponent<BoxCollider>() ?? btn.AddComponent<BoxCollider>();
			((Collider)col).isTrigger = true;
			Vector2 rectSize = new Vector2(6f, 1.5f);
			if ((Object)(object)label != (Object)null)
			{
				Vector2 sd = label.rectTransform.sizeDelta;
				if (sd.x > 0.01f && sd.y > 0.01f)
				{
					rectSize = sd;
				}
			}
			col.size = new Vector3(rectSize.x, rectSize.y, 0.05f);

			ScryptoUtilsPad.Core.MenuButton menuButton = btn.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
			menuButton.OnPress = new System.Action(ToggleUnknownView);
		}

		private void ToggleUnknownView()
		{
			_showingUnknown = !_showingUnknown;
			if ((Object)(object)_viewUnknownLabel != (Object)null)
			{
				_viewUnknownLabel.text = (_showingUnknown ? "Hide Unknown" : "View Unknown");
			}
			if (_showingUnknown)
			{
				ShowUnknown();
			}
			else if ((Object)(object)_modsText != (Object)null)
			{
				_modsText.text = "No user selected";
			}
		}

		private void ShowUnknown()
		{
			if ((Object)(object)_modsText == (Object)null)
			{
				return;
			}
			System.Collections.Generic.List<string> unknown = ScryptoUtilsPad.Core.CheatSigGetter.AllUnknown;
			if ((Graphic)(object)_modsText != (Graphic)null)
			{
				((Graphic)_modsText).color = Color.white;
			}
			if (unknown == null || unknown.Count == 0)
			{
				_modsText.text = "No unknown signatures collected yet.";
				return;
			}
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Unknown Signatures (").Append(unknown.Count).Append("):\n");
			for (int i = 0; i < unknown.Count; i++)
			{
				sb.Append(unknown[i]);
				if (i < unknown.Count - 1)
				{
					sb.Append("\n");
				}
			}
			_modsText.text = sb.ToString();
		}

		public void ShowRig(VRRig rig)
		{
			if (_showingUnknown)
			{
				return;
			}
			if ((Object)(object)rig == (Object)null || rig.creator == null)
			{
				if ((Object)(object)_nameText != (Object)null)
				{
					_nameText.text = string.Empty;
				}
				if ((Object)(object)_modsText != (Object)null)
				{
					_modsText.text = "No user selected";
				}
				return;
			}
			string userId = rig.creator.UserId;
			string sanitizedNickName = rig.creator.SanitizedNickName;
			if ((Object)(object)_nameText != (Object)null)
			{
				string text = AdminBadge(userId);
				_nameText.text = ((text != null) ? string.Concat(sanitizedNickName, "\n", text) : sanitizedNickName);
			}
			System.Collections.Generic.List<string> list = CheckProperties(rig);
			if ((Object)(object)_modsText != (Object)null)
			{
				_modsText.text = ((list.Count > 0) ? string.Join("\n", list) : "No mods detected");
			}
		}

		private static string AdminBadge(string userId)
		{
			// SUPC has no remote admin system (removed Hamburbur/Seralyth injection).
			return null;
		}

		private static System.Collections.Generic.List<string> CheckProperties(VRRig rig)
		{
			System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
			NetPlayer creator = rig.creator;
			object obj;
			if (creator == null)
			{
				obj = null;
			}
			else
			{
				Player playerRef = creator.GetPlayerRef();
				obj = ((playerRef != null) ? playerRef.CustomProperties : null);
			}
			Hashtable val = (Hashtable)obj;
			if (val == null)
			{
				return list;
			}
			System.Collections.Generic.Dictionary<object, object>.KeyCollection.Enumerator enumerator = ((System.Collections.Generic.Dictionary<object, object>)(object)val).Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					string key = current.ToString().ToLower();
					string value;
					string value2;
					if (ScryptoUtilsPad.Core.SUPCData.KnownCheats.TryGetValue(key, out value))
					{
						string entry = string.Concat("[<color=red>", value, "</color>]");
						if (!list.Contains(entry))
						{
							list.Add(entry);
						}
					}
					else if (ScryptoUtilsPad.Core.SUPCData.KnownMods.TryGetValue(key, out value2))
					{
						string entry2 = string.Concat("[<color=green>", value2, "</color>]");
						if (!list.Contains(entry2))
						{
							list.Add(entry2);
						}
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			return list;
		}

		public static bool HasAnyDetectedMod(VRRig rig)
		{
			if (!ScryptoUtilsPad.Core.SUPCData.IsLoaded)
			{
				return false;
			}
			object obj;
			if (rig == null)
			{
				obj = null;
			}
			else
			{
				NetPlayer creator = rig.creator;
				if (creator == null)
				{
					obj = null;
				}
				else
				{
					Player playerRef = creator.GetPlayerRef();
					obj = ((playerRef != null) ? playerRef.CustomProperties : null);
				}
			}
			Hashtable val = (Hashtable)obj;
			if (val == null)
			{
				return false;
			}
			System.Collections.Generic.Dictionary<object, object>.KeyCollection.Enumerator enumerator = ((System.Collections.Generic.Dictionary<object, object>)(object)val).Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					string key = current.ToString().ToLower();
					if (ScryptoUtilsPad.Core.SUPCData.KnownCheats.ContainsKey(key) || ScryptoUtilsPad.Core.SUPCData.KnownMods.ContainsKey(key))
					{
						return true;
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			return false;
		}

		public static int CountDetectedMods(VRRig rig)
		{
			if (!ScryptoUtilsPad.Core.SUPCData.IsLoaded)
			{
				return 0;
			}
			object obj;
			if (rig == null)
			{
				obj = null;
			}
			else
			{
				NetPlayer creator = rig.creator;
				if (creator == null)
				{
					obj = null;
				}
				else
				{
					Player playerRef = creator.GetPlayerRef();
					obj = ((playerRef != null) ? playerRef.CustomProperties : null);
				}
			}
			Hashtable val = (Hashtable)obj;
			if (val == null)
			{
				return 0;
			}
			int num = 0;
			System.Collections.Generic.Dictionary<object, object>.KeyCollection.Enumerator enumerator = ((System.Collections.Generic.Dictionary<object, object>)(object)val).Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					string key = current.ToString().ToLower();
					if (ScryptoUtilsPad.Core.SUPCData.KnownCheats.ContainsKey(key) || ScryptoUtilsPad.Core.SUPCData.KnownMods.ContainsKey(key))
					{
						num++;
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			return num;
		}

		public static bool HasIllegalMod(VRRig rig)
		{
			if (!ScryptoUtilsPad.Core.SUPCData.IsLoaded)
			{
				return false;
			}
			object obj;
			if (rig == null)
			{
				obj = null;
			}
			else
			{
				NetPlayer creator = rig.creator;
				if (creator == null)
				{
					obj = null;
				}
				else
				{
					Player playerRef = creator.GetPlayerRef();
					obj = ((playerRef != null) ? playerRef.CustomProperties : null);
				}
			}
			Hashtable val = (Hashtable)obj;
			if (val == null)
			{
				return false;
			}
			System.Collections.Generic.Dictionary<object, object>.KeyCollection.Enumerator enumerator = ((System.Collections.Generic.Dictionary<object, object>)(object)val).Keys.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					if (ScryptoUtilsPad.Core.SUPCData.KnownCheats.ContainsKey(current.ToString().ToLower()))
					{
						return true;
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
			}
			return false;
		}
	}
}
