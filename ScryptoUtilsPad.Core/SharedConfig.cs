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
	public static class SharedConfig
	{
		private const string FileName = "SUPC_config.txt";

		private static readonly ManualLogSource Log = Logger.CreateLogSource("SUPCConfig");

		public static string FilePath
		{
			get
			{
				return Path.Combine(Directory.GetCurrentDirectory(), FileName);
			}
		}

		public static bool AutoSave = true;

		private static bool _dirty;

		private static float _dirtyAt;

		private static bool _ready;

		public static void EnableAutoSave()
		{
			_ready = true;
			AutoSave = PlayerPrefs.GetInt("ScryptoUtilsPad.AutoSave", 1) == 1;
		}

		public static void SetAutoSave(bool on)
		{
			AutoSave = on;
			PlayerPrefs.SetInt("ScryptoUtilsPad.AutoSave", on ? 1 : 0);
			PlayerPrefs.Save();
		}

		public static void MarkDirty()
		{
			if (!_ready || !AutoSave)
			{
				return;
			}
			_dirty = true;
			_dirtyAt = Time.time;
		}

		public static string TargetPath
		{
			get
			{
				string active = ScryptoUtilsPad.Core.UserFiles.ActiveProfile;
				if (!string.IsNullOrEmpty(active))
				{
					string p = Path.Combine(ScryptoUtilsPad.Core.UserFiles.ConfigsDir, active + ".txt");
					if (File.Exists(p))
					{
						return p;
					}
				}
				return FilePath;
			}
		}

		public static void Tick()
		{
			if (!_dirty || !AutoSave)
			{
				return;
			}
			if (Time.time - _dirtyAt < 1.5f)
			{
				return;
			}
			_dirty = false;
			SaveTo(TargetPath);
		}

		public static void Load()
		{
			LoadFrom(FilePath);
		}

		public static void LoadFrom(string path)
		{
			if (!File.Exists(path))
			{
				return;
			}
			Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			try
			{
				string[] lines = File.ReadAllLines(path);
				int i = 0;
				while (i < lines.Length)
				{
					string line = lines[i];
					i++;
					if (string.IsNullOrEmpty(line))
					{
						continue;
					}
					string trimmed = line.Trim();
					if (trimmed.Length == 0 || trimmed[0] == '#')
					{
						continue;
					}
					int eq = trimmed.IndexOf('=');
					if (eq <= 0)
					{
						continue;
					}
					map[trimmed.Substring(0, eq).Trim()] = trimmed.Substring(eq + 1).Trim();
				}
			}
			catch (Exception e)
			{
				Log.LogWarning("[SUPC] Could not read " + FileName + ": " + e.Message);
				return;
			}

			ApplyInt(map, "Theme", "ScryptoUtilsPad.ThemeIndex");
			ApplyInt(map, "MenuSize", "ScryptoUtilsPad.MenuSize");
			ApplyInt(map, "Mode", "ScryptoUtilsPad.ModeIndex");
			ApplyInt(map, "SelectMode", "ScryptoUtilsPad.SelectMode");
			ApplyInt(map, "NametagFont", "ScryptoUtilsPad.NametagFont");
			ApplyInt(map, "ClickSound", "ScryptoUtilsPad.ClickSound");
			ApplyInt(map, "StartupSound", "ScryptoUtilsPad.MenuSound");
			ApplyBool(map, "Nametags", "ScryptoUtilsPad.Nametags");
			ApplyBool(map, "TagFps", "ScryptoUtilsPad.TagFps");
			ApplyBool(map, "TagPlatform", "ScryptoUtilsPad.TagPlatform");
			ApplyBool(map, "CheatAlerts", "ScryptoUtilsPad.CheatAlerts");
			ApplyBool(map, "ModAlerts", "ScryptoUtilsPad.ModAlerts");
			ApplyInt(map, "ColorPreset", "ScryptoUtilsPad.ColorPreset");
			ApplyInt(map, "ColorR", "ScryptoUtilsPad.ColorR");
			ApplyInt(map, "ColorG", "ScryptoUtilsPad.ColorG");
			ApplyInt(map, "ColorB", "ScryptoUtilsPad.ColorB");
			ApplyInt(map, "RigSize", "ScryptoUtilsPad.RigSize");
			ApplyInt(map, "RigYaw", "ScryptoUtilsPad.RigYaw");
			ApplyInt(map, "RigPitch", "ScryptoUtilsPad.RigPitch");
			ApplyInt(map, "RigRoll", "ScryptoUtilsPad.RigRoll");
			ApplyInt(map, "RigX", "ScryptoUtilsPad.RigX");
			ApplyInt(map, "RigY", "ScryptoUtilsPad.RigY");
			ApplyInt(map, "RigZ", "ScryptoUtilsPad.RigZ");
			ApplyInt(map, "RigSpin", "ScryptoUtilsPad.RigSpin");
			ApplyBool(map, "RigPreview", "ScryptoUtilsPad.RigPreview");
			ApplyBool(map, "DesktopFirstPerson", "ScryptoUtilsPad.DesktopFirstPerson");
			ApplyInt(map, "DesktopFov", "ScryptoUtilsPad.DesktopFov");
			ApplyInt(map, "DesktopSmooth", "ScryptoUtilsPad.DesktopSmooth");
			ApplyBool(map, "AutoSave", "ScryptoUtilsPad.AutoSave");
			Log.LogInfo("[SUPC] Loaded shared config from " + FileName + " (" + map.Count + " entries).");
		}

		public static void Save()
		{
			SaveTo(FilePath);
		}

		public static void SaveTo(string targetPath)
		{
			try
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.Append("# SUPC shared config — drop this next to Gorilla Tag.exe\n");
				sb.Append("# Share this file to give someone your exact setup.\n");
				sb.Append("# Values are applied at startup; delete a line to keep your own.\n\n");
				WriteInt(sb, "Theme", "ScryptoUtilsPad.ThemeIndex");
				WriteInt(sb, "MenuSize", "ScryptoUtilsPad.MenuSize");
				WriteInt(sb, "Mode", "ScryptoUtilsPad.ModeIndex");
				WriteInt(sb, "SelectMode", "ScryptoUtilsPad.SelectMode");
				WriteInt(sb, "NametagFont", "ScryptoUtilsPad.NametagFont");
				WriteInt(sb, "ClickSound", "ScryptoUtilsPad.ClickSound");
				WriteInt(sb, "StartupSound", "ScryptoUtilsPad.MenuSound");
				WriteBool(sb, "Nametags", "ScryptoUtilsPad.Nametags");
				WriteBool(sb, "TagFps", "ScryptoUtilsPad.TagFps");
				WriteBool(sb, "TagPlatform", "ScryptoUtilsPad.TagPlatform");
				WriteBool(sb, "CheatAlerts", "ScryptoUtilsPad.CheatAlerts");
				WriteBool(sb, "ModAlerts", "ScryptoUtilsPad.ModAlerts");
				WriteInt(sb, "ColorPreset", "ScryptoUtilsPad.ColorPreset");
				WriteInt(sb, "ColorR", "ScryptoUtilsPad.ColorR");
				WriteInt(sb, "ColorG", "ScryptoUtilsPad.ColorG");
				WriteInt(sb, "ColorB", "ScryptoUtilsPad.ColorB");
				sb.Append("\n# Rig preview (size/offsets in cm, angles in degrees)\n");
				WriteIntDef(sb, "RigSize", "ScryptoUtilsPad.RigSize", 14);
				WriteIntDef(sb, "RigYaw", "ScryptoUtilsPad.RigYaw", 217);
				WriteIntDef(sb, "RigPitch", "ScryptoUtilsPad.RigPitch", 273);
				WriteIntDef(sb, "RigRoll", "ScryptoUtilsPad.RigRoll", 323);
				WriteIntDef(sb, "RigX", "ScryptoUtilsPad.RigX", 40);
				WriteIntDef(sb, "RigY", "ScryptoUtilsPad.RigY", -11);
				WriteIntDef(sb, "RigZ", "ScryptoUtilsPad.RigZ", 3);
				WriteIntDef(sb, "RigSpin", "ScryptoUtilsPad.RigSpin", 0);
				WriteBool(sb, "RigPreview", "ScryptoUtilsPad.RigPreview");
				sb.Append("\n# Desktop (monitor) camera\n");
				WriteBool(sb, "DesktopFirstPerson", "ScryptoUtilsPad.DesktopFirstPerson");
				WriteIntDef(sb, "DesktopFov", "ScryptoUtilsPad.DesktopFov", 90);
				WriteIntDef(sb, "DesktopSmooth", "ScryptoUtilsPad.DesktopSmooth", 5);
				WriteBool(sb, "AutoSave", "ScryptoUtilsPad.AutoSave");
				File.WriteAllText(targetPath, sb.ToString());
				Log.LogInfo("[SUPC] Saved shared config to " + targetPath);
			}
			catch (Exception e)
			{
				Log.LogWarning("[SUPC] Could not write " + FileName + ": " + e.Message);
			}
		}

		private static void ApplyInt(Dictionary<string, string> map, string key, string pref)
		{
			string raw;
			int v;
			if (map.TryGetValue(key, out raw) && int.TryParse(raw, out v))
			{
				PlayerPrefs.SetInt(pref, v);
			}
		}

		private static void ApplyBool(Dictionary<string, string> map, string key, string pref)
		{
			string raw;
			if (!map.TryGetValue(key, out raw))
			{
				return;
			}
			bool on = (raw == "1" || raw.Equals("true", StringComparison.OrdinalIgnoreCase) || raw.Equals("on", StringComparison.OrdinalIgnoreCase));
			PlayerPrefs.SetInt(pref, on ? 1 : 0);
		}

		private static void WriteIntDef(System.Text.StringBuilder sb, string key, string pref, int def)
		{
			sb.Append(key).Append(" = ").Append(PlayerPrefs.GetInt(pref, def)).Append("\n");
		}

		private static void WriteInt(System.Text.StringBuilder sb, string key, string pref)
		{
			sb.Append(key).Append(" = ").Append(PlayerPrefs.GetInt(pref, 0)).Append("\n");
		}

		private static void WriteBool(System.Text.StringBuilder sb, string key, string pref)
		{
			sb.Append(key).Append(" = ").Append((PlayerPrefs.GetInt(pref, 0) == 1) ? "true" : "false").Append("\n");
		}
	}
}
