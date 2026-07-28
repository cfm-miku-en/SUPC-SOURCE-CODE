namespace ScryptoUtilsPad.Core
{
	public static class Branding
	{
		private static readonly ManualLogSource Log = Logger.CreateLogSource("Branding");

		public const string PadTitle = "Scrypto's Utils Pad Continued";

		private static readonly string NormalizedPadTitle = Normalize(PadTitle);

		private static readonly string[] OldTitleKeys = new string[2] { "scryptosutilspad", "scryptoutilspad" };

		public static void Apply(GameObject menu)
		{
			if ((Object)(object)menu == (Object)null)
			{
				return;
			}
			int changed = 0;
			TMP_Text[] tmps = menu.GetComponentsInChildren<TMP_Text>(true);
			int i = 0;
			while (i < tmps.Length)
			{
				TMP_Text t = tmps[i];
				if (ShouldRetitle(t.text))
				{
					LogRetitle(t.text, ((Component)t).transform);
					t.text = PadTitle;
					changed++;
				}
				i++;
			}
			Text[] texts = menu.GetComponentsInChildren<Text>(true);
			int j = 0;
			while (j < texts.Length)
			{
				Text t2 = texts[j];
				if (ShouldRetitle(t2.text))
				{
					LogRetitle(t2.text, ((Component)t2).transform);
					t2.text = PadTitle;
					changed++;
				}
				j++;
			}
			TextMesh[] meshes = menu.GetComponentsInChildren<TextMesh>(true);
			int k = 0;
			while (k < meshes.Length)
			{
				TextMesh t3 = meshes[k];
				if (ShouldRetitle(t3.text))
				{
					LogRetitle(t3.text, ((Component)t3).transform);
					t3.text = PadTitle;
					changed++;
				}
				k++;
			}
			if (changed > 0)
			{
				Log.LogInfo("[Branding] Retitled " + changed + " text(s) on " + ((Object)menu).name + ".");
				return;
			}
			Log.LogWarning("[Branding] Nothing on \"" + ((Object)menu).name + "\" carried the old name — the title may be part of a texture. Texts found:");
			int n = 0;
			while (n < tmps.Length)
			{
				Log.LogWarning("[Branding]   " + PathOf(((Component)tmps[n]).transform) + " = \"" + tmps[n].text + "\"");
				n++;
			}
			int n2 = 0;
			while (n2 < texts.Length)
			{
				Log.LogWarning("[Branding]   " + PathOf(((Component)texts[n2]).transform) + " = \"" + texts[n2].text + "\"");
				n2++;
			}
			int n3 = 0;
			while (n3 < meshes.Length)
			{
				Log.LogWarning("[Branding]   " + PathOf(((Component)meshes[n3]).transform) + " = \"" + meshes[n3].text + "\"");
				n3++;
			}
		}

		private static void LogRetitle(string current, Transform tr)
		{
			Log.LogInfo("[Branding] \"" + current + "\" -> \"" + PadTitle + "\" (" + PathOf(tr) + ")");
		}

		private static bool ShouldRetitle(string current)
		{
			if (string.IsNullOrEmpty(current))
			{
				return false;
			}
			string key = Normalize(current);
			if (key.Length == 0 || key == NormalizedPadTitle)
			{
				return false;
			}
			int i = 0;
			while (i < OldTitleKeys.Length)
			{
				if (key.Contains(OldTitleKeys[i]))
				{
					return true;
				}
				i++;
			}
			return false;
		}

		private static string Normalize(string s)
		{
			StringBuilder sb = new StringBuilder(s.Length);
			bool inTag = false;
			int i = 0;
			while (i < s.Length)
			{
				char c = s[i];
				if (c == '<')
				{
					inTag = true;
				}
				else if (c == '>')
				{
					inTag = false;
				}
				else if (!inTag && char.IsLetterOrDigit(c))
				{
					sb.Append(char.ToLowerInvariant(c));
				}
				i++;
			}
			return sb.ToString();
		}

		private static string PathOf(Transform t)
		{
			string path = ((Object)t).name;
			Transform p = t.parent;
			while ((Object)(object)p != (Object)null)
			{
				path = ((Object)p).name + "/" + path;
				p = p.parent;
			}
			return path;
		}
	}
}
