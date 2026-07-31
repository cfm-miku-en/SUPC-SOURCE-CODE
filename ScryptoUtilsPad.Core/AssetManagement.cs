namespace ScryptoUtilsPad.Core
{
	public class AssetManagement : MonoBehaviour
	{
		public static AssetBundle LoadBundle(string bundleName)
		{
			return AssetBundle.LoadFromStream(System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Concat("ScryptoUtilsPad.Resources.", bundleName)));
		}

		public static AudioClip LoadEmbeddedWav(string name)
		{
			System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Concat("ScryptoUtilsPad.Resources.", name, ".wav"));
			if (stream == null)
			{
				return null;
			}
			byte[] data;
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
			{
				stream.CopyTo(ms);
				data = ms.ToArray();
			}
			stream.Dispose();
			try
			{
				return WavToAudioClip(data, name);
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
				return null;
			}
		}

		private static AudioClip WavToAudioClip(byte[] data, string name)
		{
			int channels = 1;
			int sampleRate = 44100;
			int bitsPerSample = 16;
			int formatTag = 1;
			int dataOffset = 0;
			int dataLength = 0;
			int pos = 12;
			while (pos + 8 <= data.Length)
			{
				string chunkId = System.Text.Encoding.ASCII.GetString(data, pos, 4);
				int chunkSize = System.BitConverter.ToInt32(data, pos + 4);
				int chunkBody = pos + 8;
				if (chunkId == "fmt ")
				{
					formatTag = System.BitConverter.ToInt16(data, chunkBody);
					channels = System.BitConverter.ToInt16(data, chunkBody + 2);
					sampleRate = System.BitConverter.ToInt32(data, chunkBody + 4);
					bitsPerSample = System.BitConverter.ToInt16(data, chunkBody + 14);
				}
				else if (chunkId == "data")
				{
					dataOffset = chunkBody;
					dataLength = chunkSize;
				}
				pos = chunkBody + chunkSize + (chunkSize & 1);
			}
			if (dataLength <= 0 || dataOffset + dataLength > data.Length)
			{
				dataLength = data.Length - dataOffset;
			}
			int bytesPerSample = bitsPerSample / 8;
			if (bytesPerSample <= 0 || channels <= 0)
			{
				return null;
			}
			int sampleCount = dataLength / bytesPerSample;
			float[] samples = new float[sampleCount];
			int di = dataOffset;
			int i = 0;
			while (i < sampleCount)
			{
				if (formatTag == 3 && bitsPerSample == 32)
				{
					samples[i] = System.BitConverter.ToSingle(data, di);
				}
				else if (bitsPerSample == 16)
				{
					samples[i] = (float)System.BitConverter.ToInt16(data, di) / 32768f;
				}
				else if (bitsPerSample == 8)
				{
					samples[i] = ((float)(int)data[di] - 128f) / 128f;
				}
				else if (bitsPerSample == 24)
				{
					int v = data[di] | (data[di + 1] << 8) | (data[di + 2] << 16);
					if ((v & 0x800000) != 0)
					{
						v |= unchecked((int)0xFF000000u);
					}
					samples[i] = (float)v / 8388608f;
				}
				else if (bitsPerSample == 32)
				{
					samples[i] = (float)System.BitConverter.ToInt32(data, di) / 2147483648f;
				}
				di += bytesPerSample;
				i++;
			}
			int frames = sampleCount / channels;
			if (frames <= 0)
			{
				return null;
			}
			AudioClip clip = AudioClip.Create(name, frames, channels, sampleRate, false);
			clip.SetData(samples, 0);
			return clip;
		}

		public static TMP_FontAsset LoadEmbeddedFont(string name)
		{
			System.IO.Stream manifestResourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Concat("ScryptoUtilsPad.Resources.", name, ".ttf"));
			byte[] array = new byte[manifestResourceStream.Length];
			manifestResourceStream.Read(array, 0, array.Length);
			string text = System.IO.Path.Combine(Application.temporaryCachePath, string.Concat("TempFont_", name, ".ttf"));
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
