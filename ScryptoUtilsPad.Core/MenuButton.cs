namespace ScryptoUtilsPad.Core
{
	public class MenuButton : MonoBehaviour
	{
		private const float DebounceTime = 0.2f;

		private const float PressDepth = 0.0003f;

		private const float AnimSpeed = 20f;

		public System.Action OnPress;

		public bool rightHandOnly;

		private float _touchTime;

		private Vector3 _restPos;

		private float _pressT;

		private void Awake()
		{
			UnityLayerExtensions.SetLayer(((Component)this).gameObject, (UnityLayer)18);
			_restPos = ((Component)this).transform.localPosition;
		}

		private void Update()
		{
			_pressT = Mathf.Lerp(_pressT, 0f, Time.deltaTime * 20f);
			((Component)this).transform.localPosition = _restPos + Vector3.down * (0.0003f * _pressT);
		}

		public void SetLabel(string text)
		{
			TMP_Text componentInChildren = ((Component)this).GetComponentInChildren<TMP_Text>();
			if ((Object)(object)componentInChildren != (Object)null)
			{
				componentInChildren.text = text;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (Time.time - _touchTime < 0.2f)
			{
				return;
			}
			GorillaTriggerColliderHandIndicator componentInParent = ((Component)other).GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if ((Object)(object)componentInParent == (Object)null || (rightHandOnly && componentInParent.isLeftHand))
			{
				return;
			}
			_touchTime = Time.time;
			_pressT = 1f;
			try
			{
				System.Action onPress = OnPress;
				if (onPress != null)
				{
					onPress();
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogException(ex);
			}
			ScryptoUtilsPad.Plugin instance = ScryptoUtilsPad.Plugin.Instance;
			if ((Object)(object)((instance != null) ? instance.ButtonClickSound : null) != (Object)null)
			{
				AudioSource val = (componentInParent.isLeftHand ? GorillaTagger.Instance.offlineVRRig.leftHandPlayer : GorillaTagger.Instance.offlineVRRig.rightHandPlayer);
				GTAudioSourceExtensions.GTPlayOneShot(val, ScryptoUtilsPad.Plugin.Instance.ButtonClickSound, 2f);
			}
			GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, 0.2f, 0.2f);
		}
	}
}
