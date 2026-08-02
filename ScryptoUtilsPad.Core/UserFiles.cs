/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using BepInEx.Logging;
using UnityEngine;

namespace ScryptoUtilsPad.Core
{
	public static class UserFiles
	{
		private static readonly ManualLogSource Log = Logger.CreateLogSource("SUPCFiles");

		public static string RootDir
		{
			get
			{
				return Path.Combine(Directory.GetCurrentDirectory(), "SUPC");
			}
		}

		public static string ThemesDir
		{
			get
			{
				return Path.Combine(RootDir, "Themes");
			}
		}

		public static string ConfigsDir
		{
			get
			{
				return Path.Combine(RootDir, "Configs");
			}
		}

		public static readonly List<string> ConfigNames = new List<string>();

		private static int _configIndex;

		public static string CurrentConfigName
		{
			get
			{
				if (ConfigNames.Count == 0)
				{
					return "None";
				}
				return ConfigNames[Mathf.Clamp(_configIndex, 0, ConfigNames.Count - 1)];
			}
		}

		public static int ConfigIndex
		{
			get
			{
				return _configIndex;
			}
			set
			{
				if (ConfigNames.Count == 0)
				{
					_configIndex = 0;
					return;
				}
				_configIndex = ((value % ConfigNames.Count) + ConfigNames.Count) % ConfigNames.Count;
			}
		}

		public static void EnsureFolders()
		{
			try
			{
				if (!Directory.Exists(ThemesDir))
				{
					Directory.CreateDirectory(ThemesDir);
					File.WriteAllText(Path.Combine(ThemesDir, "README.txt"),
						"Custom SUPC themes.\n\n" +
						"Create a .txt file here — the FILE NAME becomes the theme name.\n" +
						"Example: uwu.txt  ->  a theme called \"uwu\"\n\n" +
						"File contents (hex or r,g,b 0-255):\n" +
						"primary = #FF66CC\n" +
						"secondary = #1A0A14\n\n" +
						"primary   = the bright/accent colour\n" +
						"secondary = the dark/background colour\n");
				}
				if (!Directory.Exists(ConfigsDir))
				{
					Directory.CreateDirectory(ConfigsDir);
					File.WriteAllText(Path.Combine(ConfigsDir, "README.txt"),
						"SUPC config profiles.\n\n" +
						"Put SUPC_config.txt copies here and rename them, e.g.\n" +
						"  recording.txt\n" +
						"  everyday.txt\n\n" +
						"Then switch between them from Settings -> Config Profile,\n" +
						"and press 'Load Profile' to apply it.\n");
				}
			}
			catch (Exception e)
			{
				Log.LogWarning("[SUPC] Could not create SUPC folders: " + e.Message);
			}
		}

