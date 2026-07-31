namespace ScryptoUtilsPad.Core
{
	public static class NametagFontSettings
	{
		public static readonly string[] Names = new string[2] { "JetBrains Mono", "Pixel" };

		private static readonly TMP_FontAsset[] _fonts = new TMP_FontAsset[2];

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
				PlayerPrefs.SetInt("ScryptoUtilsPad.NametagFont", _index);
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

		public static TMP_FontAsset CurrentFont
		{
			get
			{
				TMP_FontAsset font = _fonts[_index];
				return ((Object)(object)font != (Object)null) ? font : _fonts[0];
			}
		}

		public static void Init(TMP_FontAsset defaultFont)
		{
			_fonts[0] = defaultFont;
			_fonts[1] = ScryptoUtilsPad.Core.AssetManagement.LoadEmbeddedFont("pixel");
			Load();
		}

		public static void Load()
		{
			_index = PlayerPrefs.GetInt("ScryptoUtilsPad.NametagFont", 0);
			if (_index < 0 || _index >= Names.Length)
			{
				_index = 0;
			}
			Apply();
		}

		private static void Apply()
		{
			ScryptoUtilsPad.Core.NametagManager instance = ScryptoUtilsPad.Core.NametagManager.Instance;
			if ((Object)(object)instance != (Object)null)
			{
				instance.RefreshFont();
			}
			ScryptoUtilsPad.Core.OwnerNametags.RefreshFont();
		}
	}
}
