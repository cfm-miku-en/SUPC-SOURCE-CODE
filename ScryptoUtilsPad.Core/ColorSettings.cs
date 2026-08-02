/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using UnityEngine;

namespace ScryptoUtilsPad.Core
{
	public static class ColorSettings
	{
		private static readonly BepInEx.Logging.ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("SUPCColor");

		public static readonly string[] Names = new string[10]
		{
			"Custom", "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Pink", "White"
		};

		private static readonly int[][] Presets = new int[10][]
		{
			new int[3] { 5, 5, 5 },
			new int[3] { 9, 0, 0 },
			new int[3] { 9, 4, 0 },
			new int[3] { 9, 9, 0 },
			new int[3] { 0, 9, 0 },
			new int[3] { 0, 9, 9 },
			new int[3] { 0, 0, 9 },
			new int[3] { 6, 0, 9 },
			new int[3] { 9, 3, 6 },
			new int[3] { 9, 9, 9 }
		};

		private static int _index;

		public static int Red = 5;

		public static int Green = 5;

		public static int Blue = 5;

		public static int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = ((value % Names.Length) + Names.Length) % Names.Length;
				if (_index != 0)
				{
					Red = Presets[_index][0];
					Green = Presets[_index][1];
					Blue = Presets[_index][2];
				}
				PlayerPrefs.SetInt("ScryptoUtilsPad.ColorPreset", _index);
			}
		}

		public static string CurrentName
		{
			get
			{
				if (_index == 0)
				{
					return Red + "/" + Green + "/" + Blue;
				}
				return Names[_index];
			}
		}

		public static void Load()
		{
			_index = PlayerPrefs.GetInt("ScryptoUtilsPad.ColorPreset", 0);
			if (_index < 0 || _index >= Names.Length)
			{
				_index = 0;
			}
			Red = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.ColorR", 5), 0, 9);
			Green = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.ColorG", 5), 0, 9);
			Blue = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.ColorB", 5), 0, 9);
		}

		public static void SaveChannels()
		{
			PlayerPrefs.SetInt("ScryptoUtilsPad.ColorR", Red);
			PlayerPrefs.SetInt("ScryptoUtilsPad.ColorG", Green);
			PlayerPrefs.SetInt("ScryptoUtilsPad.ColorB", Blue);
		}

		public static void StepChannel(int channel, int delta)
		{
			_index = 0;
			if (channel == 0)
			{
				Red = Wrap(Red + delta);
			}
			else if (channel == 1)
			{
				Green = Wrap(Green + delta);
			}
			else
			{
				Blue = Wrap(Blue + delta);
			}
			SaveChannels();
			PlayerPrefs.SetInt("ScryptoUtilsPad.ColorPreset", 0);
		}

		private static int Wrap(int v)
		{
			return ((v % 10) + 10) % 10;
		}

		public static void Apply()
		{
			float r = (float)Red / 9f;
			float g = (float)Green / 9f;
			float b = (float)Blue / 9f;
			PlayerPrefs.SetFloat("redValue", r);
			PlayerPrefs.SetFloat("greenValue", g);
			PlayerPrefs.SetFloat("blueValue", b);
			PlayerPrefs.Save();
			try
			{
				GorillaComputer computer = GorillaComputer.instance;
				if ((Object)(object)computer != (Object)null)
				{
					computer.UpdateColor(r, g, b);
				}
			}
			catch (System.Exception e)
			{
				Log.LogWarning("[SUPC] Computer.UpdateColor failed: " + e.Message);
			}
			try
			{
				GorillaTagger tagger = GorillaTagger.Instance;
				VRRig rig = (((Object)(object)tagger != (Object)null) ? tagger.offlineVRRig : null);
				if ((Object)(object)rig != (Object)null)
				{
					rig.InitializeNoobMaterialLocal(r, g, b);
					if (Photon.Pun.PhotonNetwork.InRoom)
					{
						PhotonView pv = ((Component)rig).GetComponent<PhotonView>();
						if ((Object)(object)pv == (Object)null)
						{
							pv = ((Component)rig).GetComponentInParent<PhotonView>();
						}
						if ((Object)(object)pv != (Object)null)
						{
							pv.RPC("InitializeNoobMaterial", (Photon.Pun.RpcTarget)1, new object[3] { r, g, b });
						}
					}
					Log.LogInfo("[SUPC] Colour applied: " + Red + "/" + Green + "/" + Blue);
				}
				else
				{
					Log.LogWarning("[SUPC] Colour: local rig not ready yet.");
				}
			}
			catch (System.Exception e2)
			{
				Log.LogWarning("[SUPC] Colour apply failed: " + e2.Message);
			}
		}
	}
}
