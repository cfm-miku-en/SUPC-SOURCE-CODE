/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace ScryptoUtilsPad.Core
{
	public static class PropertyWipe
	{
		private const float GraceSeconds = 12f;

		private static readonly BepInEx.Logging.ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("PropertyWipe");

		private static readonly Dictionary<string, float> _firstSeen = new Dictionary<string, float>(StringComparer.Ordinal);

		private static readonly HashSet<string> _flagged = new HashSet<string>(StringComparer.Ordinal);

		private static bool _lobbyWarned;

		private static float _nextCheck;

		public static void Reset()
		{
			_firstSeen.Clear();
			_flagged.Clear();
			_lobbyWarned = false;
			_nextCheck = 0f;
		}

		public static bool IsWiped(Player p)
		{
			if (p == null)
			{
				return false;
			}
			Hashtable props = p.CustomProperties;
			int count = ((props != null) ? ((Dictionary<object, object>)(object)props).Count : 0);
			if (count > 1)
			{
				return false;
			}
			string id = p.UserId;
			if (string.IsNullOrEmpty(id))
			{
				return false;
			}
			float t;
			if (!_firstSeen.TryGetValue(id, out t))
			{
				_firstSeen[id] = Time.time;
				return false;
			}
			return Time.time - t >= GraceSeconds;
		}

		public static bool IsWiped(VRRig rig)
		{
			if ((UnityEngine.Object)(object)rig == (UnityEngine.Object)null)
			{
				return false;
			}
			NetPlayer creator = rig.creator;
			if (creator == null)
			{
				return false;
			}
			return IsWiped(creator.GetPlayerRef());
		}

		public static void Tick()
		{
			if (!PhotonNetwork.InRoom || Time.time < _nextCheck)
			{
				return;
			}
			_nextCheck = Time.time + 3f;

			Player[] others = PhotonNetwork.PlayerListOthers;
			if (others == null || others.Length == 0)
			{
				return;
			}

			int total = 0;
			int wiped = 0;
			List<Player> wipedPlayers = new List<Player>();
			int i = 0;
			while (i < others.Length)
			{
				Player p = others[i];
				i++;
				if (p == null || string.IsNullOrEmpty(p.UserId))
				{
					continue;
				}
				total++;
				if (IsWiped(p))
				{
					wiped++;
					wipedPlayers.Add(p);
				}
			}

			if (total == 0 || wiped == 0)
			{
				return;
			}

			if (wiped == total && total >= 2)
			{
				if (!_lobbyWarned)
				{
					_lobbyWarned = true;
					Log.LogWarning("[SUPC] Lobby-wide property wipe detected (" + wiped + "/" + total + ").");
					ScryptoUtilsPad.Core.NotificationManager.Notify("<color=red>A cheater has wiped property keys. SUPC can not detect mods in this lobby.</color>");
				}
				return;
			}

			int w = 0;
			while (w < wipedPlayers.Count)
			{
				Player p = wipedPlayers[w];
				w++;
				string id = p.UserId;
				if (_flagged.Contains(id))
				{
					continue;
				}
				_flagged.Add(id);
				string nick = p.NickName;
				if (string.IsNullOrEmpty(nick))
				{
					nick = "Player";
				}
				Log.LogWarning("[SUPC] Property wipe on single player: " + nick);
				ScryptoUtilsPad.Core.NotificationManager.Notify("<color=red>" + nick + " has wiped their properties to evade mod checkers.</color>");
			}
		}
	}
}
