using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx.Logging;
using ExitGames.Client.Photon;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Networking;
namespace ScryptoUtilsPad.Core
{
	public static class CheatSigGetter
	{
		private static readonly ManualLogSource Log = Logger.CreateLogSource("CheatSigGetter");
		public static bool Enabled = true;
		public static bool AutoUpload = true;
		public static string UploadUrl = "https://supc.mikuuu.xyz/sig";
		public static string UploadAuth = "4775ebbe8e8fdb4771d60e9e25db2b8ce8ed0285151a34d9";
		private static readonly HashSet<string> _seen = new HashSet<string>(StringComparer.Ordinal);
		private static readonly HashSet<string> _uploaded = new HashSet<string>(StringComparer.Ordinal);
		private static readonly List<string> _pending = new List<string>();
		private static readonly List<string> _allUnknown = new List<string>();
		public static List<string> AllUnknown
		{
			get
			{
				return _allUnknown;
			}
		}
		private static MonoBehaviour _host;
		private static float _lastFlush;
		private const float FlushInterval = 15f;
		private static readonly HashSet<string> _ignored = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"didtutorial", "gtag_platform", "release", "platform"
		};
		public static void Init(MonoBehaviour host)
		{
			_host = host;
		}
		public static void Observe(Hashtable properties)
		{
			if (!Enabled || properties == null)
			{
				return;
			}
			foreach (object keyObj in ((IDictionary)properties).Keys)
			{
				string raw = (keyObj != null) ? keyObj.ToString() : null;
				if (string.IsNullOrEmpty(raw))
				{
					continue;
				}
				string key = raw.ToLower();
				if (_seen.Contains(key) || _ignored.Contains(key))
				{
					continue;
				}
				if (SUPCData.KnownCheats.ContainsKey(key) || SUPCData.KnownMods.ContainsKey(key))
				{
					continue;
				}
				_seen.Add(key);
				_pending.Add(raw);
				_allUnknown.Add(raw);
				Log.LogInfo("[SigGetter] New unknown signature: \"" + raw + "\"");
			}
		}
		public static void Tick()
		{
			if (!Enabled || !AutoUpload || _pending.Count == 0)
			{
				return;
			}
			if (Time.time - _lastFlush < FlushInterval)
			{
				return;
			}
			_lastFlush = Time.time;
			Flush();
		}
		public static void Flush()
		{
			if (_host == null || _pending.Count == 0 || string.IsNullOrEmpty(UploadUrl))
			{
				return;
			}
			List<string> batch = new List<string>();
			for (int i = 0; i < _pending.Count; i++)
			{
				if (!_uploaded.Contains(_pending[i]))
				{
					batch.Add(_pending[i]);
				}
			}
			_pending.Clear();
			if (batch.Count == 0)
			{
				return;
			}
			for (int i = 0; i < batch.Count; i++)
			{
				_uploaded.Add(batch[i]);
			}
			_host.StartCoroutine(Upload(batch));
		}
		private static IEnumerator Upload(List<string> batch)
		{
			string payload = JsonConvert.SerializeObject(new
			{
				source = "SUPC",
				keys = batch.ToArray()
			});
			byte[] body = System.Text.Encoding.UTF8.GetBytes(payload);
			UnityWebRequest req = new UnityWebRequest(UploadUrl, "POST");
			req.uploadHandler = new UploadHandlerRaw(body);
			req.downloadHandler = new DownloadHandlerBuffer();
			req.SetRequestHeader("Content-Type", "application/json");
			if (!string.IsNullOrEmpty(UploadAuth))
			{
				req.SetRequestHeader("X-SUPC-Auth", UploadAuth);
			}
			yield return req.SendWebRequest();
			if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
			{
				Log.LogWarning("[SigGetter] Upload failed (" + req.error + ") — will retry next batch.");
				for (int i = 0; i < batch.Count; i++)
				{
					_uploaded.Remove(batch[i]);
					if (!_pending.Contains(batch[i]))
					{
						_pending.Add(batch[i]);
					}
				}
			}
			else
			{
				Log.LogInfo("[SigGetter] Uploaded " + batch.Count + " new signature(s).");
			}
			req.Dispose();
		}
	}
}
