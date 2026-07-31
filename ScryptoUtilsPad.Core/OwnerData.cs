/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Logging;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace ScryptoUtilsPad.Core
{
	public static class OwnerData
	{
		private const string OwnersUrl = "https://supc.mikuuu.xyz/owners";

		private static readonly ManualLogSource Log = Logger.CreateLogSource("OwnerData");

		private static readonly Dictionary<string, string> _labels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		private static bool _fetchStarted;

		public static bool IsLoaded { get; private set; }

		public static int Count
		{
			get
			{
				return _labels.Count;
			}
		}

		public static bool TryGetLabel(string userId, out string label)
		{
			label = null;
			if (string.IsNullOrEmpty(userId) || _labels.Count == 0)
			{
				return false;
			}
			return _labels.TryGetValue(userId, out label);
		}

		public static void FetchAsync(MonoBehaviour host)
		{
			if (_fetchStarted || (UnityEngine.Object)(object)host == (UnityEngine.Object)null)
			{
				return;
			}
			_fetchStarted = true;
			host.StartCoroutine(FetchCoroutine());
		}

		private static IEnumerator FetchCoroutine()
		{
			UnityWebRequest req = UnityWebRequest.Get(OwnersUrl);
			yield return req.SendWebRequest();
			bool ok = (req.result != UnityWebRequest.Result.ConnectionError && req.result != UnityWebRequest.Result.ProtocolError);
			if (!ok)
			{
				Log.LogWarning("[SUPC] Could not fetch owner list: " + req.error);
				req.Dispose();
				IsLoaded = true;
				yield break;
			}
			string body = req.downloadHandler.text;
			req.Dispose();
			try
			{
				JObject obj = JObject.Parse(body);
				foreach (KeyValuePair<string, JToken> kv in obj)
				{
					if (string.IsNullOrEmpty(kv.Key))
					{
						continue;
					}
					JToken v = kv.Value;
					string label = (((v != null) ? v.ToString() : null) ?? "Owner");
					if (string.IsNullOrEmpty(label))
					{
						label = "Owner";
					}
					_labels[kv.Key] = label;
				}
			}
			catch (System.Exception e)
			{
				Log.LogWarning("[SUPC] Could not parse owner list: " + e.Message);
			}
			IsLoaded = true;
			Log.LogInfo("[SUPC] Owner list loaded. entries=" + _labels.Count);
		}
	}
}
