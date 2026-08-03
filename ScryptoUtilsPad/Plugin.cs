namespace ScryptoUtilsPad
{
	[BepInPlugin("com.lowmiku.utilspad", "SUPC", "2.0.3")]
	public class Plugin : BaseUnityPlugin
	{
		public enum OpenButton
		{
			LeftPrimary,
			RightPrimary,
			LeftSecondary,
			RightSecondary
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static ScryptoUtilsPad.Plugin _003CInstance_003Ek__BackingField;

		public ConfigEntry<ScryptoUtilsPad.Plugin.OpenButton> Keybind;

		public AudioClip MenuOpenSound;

		public AudioClip MenuCloseSound;

		public AudioClip ButtonClickSound;

		public TMP_FontAsset Font;

		private AssetBundle _soundBundle;

		public static ScryptoUtilsPad.Plugin Instance
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CInstance_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CInstance_003Ek__BackingField = value;
			}
		}

		internal static bool DumpMenuHierarchy;

		private static void DumpHierarchy(Transform t, string path)
		{
			string self = (string.IsNullOrEmpty(path) ? ((Object)t).name : (path + "/" + ((Object)t).name));
			BepInEx.Logging.ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource("SUPCDump");
			log.LogInfo("[Hierarchy] " + self);
			int i = 0;
			while (i < t.childCount)
			{
				DumpHierarchy(t.GetChild(i), self);
				i++;
			}
		}

		private static void NeutralizeColliders(GameObject root)
		{
			if ((Object)(object)root == (Object)null)
			{
				return;
			}
			Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
			int i = 0;
			while (i < colliders.Length)
			{
				Collider col = colliders[i];
				if ((Object)(object)col != (Object)null)
				{
					col.isTrigger = true;
					UnityLayerExtensions.SetLayer(((Component)col).gameObject, (UnityLayer)18);
				}
				i++;
			}
		}

		private void Awake()
		{
			Instance = this;
			ScryptoUtilsPad.Core.BadLinkKiller.Apply();
			Keybind = ((BaseUnityPlugin)this).Config.Bind<ScryptoUtilsPad.Plugin.OpenButton>("Settings", "OpenButton", ScryptoUtilsPad.Plugin.OpenButton.LeftPrimary, "Button used to open/close the menu");
			ScryptoUtilsPad.Core.CheatSigGetter.Enabled = ((BaseUnityPlugin)this).Config.Bind<bool>("Signatures", "Collect Signatures", true, "Collect unknown mod/cheat signature keys seen in lobbies.").Value;
			ScryptoUtilsPad.Core.CheatSigGetter.AutoUpload = ((BaseUnityPlugin)this).Config.Bind<bool>("Signatures", "Auto Upload", true, "Upload new signature keys (keys only, no player info) to help keep the lists current.").Value;
			DumpMenuHierarchy = ((BaseUnityPlugin)this).Config.Bind<bool>("Debug", "Dump Menu Hierarchy", false, "Log the full menu prefab hierarchy once at startup (for finding UI element names).").Value;
			ScryptoUtilsPad.Core.CheatSigGetter.Init((MonoBehaviour)(object)this);
			AssetBundle val = ScryptoUtilsPad.Core.AssetManagement.LoadBundle("cychecker");
			if ((Object)(object)val == (Object)null)
			{
				Logger.LogError((object)"[ScryptoUtilsPad] Failed to load asset bundle 'cychecker'. Is it embedded as a resource?");
				return;
			}
			GameObject val2 = val.LoadAsset<GameObject>("EmptyMenu");
			if ((Object)(object)val2 == (Object)null)
			{
				Logger.LogWarning((object)"[ScryptoUtilsPad] 'EmptyMenu' asset not found in bundle — remote menus will be disabled.");
			}
			ScryptoUtilsPad.Core.NetworkManager networkManager = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.NetworkManager>();
			networkManager.Init(val2);
			GameObject val3 = val.LoadAsset<GameObject>("Menu");
			if ((Object)(object)val3 == (Object)null)
			{
				string text = string.Join(", ", val.GetAllAssetNames());
				Logger.LogError((object)string.Concat("[ScryptoUtilsPad] 'Menu' asset not found in bundle. Available assets: [", text, "]"));
				return;
			}
			GameObject val4 = Object.Instantiate<GameObject>(val3);
			ScryptoUtilsPad.Core.ConfigMigration.RunIfNeeded(((BaseUnityPlugin)this).Config);
			ScryptoUtilsPad.Core.UserFiles.EnsureFolders();
			ScryptoUtilsPad.Core.UserFiles.LoadThemes();
			ScryptoUtilsPad.Core.UserFiles.RefreshConfigList();
			ScryptoUtilsPad.Core.SharedConfig.Load();
			ScryptoUtilsPad.Core.UserFiles.LoadActiveProfileAtStartup();
			ScryptoUtilsPad.Core.SharedConfig.EnableAutoSave();
			ScryptoUtilsPad.Core.MenuSizeSettings.Load();
			ScryptoUtilsPad.Core.ColorSettings.Load();
			ScryptoUtilsPad.Core.RigPreview.LoadPrefs();
			ScryptoUtilsPad.Core.DesktopCamera.LoadPrefs();
			((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.DesktopCamera>();
			ScryptoUtilsPad.Core.NametagExtras.Load();
			ScryptoUtilsPad.Core.MenuSizeSettings.MenuRoot = val4.transform;
			val4.transform.localScale = Vector3.one * (0.85f * ScryptoUtilsPad.Core.MenuSizeSettings.CurrentScale);
			Object.DontDestroyOnLoad((Object)(object)val4);
			val4.SetActive(false);
			NeutralizeColliders(val4);
			Shader val5 = Shader.Find("GorillaTag/UberShader");
			if ((Object)(object)val5 != (Object)null)
			{
				Renderer[] componentsInChildren = val4.GetComponentsInChildren<Renderer>(true);
				int num = 0;
				while (num < componentsInChildren.Length)
				{
					Renderer val6 = componentsInChildren[num];
					Material[] materials = val6.materials;
					int num2 = 0;
					while (num2 < materials.Length)
					{
						Material val7 = materials[num2];
						if (((Object)val7.shader).name != "UI/Default")
						{
							val7.shader = val5;
						}
						num2++;
					}
					num++;
				}
			}
			Font = ScryptoUtilsPad.Core.AssetManagement.LoadEmbeddedFont("JetBrainsMono-Bold");
			if ((Object)(object)Font != (Object)null)
			{
				TMP_Text[] componentsInChildren2 = val4.GetComponentsInChildren<TMP_Text>(true);
				int num3 = 0;
				while (num3 < componentsInChildren2.Length)
				{
					TMP_Text val8 = componentsInChildren2[num3];
					val8.font = Font;
					num3++;
				}
			}
			ScryptoUtilsPad.Core.Branding.Apply(val4);
			ScryptoUtilsPad.Core.Branding.Apply(val2);
			ScryptoUtilsPad.Core.PositionHandler.Checker = val4;
			MenuOpenSound = val.LoadAsset<AudioClip>("CYOpen");
			MenuCloseSound = val.LoadAsset<AudioClip>("CYClose");
			ButtonClickSound = val.LoadAsset<AudioClip>("CYClick");
			ScryptoUtilsPad.Core.ClickSoundSettings.Init(ButtonClickSound);
			ScryptoUtilsPad.Core.MenuSoundSettings.Init(MenuOpenSound, MenuCloseSound);
			ScryptoUtilsPad.Core.NametagFontSettings.Init(Font);
			_soundBundle = val;
			Rigidbody val9 = val4.AddComponent<Rigidbody>();
			val9.isKinematic = true;
			val9.useGravity = false;
			((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.PositionHandler>();
			((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Tools.MediaManager>();
			Transform val10 = val4.transform.Find("Handle");
			if ((Object)(object)val10 != (Object)null)
			{
				((Component)val10).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuHandle>();
			}
			GameObject cameraPrefab = val.LoadAsset<GameObject>("Camera");
			if (DumpMenuHierarchy)
			{
				DumpHierarchy(val4.transform, "");
			}
			ScryptoUtilsPad.Core.MenuManager menuManager = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuManager>();
			menuManager.Init(val4);
			ScryptoUtilsPad.Tools.CameraPage cameraPage = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Tools.CameraPage>();
			cameraPage.Init(val4, cameraPrefab);
			((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.GunSelector>();
			ScryptoUtilsPad.Core.RigPreview rigPreview = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.RigPreview>();
			rigPreview.Init(val4.transform);
			ScryptoUtilsPad.Core.PlayersPage playersPage = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.PlayersPage>();
			playersPage.Init(val4.transform);
			ScryptoUtilsPad.Core.RoomPage roomPage = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.RoomPage>();
			roomPage.Init(val4.transform);
			ScryptoUtilsPad.Core.ModsPage modsPage = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.ModsPage>();
			modsPage.Init(val4.transform);
			ScryptoUtilsPad.Core.TimePage timePage = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.TimePage>();
			timePage.Init(val4.transform);
			ScryptoUtilsPad.Core.MusicPage musicPage = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.MusicPage>();
			musicPage.Init(val4.transform);
			ScryptoUtilsPad.Core.SettingsPage settingsPage = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.SettingsPage>();
			settingsPage.Init(val4.transform);
			ScryptoUtilsPad.Core.NotificationManager notificationManager = ((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.NotificationManager>();
			notificationManager.Init(val.LoadAsset<GameObject>("Notification"));
			((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.NametagManager>();
			((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.OwnerNametagsDriver>();
			((Component)this).gameObject.AddComponent<ScryptoUtilsPad.Core.DiscordRPC>();
			ScryptoUtilsPad.Core.DiscordRPC.SetPresence("In Menu", "SUPC");
		}


	}
}
