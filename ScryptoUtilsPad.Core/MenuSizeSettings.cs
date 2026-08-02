/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using UnityEngine;

namespace ScryptoUtilsPad.Core
{
	public static class MenuSizeSettings
	{
		public static readonly string[] Names = new string[5] { "Tiny", "Small", "Normal", "Large", "Huge" };

		private static readonly float[] Scales = new float[5] { 0.7f, 0.85f, 1f, 1.2f, 1.45f };

		private static int _index = 2;

		public static Transform MenuRoot;

		public static int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = ((value % Names.Length) + Names.Length) % Names.Length;
				PlayerPrefs.SetInt("ScryptoUtilsPad.MenuSize", _index);
			}
		}

		public static string CurrentName
		{
			get
			{
				return Names[_index];
			}
		}

		public static float CurrentScale
		{
			get
			{
				return Scales[_index];
			}
		}

		public static void Load()
		{
			_index = PlayerPrefs.GetInt("ScryptoUtilsPad.MenuSize", 2);
			if (_index < 0 || _index >= Names.Length)
			{
				_index = 2;
			}
		}

		public static void Apply()
		{
			if ((Object)(object)MenuRoot == (Object)null)
			{
				return;
			}
			MenuRoot.localScale = Vector3.one * (0.85f * CurrentScale);
		}
	}

	public static class NametagExtras
	{
		public static bool ShowFps;

		public static bool ShowPlatform;

		public static void Load()
		{
			ShowFps = PlayerPrefs.GetInt("ScryptoUtilsPad.TagFps", 0) == 1;
			ShowPlatform = PlayerPrefs.GetInt("ScryptoUtilsPad.TagPlatform", 0) == 1;
		}

		public static void Save()
		{
			PlayerPrefs.SetInt("ScryptoUtilsPad.TagFps", ShowFps ? 1 : 0);
			PlayerPrefs.SetInt("ScryptoUtilsPad.TagPlatform", ShowPlatform ? 1 : 0);
		}

		public static string BuildSuffix(VRRig rig)
		{
			if ((Object)(object)rig == (Object)null || (!ShowFps && !ShowPlatform))
			{
				return string.Empty;
			}
			string platform = string.Empty;
			if (ShowPlatform)
			{
				try
				{
					platform = ScryptoUtilsPad.Core.PlayersPage.GetPlatform(rig) ?? string.Empty;
				}
				catch
				{
					platform = string.Empty;
				}
			}
			string fps = string.Empty;
			if (ShowFps)
			{
				int f = rig.fps;
				if (f > 0)
				{
					fps = f + " FPS";
				}
			}
			if (platform.Length > 0 && fps.Length > 0)
			{
				return "\n<size=70%>" + platform + " | " + fps + "</size>";
			}
			if (platform.Length > 0)
			{
				return "\n<size=70%>" + platform + "</size>";
			}
			if (fps.Length > 0)
			{
				return "\n<size=70%>" + fps + "</size>";
			}
			return string.Empty;
		}

		private static string GetPlatform(VRRig rig)
		{
			try
			{
				NetPlayer creator = rig.creator;
				if (creator == null)
				{
					return string.Empty;
				}
				Photon.Realtime.Player p = creator.GetPlayerRef();
				if (p == null || p.CustomProperties == null)
				{
					return string.Empty;
				}
				object val;
				if (!p.CustomProperties.TryGetValue("gtag_platform", out val) || val == null)
				{
					return string.Empty;
				}
				string s = val.ToString();
				if (s.IndexOf("stand", System.StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return "Quest";
				}
				if (s.IndexOf("steam", System.StringComparison.OrdinalIgnoreCase) >= 0)
				{
					return "Steam";
				}
				return s;
			}
			catch
			{
				return string.Empty;
			}
		}
	}
}
