using System;
using System.Collections.Generic;
using BepInEx.Logging;
using Photon.Pun;
using Photon.Realtime;
namespace ScryptoUtilsPad.Core
{
	public static class OwnerNotifier
	{
		
		public const string OwnerUserId = "446F8AB9D8C7EAOO";

		private const string JoinMessage = "Owner Scrypto has joined your lobby.";

		private static readonly HashSet<string> _announced = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private static readonly ManualLogSource Log = Logger.CreateLogSource("OwnerNotifier");

		public static bool IsOwner(string userId)
		{
			return !string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(OwnerUserId) && string.Equals(userId, OwnerUserId, StringComparison.OrdinalIgnoreCase);
		}

		public static void Check(Player player)
		{
			if (player == null)
			{
				return;
			}
			string id = player.UserId;
			if (!IsOwner(id) || _announced.Contains(id))
			{
				return;
			}
			_announced.Add(id);
			Log.LogInfo("[OwnerNotifier] Owner joined the lobby (" + (player.NickName ?? "?") + ").");
			ScryptoUtilsPad.Core.NotificationManager.Notify(JoinMessage);
		}

		public static void CheckAll()
		{
			if (!PhotonNetwork.InRoom)
			{
				return;
			}
			Player[] players = PhotonNetwork.PlayerListOthers;
			if (players == null)
			{
				return;
			}
			for (int i = 0; i < players.Length; i++)
			{
				Check(players[i]);
			}
		}

		public static void Forget(Player player)
		{
			string id = ((player != null) ? player.UserId : null);
			if (!string.IsNullOrEmpty(id))
			{
				_announced.Remove(id);
			}
		}

		public static void Reset()
		{
			_announced.Clear();
		}
	}
}
