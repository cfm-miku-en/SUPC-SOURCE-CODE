using System.Collections;
using System.Collections.Generic;
using BepInEx.Logging;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Networking;
namespace ScryptoUtilsPad.Core
{
	public static class SUPCData
	{
		private const string CheatsUrl = "https://raw.githubusercontent.com/cfm-miku-en/SUPC/refs/heads/main/Cheats.txt";
		private const string ModsUrl = "https://raw.githubusercontent.com/cfm-miku-en/SUPC/refs/heads/main/Mods.txt";

		private const string PartialUrl = "https://raw.githubusercontent.com/cfm-miku-en/SUPC/refs/heads/main/Cheats_Partial.txt";
		private static readonly ManualLogSource Log = Logger.CreateLogSource("SUPCData");
		public static Dictionary<string, string> KnownCheats { get; private set; } = new Dictionary<string, string>();

		public static Dictionary<string, string> KnownMods { get; private set; } = new Dictionary<string, string>();

		public static Dictionary<string, string> PartialCheats { get; private set; } = new Dictionary<string, string>();

		public static bool TryMatchPartial(string loweredKey, out string label)
		{
			label = null;
			if (string.IsNullOrEmpty(loweredKey) || PartialCheats.Count == 0)
			{
				return false;
			}
			foreach (KeyValuePair<string, string> kv in PartialCheats)
			{
				if (kv.Key.Length >= 4 && loweredKey.IndexOf(kv.Key, System.StringComparison.Ordinal) >= 0)
				{
					label = kv.Value;
					return true;
				}
			}
			return false;
		}

		public static bool IsLoaded { get; private set; }

		private static bool _fetchStarted;

		public static void FetchAsync(MonoBehaviour host)
		{
			if (_fetchStarted || host == null)
			{
				return;
			}
			_fetchStarted = true;
			host.StartCoroutine(FetchCoroutine());
		}
		private static IEnumerator FetchCoroutine()
		{
			yield return FetchInto(CheatsUrl, KnownCheats, "cheats");
			yield return FetchInto(ModsUrl, KnownMods, "mods");
			yield return FetchInto(PartialUrl, PartialCheats, "partial");
			IsLoaded = true;
			Log.LogInfo("[SUPC] Lists loaded. cheats=" + KnownCheats.Count + " mods=" + KnownMods.Count + " partial=" + PartialCheats.Count);
		}
		private static IEnumerator FetchInto(string url, Dictionary<string, string> target, string label)
		{
			UnityWebRequest req = UnityWebRequest.Get(url);
			yield return req.SendWebRequest();
			bool ok = (req.result != UnityWebRequest.Result.ConnectionError && req.result != UnityWebRequest.Result.ProtocolError);
			if (!ok)
			{
				Log.LogWarning("[SUPC] Failed to fetch " + label + " list: " + req.error);
				req.Dispose();
				yield break;
			}
			string body = req.downloadHandler.text;
			req.Dispose();
			try
			{
				JObject obj = JObject.Parse(body);
				foreach (KeyValuePair<string, JToken> kv in obj)
				{
					string key = kv.Key.ToLower();
					JToken v = kv.Value;
					target[key] = (((v != null) ? v.ToString() : null) ?? kv.Key);
				}
			}
			catch (System.Exception e)
			{
				Log.LogWarning("[SUPC] Could not parse " + label + " list: " + e.Message);
			}
		}
	}
}
