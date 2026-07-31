namespace ScryptoUtilsPad.Core
{
	public static class MenuSoundSettings
	{
		public static readonly string[] Names = new string[3] { "Default", "Splash", "Half Life" };

		private static readonly AudioClip[] _clips = new AudioClip[3];

		private static AudioClip _defaultOpen;

		private static AudioClip _defaultClose;

		private static int _index;

		public static int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = ((value % Names.Length) + Names.Length) % Names.Length;
				PlayerPrefs.SetInt("ScryptoUtilsPad.MenuSound", _index);
				Apply();
			}
		}

		public static string CurrentName
		{
			get
			{
				return Names[_index];
			}
		}

		public static void Init(AudioClip defaultOpen, AudioClip defaultClose)
		{
			_defaultOpen = defaultOpen;
			_defaultClose = defaultClose;
			_clips[1] = ScryptoUtilsPad.Core.AssetManagement.LoadEmbeddedWav("Splash");
			_clips[2] = ScryptoUtilsPad.Core.AssetManagement.LoadEmbeddedWav("Half-Life");
			Load();
		}

		public static void Load()
		{
			_index = PlayerPrefs.GetInt("ScryptoUtilsPad.MenuSound", 0);
			if (_index < 0 || _index >= Names.Length)
			{
				_index = 0;
			}
			Apply();
		}

		private static void Apply()
		{
			ScryptoUtilsPad.Plugin instance = ScryptoUtilsPad.Plugin.Instance;
			if (instance == null)
			{
				return;
			}
			if (_index == 0)
			{
				if ((Object)(object)_defaultOpen != (Object)null)
				{
					instance.MenuOpenSound = _defaultOpen;
				}
				if ((Object)(object)_defaultClose != (Object)null)
				{
					instance.MenuCloseSound = _defaultClose;
				}
				return;
			}
			AudioClip clip = _clips[_index];
			if ((Object)(object)clip != (Object)null)
			{
				instance.MenuOpenSound = clip;
				instance.MenuCloseSound = clip;
			}
		}
	}
}
