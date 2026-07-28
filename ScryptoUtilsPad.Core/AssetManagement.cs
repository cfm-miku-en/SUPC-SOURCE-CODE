namespace ScryptoUtilsPad.Core
{
	public class AssetManagement : MonoBehaviour
	{
		public static AssetBundle LoadBundle(string bundleName)
		{
			return AssetBundle.LoadFromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Concat("ScryptoUtilsPad.Resources.", bundleName)));
		}

		public static TMP_FontAsset LoadEmbeddedFont(string name)
		{
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Expected O, but got Unknown
			System.IO.Stream manifestResourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Concat("ScryptoUtilsPad.Resources.", name, ".ttf"));
			byte[] array = new byte[manifestResourceStream.Length];
			manifestResourceStream.Read(array, 0, array.Length);
			string text = System.IO.Path.Combine(Application.temporaryCachePath, "TempFont.ttf");
			System.IO.File.WriteAllBytes(text, array);
			TMP_FontAsset result = TMP_FontAsset.CreateFontAsset(new Font(text));
			if (manifestResourceStream != null)
			{
				manifestResourceStream.Dispose();
			}
			return result;
		}
	}
}
