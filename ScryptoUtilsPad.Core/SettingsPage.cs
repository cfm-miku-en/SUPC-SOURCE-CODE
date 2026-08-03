namespace ScryptoUtilsPad.Core
{
	public class SettingsPage : MonoBehaviour
	{
		private sealed class SettingEntry
		{
			public System.Func<string> Label;

			public System.Action Next;

			public System.Action Prev;

			public bool Spacer;
		}

		private sealed class SettingSlot
		{
			public Transform Row;

			public TMP_Text Label;

			public Transform Plus;

			public Transform Negative;
		}

		private static readonly ManualLogSource Log = Logger.CreateLogSource("SettingsPage");

		private static readonly Color DefaultMat1 = new Color(0f, 1f, 0.667f);

		private static readonly Color DefaultMat2 = new Color(0.169f, 0.169f, 0.169f);

		internal static System.ValueTuple<string, Color, Color>[] Themes;

		private static readonly string[] ModeNames;

		private static readonly string[][] SlotNames;

		private static readonly string[] BackPageNames = new string[5] { "BackPage", "SettingsBackPage", "PageNegative", "SettingsPageNegative", "PrevPage" };

		private static readonly string[] ForwardPageNames = new string[5] { "ForwardPage", "SettingsForwardPage", "PagePlus", "SettingsPagePlus", "NextPage" };

		private static readonly string[] PageTextNames = new string[3] { "PageText", "PageNumber", "PageLabel" };

		private static Color _themeMat1 = DefaultMat1;

		private static Color _themeMat2 = DefaultMat2;

		private static bool _themeLoaded;

		public static Color ThemeMat1
		{
			get
			{
				EnsureThemeLoaded();
				return _themeMat1;
			}
		}

		public static Color ThemeMat2
		{
			get
			{
				EnsureThemeLoaded();
				return _themeMat2;
			}
		}

		private static void EnsureThemeLoaded()
		{
			if (_themeLoaded)
			{
				return;
			}
			_themeLoaded = true;
			int idx = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.ThemeIndex", 0), 0, Themes.Length - 1);
			_themeMat1 = Themes[idx].Item2;
			_themeMat2 = Themes[idx].Item3;
		}

		private int _themeIndex;

		private int _modeIndex;

		private int _page;

		private Color _currentMat1 = DefaultMat1;

		private Color _currentMat2 = DefaultMat2;

		private GameObject _menu;

		private Transform _pageTr;

		private TMP_Text _pageText;

		private Transform _backArrow;

		private Transform _forwardArrow;

		private readonly System.Collections.Generic.List<SettingSlot> _slots = new System.Collections.Generic.List<SettingSlot>();

		private readonly System.Collections.Generic.List<SettingEntry> _entries = new System.Collections.Generic.List<SettingEntry>();

		public string CurrentThemeName
		{
			get
			{
				return Themes[Mathf.Clamp(_themeIndex, 0, Themes.Length - 1)].Item1;
			}
		}

		public string CurrentModeName
		{
			get
			{
				return ModeNames[Mathf.Clamp(_modeIndex, 0, ModeNames.Length - 1)];
			}
		}

		private int MaxPage
		{
			get
			{
				if (_slots.Count == 0)
				{
					return 0;
				}
				return Mathf.Max(0, Mathf.CeilToInt((float)_entries.Count / (float)_slots.Count) - 1);
			}
		}

		public void Init(Transform root)
		{
			_menu = ((Component)root).gameObject;
			_pageTr = root.Find("SettingsPage");
			if ((Object)(object)_pageTr == (Object)null)
			{
				Log.LogWarning((object)"[SettingsPage] SettingsPage not found in menu hierarchy");
				return;
			}
			CollectSlots();
			BuildEntries();
			SetupPageArrows();
			WireSlots();
			_themeIndex = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.ThemeIndex", 0), 0, Themes.Length - 1);
			_modeIndex = PlayerPrefs.GetInt("ScryptoUtilsPad.ModeIndex", 0);
			if (_modeIndex < 0 || _modeIndex >= ModeNames.Length)
			{
				_modeIndex = 0;
			}
			ScryptoUtilsPad.Core.SelectionSettings.Load();
			LoadAlertPrefs();
			ApplyTheme();
			ApplyMode();
			Render();
		}

		private void CollectSlots()
		{
			int i = 0;
			while (i < SlotNames.Length)
			{
				AddSlot(SlotNames[i][0], SlotNames[i][1], SlotNames[i][2]);
				i++;
			}
			int n = 1;
			while (n <= 8)
			{
				string row = string.Format("Setting{0}", n);
				AddSlot(row, string.Concat(row, "Plus"), string.Concat(row, "Negative"));
				n++;
			}
			if (_slots.Count == 0)
			{
				Log.LogWarning((object)"[SettingsPage] No setting rows found on SettingsPage");
			}
		}

		private void AddSlot(string rowName, string plusName, string negativeName)
		{
			Transform row = FindDeep(_pageTr, rowName);
			if ((Object)(object)row == (Object)null)
			{
				return;
			}
			SettingSlot slot = new SettingSlot();
			slot.Row = row;
			Transform labelTr = row.Find("Text (TMP)");
			slot.Label = (((Object)(object)labelTr != (Object)null) ? ((Component)labelTr).GetComponent<TMP_Text>() : ((Component)row).GetComponentInChildren<TMP_Text>(true));
			slot.Plus = FindDeep(_pageTr, plusName);
			slot.Negative = FindDeep(_pageTr, negativeName);
			_slots.Add(slot);
		}

		private void BuildEntries()
		{
			_entries.Clear();
			AddEntry(() => string.Concat("Select: ", ScryptoUtilsPad.Core.SelectionSettings.CurrentName), new System.Action(NextSelectMode), new System.Action(PrevSelectMode));
			AddEntry(() => string.Concat("Mode: ", ModeNames[_modeIndex]), new System.Action(NextMode), new System.Action(PrevMode));
			AddEntry(() => string.Concat("Theme: ", Themes[_themeIndex].Item1), new System.Action(NextTheme), new System.Action(PrevTheme));
			AddToggle(() => string.Concat("Nametags: ", ScryptoUtilsPad.Core.NametagManager.Enabled ? "On" : "Off"), new System.Action(ToggleNametags));
			AddToggle(() => string.Concat("Cheat Alerts: ", ScryptoUtilsPad.Core.CheatDetector.NotifyCheaters ? "On" : "Off"), new System.Action(ToggleCheatAlerts));
			AddToggle(() => string.Concat("Mod Alerts: ", ScryptoUtilsPad.Core.CheatDetector.NotifyModders ? "On" : "Off"), new System.Action(ToggleModAlerts));
			AddEntry(() => string.Concat("Click Sound: ", ScryptoUtilsPad.Core.ClickSoundSettings.CurrentName), new System.Action(NextClickSound), new System.Action(PrevClickSound));
			PadToNewPage();
			AddEntry(() => string.Concat("Nametag Font: ", ScryptoUtilsPad.Core.NametagFontSettings.CurrentName), new System.Action(NextNametagFont), new System.Action(PrevNametagFont));
			AddEntry(() => string.Concat("Startup Sound: ", ScryptoUtilsPad.Core.MenuSoundSettings.CurrentName), new System.Action(NextMenuSound), new System.Action(PrevMenuSound));
			AddEntry(() => string.Concat("Menu Size: ", ScryptoUtilsPad.Core.MenuSizeSettings.CurrentName), new System.Action(NextMenuSize), new System.Action(PrevMenuSize));
			AddToggle(() => string.Concat("Tag FPS: ", ScryptoUtilsPad.Core.NametagExtras.ShowFps ? "On" : "Off"), new System.Action(ToggleTagFps));
			AddToggle(() => string.Concat("Tag Platform: ", ScryptoUtilsPad.Core.NametagExtras.ShowPlatform ? "On" : "Off"), new System.Action(ToggleTagPlatform));
			AddToggle(() => "Join Discord", new System.Action(OpenDiscord));
			AddToggle(() => "Export Config", new System.Action(ExportConfig));
			AddToggle(() => "Reload Config", new System.Action(ReloadConfig));
			AddEntry(() => string.Concat("Profile: ", ScryptoUtilsPad.Core.UserFiles.CurrentConfigName), new System.Action(NextProfile), new System.Action(PrevProfile));
			AddToggle(() => "Load Profile", new System.Action(LoadProfile));
			AddToggle(() => "Save As Profile", new System.Action(SaveProfile));
			AddToggle(() => "Use Default Config", new System.Action(ClearProfile));
			AddToggle(() => string.Concat("Auto Save: ", ScryptoUtilsPad.Core.SharedConfig.AutoSave ? "On" : "Off"), new System.Action(ToggleAutoSave));
			AddToggle(() => string.Concat("Rig Preview: ", ScryptoUtilsPad.Core.RigPreview.Enabled ? "On" : "Off"), new System.Action(ToggleRigPreview));
			AddToggle(() => "Reset Rig Preview", new System.Action(ResetRigPreview));
			AddToggle(() => string.Concat("Desktop 1stP (BROKEN): ", ScryptoUtilsPad.Core.DesktopCamera.FirstPerson ? "On" : "Off"), new System.Action(ToggleDesktopFirstPerson));
			AddEntry(() => string.Concat("Desktop FOV: ", ScryptoUtilsPad.Core.DesktopCamera.Fov), new System.Action(DesktopFovUp), new System.Action(DesktopFovDown));
			AddEntry(() => string.Concat("Desktop Smooth: ", (ScryptoUtilsPad.Core.DesktopCamera.Smooth <= 0) ? "Off" : ScryptoUtilsPad.Core.DesktopCamera.Smooth.ToString()), new System.Action(DesktopSmoothUp), new System.Action(DesktopSmoothDown));
			PadToNewPage();
			AddEntry(() => string.Concat("Color: ", ScryptoUtilsPad.Core.ColorSettings.CurrentName), new System.Action(NextColor), new System.Action(PrevColor));
			AddEntry(() => string.Concat("Red: ", ScryptoUtilsPad.Core.ColorSettings.Red), new System.Action(RedUp), new System.Action(RedDown));
			AddEntry(() => string.Concat("Green: ", ScryptoUtilsPad.Core.ColorSettings.Green), new System.Action(GreenUp), new System.Action(GreenDown));
			AddEntry(() => string.Concat("Blue: ", ScryptoUtilsPad.Core.ColorSettings.Blue), new System.Action(BlueUp), new System.Action(BlueDown));
			PadToNewPage();
			AddToggle(() => string.Concat("Discord RPC: ", ScryptoUtilsPad.Core.DiscordRPC.Enabled ? "On" : "Off"), new System.Action(ToggleDiscordRPC));
		}

		private void PadToNewPage()
		{
			if (_slots.Count == 0)
			{
				return;
			}
			while (_entries.Count % _slots.Count != 0)
			{
				SettingEntry spacer = new SettingEntry();
				spacer.Label = () => string.Empty;
				spacer.Spacer = true;
				_entries.Add(spacer);
			}
		}

		private void AddEntry(System.Func<string> label, System.Action next, System.Action prev)
		{
			SettingEntry entry = new SettingEntry();
			entry.Label = label;
			entry.Next = next;
			entry.Prev = prev;
			_entries.Add(entry);
		}

		private void AddToggle(System.Func<string> label, System.Action toggle)
		{
			AddEntry(label, toggle, toggle);
		}

		private void WireSlots()
		{
			int i = 0;
			while (i < _slots.Count)
			{
				int captured = i;
				SettingSlot slot = _slots[i];
				SetupButton(slot.Plus, delegate
				{
					SlotNext(captured);
				});
				SetupButton(slot.Negative, delegate
				{
					SlotPrev(captured);
				});
				if ((Object)(object)slot.Plus == (Object)null && (Object)(object)slot.Negative == (Object)null)
				{
					SetupButton(slot.Row, delegate
					{
						SlotNext(captured);
					});
				}
				i++;
			}
		}

		private SettingEntry EntryForSlot(int slotIndex)
		{
			if (_slots.Count == 0)
			{
				return null;
			}
			int idx = _page * _slots.Count + slotIndex;
			if (idx < 0 || idx >= _entries.Count)
			{
				return null;
			}
			return _entries[idx];
		}

		private void SlotNext(int slotIndex)
		{
			SettingEntry entry = EntryForSlot(slotIndex);
			if (entry != null && entry.Next != null)
			{
				entry.Next();
			}
		}

		private void SlotPrev(int slotIndex)
		{
			SettingEntry entry = EntryForSlot(slotIndex);
			if (entry != null && entry.Prev != null)
			{
				entry.Prev();
			}
		}

		public void NextPage()
		{
			if (_page < MaxPage)
			{
				_page++;
				Render();
			}
		}

		public void PrevPage()
		{
			if (_page > 0)
			{
				_page--;
				Render();
			}
		}

		private void SetupPageArrows()
		{
			Transform back = FindDeepAny(_pageTr, BackPageNames);
			Transform forward = FindDeepAny(_pageTr, ForwardPageNames);
			Transform pageTextTr = FindDeepAny(_pageTr, PageTextNames);
			float spacing = RowSpacing();
			if (spacing <= 0f)
			{
				if ((Object)(object)back == (Object)null || (Object)(object)forward == (Object)null)
				{
					Log.LogWarning((object)"[SettingsPage] Could not work out row spacing - add objects named 'BackPage' and 'ForwardPage' under SettingsPage in the model");
				}
			}
			else
			{
				float textY = HighestRowY() + spacing * 1.65f;
				if ((Object)(object)pageTextTr == (Object)null)
				{
					pageTextTr = CloneRowAsLabel("PageText", textY);
				}
				else
				{
					MoveToY(pageTextTr, textY);
				}
				if ((Object)(object)back == (Object)null)
				{
					back = CloneArrow(FirstNegativeArrow(), "BackPage", textY);
				}
				if ((Object)(object)forward == (Object)null)
				{
					forward = CloneArrow(FirstPlusArrow(), "ForwardPage", textY);
				}
				if ((Object)(object)pageTextTr != (Object)null)
				{
					float arrowUp = spacing * 3f;
					float arrowSpread = spacing * 1f;
					Vector3 scale = Vector3.one;
					Transform rowParent = _slots[0].Row.parent;
					if ((Object)(object)rowParent != (Object)null)
					{
						scale = rowParent.lossyScale;
					}
					Vector3 center = pageTextTr.position;
					float arrowWorldY = center.y + arrowUp * scale.y;
					Transform srcNeg = FirstNegativeArrow();
					Transform srcPlus = FirstPlusArrow();
					float backX = (((Object)(object)srcNeg != (Object)null) ? srcNeg.position.x : (center.x - arrowSpread * scale.x));
					float forwardX = (((Object)(object)srcPlus != (Object)null) ? srcPlus.position.x : (center.x + arrowSpread * scale.x));
					SetWorldXY(back, backX, arrowWorldY);
					SetWorldXY(forward, forwardX, arrowWorldY);
				}
			}
			_backArrow = back;
			_forwardArrow = forward;
			SetupButton(back, new System.Action(PrevPage));
			SetupButton(forward, new System.Action(NextPage));
			if ((Object)(object)pageTextTr != (Object)null)
			{
				Transform inner = pageTextTr.Find("Text (TMP)");
				_pageText = (((Object)(object)inner != (Object)null) ? ((Component)inner).GetComponent<TMP_Text>() : ((Component)pageTextTr).GetComponentInChildren<TMP_Text>(true));
			}
			if ((Object)(object)back == (Object)null || (Object)(object)forward == (Object)null)
			{
				Log.LogWarning((object)"[SettingsPage] Page arrows missing - name them 'BackPage' and 'ForwardPage' under SettingsPage");
			}
		}

		private Transform FirstPlusArrow()
		{
			int i = 0;
			while (i < _slots.Count)
			{
				if ((Object)(object)_slots[i].Plus != (Object)null)
				{
					return _slots[i].Plus;
				}
				i++;
			}
			return null;
		}

		private Transform FirstNegativeArrow()
		{
			int i = 0;
			while (i < _slots.Count)
			{
				if ((Object)(object)_slots[i].Negative != (Object)null)
				{
					return _slots[i].Negative;
				}
				i++;
			}
			return null;
		}

		private float HighestRowY()
		{
			float highest = float.MinValue;
			int i = 0;
			while (i < _slots.Count)
			{
				float y = _slots[i].Row.localPosition.y;
				if (y > highest)
				{
					highest = y;
				}
				i++;
			}
			return ((highest == float.MinValue) ? 0f : highest);
		}

		private static void MoveToY(Transform tr, float y)
		{
			if (!((Object)(object)tr == (Object)null))
			{
				StripMenuButton(((Component)tr).gameObject);
				Vector3 p = tr.localPosition;
				tr.localPosition = new Vector3(p.x, y, p.z);
			}
		}

		private static void SetWorldXY(Transform tr, float worldX, float worldY)
		{
			if (!((Object)(object)tr == (Object)null))
			{
				StripMenuButton(((Component)tr).gameObject);
				Vector3 p = tr.position;
				tr.position = new Vector3(worldX, worldY, p.z);
			}
		}

		private float RowSpacing()
		{
			if (_slots.Count < 2)
			{
				return 0f;
			}
			System.Collections.Generic.List<float> ys = new System.Collections.Generic.List<float>();
			int i = 0;
			while (i < _slots.Count)
			{
				ys.Add(_slots[i].Row.localPosition.y);
				i++;
			}
			ys.Sort();
			float total = 0f;
			int count = 0;
			int j = 1;
			while (j < ys.Count)
			{
				float d = Mathf.Abs(ys[j] - ys[j - 1]);
				if (d > 0.0001f)
				{
					total += d;
					count++;
				}
				j++;
			}
			return ((count > 0) ? (total / (float)count) : 0f);
		}

		private static Transform CloneArrow(Transform source, string name, float y)
		{
			if ((Object)(object)source == (Object)null)
			{
				return null;
			}
			GameObject clone = Object.Instantiate<GameObject>(((Component)source).gameObject, source.parent);
			((Object)clone).name = name;
			StripMenuButton(clone);
			clone.transform.localRotation = source.localRotation;
			clone.transform.localScale = source.localScale;
			Vector3 p = source.localPosition;
			clone.transform.localPosition = new Vector3(p.x, y, p.z);
			clone.SetActive(true);
			return clone.transform;
		}

		private Transform CloneRowAsLabel(string name, float y)
		{
			if (_slots.Count == 0)
			{
				return null;
			}
			Transform source = _slots[0].Row;
			GameObject clone = Object.Instantiate<GameObject>(((Component)source).gameObject, source.parent);
			((Object)clone).name = name;
			StripMenuButton(clone);
			Collider[] colliders = clone.GetComponentsInChildren<Collider>(true);
			int i = 0;
			while (i < colliders.Length)
			{
				Object.DestroyImmediate((Object)(object)colliders[i]);
				i++;
			}
			clone.transform.localRotation = source.localRotation;
			clone.transform.localScale = source.localScale;
			Vector3 p = source.localPosition;
			clone.transform.localPosition = new Vector3(p.x, y, p.z);
			clone.SetActive(true);
			return clone.transform;
		}

		private static void StripMenuButton(GameObject go)
		{
			ScryptoUtilsPad.Core.MenuButton[] buttons = go.GetComponentsInChildren<ScryptoUtilsPad.Core.MenuButton>(true);
			int i = 0;
			while (i < buttons.Length)
			{
				Object.DestroyImmediate((Object)(object)buttons[i]);
				i++;
			}
		}

		public void ToggleNametags()
		{
			ScryptoUtilsPad.Core.NametagManager.Enabled = !ScryptoUtilsPad.Core.NametagManager.Enabled;
			UpdateTexts();
		}

		public void ToggleCheatAlerts()
		{
			ScryptoUtilsPad.Core.CheatDetector.NotifyCheaters = !ScryptoUtilsPad.Core.CheatDetector.NotifyCheaters;
			PlayerPrefs.SetInt("ScryptoUtilsPad.NotifyCheaters", ScryptoUtilsPad.Core.CheatDetector.NotifyCheaters ? 1 : 0);
			UpdateTexts();
		}

		public void ToggleModAlerts()
		{
			ScryptoUtilsPad.Core.CheatDetector.NotifyModders = !ScryptoUtilsPad.Core.CheatDetector.NotifyModders;
			PlayerPrefs.SetInt("ScryptoUtilsPad.NotifyModders", ScryptoUtilsPad.Core.CheatDetector.NotifyModders ? 1 : 0);
			UpdateTexts();
		}

		public void ToggleDiscordRPC()
		{
			ScryptoUtilsPad.Core.DiscordRPC.Enabled = !ScryptoUtilsPad.Core.DiscordRPC.Enabled;
			UpdateTexts();
		}

		private static void LoadAlertPrefs()
		{
			ScryptoUtilsPad.Core.CheatDetector.NotifyCheaters = PlayerPrefs.GetInt("ScryptoUtilsPad.NotifyCheaters", 1) == 1;
			ScryptoUtilsPad.Core.CheatDetector.NotifyModders = PlayerPrefs.GetInt("ScryptoUtilsPad.NotifyModders", 1) == 1;
		}

		public void NextSelectMode()
		{
			ScryptoUtilsPad.Core.SelectionSettings.Index = ScryptoUtilsPad.Core.SelectionSettings.Index + 1;
			UpdateTexts();
		}

		public void PrevSelectMode()
		{
			ScryptoUtilsPad.Core.SelectionSettings.Index = ScryptoUtilsPad.Core.SelectionSettings.Index - 1;
			UpdateTexts();
		}

		public void NextMode()
		{
			_modeIndex = (_modeIndex + 1) % ModeNames.Length;
			ApplyMode();
		}

		public void PrevMode()
		{
			_modeIndex = (_modeIndex - 1 + ModeNames.Length) % ModeNames.Length;
			ApplyMode();
		}

		public void HotApplyAll()
		{
			_themeIndex = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.ThemeIndex", 0), 0, Themes.Length - 1);
			_modeIndex = PlayerPrefs.GetInt("ScryptoUtilsPad.ModeIndex", 0);
			if (_modeIndex < 0 || _modeIndex >= ModeNames.Length)
			{
				_modeIndex = 0;
			}
			ScryptoUtilsPad.Core.SelectionSettings.Load();
			ScryptoUtilsPad.Core.NametagFontSettings.Load();
			ScryptoUtilsPad.Core.ClickSoundSettings.Load();
			ScryptoUtilsPad.Core.MenuSoundSettings.Load();
			ScryptoUtilsPad.Core.NametagExtras.Load();
			ScryptoUtilsPad.Core.MenuSizeSettings.Load();
			ScryptoUtilsPad.Core.ColorSettings.Load();
			ScryptoUtilsPad.Core.RigPreview.LoadPrefs();
			ScryptoUtilsPad.Core.DesktopCamera.LoadPrefs();
			LoadAlertPrefs();

			ApplyTheme();
			ApplyMode();
			ScryptoUtilsPad.Core.MenuSizeSettings.Apply();
			ScryptoUtilsPad.Core.ColorSettings.Apply();
			ScryptoUtilsPad.Core.NametagManager.Refresh();
			UpdateTexts();
		}

		private void ApplyMode()
		{
			PlayerPrefs.SetInt("ScryptoUtilsPad.ModeIndex", _modeIndex);
			ScryptoUtilsPad.Core.PositionHandler instance = ScryptoUtilsPad.Core.PositionHandler.Instance;
			if (instance != null)
			{
				instance.SetMode((_modeIndex != 0) ? ScryptoUtilsPad.Core.PositionHandler.MenuMode.Hold : ScryptoUtilsPad.Core.PositionHandler.MenuMode.Float);
			}
			UpdateTexts();
		}

		public void NextClickSound()
		{
			ScryptoUtilsPad.Core.ClickSoundSettings.Index = ScryptoUtilsPad.Core.ClickSoundSettings.Index + 1;
			UpdateTexts();
		}

		public void PrevClickSound()
		{
			ScryptoUtilsPad.Core.ClickSoundSettings.Index = ScryptoUtilsPad.Core.ClickSoundSettings.Index - 1;
			UpdateTexts();
		}

		public void NextMenuSound()
		{
			ScryptoUtilsPad.Core.MenuSoundSettings.Index = ScryptoUtilsPad.Core.MenuSoundSettings.Index + 1;
			UpdateTexts();
		}

		public void PrevMenuSound()
		{
			ScryptoUtilsPad.Core.MenuSoundSettings.Index = ScryptoUtilsPad.Core.MenuSoundSettings.Index - 1;
			UpdateTexts();
		}

		public void NextNametagFont()
		{
			ScryptoUtilsPad.Core.NametagFontSettings.Index = ScryptoUtilsPad.Core.NametagFontSettings.Index + 1;
			UpdateTexts();
		}

		public void PrevNametagFont()
		{
			ScryptoUtilsPad.Core.NametagFontSettings.Index = ScryptoUtilsPad.Core.NametagFontSettings.Index - 1;
			UpdateTexts();
		}

		public void NextMenuSize()
		{
			ScryptoUtilsPad.Core.MenuSizeSettings.Index = ScryptoUtilsPad.Core.MenuSizeSettings.Index + 1;
			ScryptoUtilsPad.Core.MenuSizeSettings.Apply();
			UpdateTexts();
		}

		public void PrevMenuSize()
		{
			ScryptoUtilsPad.Core.MenuSizeSettings.Index = ScryptoUtilsPad.Core.MenuSizeSettings.Index - 1;
			ScryptoUtilsPad.Core.MenuSizeSettings.Apply();
			UpdateTexts();
		}

		public void ToggleTagFps()
		{
			ScryptoUtilsPad.Core.NametagExtras.ShowFps = !ScryptoUtilsPad.Core.NametagExtras.ShowFps;
			ScryptoUtilsPad.Core.NametagExtras.Save();
			UpdateTexts();
		}

		public void ToggleTagPlatform()
		{
			ScryptoUtilsPad.Core.NametagExtras.ShowPlatform = !ScryptoUtilsPad.Core.NametagExtras.ShowPlatform;
			ScryptoUtilsPad.Core.NametagExtras.Save();
			UpdateTexts();
		}

		private void ColorStep(int channel, int delta)
		{
			ScryptoUtilsPad.Core.ColorSettings.StepChannel(channel, delta);
			ScryptoUtilsPad.Core.ColorSettings.Apply();
			UpdateTexts();
		}

		public void NextColor()
		{
			ScryptoUtilsPad.Core.ColorSettings.Index = ScryptoUtilsPad.Core.ColorSettings.Index + 1;
			ScryptoUtilsPad.Core.ColorSettings.Apply();
			UpdateTexts();
		}

		public void PrevColor()
		{
			ScryptoUtilsPad.Core.ColorSettings.Index = ScryptoUtilsPad.Core.ColorSettings.Index - 1;
			ScryptoUtilsPad.Core.ColorSettings.Apply();
			UpdateTexts();
		}

		public void RedUp() { ColorStep(0, 1); }

		public void RedDown() { ColorStep(0, -1); }

		public void GreenUp() { ColorStep(1, 1); }

		public void GreenDown() { ColorStep(1, -1); }

		public void BlueUp() { ColorStep(2, 1); }

		public void BlueDown() { ColorStep(2, -1); }

		public void ToggleDesktopFirstPerson()
		{
			ScryptoUtilsPad.Core.DesktopCamera.FirstPerson = !ScryptoUtilsPad.Core.DesktopCamera.FirstPerson;
			ScryptoUtilsPad.Core.DesktopCamera.Save();
			ScryptoUtilsPad.Core.DesktopCamera.LogState("Toggled ->");
			UpdateTexts();
		}

		private static int WrapRange(int v, int min, int max)
		{
			int span = max - min + 1;
			return ((v - min) % span + span) % span + min;
		}

		public void DesktopFovUp()
		{
			ScryptoUtilsPad.Core.DesktopCamera.Fov = WrapRange(ScryptoUtilsPad.Core.DesktopCamera.Fov + 5, 30, 140);
			ScryptoUtilsPad.Core.DesktopCamera.Save();
			UpdateTexts();
		}

		public void DesktopFovDown()
		{
			ScryptoUtilsPad.Core.DesktopCamera.Fov = WrapRange(ScryptoUtilsPad.Core.DesktopCamera.Fov - 5, 30, 140);
			ScryptoUtilsPad.Core.DesktopCamera.Save();
			UpdateTexts();
		}

		public void DesktopSmoothUp()
		{
			ScryptoUtilsPad.Core.DesktopCamera.Smooth = WrapRange(ScryptoUtilsPad.Core.DesktopCamera.Smooth + 1, 0, 10);
			ScryptoUtilsPad.Core.DesktopCamera.Save();
			UpdateTexts();
		}

		public void DesktopSmoothDown()
		{
			ScryptoUtilsPad.Core.DesktopCamera.Smooth = WrapRange(ScryptoUtilsPad.Core.DesktopCamera.Smooth - 1, 0, 10);
			ScryptoUtilsPad.Core.DesktopCamera.Save();
			UpdateTexts();
		}

		public void NextProfile()
		{
			ScryptoUtilsPad.Core.UserFiles.RefreshConfigList();
			ScryptoUtilsPad.Core.UserFiles.ConfigIndex = ScryptoUtilsPad.Core.UserFiles.ConfigIndex + 1;
			UpdateTexts();
		}

		public void PrevProfile()
		{
			ScryptoUtilsPad.Core.UserFiles.RefreshConfigList();
			ScryptoUtilsPad.Core.UserFiles.ConfigIndex = ScryptoUtilsPad.Core.UserFiles.ConfigIndex - 1;
			UpdateTexts();
		}

		public void LoadProfile()
		{
			ScryptoUtilsPad.Core.UserFiles.LoadCurrentConfig();
			HotApplyAll();
		}

		public void SaveProfile()
		{
			ScryptoUtilsPad.Core.UserFiles.SaveCurrentAsProfile();
			UpdateTexts();
		}

		public void ToggleRigPreview()
		{
			ScryptoUtilsPad.Core.RigPreview.SetEnabled(!ScryptoUtilsPad.Core.RigPreview.Enabled);
			UpdateTexts();
		}

		public void ResetRigPreview()
		{
			ScryptoUtilsPad.Core.RigPreview.ResetPlacement();
			UpdateTexts();
		}

		public void ToggleAutoSave()
		{
			ScryptoUtilsPad.Core.SharedConfig.SetAutoSave(!ScryptoUtilsPad.Core.SharedConfig.AutoSave);
			UpdateTexts();
		}

		public void ClearProfile()
		{
			ScryptoUtilsPad.Core.UserFiles.ClearActiveProfile();
			ScryptoUtilsPad.Core.SharedConfig.Load();
			HotApplyAll();
		}

		public void ExportConfig()
		{
			ScryptoUtilsPad.Core.SharedConfig.Save();
			UpdateTexts();
		}

		public void ReloadConfig()
		{
			ScryptoUtilsPad.Core.SharedConfig.Load();
			HotApplyAll();
		}

		public void OpenDiscord()
		{
			Application.OpenURL("https://discord.gg/bPjmq3qw6Q");
			UpdateTexts();
		}

		public void NextTheme()
		{
			_themeIndex = (_themeIndex + 1) % Themes.Length;
			ApplyTheme();
		}

		public void PrevTheme()
		{
			_themeIndex = (_themeIndex - 1 + Themes.Length) % Themes.Length;
			ApplyTheme();
		}

		public static Color CurrentThemeSecondary
		{
			get
			{
				int idx = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.ThemeIndex", 0), 0, Themes.Length - 1);
				return Themes[idx].Item3;
			}
		}

		public static Color CurrentThemePrimary
		{
			get
			{
				int idx = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.ThemeIndex", 0), 0, Themes.Length - 1);
				return Themes[idx].Item2;
			}
		}

		private void ApplyTheme()
		{
			PlayerPrefs.SetInt("ScryptoUtilsPad.ThemeIndex", _themeIndex);
			System.ValueTuple<string, Color, Color> valueTuple = Themes[_themeIndex];
			Color item = valueTuple.Item2;
			Color item2 = valueTuple.Item3;
			Renderer[] componentsInChildren = _menu.GetComponentsInChildren<Renderer>(true);
			int num = 0;
			while (num < componentsInChildren.Length)
			{
				Renderer val = componentsInChildren[num];
				Material[] materials = val.materials;
				int num2 = 0;
				while (num2 < materials.Length)
				{
					Material val2 = materials[num2];
					if (val2.HasProperty("_Color"))
					{
						if (ColorClose(val2.color, _currentMat1, 0.05f))
						{
							val2.color = item;
						}
						else if (ColorClose(val2.color, _currentMat2, 0.05f))
						{
							val2.color = item2;
						}
					}
					num2++;
				}
				num++;
			}
			_currentMat1 = item;
			_currentMat2 = item2;
			ApplyPageTheme();
			_themeMat1 = item;
			_themeMat2 = item2;
			_themeLoaded = true;
			ScryptoUtilsPad.Tools.CameraPage instance = ScryptoUtilsPad.Tools.CameraPage.Instance;
			if (instance != null)
			{
				instance.ApplyTheme(item, item2);
			}
			ScryptoUtilsPad.Core.NetworkManager instance2 = ScryptoUtilsPad.Core.NetworkManager.Instance;
			if (instance2 != null)
			{
				instance2.SendTheme(item, item2);
			}
			UpdateTexts();
		}

		private void ApplyPageTheme()
		{
			if ((Object)(object)_pageText != (Object)null)
			{
				_pageText.color = Color.white;
			}
			ColorArrow(_backArrow);
			ColorArrow(_forwardArrow);
		}

		private void ColorArrow(Transform arrow)
		{
			if ((Object)(object)arrow == (Object)null)
			{
				return;
			}
			Renderer[] renderers = ((Component)arrow).GetComponentsInChildren<Renderer>(true);
			int i = 0;
			while (i < renderers.Length)
			{
				Material[] materials = renderers[i].materials;
				int j = 0;
				while (j < materials.Length)
				{
					if (materials[j].HasProperty("_Color"))
					{
						materials[j].color = _currentMat1;
					}
					j++;
				}
				i++;
			}
		}

		private void UpdateTexts()
		{
			ScryptoUtilsPad.Core.SharedConfig.MarkDirty();
			Render();
		}

		private void Render()
		{
			if (_slots.Count == 0)
			{
				return;
			}
			if (_page > MaxPage)
			{
				_page = MaxPage;
			}
			int start = _page * _slots.Count;
			int i = 0;
			while (i < _slots.Count)
			{
				SettingSlot slot = _slots[i];
				int idx = start + i;
				bool used = idx < _entries.Count && !_entries[idx].Spacer;
				SetSlotActive(slot, used);
				if (used && (Object)(object)slot.Label != (Object)null)
				{
					slot.Label.text = _entries[idx].Label();
				}
				i++;
			}
			if ((Object)(object)_pageText != (Object)null)
			{
				_pageText.text = string.Format("Page {0}/{1}", _page + 1, MaxPage + 1);
			}
		}

		private static void SetSlotActive(SettingSlot slot, bool active)
		{
			if ((Object)(object)slot.Row != (Object)null)
			{
				((Component)slot.Row).gameObject.SetActive(active);
			}
			if ((Object)(object)slot.Plus != (Object)null)
			{
				((Component)slot.Plus).gameObject.SetActive(active);
			}
			if ((Object)(object)slot.Negative != (Object)null)
			{
				((Component)slot.Negative).gameObject.SetActive(active);
			}
		}

		private static Transform FindDeep(Transform root, string name)
		{
			if ((Object)(object)root == (Object)null || string.IsNullOrEmpty(name))
			{
				return null;
			}
			Transform direct = root.Find(name);
			if ((Object)(object)direct != (Object)null)
			{
				return direct;
			}
			Transform[] all = ((Component)root).GetComponentsInChildren<Transform>(true);
			int i = 0;
			while (i < all.Length)
			{
				if (((Object)all[i]).name == name)
				{
					return all[i];
				}
				i++;
			}
			return null;
		}

		private static Transform FindDeepAny(Transform root, string[] names)
		{
			int i = 0;
			while (i < names.Length)
			{
				Transform found = FindDeep(root, names[i]);
				if ((Object)(object)found != (Object)null)
				{
					return found;
				}
				i++;
			}
			return null;
		}

		private static void SetupButton(Transform tr, System.Action onPress)
		{
			if (!((Object)(object)tr == (Object)null))
			{
				EnsureButtonCollider(tr);
				ScryptoUtilsPad.Core.MenuButton menuButton = ((Component)tr).GetComponent<ScryptoUtilsPad.Core.MenuButton>() ?? ((Component)tr).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
				menuButton.OnPress = onPress;
			}
		}

		private static void EnsureButtonCollider(Transform tr)
		{
			if ((Object)(object)tr == (Object)null || (Object)(object)((Component)tr).GetComponent<Collider>() != (Object)null)
			{
				return;
			}
			BoxCollider box = ((Component)tr).gameObject.AddComponent<BoxCollider>();
			((Collider)box).isTrigger = true;
			MeshFilter mf = ((Component)tr).GetComponent<MeshFilter>();
			Mesh mesh = ((mf != null) ? mf.sharedMesh : null);
			if ((Object)(object)mesh != (Object)null)
			{
				Bounds lb = mesh.bounds;
				box.center = lb.center;
				Vector3 sz = lb.size;
				box.size = new Vector3(Mathf.Abs(sz.x), Mathf.Abs(sz.y), Mathf.Max(Mathf.Abs(sz.z), 0.05f));
			}
			else
			{
				box.size = new Vector3(1f, 1f, 0.2f);
			}
		}

		private static bool ColorClose(Color a, Color b, float tol = 0.05f)
		{
			return Mathf.Abs(a.r - b.r) < tol && Mathf.Abs(a.g - b.g) < tol && Mathf.Abs(a.b - b.b) < tol;
		}

		static SettingsPage()
		{
			System.ValueTuple<string, Color, Color>[] array = new System.ValueTuple<string, Color, Color>[18];
			array[0] = new System.ValueTuple<string, Color, Color>("Default Theme", new Color(0f, 1f, 0.667f), new Color(0.169f, 0.169f, 0.169f));
			array[1] = new System.ValueTuple<string, Color, Color>("Neon Theme", new Color(1f, 0f, 1f), new Color(0.102f, 0.102f, 0.188f));
			array[2] = new System.ValueTuple<string, Color, Color>("Ice And Fire Theme", new Color(0.435f, 0.024f, 0.043f), new Color(0.024f, 0.043f, 0.267f));
			array[3] = new System.ValueTuple<string, Color, Color>("Gold Theme", new Color(1f, 0.843f, 0f), new Color(0.102f, 0.078f, 0f));
			array[4] = new System.ValueTuple<string, Color, Color>("Black Theme", new Color(0.15f, 0.15f, 0.15f), new Color(0.05f, 0.05f, 0.05f));
			array[5] = new System.ValueTuple<string, Color, Color>("White Theme", new Color(0.95f, 0.95f, 0.95f), new Color(0.75f, 0.75f, 0.75f));
			array[6] = new System.ValueTuple<string, Color, Color>("Purple Theme", new Color(0.502f, 0f, 0.502f), new Color(0.078f, 0f, 0.11f));
			array[7] = new System.ValueTuple<string, Color, Color>("Red Theme", new Color(0.647f, 0.165f, 0.165f), new Color(0.102f, 0.039f, 0.02f));
			array[8] = new System.ValueTuple<string, Color, Color>("Miku Theme", new Color(0.078f, 0.212f, 0.549f), new Color(0.008f, 0.02f, 0.055f));
			array[9] = new System.ValueTuple<string, Color, Color>("Vistro Theme", new Color(1f, 0.843f, 0.098f), new Color(0.043f, 0.129f, 0.478f));
			array[10] = new System.ValueTuple<string, Color, Color>("Teto Theme", new Color(0.855f, 0.184f, 0.286f), new Color(0.118f, 0.02f, 0.031f));
			array[11] = new System.ValueTuple<string, Color, Color>("Neru Theme", new Color(0.98f, 0.82f, 0.25f), new Color(0.098f, 0.078f, 0.016f));
			array[12] = new System.ValueTuple<string, Color, Color>("Sapphire Theme", new Color(0.06f, 0.42f, 0.85f), new Color(0.004f, 0.031f, 0.098f));
			array[13] = new System.ValueTuple<string, Color, Color>("Sakura Theme", new Color(0.96f, 0.45f, 0.66f), new Color(0.114f, 0.031f, 0.063f));
			array[14] = new System.ValueTuple<string, Color, Color>("Rin/Len Theme", new Color(1f, 0.8f, 0.07f), new Color(0.122f, 0.094f, 0.008f));
			array[15] = new System.ValueTuple<string, Color, Color>("MEIKO Theme", new Color(0.85f, 0.05f, 0.11f), new Color(0.106f, 0.008f, 0.016f));
			array[16] = new System.ValueTuple<string, Color, Color>("KAITO Theme", new Color(0.13f, 0.35f, 0.86f), new Color(0.016f, 0.043f, 0.106f));
			array[17] = new System.ValueTuple<string, Color, Color>("GUMI Theme", new Color(0.4f, 0.8f, 0.16f), new Color(0.047f, 0.098f, 0.02f));
            Themes = array;
			string[] array2 = new string[2];
			array2[0] = "Float";
			array2[1] = "Hold";
			ModeNames = array2;
			SlotNames = new string[4][]
			{
				new string[3] { "SelectMode", "SelectModePlus", "SelectModeNegative" },
				new string[3] { "Mode", "ModePlus", "ModeNegative" },
				new string[3] { "Theme", "ThemePlus", "ThemeNegative" },
				new string[3] { "Nametags", "NametagsPlus", "NametagsNegative" }
			};
		}
	}
}
