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
		}

		private void ShowPlayerInfo(VRRig rig)
		{
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

		private string GetFps(VRRig rig)
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

		private static string ColorCode(Color c)
		{
			return string.Concat(string.Format("<color=red>{0}</color> ", System.Math.Round(c.r * 9f)), string.Format("<color=green>{0}</color> ", System.Math.Round(c.g * 9f)), string.Format("<color=#4985e6>{0}</color>", System.Math.Round(c.b * 9f)));
		}

		private static string ColoredFps(string fps)
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
						_slots[num2].text = (ScryptoUtilsPad.Core.ModsPage.HasAnyDetectedMod(_rigs[num3]) ? string.Concat("<color=red>", text, "</color>") : text);
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

		private void ReportPlayer(ButtonType type)
		{
			if ((Object)(object)_selectedRig == (Object)null)
			{
				return;
			}
			EnsureButtonReflection();
			NetPlayer creator = _selectedRig.Creator;
			System.Type type2 = ((creator != null) ? ((object)creator).GetType() : null);
			GorillaPlayerLineButton[] array = Object.FindObjectsByType<GorillaPlayerLineButton>((FindObjectsInactive)1, (FindObjectsSortMode)0);
			int num = 0;
			while (num < array.Length)
			{
				GorillaPlayerLineButton val = array[num];
				if (_btnTypeField != null)
				{
					object value = _btnTypeField.GetValue(val);
					if (value == null || (ButtonType)value != type)
					{
						goto IL_01cc;
					}
				}
				bool flag = false;
				Transform parent = ((Component)val).transform.parent;
				while ((Object)(object)parent != (Object)null && !flag)
				{
					Component[] components = ((Component)parent).GetComponents<Component>();
					int num2 = 0;
					while (num2 < components.Length)
					{
						Component val2 = components[num2];
						if (!((Object)(object)val2 == (Object)null))
						{
							System.Reflection.FieldInfo[] fields = ((object)val2).GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
							int num3 = 0;
							while (num3 < fields.Length)
							{
								System.Reflection.FieldInfo fieldInfo = fields[num3];
								if ((!(type2 != null) || !(fieldInfo.FieldType != type2)) && object.Equals(fieldInfo.GetValue(val2), creator))
								{
									flag = true;
									break;
								}
								num3++;
							}
							if (flag)
							{
								break;
							}
						}
						num2++;
					}
					parent = parent.parent;
				}
				if (!flag)
				{
					goto IL_01cc;
				}
				System.Reflection.MethodInfo btnActivateMethod = _btnActivateMethod;
				if ((object)btnActivateMethod != null)
				{
					btnActivateMethod.Invoke(val, null);
				}
				ManualLogSource log = Log;
				NetPlayer creator2 = _selectedRig.creator;
				log.LogInfo((object)string.Format("[PlayersPage] Reported {0} for {1}", (creator2 != null) ? creator2.SanitizedNickName : null, type));
				return;
				IL_01cc:
				num++;
			}
			ManualLogSource log2 = Log;
			NetPlayer creator3 = _selectedRig.creator;
			log2.LogWarning((object)string.Concat("[PlayersPage] No report button found for ", (creator3 != null) ? creator3.SanitizedNickName : null));
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

		private static string GetPlatform(VRRig rig)
		{
			EnsurePlatformFields();
			string text = string.Empty;
			if (_cosmeticsField != null)
			{
				try
				{
					text = string.Concat((System.Collections.IEnumerable)_cosmeticsField.GetValue(rig));
				}
				catch
				{
				}
			}
			if (text.Contains("S. FIRST LOGIN"))
			{
				return "STEAM";
			}
			int num = 0;
			if (_rankedTierField != null)
			{
				try
				{
					num = System.Convert.ToInt32(_rankedTierField.GetValue(rig));
				}
				catch
				{
				}
			}
			NetPlayer creator = rig.creator;
			object obj3;
			if (creator == null)
			{
				obj3 = null;
			}
			else
			{
				Player playerRef = creator.GetPlayerRef();
				obj3 = ((playerRef != null) ? playerRef.CustomProperties : null);
			}
			Hashtable val = (Hashtable)obj3;
			if (text.Contains("FIRST LOGIN") || (val != null && ((System.Collections.Generic.Dictionary<object, object>)(object)val).Count >= 2) || num > 0)
			{
				return "PC";
			}
			return "Standalone";
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
