/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.Networking;

namespace ScryptoUtilsPad.Core
{
	public static class DeHamburbur
	{
		private static readonly ManualLogSource Log = Logger.CreateLogSource("DeHamburbur");

		private static readonly string[] BlockedHosts = new string[]
		{
			"hamburbur.org",
			"hamburbur.com",
			"hamburbur",
			"seralyth"
		};

		private static bool _applied;

		public static void Apply()
		{
			if (_applied)
			{
				return;
			}
			_applied = true;
			try
			{
				Harmony harmony = new Harmony("com.supc.dehamburbur");
				harmony.PatchAll(typeof(DeHamburbur).Assembly);
				Log.LogInfo("[DeHamburbur] Active — Hamburbur connections will be blocked.");
			}
			catch (Exception e)
			{
				Log.LogWarning("[DeHamburbur] Failed to apply: " + e.Message);
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
			Log.LogWarning("[DeHamburbur] Blocked " + where + " -> " + url);
		}

		[HarmonyPatch(typeof(UnityWebRequest), "SendWebRequest")]
		[HarmonyPrefix]
		private static bool Block_UnityWebRequest(UnityWebRequest __instance)
		{
			if (__instance != null && IsBlocked(__instance.url))
			{
				Report("UnityWebRequest", __instance.url);
				__instance.Abort();
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(HttpClient), "SendAsync", new Type[] { typeof(HttpRequestMessage), typeof(System.Threading.CancellationToken) })]
		[HarmonyPrefix]
		private static bool Block_HttpClient(HttpRequestMessage request)
		{
			if (request != null && request.RequestUri != null && IsBlocked(request.RequestUri.ToString()))
			{
				Report("HttpClient", request.RequestUri.ToString());
				throw new HttpRequestException("[DeHamburbur] Connection to Hamburbur blocked by SUPC.");
			}
			return true;
		}

		[HarmonyPatch(typeof(WebRequest), "Create", new Type[] { typeof(Uri) })]
		[HarmonyPrefix]
		private static bool Block_WebRequest(Uri requestUri)
		{
			if (requestUri != null && IsBlocked(requestUri.ToString()))
			{
				Report("WebRequest", requestUri.ToString());
				throw new WebException("[DeHamburbur] Connection to Hamburbur blocked by SUPC.");
			}
			return true;
		}
	}
}
