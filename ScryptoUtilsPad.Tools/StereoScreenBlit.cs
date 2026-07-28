using UnityEngine;

namespace ScryptoUtilsPad.Tools
{
	public class StereoScreenBlit : MonoBehaviour
	{
		public RenderTexture Source;

		public Renderer Target;

		private Material _mat;

		private void Start()
		{
			Apply();
		}

		private void Apply()
		{
			if ((Object)(object)Target == (Object)null || (Object)(object)Source == (Object)null)
			{
				return;
			}
			_mat = Target.material;
			if ((Object)(object)_mat != (Object)null)
			{
				_mat.mainTexture = (Texture)(object)Source;
				if (_mat.HasProperty("_BaseMap"))
				{
					_mat.SetTexture("_BaseMap", (Texture)(object)Source);
				}
				if (_mat.HasProperty("_EmissionMap"))
				{
					_mat.EnableKeyword("_EMISSION");
					_mat.SetTexture("_EmissionMap", (Texture)(object)Source);
					_mat.SetColor("_EmissionColor", Color.white);
				}
			}
		}

		private void LateUpdate()
		{
			if ((Object)(object)_mat == (Object)null && (Object)(object)Target != (Object)null)
			{
				Apply();
			}
		}
	}
}
