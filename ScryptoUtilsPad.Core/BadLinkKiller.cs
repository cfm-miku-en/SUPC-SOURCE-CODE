using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.Networking;
namespace ScryptoUtilsPad.Core
{
	public static class BadLinkKiller
	{
		private static readonly ManualLogSource Log = Logger.CreateLogSource("BadLinkKiller");
		private static readonly string[] BlockedHosts = new string[]
		{
			"hamburbur.org",
			"hamburbur.com",
			"hamburbur",
			"seralyth",
			"menu.seralyth.software",
			"seralyth.software"
		};
		private static bool _applied;

		internal const string EmptyDataJson = "{\"modVersionInfo\":[],\"admins\":[],\"superAdmins\":[],\"seralythAdmins\":[],\"knownCheats\":{},\"knownMods\":{}}";
		public static void Apply()
		{
			if (_applied)
			{
				return;
			}
			_applied = true;
			try
			{
				Harmony harmony = new Harmony("com.supc.badlinkkiller");
				harmony.PatchAll(typeof(BadLinkKiller).Assembly);
				int patched = 0;
				foreach (System.Reflection.MethodBase mb in harmony.GetPatchedMethods())
				{
					patched++;
				}
				Log.LogInfo("[BadLinkKiller] Active — " + patched + " method(s) patched; known malicious/menu links will be blocked.");
			}
			catch (Exception e)
			{
				Log.LogWarning("[BadLinkKiller] Failed to apply: " + e.Message);
			}
		}
		internal static bool IsBlocked(string url)
		{
			if (string.IsNullOrEmpty(url))
			{
				return false;
			}
			string lower = url.ToLowerInvariant();
			for (int i = 0; i < BlockedHosts.Length; i++)
			{
				if (lower.IndexOf(BlockedHosts[i], StringComparison.Ordinal) >= 0)
				{
					return true;
				}
			}
			return false;
		}
		internal static void Report(string where, string url)
		{
			Log.LogWarning("[BadLinkKiller] Blocked " + where + " -> " + url);
		}
		[HarmonyPatch]
		private static class UnityWebRequestPatch
		{
			[HarmonyTargetMethods]
			private static System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> Targets()
			{
				System.Reflection.MethodInfo[] all = typeof(UnityWebRequest).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				int i = 0;
				while (i < all.Length)
				{
					System.Reflection.MethodInfo m = all[i];
					i++;
					if (m.Name == "SendWebRequest")
					{
						yield return m;
					}
				}
			}

			[HarmonyPrefix]
			private static bool Prefix(UnityWebRequest __instance)
			{
				if (__instance == null)
				{
					return true;
				}
				try
				{
					string u = __instance.url;
					if (IsBlocked(u))
					{
						Report("UnityWebRequest", u);
						__instance.Abort();
						return false;
					}
				}
				catch
				{
				}
				return true;
			}
		}
		[HarmonyPatch]
		private static class HttpClientPatch
		{
			[HarmonyTargetMethods]
			private static System.Collections.Generic.IEnumerable<System.Reflection.MethodBase> Targets()
			{
				System.Reflection.MethodInfo[] all = typeof(HttpClient).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				int i = 0;
				while (i < all.Length)
				{
					System.Reflection.MethodInfo m = all[i];
					i++;
					if (m.Name == "SendAsync")
					{
						yield return m;
					}
				}
			}

			[HarmonyPrefix]
			private static bool Prefix(HttpRequestMessage request, ref System.Threading.Tasks.Task<HttpResponseMessage> __result)
			{
				if (request == null || request.RequestUri == null)
				{
					return true;
				}
				string url = request.RequestUri.ToString();
				if (!IsBlocked(url))
				{
					return true;
				}
				Report("HttpClient", url);
				HttpResponseMessage stub = new HttpResponseMessage(HttpStatusCode.OK);
				stub.Content = new StringContent(EmptyDataJson, System.Text.Encoding.UTF8, "application/json");
				__result = System.Threading.Tasks.Task.FromResult<HttpResponseMessage>(stub);
				return false;
			}
		}

		[HarmonyPatch(typeof(WebRequest), "Create", new Type[] { typeof(Uri) })]
		[HarmonyPrefix]
		private static bool Block_WebRequest(Uri requestUri)
		{
			if (requestUri != null && IsBlocked(requestUri.ToString()))
			{
				Report("WebRequest", requestUri.ToString());
				throw new WebException("[BadLinkKiller] Connection to Hamburbur blocked by SUPC.");
			}
			return true;
		}
	}
}
