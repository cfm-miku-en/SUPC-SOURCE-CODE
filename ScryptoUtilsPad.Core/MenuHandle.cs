namespace ScryptoUtilsPad.Core
{
	public class MenuHandle : MonoBehaviour
	{
		private bool _leftIn;

		private bool _rightIn;

		private bool _isGrabbing;

		private bool _grabIsLeft;

		private void Awake()
		{
			UnityLayerExtensions.SetLayer(((Component)this).gameObject, (UnityLayer)18);
			Collider[] components = ((Component)this).GetComponents<Collider>();
			int num = 0;
			while (num < components.Length)
			{
				Collider val = components[num];
				val.enabled = false;
				num++;
			}
			BoxCollider val2 = ((Component)this).gameObject.AddComponent<BoxCollider>();
			((Collider)val2).isTrigger = true;
			val2.center = Vector3.zero;
			val2.size = new Vector3(0.05f, 0.02f, 0.4f);
		}

		private void OnTriggerEnter(Collider other)
		{
			GorillaTriggerColliderHandIndicator componentInParent = ((Component)other).GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (!((Object)(object)componentInParent == (Object)null))
			{
				if (componentInParent.isLeftHand)
				{
					_leftIn = true;
				}
				else
				{
					_rightIn = true;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			GorillaTriggerColliderHandIndicator componentInParent = ((Component)other).GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (!((Object)(object)componentInParent == (Object)null))
			{
				if (componentInParent.isLeftHand)
				{
					_leftIn = false;
				}
				else
				{
					_rightIn = false;
				}
			}
		}

		private void Update()
		{
			ScryptoUtilsPad.Core.PositionHandler instance = ScryptoUtilsPad.Core.PositionHandler.Instance;
			if (instance != null && instance.CurrentMode == ScryptoUtilsPad.Core.PositionHandler.MenuMode.Hold)
			{
				if (_isGrabbing)
				{
					_isGrabbing = false;
					ScryptoUtilsPad.Core.PositionHandler.Instance.ReleaseGrab();
				}
				return;
			}
			ControllerInputPoller instance2 = ControllerInputPoller.instance;
			if ((Object)(object)instance2 == (Object)null)
			{
				return;
			}
			bool flag = instance2.leftControllerGripFloat > 0.5f;
			bool flag2 = instance2.rightControllerGripFloat > 0.5f;
			if (_isGrabbing)
			{
				if (!(_grabIsLeft ? flag : flag2))
				{
					_isGrabbing = false;
					ScryptoUtilsPad.Core.PositionHandler instance3 = ScryptoUtilsPad.Core.PositionHandler.Instance;
					if (instance3 != null)
					{
						instance3.ReleaseGrab();
					}
				}
				return;
			}
			bool flag3 = _leftIn && flag;
			bool flag4 = _rightIn && flag2;
			if (flag3)
			{
				_isGrabbing = true;
				_grabIsLeft = true;
				ScryptoUtilsPad.Core.PositionHandler instance4 = ScryptoUtilsPad.Core.PositionHandler.Instance;
				if (instance4 != null)
				{
					instance4.StartGrab(GorillaTagger.Instance.leftHandTransform);
				}
			}
			else if (flag4)
			{
				_isGrabbing = true;
				_grabIsLeft = false;
				ScryptoUtilsPad.Core.PositionHandler instance5 = ScryptoUtilsPad.Core.PositionHandler.Instance;
				if (instance5 != null)
				{
					instance5.StartGrab(GorillaTagger.Instance.rightHandTransform);
				}
			}
		}
	}
}
