namespace ScryptoUtilsPad.Core
{
	public static class ClickSoundSettings
	{
		public static readonly string[] Names = new string[5] { "Default", "Clicky", "Woody", "Border", "Creamy" };

		private static readonly AudioClip[] _clips = new AudioClip[5];

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
				PlayerPrefs.SetInt("ScryptoUtilsPad.ClickSound", _index);
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

		public static void Init(AudioClip defaultClip)
		{
			_clips[0] = defaultClip;
			_clips[1] = ScryptoUtilsPad.Core.AssetManagement.LoadEmbeddedWav("Clicky");
			_clips[2] = ScryptoUtilsPad.Core.AssetManagement.LoadEmbeddedWav("Woody Click");
			_clips[3] = ScryptoUtilsPad.Core.AssetManagement.LoadEmbeddedWav("Border");
            _clips[4] = ScryptoUtilsPad.Core.AssetManagement.LoadEmbeddedWav("Creamy");
            Load();
		}

		public static void Load()
		{
			_index = PlayerPrefs.GetInt("ScryptoUtilsPad.ClickSound", 0);
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
			AudioClip clip = _clips[_index];
			if ((Object)(object)clip != (Object)null)
			{
				instance.ButtonClickSound = clip;
			}
		}
	}
}