		public static void LoadThemes()
		{
			try
			{
				if (!Directory.Exists(ThemesDir))
				{
					return;
				}
				string[] files = Directory.GetFiles(ThemesDir, "*.txt");
				List<System.ValueTuple<string, Color, Color>> extra = new List<System.ValueTuple<string, Color, Color>>();
				int i = 0;
				while (i < files.Length)
				{
					string f = files[i];
					i++;
					string name = Path.GetFileNameWithoutExtension(f);
					if (string.IsNullOrEmpty(name) || name.Equals("README", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					Color primary = new Color(0.5f, 0.5f, 0.5f);
					Color secondary = new Color(0.05f, 0.05f, 0.05f);
					bool got = false;
					string[] lines = File.ReadAllLines(f);
					int j = 0;
					while (j < lines.Length)
					{
						string line = lines[j];
						j++;
						if (line == null)
						{
							continue;
						}
						string t = line.Trim();
						if (t.Length == 0 || t[0] == '#' && t.IndexOf('=') < 0)
						{
							continue;
						}
						int eq = t.IndexOf('=');
						if (eq <= 0)
						{
							continue;
						}
						string key = t.Substring(0, eq).Trim().ToLower();
						string val = t.Substring(eq + 1).Trim();
						Color c;
						if (!TryParseColor(val, out c))
						{
							continue;
						}
						if (key == "primary")
						{
							primary = c;
							got = true;
						}
						else if (key == "secondary")
						{
							secondary = c;
							got = true;
						}
					}
					if (got)
					{
						extra.Add(new System.ValueTuple<string, Color, Color>(name, primary, secondary));
					}
				}
				if (extra.Count == 0)
				{
					return;
				}
				System.ValueTuple<string, Color, Color>[] old = ScryptoUtilsPad.Core.SettingsPage.Themes;
				System.ValueTuple<string, Color, Color>[] merged = new System.ValueTuple<string, Color, Color>[old.Length + extra.Count];
				int k = 0;
				while (k < old.Length)
				{
					merged[k] = old[k];
					k++;
				}
				int e2 = 0;
				while (e2 < extra.Count)
				{
					merged[old.Length + e2] = extra[e2];
					e2++;
				}
				ScryptoUtilsPad.Core.SettingsPage.Themes = merged;
				Log.LogInfo("[SUPC] Loaded " + extra.Count + " custom theme(s) from SUPC/Themes.");
			}
			catch (Exception e)
			{
				Log.LogWarning("[SUPC] Could not load custom themes: " + e.Message);
			}
		}

		public static void RefreshConfigList()
		{
			ConfigNames.Clear();
			try
			{
				if (!Directory.Exists(ConfigsDir))
				{
					return;
				}
				string[] files = Directory.GetFiles(ConfigsDir, "*.txt");
				int i = 0;
				while (i < files.Length)
				{
					string name = Path.GetFileNameWithoutExtension(files[i]);
					i++;
					if (!string.IsNullOrEmpty(name) && !name.Equals("README", StringComparison.OrdinalIgnoreCase))
					{
						ConfigNames.Add(name);
					}
				}
			}
			catch (Exception e)
			{
				Log.LogWarning("[SUPC] Could not list config profiles: " + e.Message);
			}
		}

		public static string ActiveProfile
		{
			get
			{
				return PlayerPrefs.GetString("ScryptoUtilsPad.ActiveProfile", "");
			}
			set
			{
				PlayerPrefs.SetString("ScryptoUtilsPad.ActiveProfile", value ?? "");
				PlayerPrefs.Save();
			}
		}

		public static void ClearActiveProfile()
		{
			ActiveProfile = "";
			Log.LogInfo("[SUPC] Active profile cleared — using SUPC_config.txt.");
		}

		public static void LoadActiveProfileAtStartup()
		{
			string name = ActiveProfile;
			if (string.IsNullOrEmpty(name))
			{
				return;
			}
			string path = Path.Combine(ConfigsDir, name + ".txt");
			if (!File.Exists(path))
			{
				Log.LogWarning("[SUPC] Active profile '" + name + "' is missing — falling back to SUPC_config.txt.");
				return;
			}
			int idx = ConfigNames.IndexOf(name);
			if (idx >= 0)
			{
				ConfigIndex = idx;
			}
			ScryptoUtilsPad.Core.SharedConfig.LoadFrom(path);
			Log.LogInfo("[SUPC] Loaded active profile '" + name + "' at startup.");
		}

		public static void LoadCurrentConfig()
		{
			if (ConfigNames.Count == 0)
			{
				Log.LogWarning("[SUPC] No config profiles in SUPC/Configs.");
				return;
			}
			string path = Path.Combine(ConfigsDir, CurrentConfigName + ".txt");
			if (!File.Exists(path))
			{
				Log.LogWarning("[SUPC] Config profile not found: " + path);
				return;
			}
			ScryptoUtilsPad.Core.SharedConfig.LoadFrom(path);
			ActiveProfile = CurrentConfigName;
			Log.LogInfo("[SUPC] Applied config profile '" + CurrentConfigName + "' (live, and will load on next launch).");
		}

		public static void SaveCurrentAsProfile()
		{
			try
			{
				EnsureFolders();
				string name = "profile" + System.DateTime.Now.ToString("HHmmss");
				string path = Path.Combine(ConfigsDir, name + ".txt");
				ScryptoUtilsPad.Core.SharedConfig.SaveTo(path);
				RefreshConfigList();
				Log.LogInfo("[SUPC] Saved current settings as profile '" + name + "'.");
			}
			catch (Exception e)
			{
				Log.LogWarning("[SUPC] Could not save profile: " + e.Message);
			}
		}

		private static bool TryParseColor(string raw, out Color c)
		{
			c = Color.white;
			if (string.IsNullOrEmpty(raw))
			{
				return false;
			}
			string v = raw.Trim();
			if (v.StartsWith("#"))
			{
				v = v.Substring(1);
			}
			if (v.Length == 6)
			{
				try
				{
					int r = System.Convert.ToInt32(v.Substring(0, 2), 16);
					int g = System.Convert.ToInt32(v.Substring(2, 2), 16);
					int b = System.Convert.ToInt32(v.Substring(4, 2), 16);
					c = new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
					return true;
				}
				catch
				{
					return false;
				}
			}
			string[] parts = v.Split(',');
			if (parts.Length >= 3)
			{
				float rr;
				float gg;
				float bb;
				if (float.TryParse(parts[0].Trim(), out rr) && float.TryParse(parts[1].Trim(), out gg) && float.TryParse(parts[2].Trim(), out bb))
				{
					if (rr > 1f || gg > 1f || bb > 1f)
					{
						rr /= 255f;
						gg /= 255f;
						bb /= 255f;
					}
					c = new Color(rr, gg, bb);
					return true;
				}
			}
			return false;
		}
	}
}
