/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using System;
using System.IO;
using BepInEx.Logging;
using UnityEngine;

namespace ScryptoUtilsPad.Core
{
	public static class ConfigMigration
	{
		private static readonly ManualLogSource Log = Logger.CreateLogSource("SUPCMigration");

		private static readonly string[] Keys = new string[32]
		{
			"ScryptoUtilsPad.ActiveProfile",
			"ScryptoUtilsPad.AutoSave",
			"ScryptoUtilsPad.CheatAlerts",
			"ScryptoUtilsPad.ClickSound",
			"ScryptoUtilsPad.ColorB",
			"ScryptoUtilsPad.ColorG",
			"ScryptoUtilsPad.ColorPreset",
			"ScryptoUtilsPad.ColorR",
			"ScryptoUtilsPad.DesktopFirstPerson",
			"ScryptoUtilsPad.DesktopFov",
			"ScryptoUtilsPad.DesktopSmooth",
			"ScryptoUtilsPad.MenuSize",
			"ScryptoUtilsPad.MenuSound",
			"ScryptoUtilsPad.ModAlerts",
			"ScryptoUtilsPad.ModeIndex",
			"ScryptoUtilsPad.NametagFont",
			"ScryptoUtilsPad.Nametags",
			"ScryptoUtilsPad.NotifyCheaters",
			"ScryptoUtilsPad.NotifyModders",
			"ScryptoUtilsPad.RigPitch",
			"ScryptoUtilsPad.RigPreview",
			"ScryptoUtilsPad.RigRoll",
			"ScryptoUtilsPad.RigSize",
			"ScryptoUtilsPad.RigSpin",
			"ScryptoUtilsPad.RigX",
			"ScryptoUtilsPad.RigY",
			"ScryptoUtilsPad.RigYaw",
			"ScryptoUtilsPad.RigZ",
			"ScryptoUtilsPad.SelectMode",
			"ScryptoUtilsPad.TagFps",
			"ScryptoUtilsPad.TagPlatform",
			"ScryptoUtilsPad.ThemeIndex"
		};

		public static void RunIfNeeded(BepInEx.Configuration.ConfigFile config)
		{
			BepInEx.Configuration.ConfigEntry<bool> done = config.Bind<bool>("Migration", "Reset Done For 2.0.0", false,
				"Set to false to force SUPC to reset all settings back to 2.0.0 defaults on next launch.");
			if (done.Value)
			{
				return;
			}

			int cleared = 0;
			int i = 0;
			while (i < Keys.Length)
			{
				string k = Keys[i];
				i++;
				if (PlayerPrefs.HasKey(k))
				{
					PlayerPrefs.DeleteKey(k);
					cleared++;
				}
			}
			PlayerPrefs.Save();

			try
			{
				string cfg = ScryptoUtilsPad.Core.SharedConfig.FilePath;
				if (File.Exists(cfg))
				{
					string backup = cfg + ".1.1.9.bak";
					if (File.Exists(backup))
					{
						File.Delete(backup);
					}
					File.Move(cfg, backup);
					Log.LogInfo("[SUPC] Old config backed up to " + Path.GetFileName(backup) + ".");
				}
			}
			catch (Exception e)
			{
				Log.LogWarning("[SUPC] Could not back up old config: " + e.Message);
			}

			done.Value = true;
			Log.LogInfo("[SUPC] 2.0.0 first-run reset complete (" + cleared + " old setting(s) cleared). This runs once.");
		}
	}
}
