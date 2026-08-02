namespace ScryptoUtilsPad.Core
{
	public class PlayersPage : MonoBehaviour
	{
		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass25_0
		{
			public int captured;

			public ScryptoUtilsPad.Core.PlayersPage _003C_003E4__this;

			internal void _003CInit_003Eb__3()
			{
				_003C_003E4__this.OnPlayerClicked(captured);
			}
		}

		public static ScryptoUtilsPad.Core.PlayersPage Instance;

		private const int PerPage = 4;

		private TMP_Text[] _slots;

		private readonly System.Collections.Generic.List<VRRig> _rigs = new System.Collections.Generic.List<VRRig>();

		private int _page;

		private float _refreshTimer;

		private float _fpsRefreshTimer;

		private Transform _pageTr;

		private GameObject _secondaryMenu;

		private TMP_Text _nameText;

		private TMP_Text _modText;

		private TMP_Text _dateText;

		private TMP_Text _fpsText;

		private TMP_Text _platformText;

		private TMP_Text _colorText;

		private VRRig _selectedRig;

		private GameObject _selectionDot;

		private GorillaParent _gorillaParent;

		private System.Reflection.FieldInfo _rigsField;

		private bool _rigsFieldSearched;

		private System.Reflection.FieldInfo _fpsField;

		private bool _fpsFieldSearched;

		private static readonly ManualLogSource Log = Logger.CreateLogSource("PlayersPage");

		private static System.Reflection.FieldInfo _btnTypeField;

		private static System.Reflection.MethodInfo _btnActivateMethod;

		private static bool _btnFieldsSearched;

		private static System.Reflection.FieldInfo _cosmeticsField;

		private static System.Reflection.FieldInfo _rankedTierField;

		private static bool _platformFieldsSearched;

		public static bool IsOpen
		{
			get
			{
				ScryptoUtilsPad.Core.PlayersPage inst = Instance;
				Transform tr = (((Object)(object)inst != (Object)null) ? inst._pageTr : null);
				return (Object)(object)tr != (Object)null && ((Component)tr).gameObject.activeInHierarchy;
			}
		}

		private int MaxPage
		{
			get
			{
				return Mathf.Max(0, Mathf.CeilToInt((float)_rigs.Count / 4f) - 1);
			}
		}

		private void Awake()
		{
			Instance = this;
		}

		private void OnDestroy()
		{
			if ((Object)(object)_selectionDot != (Object)null)
			{
				Object.Destroy((Object)(object)_selectionDot);
			}
		}

		public void Init(Transform menuRoot)
		{
			_pageTr = menuRoot.Find("PlayersPage");
			if ((Object)(object)_pageTr == (Object)null)
			{
				return;
			}
			Transform val = menuRoot.Find("SecondaryMenu");
			if ((Object)(object)val != (Object)null)
			{
				_secondaryMenu = ((Component)val).gameObject;
				Transform val2 = val.Find("InfoHolder");
				if ((Object)(object)val2 != (Object)null)
				{
					Transform obj = val2.Find("NameText");
					_nameText = ((obj != null) ? ((Component)obj).GetComponent<TMP_Text>() : null);
					Transform obj2 = val2.Find("ModText");
					_modText = ((obj2 != null) ? ((Component)obj2).GetComponent<TMP_Text>() : null);
					Transform obj3 = val2.Find("DateText");
					_dateText = ((obj3 != null) ? ((Component)obj3).GetComponent<TMP_Text>() : null);
					Transform obj4 = val2.Find("FPSText");
					_fpsText = ((obj4 != null) ? ((Component)obj4).GetComponent<TMP_Text>() : null);
					Transform obj5 = val2.Find("PlatformText");
					_platformText = ((obj5 != null) ? ((Component)obj5).GetComponent<TMP_Text>() : null);
					Transform obj6 = val2.Find("ColorText");
					_colorText = ((obj6 != null) ? ((Component)obj6).GetComponent<TMP_Text>() : null);
					SetupButton(val2.Find("ReportCheating"), new System.Action(_003CInit_003Eb__25_0));
					SetupButton(val2.Find("ReportToxicity"), new System.Action(_003CInit_003Eb__25_1));
					SetupButton(val2.Find("ReportHateSpeech"), new System.Action(_003CInit_003Eb__25_2));
				}
			}
			_slots = (TMP_Text[])(object)new TMP_Text[4];
			int num = 0;
			while (num < 4)
			{
				ScryptoUtilsPad.Core.PlayersPage._003C_003Ec__DisplayClass25_0 _003C_003Ec__DisplayClass25_1 = new ScryptoUtilsPad.Core.PlayersPage._003C_003Ec__DisplayClass25_0();
				_003C_003Ec__DisplayClass25_1._003C_003E4__this = this;
				Transform val3 = _pageTr.Find(string.Format("Player{0}", num + 1));
				if (!((Object)(object)val3 == (Object)null))
				{
					_slots[num] = ((Component)val3).GetComponentInChildren<TMP_Text>();
					_003C_003Ec__DisplayClass25_1.captured = num;
					ScryptoUtilsPad.Core.MenuButton menuButton = ((Component)val3).GetComponent<ScryptoUtilsPad.Core.MenuButton>() ?? ((Component)val3).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
					menuButton.OnPress = new System.Action(_003C_003Ec__DisplayClass25_1._003CInit_003Eb__3);
					((Component)val3).gameObject.layer = 18;
				}
				num++;
			}
			SetupButton(_pageTr.Find("BackPage"), new System.Action(PrevPage));
			Transform val4 = menuRoot.Find("ForwardPage") ?? _pageTr.Find("ForwardPage");
			if ((Object)(object)val4 != (Object)null)
			{
				ScryptoUtilsPad.Core.MenuButton menuButton2 = ((Component)val4).GetComponent<ScryptoUtilsPad.Core.MenuButton>() ?? ((Component)val4).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
				menuButton2.OnPress = new System.Action(NextPage);
				((Component)val4).gameObject.layer = 18;
			}
			else
			{
				Log.LogWarning((object)"[PlayersPage] ForwardPage not found under menu root or PlayersPage");
			}
		}

		private void OnPlayerClicked(int slotIndex)
		{
			int num = _page * 4 + slotIndex;
			if (num >= _rigs.Count)
			{
				return;
			}
			VRRig val = _rigs[num];
			if (!((Object)(object)val == (Object)null))
			{
				ShowPlayerInfo(val);
				ScryptoUtilsPad.Core.ModsPage instance = ScryptoUtilsPad.Core.ModsPage.Instance;
				if (instance != null)
				{
					instance.ShowRig(val);
				}
				PlaceSelectionDot(val);
			}
		}

		public void SelectRig(VRRig rig)
		{
			if ((Object)(object)rig == (Object)null)
			{
				return;
			}
			ShowPlayerInfo(rig);
			ScryptoUtilsPad.Core.ModsPage modsPage = ScryptoUtilsPad.Core.ModsPage.Instance;
			if (modsPage != null)
			{
				modsPage.ShowRig(rig);
			}
			PlaceSelectionDot(rig);
			ScryptoUtilsPad.Core.RigPreview preview = ScryptoUtilsPad.Core.RigPreview.Instance;
			if ((Object)(object)preview != (Object)null)
			{
				preview.Show(rig);
			}
		}

		private void ShowPlayerInfo(VRRig rig)
		{
			ScryptoUtilsPad.Core.RigPreview rigPreview = ScryptoUtilsPad.Core.RigPreview.Instance;
			if ((Object)(object)rigPreview != (Object)null)
			{
				rigPreview.Show(rig);
			}
			_selectedRig = rig;
			if ((Object)(object)_secondaryMenu != (Object)null)
			{
				_secondaryMenu.SetActive(true);
			}
			if ((Object)(object)_nameText != (Object)null)
			{
				TMP_Text nameText = _nameText;
				NetPlayer creator = rig.creator;
				nameText.text = string.Concat("Name: ", ((creator != null) ? creator.SanitizedNickName : null) ?? "Unknown");
			}
			if ((Object)(object)_fpsText != (Object)null)
			{
				_fpsText.text = string.Concat("FPS: ", ColoredFps(GetFps(rig)));
			}
			if ((Object)(object)_modText != (Object)null)
			{
				_modText.text = string.Format("Mods Detected: {0}", ScryptoUtilsPad.Core.ModsPage.CountDetectedMods(rig));
			}
			if ((Object)(object)_platformText != (Object)null)
			{
				_platformText.textWrappingMode = (TextWrappingModes)0;
				_platformText.overflowMode = (TextOverflowModes)0;
				_platformText.text = string.Concat("Platform: ", GetPlatform(rig));
			}
			if ((Object)(object)_colorText != (Object)null)
			{
				_colorText.text = string.Concat("Color Code: ", ColorCode(rig.playerColor));
			}
			if ((Object)(object)_dateText != (Object)null)
			{
				_dateText.text = "Join Date: ...";
			}
			FetchJoinDate(rig);
		}

		private void FetchJoinDate(VRRig rig)
		{
			NetPlayer creator = rig.creator;
			string text = ((creator != null) ? creator.UserId : null);
			if (string.IsNullOrEmpty(text))
			{
				if ((Object)(object)_dateText != (Object)null)
				{
					_dateText.text = "Join Date: N/A";
				}
			}
			else
			{
				GetAccountInfoRequest val = new GetAccountInfoRequest();
				val.PlayFabId = text;
				PlayFabClientAPI.GetAccountInfo(val, new System.Action<GetAccountInfoResult>(_003CFetchJoinDate_003Eb__28_0), new System.Action<PlayFabError>(_003CFetchJoinDate_003Eb__28_1), (object)null, (System.Collections.Generic.Dictionary<string, string>)null);
			}
		}

		public string GetFps(VRRig rig)
		{
			if (!_fpsFieldSearched)
			{
				_fpsFieldSearched = true;
				System.Reflection.FieldInfo[] fields = typeof(VRRig).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				int num = 0;
				while (num < fields.Length)
				{
					System.Reflection.FieldInfo fieldInfo = fields[num];
					if ((fieldInfo.FieldType == typeof(float) || fieldInfo.FieldType == typeof(int)) && fieldInfo.Name.IndexOf("fps", System.StringComparison.OrdinalIgnoreCase) >= 0)
					{
						_fpsField = fieldInfo;
						Log.LogInfo((object)string.Concat("[PlayersPage] Found VRRig FPS field '", fieldInfo.Name, "'"));
						break;
					}
					num++;
				}
				if (_fpsField == null)
				{
					Log.LogWarning((object)"[PlayersPage] No FPS field found on VRRig");
				}
			}
			if (_fpsField != null)
			{
				try
				{
					object value = _fpsField.GetValue(rig);
					if (value != null)
					{
						return Mathf.RoundToInt(System.Convert.ToSingle(value)).ToString();
					}
				}
				catch
				{
				}
			}
			return "N/A";
		}

		public static string ColorCode(Color c)
		{
			return string.Concat(string.Format("<color=red>{0}</color> ", System.Math.Round(c.r * 9f)), string.Format("<color=green>{0}</color> ", System.Math.Round(c.g * 9f)), string.Format("<color=#4985e6>{0}</color>", System.Math.Round(c.b * 9f)));
		}

		public static string ColoredFps(string fps)
		{
			int result;
			if (!int.TryParse(fps, out result))
			{
				return fps;
			}
			string text = ((result >= 60) ? "#00FF00" : ((result >= 30) ? "#FFFF00" : "#FF4444"));
			string[] array = new string[5];
			array[0] = "<color=";
			array[1] = text;
			array[2] = ">";
			array[3] = fps;
			array[4] = "</color>";
			return string.Concat(array);
		}

		private void PlaceSelectionDot(VRRig rig)
		{
			if ((Object)(object)_selectionDot != (Object)null)
			{
				Object.Destroy((Object)(object)_selectionDot);
			}
			_selectionDot = GameObject.CreatePrimitive((PrimitiveType)0);
			Object.Destroy((Object)(object)_selectionDot.GetComponent<Collider>());
			_selectionDot.transform.localScale = Vector3.one * 0.1f;
			Renderer component = _selectionDot.GetComponent<Renderer>();
			Material val = new Material(Shader.Find("GorillaTag/UberShader") ?? component.sharedMaterial.shader);
			val.color = Color.yellow;
			component.material = val;
		}

		private void Update()
		{
			if ((Object)(object)_selectionDot != (Object)null)
			{
				if ((Object)(object)_selectedRig == (Object)null || (Object)(object)_selectedRig.headMesh == (Object)null)
				{
					Object.Destroy((Object)(object)_selectionDot);
					_selectionDot = null;
				}
				else
				{
					float scaleFactor = _selectedRig.scaleFactor;
					_selectionDot.transform.position = _selectedRig.headMesh.transform.position + Vector3.up * (0.35f * scaleFactor);
					_selectionDot.transform.localScale = Vector3.one * (0.1f * scaleFactor);
				}
			}
			_fpsRefreshTimer -= Time.deltaTime;
			if (_fpsRefreshTimer <= 0f && (Object)(object)_selectedRig != (Object)null && _selectedRig.creator != null && (Object)(object)_secondaryMenu != (Object)null && _secondaryMenu.activeSelf && (Object)(object)_fpsText != (Object)null)
			{
				_fpsRefreshTimer = 0.5f;
				_fpsText.text = string.Concat("FPS: ", ColoredFps(GetFps(_selectedRig)));
			}
			if ((Object)(object)_pageTr == (Object)null || !((Component)_pageTr).gameObject.activeInHierarchy)
			{
				return;
			}
			_refreshTimer -= Time.deltaTime;
			if (!(_refreshTimer > 0f))
			{
				_refreshTimer = 2f;
				LoadRigs();
				if (_page > MaxPage)
				{
					_page = MaxPage;
				}
				Render();
			}
		}

		private void NextPage()
		{
			LoadRigs();
			if (_page < MaxPage)
			{
				_page++;
				Render();
			}
		}

		private void PrevPage()
		{
			if (_page > 0)
			{
				LoadRigs();
				_page--;
				if (_page > MaxPage)
				{
					_page = MaxPage;
				}
				Render();
			}
		}

		private void LoadRigs()
		{
			_rigs.Clear();
			VRRig[] array = System.Linq.Enumerable.ToArray<VRRig>(VRRigCache.ActiveRigs);
			int num = 0;
			while (num < array.Length)
			{
				VRRig val = array[num];
				if (!((Object)(object)val == (Object)null) && !val.isLocal)
				{
					NetPlayer creator = val.creator;
					if (!string.IsNullOrEmpty((creator != null) ? creator.SanitizedNickName : null))
					{
						_rigs.Add(val);
					}
				}
				num++;
			}
			Log.LogDebug((object)string.Format("[PlayersPage] LoadRigs found {0} players", _rigs.Count));
		}

		private bool TryLoadRigsFromGorillaParent()
		{
			if ((Object)(object)_gorillaParent == (Object)null)
			{
				_gorillaParent = Object.FindAnyObjectByType<GorillaParent>();
			}
			if ((Object)(object)_gorillaParent == (Object)null)
			{
				return false;
			}
			if (!_rigsFieldSearched)
			{
				_rigsFieldSearched = true;
				System.Reflection.FieldInfo[] fields = typeof(GorillaParent).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				int num = 0;
				while (num < fields.Length)
				{
					System.Reflection.FieldInfo fieldInfo = fields[num];
					System.Type fieldType = fieldInfo.FieldType;
					if (fieldType == typeof(VRRig[]) || (fieldType.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(fieldType) && fieldType.GetGenericArguments().Length != 0 && fieldType.GetGenericArguments()[0] == typeof(VRRig)))
					{
						_rigsField = fieldInfo;
						ManualLogSource log = Log;
						string[] array = new string[5];
						array[0] = "[PlayersPage] Found GorillaParent rig field '";
						array[1] = fieldInfo.Name;
						array[2] = "' (";
						array[3] = fieldType.Name;
						array[4] = ")";
						log.LogInfo((object)string.Concat(array));
						break;
					}
					num++;
				}
				if (_rigsField == null)
				{
					Log.LogWarning((object)"[PlayersPage] No VRRig collection on GorillaParent — using FindObjectsOfType fallback");
				}
			}
			if (_rigsField == null)
			{
				return false;
			}
			object value = _rigsField.GetValue(_gorillaParent);
			System.Collections.IEnumerable enumerable = value as System.Collections.IEnumerable;
			if (enumerable == null)
			{
				return false;
			}
			System.Collections.IEnumerator enumerator = enumerable.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					VRRig val = (VRRig)((current is VRRig) ? current : null);
					if (val != null && !val.isLocal)
					{
						NetPlayer creator = val.creator;
						if (!string.IsNullOrEmpty((creator != null) ? creator.SanitizedNickName : null))
						{
							_rigs.Add(val);
						}
					}
				}
			}
			finally
			{
				System.IDisposable disposable = enumerator as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return true;
		}

		private void Render()
		{
			int num = _page * 4;
			int num2 = 0;
			while (num2 < 4)
			{
				if (!((Object)(object)_slots[num2] == (Object)null))
				{
					int num3 = num + num2;
					if (num3 >= _rigs.Count)
					{
						_slots[num2].text = string.Empty;
					}
					else
					{
						NetPlayer creator = _rigs[num3].creator;
						string text = ((creator != null) ? creator.SanitizedNickName : null) ?? string.Empty;
						if (ScryptoUtilsPad.Core.ModsPage.HasIllegalMod(_rigs[num3]))
						{
							_slots[num2].text = string.Concat("<color=red>", text, "</color>");
						}
						else if (ScryptoUtilsPad.Core.ModsPage.HasAnyDetectedMod(_rigs[num3]))
						{
							_slots[num2].text = string.Concat("<color=green>", text, "</color>");
						}
						else
						{
							_slots[num2].text = text;
						}
					}
				}
				num2++;
			}
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

		private static void EnsureButtonReflection()
		{
			if (_btnFieldsSearched)
			{
				return;
			}
			_btnFieldsSearched = true;
			System.Type type = typeof(GorillaPlayerLineButton);
			System.Reflection.FieldInfo[] fields = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			int num = 0;
			while (num < fields.Length)
			{
				System.Reflection.FieldInfo fieldInfo = fields[num];
				if (fieldInfo.FieldType == typeof(ButtonType))
				{
					_btnTypeField = fieldInfo;
					break;
				}
				num++;
			}
			while (type != null && _btnActivateMethod == null)
			{
				System.Reflection.MethodInfo[] methods = type.GetMethods(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
				int num2 = 0;
				while (num2 < methods.Length)
				{
					System.Reflection.MethodInfo methodInfo = methods[num2];
					string name = methodInfo.Name;
					if (name.IndexOf("Activation", System.StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("PressButton", System.StringComparison.OrdinalIgnoreCase) >= 0 || name.IndexOf("Report", System.StringComparison.OrdinalIgnoreCase) >= 0)
					{
						_btnActivateMethod = methodInfo;
						break;
					}
					num2++;
				}
				type = type.BaseType;
			}
			ManualLogSource log = Log;
			System.Reflection.FieldInfo btnTypeField = _btnTypeField;
			string obj = (((object)btnTypeField != null) ? btnTypeField.Name : null) ?? "null";
			System.Reflection.MethodInfo btnActivateMethod = _btnActivateMethod;
			log.LogInfo((object)string.Concat("[PlayersPage] Report reflection — typeField: ", obj, ", activateMethod: ", (((object)btnActivateMethod != null) ? btnActivateMethod.Name : null) ?? "null"));
		}

		public void ReportPlayer(ButtonType type)
		{
			if ((Object)(object)_selectedRig == (Object)null)
			{
				Log.LogWarning("[PlayersPage] Report: no player selected.");
				return;
			}
			NetPlayer target = _selectedRig.creator;
			if (target == null)
			{
				Log.LogWarning("[PlayersPage] Report: selected rig has no creator.");
				return;
			}
			string targetId = target.UserId;
			GorillaPlayerLineButton[] array = Object.FindObjectsByType<GorillaPlayerLineButton>((FindObjectsInactive)1, (FindObjectsSortMode)0);
			int i = 0;
			while (i < array.Length)
			{
				GorillaPlayerLineButton btn = array[i];
				i++;
				if ((Object)(object)btn == (Object)null || btn.buttonType != type)
				{
					continue;
				}
				GorillaPlayerScoreboardLine line = btn.parentLine;
				NetPlayer linePlayer = (((Object)(object)line != (Object)null) ? line.linePlayer : null);
				if (linePlayer == null)
				{
					continue;
				}
				if (!string.IsNullOrEmpty(targetId) && linePlayer.UserId != targetId)
				{
					continue;
				}
				if (string.IsNullOrEmpty(targetId) && linePlayer != target)
				{
					continue;
				}
				try
				{
					btn.Click(true);
					Log.LogInfo("[PlayersPage] Reported " + target.SanitizedNickName + " (" + type.ToString() + ").");
				}
				catch (System.Exception e)
				{
					Log.LogWarning("[PlayersPage] Report click failed: " + e.Message);
				}
				return;
			}
			Log.LogWarning("[PlayersPage] No report button found for " + target.SanitizedNickName
				+ " (scanned " + array.Length + " buttons; open the in-game scoreboard at least once so the lines exist).");
		}

		private static void EnsurePlatformFields()
		{
			if (_platformFieldsSearched)
			{
				return;
			}
			_platformFieldsSearched = true;
			System.Reflection.FieldInfo[] fields = typeof(VRRig).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			int num = 0;
			while (num < fields.Length)
			{
				System.Reflection.FieldInfo fieldInfo = fields[num];
				if (_cosmeticsField == null && fieldInfo.Name.IndexOf("cosmetic", System.StringComparison.OrdinalIgnoreCase) >= 0 && typeof(System.Collections.IEnumerable).IsAssignableFrom(fieldInfo.FieldType) && fieldInfo.FieldType != typeof(string))
				{
					_cosmeticsField = fieldInfo;
				}
				if (_rankedTierField == null && (fieldInfo.FieldType == typeof(int) || fieldInfo.FieldType == typeof(float)) && fieldInfo.Name.IndexOf("rankedSubTierPC", System.StringComparison.OrdinalIgnoreCase) >= 0)
				{
					_rankedTierField = fieldInfo;
				}
				num++;
			}
		}

		/*
		 * Platform detection adapted from BingusNametags++ (MIT License),
		 * Copyright (c) SirKingBinx. Used and modified under the MIT License.
		 * https://github.com/SirKingBinx/BingusNametagsPlusPlus
		 */
		private static readonly System.Collections.Generic.Dictionary<string, string> _platformCache = new System.Collections.Generic.Dictionary<string, string>(System.StringComparer.Ordinal);

		public static void ClearPlatformCache()
		{
			_platformCache.Clear();
		}


		public static string GetPlatform(VRRig rig)
		{
			if ((Object)(object)rig == (Object)null)
			{
				return "Unknown";
			}
			NetPlayer creator = rig.creator;
			string userId = ((creator != null) ? creator.UserId : null);
			if (!string.IsNullOrEmpty(userId))
			{
				string cached;
				if (_platformCache.TryGetValue(userId, out cached))
				{
					return cached;
				}
			}
			if (!rig.InitializedCosmetics)
			{
				return "Unknown";
			}
			string platform = "Unknown";
			try
			{
				int props = 0;
				if (creator != null)
				{
					Player pr = creator.GetPlayerRef();
					Hashtable cp = ((pr != null) ? pr.CustomProperties : null);
					if (cp != null)
					{
						props = ((System.Collections.Generic.Dictionary<object, object>)(object)cp).Count;
					}
				}
				if (rig.currentRankedSubTierPC > 0 || props > 1)
				{
					platform = "PC";
				}
				if (rig.currentRankedSubTierQuest > 0)
				{
					platform = "Quest";
				}
				else
				{
					System.Collections.Generic.HashSet<string> owned = rig._playerOwnedCosmetics;
					if (owned != null)
					{
						bool steam = false;
						bool first = false;
						foreach (string c in owned)
						{
							if (c == null)
							{
								continue;
							}
							string lc = c.ToLower();
							if (lc == "s. first login")
							{
								steam = true;
							}
							else if (lc == "first login")
							{
								first = true;
							}
						}
						if (steam)
						{
							platform = "Steam";
						}
						else if (first)
						{
							platform = "Oculus";
						}
					}
				}
			}
			catch
			{
				return "Unknown";
			}
			if (platform != "Unknown" && !string.IsNullOrEmpty(userId))
			{
				_platformCache[userId] = platform;
			}
			return platform;
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private void _003CInit_003Eb__25_0()
		{
			ReportPlayer((ButtonType)1);
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private void _003CInit_003Eb__25_1()
		{
			ReportPlayer((ButtonType)2);
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private void _003CInit_003Eb__25_2()
		{
			ReportPlayer((ButtonType)0);
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private void _003CFetchJoinDate_003Eb__28_0(GetAccountInfoResult result)
		{
			string text = result.AccountInfo.Created.ToLocalTime().ToString("M/d/yy");
			if ((Object)(object)_dateText != (Object)null)
			{
				_dateText.text = string.Concat("Join Date: ", text);
			}
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private void _003CFetchJoinDate_003Eb__28_1(PlayFabError error)
		{
			if ((Object)(object)_dateText != (Object)null)
			{
				_dateText.text = "Join Date: N/A";
			}
			Log.LogWarning((object)string.Concat("[PlayersPage] GetAccountInfo failed: ", error.ErrorMessage));
		}
	}
}
