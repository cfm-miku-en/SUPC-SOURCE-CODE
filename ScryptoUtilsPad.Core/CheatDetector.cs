using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
namespace ScryptoUtilsPad.Core
{
	public static class CheatDetector
	{
		public static bool NotifyCheaters = true;
		public static bool NotifyModders = true;
		private static readonly HashSet<string> _notified = new HashSet<string>(StringComparer.Ordinal);

		public static void Reset()
		{
			_notified.Clear();
		}
		public static void Check(Player player)
		{
			if (player == null || !ScryptoUtilsPad.Core.SUPCData.IsLoaded)
			{
				return;
			}
			if (!NotifyCheaters && !NotifyModders)
			{
				return;
			}
			string id = player.UserId;
			if (string.IsNullOrEmpty(id) || _notified.Contains(id))
			{
				return;
			}
			Hashtable props = player.CustomProperties;
			if (props == null)
			{
				return;
			}
			string cheatName = null;
			string modName = null;
			foreach (object keyObj in ((IDictionary)props).Keys)
			{
				string raw = ((keyObj != null) ? keyObj.ToString() : null);
				if (string.IsNullOrEmpty(raw))
				{
					continue;
				}
				string key = raw.ToLower();
				string found;
				if (cheatName == null && ScryptoUtilsPad.Core.SUPCData.KnownCheats.TryGetValue(key, out found))
				{
					cheatName = found;
				}
				else if (modName == null && ScryptoUtilsPad.Core.SUPCData.KnownMods.TryGetValue(key, out found))
				{
					modName = found;
				}
			}
			if (cheatName == null && modName == null)
			{
				return;
			}
			string nick = player.NickName;
			if (string.IsNullOrEmpty(nick))
			{
				nick = "Player";
			}
			if (cheatName != null && NotifyCheaters)
			{
				_notified.Add(id);
				ScryptoUtilsPad.Core.NotificationManager.Notify("[Illegal] " + nick + " - " + cheatName, Color.red);
			}
			else if (modName != null && NotifyModders)
			{
				_notified.Add(id);
				ScryptoUtilsPad.Core.NotificationManager.Notify("[Legal] " + nick + " - " + modName, Color.green);
			}
		}
	}
}
