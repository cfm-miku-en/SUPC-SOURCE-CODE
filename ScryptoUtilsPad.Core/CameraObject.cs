namespace ScryptoUtilsPad.Core
{
	public class CameraObject : MonoBehaviour
	{
		private enum NetState
		{
			Spawning,
			Idle,
			Grabbed,
			Falling
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003CSpawnAnim_003Ed__28 : System.Collections.Generic.IEnumerator<object>, System.Collections.IEnumerator, System.IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ScryptoUtilsPad.Core.CameraObject _003C_003E4__this;

			private float _003Celapsed_003E5__1;

			private float _003Ct_003E5__2;

			private float _003Csmooth_003E5__3;

			private float _003Covershoot_003E5__4;

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
			public _003CSpawnAnim_003Ed__28(int _003C_003E1__state)
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
				}
				else
				{
					_003C_003E1__state = -1;
					((Component)_003C_003E4__this).transform.localScale = Vector3.zero;
					_003Celapsed_003E5__1 = 0f;
				}
				if (_003Celapsed_003E5__1 < 0.35f)
				{
					_003Celapsed_003E5__1 += Time.deltaTime;
					_003Ct_003E5__2 = Mathf.Clamp01(_003Celapsed_003E5__1 / 0.35f);
					_003Csmooth_003E5__3 = Mathf.SmoothStep(0f, 1f, _003Ct_003E5__2);
					_003Covershoot_003E5__4 = 1f + Mathf.Sin(_003Ct_003E5__2 * System.MathF.PI) * 0.2f;
					((Component)_003C_003E4__this).transform.localScale = _003C_003E4__this._spawnScale * (_003Csmooth_003E5__3 * _003Covershoot_003E5__4);
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				((Component)_003C_003E4__this).transform.localScale = _003C_003E4__this._spawnScale;
				_003C_003E4__this._state = ScryptoUtilsPad.Core.CameraObject.NetState.Idle;
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

		public bool IsRemote;

		private ScryptoUtilsPad.Core.CameraObject.NetState _state = ScryptoUtilsPad.Core.CameraObject.NetState.Spawning;

		private Vector3 _targetPos;

		private Quaternion _targetRot;

		private Vector3 _fallVelocity;

		private float _fallTime;

		private Vector3 _spawnScale;

		private Color _currentMat1 = new Color(0f, 1f, 0.667f);

		private Color _currentMat2 = new Color(0.169f, 0.169f, 0.169f);

		private const float GrabLerp = 25f;

		private const float IdleLerp = 10f;

		private const float MaxFallTime = 2f;

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
				val.isTrigger = true;
				num++;
			}
			_spawnScale = ((Component)this).transform.localScale;
			_targetPos = ((Component)this).transform.position;
			_targetRot = ((Component)this).transform.rotation;
		}

		private void Start()
		{
			if (IsRemote)
			{
				((MonoBehaviour)this).StartCoroutine(SpawnAnim());
			}
			else
			{
				_state = ScryptoUtilsPad.Core.CameraObject.NetState.Idle;
			}
		}

		public void SetTarget(Vector3 pos, Quaternion rot)
		{
			_targetPos = pos;
			_targetRot = rot;
			if (_state == ScryptoUtilsPad.Core.CameraObject.NetState.Falling)
			{
				_fallVelocity = Vector3.zero;
				_state = ScryptoUtilsPad.Core.CameraObject.NetState.Idle;
			}
		}

		public void OnGrab(Vector3 pos, Quaternion rot)
		{
			if (_state != ScryptoUtilsPad.Core.CameraObject.NetState.Spawning)
			{
				_targetPos = pos;
				_targetRot = rot;
				_fallVelocity = Vector3.zero;
				_state = ScryptoUtilsPad.Core.CameraObject.NetState.Grabbed;
			}
		}

		public void OnRelease(Vector3 pos, Quaternion rot)
		{
			_targetPos = pos;
			_targetRot = rot;
			_fallVelocity = Vector3.zero;
			_fallTime = 0f;
			_state = ScryptoUtilsPad.Core.CameraObject.NetState.Falling;
		}

		public void ApplyTheme(Color mat1, Color mat2)
		{
			Renderer[] componentsInChildren = ((Component)this).GetComponentsInChildren<Renderer>(true);
			int num = 0;
			while (num < componentsInChildren.Length)
			{
				Renderer val = componentsInChildren[num];
				Material[] materials = val.materials;
				int num2 = 0;
				while (num2 < materials.Length)
				{
					Material val2 = materials[num2];
					if (val2.HasProperty("_Color"))
					{
						if (ColorClose(val2.color, _currentMat1, 0.05f))
						{
							val2.color = mat1;
						}
						else if (ColorClose(val2.color, _currentMat2, 0.05f))
						{
							val2.color = mat2;
						}
					}
					num2++;
				}
				num++;
			}
			_currentMat1 = mat1;
			_currentMat2 = mat2;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (IsRemote)
			{
				return;
			}
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
			if (IsRemote)
			{
				return;
			}
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
			if (IsRemote)
			{
				UpdateRemote();
			}
			else
			{
				UpdateLocal();
			}
		}

		private void UpdateLocal()
		{
			ControllerInputPoller instance = ControllerInputPoller.instance;
			if ((Object)(object)instance == (Object)null)
			{
				return;
			}
			bool flag = instance.leftControllerGripFloat > 0.5f;
			bool flag2 = instance.rightControllerGripFloat > 0.5f;
			if (_isGrabbing)
			{
				if (!(_grabIsLeft ? flag : flag2))
				{
					_isGrabbing = false;
					ScryptoUtilsPad.Tools.CameraPage instance2 = ScryptoUtilsPad.Tools.CameraPage.Instance;
					if (instance2 != null)
					{
						instance2.ReleaseGrab();
					}
				}
			}
			else if (_leftIn && flag)
			{
				_isGrabbing = true;
				_grabIsLeft = true;
				ScryptoUtilsPad.Tools.CameraPage instance3 = ScryptoUtilsPad.Tools.CameraPage.Instance;
				if (instance3 != null)
				{
					instance3.StartGrab(GorillaTagger.Instance.leftHandTransform);
				}
			}
			else if (_rightIn && flag2)
			{
				_isGrabbing = true;
				_grabIsLeft = false;
				ScryptoUtilsPad.Tools.CameraPage instance4 = ScryptoUtilsPad.Tools.CameraPage.Instance;
				if (instance4 != null)
				{
					instance4.StartGrab(GorillaTagger.Instance.rightHandTransform);
				}
			}
		}

		private void UpdateRemote()
		{
			switch (_state)
			{
			case ScryptoUtilsPad.Core.CameraObject.NetState.Grabbed:
				((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, _targetPos, Time.deltaTime * 25f);
				((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, _targetRot, Time.deltaTime * 25f);
				break;
			case ScryptoUtilsPad.Core.CameraObject.NetState.Idle:
				((Component)this).transform.position = Vector3.Lerp(((Component)this).transform.position, _targetPos, Time.deltaTime * 10f);
				((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, _targetRot, Time.deltaTime * 10f);
				break;
			case ScryptoUtilsPad.Core.CameraObject.NetState.Falling:
			{
				_fallTime += Time.deltaTime;
				_fallVelocity += Vector3.down * (9.8f * Time.deltaTime);
				Transform transform = ((Component)this).transform;
				transform.position += _fallVelocity * Time.deltaTime;
				if (_fallTime >= 2f)
				{
					_fallVelocity = Vector3.zero;
					_state = ScryptoUtilsPad.Core.CameraObject.NetState.Idle;
				}
				break;
			}
			}
		}

		[System.Runtime.CompilerServices.IteratorStateMachine(typeof(ScryptoUtilsPad.Core.CameraObject._003CSpawnAnim_003Ed__28))]
		private System.Collections.IEnumerator SpawnAnim()
		{
			ScryptoUtilsPad.Core.CameraObject._003CSpawnAnim_003Ed__28 obj = new ScryptoUtilsPad.Core.CameraObject._003CSpawnAnim_003Ed__28(0);
			obj._003C_003E4__this = this;
			return obj;
		}

		private static bool ColorClose(Color a, Color b, float tol = 0.05f)
		{
			return Mathf.Abs(a.r - b.r) < tol && Mathf.Abs(a.g - b.g) < tol && Mathf.Abs(a.b - b.b) < tol;
		}
	}
}
