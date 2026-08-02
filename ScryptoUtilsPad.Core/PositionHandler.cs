namespace ScryptoUtilsPad.Core
{
	public class PositionHandler : MonoBehaviour
	{
		public enum MenuMode
		{
			Float,
			Hold
		}

		private enum MenuState
		{
			Closed,
			Open,
			Grabbed,
			Falling
		}

		[System.Serializable]
		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly ScryptoUtilsPad.Core.PositionHandler._003C_003Ec _003C_003E9 = new ScryptoUtilsPad.Core.PositionHandler._003C_003Ec();

			public static System.Func<bool> _003C_003E9__32_0;

			internal bool _003CInit_003Eb__32_0()
			{
				return (Object)(object)GorillaTagger.Instance != (Object)null && (Object)(object)ControllerInputPoller.instance != (Object)null;
			}
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003CClosePanel_003Ed__41 : System.Collections.Generic.IEnumerator<object>, System.Collections.IEnumerator, System.IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ScryptoUtilsPad.Core.PositionHandler _003C_003E4__this;

			private Vector3 _003CstartScale_003E5__1;

			private float _003Celapsed_003E5__2;

			private Rigidbody _003Crb_003E5__3;

			object System.Collections.Generic.IEnumerator<object>.Current
			{
				[System.Diagnostics.DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				[System.Diagnostics.DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			[System.Diagnostics.DebuggerHidden]
			public _003CClosePanel_003Ed__41(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[System.Diagnostics.DebuggerHidden]
			void System.IDisposable.Dispose()
			{
				_003Crb_003E5__3 = null;
				_003C_003E1__state = -2;
			}

			private bool MoveNext()
			{
				int num = _003C_003E1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
				}
				else
				{
					_003C_003E1__state = -1;
					if ((Object)(object)Checker == (Object)null)
					{
						return false;
					}
					_003C_003E4__this._isAnimating = true;
					GTAudioSourceExtensions.GTPlayOneShot(GorillaTagger.Instance.offlineVRRig.leftHandPlayer, ScryptoUtilsPad.Plugin.Instance.MenuCloseSound, 1f);
					_003CstartScale_003E5__1 = Checker.transform.localScale;
					_003Celapsed_003E5__2 = 0f;
				}
				if (_003Celapsed_003E5__2 < 0.1f)
				{
					_003Celapsed_003E5__2 += Time.deltaTime;
					Checker.transform.localScale = Vector3.Lerp(_003CstartScale_003E5__1, Vector3.one * 0.01f, _003Celapsed_003E5__2 / 0.1f);
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				_003Crb_003E5__3 = MenuRb;
				if ((Object)(object)_003Crb_003E5__3 != (Object)null)
				{
					_003Crb_003E5__3.isKinematic = true;
					_003Crb_003E5__3.useGravity = false;
				}
				_003C_003E4__this._holdingHand = null;
				_003C_003E4__this._state = ScryptoUtilsPad.Core.PositionHandler.MenuState.Closed;
				_003C_003E4__this._isAnimating = false;
				ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
				if (instance != null)
				{
					instance.SendClose();
				}
				Checker.SetActive(false);
				return false;
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				return this.MoveNext();
			}

			[System.Diagnostics.DebuggerHidden]
			void System.Collections.IEnumerator.Reset()
			{
				throw new System.NotSupportedException();
			}
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003CInit_003Ed__32 : System.Collections.Generic.IEnumerator<object>, System.Collections.IEnumerator, System.IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ScryptoUtilsPad.Core.PositionHandler _003C_003E4__this;

			object System.Collections.Generic.IEnumerator<object>.Current
			{
				[System.Diagnostics.DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				[System.Diagnostics.DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			[System.Diagnostics.DebuggerHidden]
			public _003CInit_003Ed__32(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[System.Diagnostics.DebuggerHidden]
			void System.IDisposable.Dispose()
			{
				_003C_003E1__state = -2;
			}

			private bool MoveNext()
			{
				int num = _003C_003E1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
					_003C_003E4__this._initialized = true;
					return false;
				}
				_003C_003E1__state = -1;
				_003C_003E2__current = (object)new WaitUntil(ScryptoUtilsPad.Core.PositionHandler._003C_003Ec._003C_003E9__32_0 ?? (ScryptoUtilsPad.Core.PositionHandler._003C_003Ec._003C_003E9__32_0 = new System.Func<bool>(ScryptoUtilsPad.Core.PositionHandler._003C_003Ec._003C_003E9._003CInit_003Eb__32_0)));
				_003C_003E1__state = 1;
				return true;
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				return this.MoveNext();
			}

			[System.Diagnostics.DebuggerHidden]
			void System.Collections.IEnumerator.Reset()
			{
				throw new System.NotSupportedException();
			}
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003COpenPanel_003Ed__40 : System.Collections.Generic.IEnumerator<object>, System.Collections.IEnumerator, System.IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ScryptoUtilsPad.Core.PositionHandler _003C_003E4__this;

			private Rigidbody _003Crb_003E5__1;

			private float _003Cps_003E5__2;

			private float _003Celapsed_003E5__3;

			private Vector3 _003Ctarget_003E5__4;

			private Transform _003Chand_003E5__5;

			private Transform _003Ccam_003E5__6;

			private Vector3 _003Cfwd_003E5__7;

			private Quaternion _003CyawAtOpen_003E5__8;

			object System.Collections.Generic.IEnumerator<object>.Current
			{
				[System.Diagnostics.DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			object System.Collections.IEnumerator.Current
			{
				[System.Diagnostics.DebuggerHidden]
				get
				{
					return _003C_003E2__current;
				}
			}

			[System.Diagnostics.DebuggerHidden]
			public _003COpenPanel_003Ed__40(int _003C_003E1__state)
			{
				this._003C_003E1__state = _003C_003E1__state;
			}

			[System.Diagnostics.DebuggerHidden]
			void System.IDisposable.Dispose()
			{
				_003Crb_003E5__1 = null;
				_003Chand_003E5__5 = null;
				_003Ccam_003E5__6 = null;
				_003C_003E1__state = -2;
			}

			private bool MoveNext()
			{
				int num = _003C_003E1__state;
				if (num != 0)
				{
					if (num != 1)
					{
						return false;
					}
					_003C_003E1__state = -1;
				}
				else
				{
					_003C_003E1__state = -1;
					if ((Object)(object)Checker == (Object)null)
					{
						return false;
					}
					_003Crb_003E5__1 = MenuRb;
					if ((Object)(object)_003Crb_003E5__1 != (Object)null)
					{
						_003Crb_003E5__1.isKinematic = true;
						_003Crb_003E5__1.useGravity = false;
					}
					_003Cps_003E5__2 = PlayerScale;
					if (_003C_003E4__this.CurrentMode == ScryptoUtilsPad.Core.PositionHandler.MenuMode.Hold)
					{
						_003Chand_003E5__5 = SameHand;
						if ((Object)(object)_003Chand_003E5__5 != (Object)null)
						{
							Checker.transform.position = _003Chand_003E5__5.TransformPoint(new Vector3(0.212f, -0.05f, 0f));
							Checker.transform.rotation = _003Chand_003E5__5.rotation * Quaternion.Euler(90f, 0f, 0f);
						}
						_003Chand_003E5__5 = null;
					}
					else
					{
						_003Ccam_003E5__6 = CamTransform;
						if ((Object)(object)_003Ccam_003E5__6 != (Object)null)
						{
							_003Cfwd_003E5__7 = _003Ccam_003E5__6.forward;
							_003Cfwd_003E5__7.y = 0f;
							if (_003Cfwd_003E5__7.sqrMagnitude < 0.001f)
							{
								_003Cfwd_003E5__7 = _003Ccam_003E5__6.forward;
							}
							else
							{
								_003Cfwd_003E5__7.Normalize();
							}
							Checker.transform.position = _003Ccam_003E5__6.position + _003Ccam_003E5__6.forward * (0.5f * _003Cps_003E5__2) + Vector3.up * (-0.05f * _003Cps_003E5__2);
							Checker.transform.rotation = Quaternion.LookRotation(_003Cfwd_003E5__7, Vector3.up);
							_003CyawAtOpen_003E5__8 = Quaternion.Euler(0f, _003Ccam_003E5__6.eulerAngles.y, 0f);
							_003C_003E4__this._openOffsetNorm = Quaternion.Inverse(_003CyawAtOpen_003E5__8) * ((Checker.transform.position - _003Ccam_003E5__6.position) / _003Cps_003E5__2);
						}
						_003Ccam_003E5__6 = null;
					}
					Checker.SetActive(true);
					_003C_003E4__this._state = ScryptoUtilsPad.Core.PositionHandler.MenuState.Open;
					_003C_003E4__this._isAnimating = true;
					ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
					if (instance != null)
					{
						instance.SendOpen(Checker.transform.position, Checker.transform.rotation);
					}
					GTAudioSourceExtensions.GTPlayOneShot(GorillaTagger.Instance.offlineVRRig.leftHandPlayer, ScryptoUtilsPad.Plugin.Instance.MenuOpenSound, 1f);
					_003Celapsed_003E5__3 = 0f;
					_003Ctarget_003E5__4 = Vector3.one * (0.15f * _003Cps_003E5__2);
				}
				if (_003Celapsed_003E5__3 < 0.1f)
				{
					_003Celapsed_003E5__3 += Time.deltaTime;
					Checker.transform.localScale = Vector3.Lerp(Vector3.one * 0.01f, _003Ctarget_003E5__4, _003Celapsed_003E5__3 / 0.1f);
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				Checker.transform.localScale = _003Ctarget_003E5__4;
				_003C_003E4__this._isAnimating = false;
				return false;
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				return this.MoveNext();
			}

			[System.Diagnostics.DebuggerHidden]
			void System.Collections.IEnumerator.Reset()
			{
				throw new System.NotSupportedException();
			}
		}

		public static ScryptoUtilsPad.Core.PositionHandler Instance;

		public static GameObject Checker;

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private ScryptoUtilsPad.Core.PositionHandler.MenuMode _003CCurrentMode_003Ek__BackingField = ScryptoUtilsPad.Core.PositionHandler.MenuMode.Float;

		private bool _initialized;

		private bool _xWasPressed;

		private bool _isAnimating;

		private const float BaseScale = 0.15f;

		private const float CamDistance = 0.5f;

		private const float FollowSmooth = 10f;

		private ScryptoUtilsPad.Core.PositionHandler.MenuState _state = ScryptoUtilsPad.Core.PositionHandler.MenuState.Closed;

		private Transform _holdingHand;

		private Vector3 _grabLocalPos;

		private Quaternion _grabLocalRot;

		private Vector3 _openOffsetNorm;

		private float _lastMoveSend;

		private const float MoveSendInterval = 0.05f;

		private static Rigidbody _menuRb;

		public ScryptoUtilsPad.Core.PositionHandler.MenuMode CurrentMode
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CCurrentMode_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CCurrentMode_003Ek__BackingField = value;
			}
		}

		private static Rigidbody MenuRb
		{
			get
			{
				if ((Object)(object)_menuRb == (Object)null && (Object)(object)Checker != (Object)null)
				{
					_menuRb = Checker.GetComponent<Rigidbody>();
				}
				return _menuRb;
			}
		}

		private static float PlayerScale
		{
			get
			{
				return ((Object)(object)VRRig.LocalRig != (Object)null) ? VRRig.LocalRig.scaleFactor : 1f;
			}
		}

		private static Transform CamTransform
		{
			get
			{
				GorillaTagger instance = GorillaTagger.Instance;
				object obj;
				if (instance == null)
				{
					obj = null;
				}
				else
				{
					GameObject mainCamera = instance.mainCamera;
					obj = ((mainCamera != null) ? mainCamera.transform : null);
				}
				if (obj == null)
				{
					Camera main = Camera.main;
					obj = ((main != null) ? ((Component)main).transform : null);
				}
				return (Transform)obj;
			}
		}

		private static Transform SameHand
		{
			get
			{
				if ((Object)(object)ScryptoUtilsPad.Plugin.Instance == (Object)null || (Object)(object)GorillaTagger.Instance == (Object)null)
				{
					return null;
				}
				ScryptoUtilsPad.Plugin.OpenButton value = ScryptoUtilsPad.Plugin.Instance.Keybind.Value;
				bool flag = ((value == ScryptoUtilsPad.Plugin.OpenButton.LeftPrimary || value == ScryptoUtilsPad.Plugin.OpenButton.LeftSecondary) ? true : false);
				return flag ? GorillaTagger.Instance.leftHandTransform : GorillaTagger.Instance.rightHandTransform;
			}
		}

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			((MonoBehaviour)this).StartCoroutine(Init());
		}

		[System.Runtime.CompilerServices.IteratorStateMachine(typeof(ScryptoUtilsPad.Core.PositionHandler._003CInit_003Ed__32))]
		private System.Collections.IEnumerator Init()
		{
			ScryptoUtilsPad.Core.PositionHandler._003CInit_003Ed__32 obj = new ScryptoUtilsPad.Core.PositionHandler._003CInit_003Ed__32(0);
			obj._003C_003E4__this = this;
			return obj;
		}

		private void Update()
		{
			if (_initialized)
			{
				bool inputState = GetInputState();
				if (inputState && !_xWasPressed)
				{
					ToggleMenu();
				}
				_xWasPressed = inputState;
			}
		}

		private void ToggleMenu()
		{
			if (!_isAnimating)
			{
				if (_state == ScryptoUtilsPad.Core.PositionHandler.MenuState.Closed)
				{
					((MonoBehaviour)this).StartCoroutine(OpenPanel());
				}
				else
				{
					((MonoBehaviour)this).StartCoroutine(ClosePanel());
				}
			}
		}

		public void SetMode(ScryptoUtilsPad.Core.PositionHandler.MenuMode mode)
		{
			CurrentMode = mode;
			if (_state == ScryptoUtilsPad.Core.PositionHandler.MenuState.Closed)
			{
				return;
			}
			if (mode == ScryptoUtilsPad.Core.PositionHandler.MenuMode.Hold)
			{
				Rigidbody menuRb = MenuRb;
				if ((Object)(object)menuRb != (Object)null)
				{
					menuRb.isKinematic = true;
					menuRb.useGravity = false;
				}
				_holdingHand = null;
				_state = ScryptoUtilsPad.Core.PositionHandler.MenuState.Open;
			}
			else
			{
				Transform camTransform = CamTransform;
				float playerScale = PlayerScale;
				if ((Object)(object)camTransform != (Object)null)
				{
					Quaternion val = Quaternion.Euler(0f, camTransform.eulerAngles.y, 0f);
					_openOffsetNorm = Quaternion.Inverse(val) * ((Checker.transform.position - camTransform.position) / playerScale);
				}
			}
		}

		private void LateUpdate()
		{
			if (!_initialized || (Object)(object)Checker == (Object)null || _isAnimating)
			{
				return;
			}
			float playerScale = PlayerScale;
			if (_state == ScryptoUtilsPad.Core.PositionHandler.MenuState.Grabbed && (Object)(object)_holdingHand != (Object)null)
			{
				Checker.transform.position = _holdingHand.TransformPoint(_grabLocalPos);
				Checker.transform.rotation = _holdingHand.rotation * _grabLocalRot;
				Checker.transform.localScale = Vector3.one * (0.15f * playerScale * ScryptoUtilsPad.Core.MenuSizeSettings.CurrentScale);
				if (Time.time - _lastMoveSend >= 0.05f)
				{
					_lastMoveSend = Time.time;
					ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
					if (instance != null)
					{
						instance.SendMove(Checker.transform.position, Checker.transform.rotation);
					}
				}
			}
			else
			{
				if (_state != ScryptoUtilsPad.Core.PositionHandler.MenuState.Open)
				{
					return;
				}
				if (CurrentMode == ScryptoUtilsPad.Core.PositionHandler.MenuMode.Hold)
				{
					Transform sameHand = SameHand;
					if ((Object)(object)sameHand != (Object)null)
					{
						Checker.transform.position = sameHand.TransformPoint(new Vector3(0.212f, -0.05f, 0f));
						Checker.transform.rotation = sameHand.rotation * Quaternion.Euler(90f, 0f, 0f);
						if (Time.time - _lastMoveSend >= 0.05f)
						{
							_lastMoveSend = Time.time;
							ScryptoUtilsPad.Core.NetworkManager instance2 = ScryptoUtilsPad.Core.NetworkManager.Instance;
							if (instance2 != null)
							{
								instance2.SendMove(Checker.transform.position, Checker.transform.rotation);
							}
						}
					}
				}
				else
				{
					Transform camTransform = CamTransform;
					if ((Object)(object)camTransform != (Object)null)
					{
						Quaternion val = Quaternion.Euler(0f, camTransform.eulerAngles.y, 0f);
						Vector3 val2 = camTransform.position + val * (_openOffsetNorm * playerScale);
						Checker.transform.position = Vector3.Lerp(Checker.transform.position, val2, Time.deltaTime * 10f);
						Vector3 forward = camTransform.forward;
						forward.y = 0f;
						if (forward.sqrMagnitude > 0.001f)
						{
							Quaternion val3 = Quaternion.LookRotation(forward.normalized, Vector3.up);
							Checker.transform.rotation = Quaternion.Slerp(Checker.transform.rotation, val3, Time.deltaTime * 10f);
						}
					}
				}
				Checker.transform.localScale = Vector3.one * (0.15f * playerScale * ScryptoUtilsPad.Core.MenuSizeSettings.CurrentScale);
			}
		}

		public void StartGrab(Transform hand)
		{
			if (_state != ScryptoUtilsPad.Core.PositionHandler.MenuState.Closed && CurrentMode != ScryptoUtilsPad.Core.PositionHandler.MenuMode.Hold)
			{
				Rigidbody menuRb = MenuRb;
				if ((Object)(object)menuRb != (Object)null)
				{
					menuRb.isKinematic = true;
					menuRb.useGravity = false;
				}
				_holdingHand = hand;
				_grabLocalPos = hand.InverseTransformPoint(Checker.transform.position);
				_grabLocalRot = Quaternion.Inverse(hand.rotation) * Checker.transform.rotation;
				_state = ScryptoUtilsPad.Core.PositionHandler.MenuState.Grabbed;
				ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
				if (instance != null)
				{
					instance.SendGrab(Checker.transform.position, Checker.transform.rotation);
				}
			}
		}

		public void ReleaseGrab()
		{
			if (_state == ScryptoUtilsPad.Core.PositionHandler.MenuState.Grabbed)
			{
				ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
				if (instance != null)
				{
					instance.SendRelease(Checker.transform.position, Checker.transform.rotation);
				}
				_holdingHand = null;
				_state = ScryptoUtilsPad.Core.PositionHandler.MenuState.Falling;
				Rigidbody menuRb = MenuRb;
				if ((Object)(object)menuRb != (Object)null)
				{
					menuRb.isKinematic = false;
					menuRb.useGravity = true;
				}
			}
		}

		private bool GetInputState()
		{
			ControllerInputPoller instance = ControllerInputPoller.instance;
			if ((Object)(object)instance == (Object)null)
			{
				return false;
			}
			ScryptoUtilsPad.Plugin.OpenButton value = ScryptoUtilsPad.Plugin.Instance.Keybind.Value;
			if (1 == 0)
			{
			}
			bool result;
			switch (value)
			{
			case ScryptoUtilsPad.Plugin.OpenButton.LeftPrimary:
				result = instance.leftControllerPrimaryButton;
				break;
			case ScryptoUtilsPad.Plugin.OpenButton.RightPrimary:
				result = instance.rightControllerPrimaryButton;
				break;
			case ScryptoUtilsPad.Plugin.OpenButton.LeftSecondary:
				result = instance.leftControllerSecondaryButton;
				break;
			case ScryptoUtilsPad.Plugin.OpenButton.RightSecondary:
				result = instance.rightControllerSecondaryButton;
				break;
			default:
				result = false;
				break;
			}
			if (1 == 0)
			{
			}
			return result;
		}

		[System.Runtime.CompilerServices.IteratorStateMachine(typeof(ScryptoUtilsPad.Core.PositionHandler._003COpenPanel_003Ed__40))]
		private System.Collections.IEnumerator OpenPanel()
		{
			ScryptoUtilsPad.Core.PositionHandler._003COpenPanel_003Ed__40 obj = new ScryptoUtilsPad.Core.PositionHandler._003COpenPanel_003Ed__40(0);
			obj._003C_003E4__this = this;
			return obj;
		}

		[System.Runtime.CompilerServices.IteratorStateMachine(typeof(ScryptoUtilsPad.Core.PositionHandler._003CClosePanel_003Ed__41))]
		private System.Collections.IEnumerator ClosePanel()
		{
			ScryptoUtilsPad.Core.PositionHandler._003CClosePanel_003Ed__41 obj = new ScryptoUtilsPad.Core.PositionHandler._003CClosePanel_003Ed__41(0);
			obj._003C_003E4__this = this;
			return obj;
		}
	}
}
