namespace ScryptoUtilsPad.Core
{
	public class SettingsPage : MonoBehaviour
	{
		private static readonly Color DefaultMat1 = new Color(0f, 1f, 0.667f);

		private static readonly Color DefaultMat2 = new Color(0.169f, 0.169f, 0.169f);

		private static readonly System.ValueTuple<string, Color, Color>[] Themes;

		private static readonly string[] ModeNames;

		private int _themeIndex;

		private int _modeIndex;

		private Color _currentMat1 = DefaultMat1;

		private Color _currentMat2 = DefaultMat2;

		private TMP_Text _modeText;

		private TMP_Text _selectModeText;

		private TMP_Text _themeText;

		private TMP_Text _nametagText;

		private GameObject _menu;

		public void Init(Transform root)
		{
			_menu = ((Component)root).gameObject;
			Transform val = root.Find("SettingsPage");
			if (!((Object)(object)val == (Object)null))
			{
				Transform val2 = val.Find("Mode");
				Transform val3 = val.Find("Theme");
				if ((Object)(object)val2 != (Object)null)
				{
					Transform obj = val2.Find("Text (TMP)");
					_modeText = ((obj != null) ? ((Component)obj).GetComponent<TMP_Text>() : null);
				}
				if ((Object)(object)val3 != (Object)null)
				{
					Transform obj2 = val3.Find("Text (TMP)");
					_themeText = ((obj2 != null) ? ((Component)obj2).GetComponent<TMP_Text>() : null);
				}
				Transform selTr = val.Find("SelectMode");
				if ((Object)(object)selTr != (Object)null)
				{
					Transform selLabel = selTr.Find("Text (TMP)");
					_selectModeText = ((selLabel != null) ? ((Component)selLabel).GetComponent<TMP_Text>() : null);
				}
				SetupButton(val.Find("SelectModePlus"), new System.Action(NextSelectMode));
				SetupButton(val.Find("SelectModeNegative"), new System.Action(PrevSelectMode));
				SetupButton(val.Find("ModePlus"), new System.Action(NextMode));
				SetupButton(val.Find("ModeNegative"), new System.Action(PrevMode));
				SetupButton(val.Find("ThemePlus"), new System.Action(NextTheme));
				SetupButton(val.Find("ThemeNegative"), new System.Action(PrevTheme));
				Transform val4 = val.Find("Nametags");
				if ((Object)(object)val4 != (Object)null)
				{
					Transform obj3 = val4.Find("Text (TMP)");
					_nametagText = ((obj3 != null) ? ((Component)obj3).GetComponent<TMP_Text>() : null);
					SetupButton(val4, new System.Action(ToggleNametags));
				}
				_themeIndex = PlayerPrefs.GetInt("ScryptoUtilsPad.ThemeIndex", 0);
				_modeIndex = PlayerPrefs.GetInt("ScryptoUtilsPad.ModeIndex", 0);
				ScryptoUtilsPad.Core.SelectionSettings.Load();
				ApplyTheme();
				ApplyMode();
			}
		}

		private void ToggleNametags()
		{
			ScryptoUtilsPad.Core.NametagManager.Enabled = !ScryptoUtilsPad.Core.NametagManager.Enabled;
			UpdateTexts();
		}

		private void NextSelectMode()
		{
			ScryptoUtilsPad.Core.SelectionSettings.Index = ScryptoUtilsPad.Core.SelectionSettings.Index + 1;
			UpdateTexts();
		}

		private void PrevSelectMode()
		{
			ScryptoUtilsPad.Core.SelectionSettings.Index = ScryptoUtilsPad.Core.SelectionSettings.Index - 1;
			UpdateTexts();
		}

		private void NextMode()
		{
			_modeIndex = (_modeIndex + 1) % ModeNames.Length;
			ApplyMode();
		}

		private void PrevMode()
		{
			_modeIndex = (_modeIndex - 1 + ModeNames.Length) % ModeNames.Length;
			ApplyMode();
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

		private void NextTheme()
		{
			_themeIndex = (_themeIndex + 1) % Themes.Length;
			ApplyTheme();
		}

		private void PrevTheme()
		{
			_themeIndex = (_themeIndex - 1 + Themes.Length) % Themes.Length;
			ApplyTheme();
		}

		private void ApplyTheme()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
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

		private void UpdateTexts()
		{
			if ((Object)(object)_selectModeText != (Object)null)
			{
				_selectModeText.text = string.Concat("Select: ", ScryptoUtilsPad.Core.SelectionSettings.CurrentName);
			}
			if ((Object)(object)_modeText != (Object)null)
			{
				_modeText.text = string.Concat("Mode: ", ModeNames[_modeIndex]);
			}
			if ((Object)(object)_themeText != (Object)null)
			{
				_themeText.text = string.Concat("Theme: ", Themes[_themeIndex].Item1);
			}
			if ((Object)(object)_nametagText != (Object)null)
			{
				_nametagText.text = string.Concat("Nametags: ", ScryptoUtilsPad.Core.NametagManager.Enabled ? "On" : "Off");
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

		private static bool ColorClose(Color a, Color b, float tol = 0.05f)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			return Mathf.Abs(a.r - b.r) < tol && Mathf.Abs(a.g - b.g) < tol && Mathf.Abs(a.b - b.b) < tol;
		}

		static SettingsPage()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01de: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			System.ValueTuple<string, Color, Color>[] array = new System.ValueTuple<string, Color, Color>[9];
			array[0] = new System.ValueTuple<string, Color, Color>("Default", new Color(0f, 1f, 0.667f), new Color(0.169f, 0.169f, 0.169f));
			array[1] = new System.ValueTuple<string, Color, Color>("Neon", new Color(1f, 0f, 1f), new Color(0.102f, 0.102f, 0.188f));
			array[2] = new System.ValueTuple<string, Color, Color>("Fire", new Color(1f, 0.271f, 0f), new Color(0.11f, 0.039f, 0f));
			array[3] = new System.ValueTuple<string, Color, Color>("Ice", new Color(0f, 0.749f, 1f), new Color(0.039f, 0.039f, 0.102f));
			array[4] = new System.ValueTuple<string, Color, Color>("Gold", new Color(1f, 0.843f, 0f), new Color(0.102f, 0.078f, 0f));
			array[5] = new System.ValueTuple<string, Color, Color>("Black", new Color(0.15f, 0.15f, 0.15f), new Color(0.05f, 0.05f, 0.05f));
			array[6] = new System.ValueTuple<string, Color, Color>("White", new Color(0.95f, 0.95f, 0.95f), new Color(0.75f, 0.75f, 0.75f));
			array[7] = new System.ValueTuple<string, Color, Color>("Purple", new Color(0.502f, 0f, 0.502f), new Color(0.078f, 0f, 0.11f));
			array[8] = new System.ValueTuple<string, Color, Color>("Brown", new Color(0.647f, 0.165f, 0.165f), new Color(0.102f, 0.039f, 0.02f));
			Themes = array;
			string[] array2 = new string[2];
			array2[0] = "Float";
			array2[1] = "Hold";
			ModeNames = array2;
		}
	}
}
