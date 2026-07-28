namespace ScryptoUtilsPad.Tools
{
	public class CameraPage : MonoBehaviour
	{
		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass48_0
		{
			public string tempDir;

			public string outputFile;

			public string outputDir;

			public string timestamp;

			internal void _003CSaveRecordingAsync_003Eb__0()
			{
				try
				{
					string text = FindFfmpeg();
					if (text != null)
					{
						System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
						processStartInfo.FileName = text;
						string[] array = new string[5];
						array[0] = "-r 30 -i \"";
						array[1] = System.IO.Path.Combine(tempDir, "frame_%06d.jpg");
						array[2] = "\" -c:v libx264 -pix_fmt yuv420p -y \"";
						array[3] = outputFile;
						array[4] = "\"";
						processStartInfo.Arguments = string.Concat(array);
						processStartInfo.UseShellExecute = false;
						processStartInfo.CreateNoWindow = true;
						processStartInfo.RedirectStandardOutput = true;
						processStartInfo.RedirectStandardError = true;
						System.Diagnostics.ProcessStartInfo startInfo = processStartInfo;
						System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);
						try
						{
							if (process != null)
							{
								process.WaitForExit();
							}
							System.IO.Directory.Delete(tempDir, true);
							return;
						}
						finally
						{
							if (process != null)
							{
								((System.IDisposable)process).Dispose();
							}
						}
					}
					string destDirName = System.IO.Path.Combine(outputDir, string.Concat("recording_", timestamp, "_frames"));
					System.IO.Directory.Move(tempDir, destDirName);
				}
				catch
				{
				}
			}
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003CCaptureFramesRoutine_003Ed__46 : System.Collections.Generic.IEnumerator<object>, System.Collections.IEnumerator, System.IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ScryptoUtilsPad.Tools.CameraPage _003C_003E4__this;

			private float _003Cuntil_003E5__1;

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
			public _003CCaptureFramesRoutine_003Ed__46(int _003C_003E1__state)
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
					goto IL_0059;
				}
				_003C_003E1__state = -1;
				goto IL_0079;
				IL_0059:
				if (Time.time < _003Cuntil_003E5__1 && _003C_003E4__this._isRecording)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_0079;
				IL_0079:
				if (_003C_003E4__this._isRecording)
				{
					_003C_003E4__this.CaptureFrame();
					_003Cuntil_003E5__1 = Time.time + 1f / 30f;
					goto IL_0059;
				}
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
		private sealed class _003CSaveRecordingAsync_003Ed__48 : System.Runtime.CompilerServices.IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public System.Runtime.CompilerServices.AsyncTaskMethodBuilder _003C_003Et__builder;

			public ScryptoUtilsPad.Tools.CameraPage _003C_003E4__this;

			private ScryptoUtilsPad.Tools.CameraPage._003C_003Ec__DisplayClass48_0 _003C_003E8__1;

			private string _003CvideosDir_003E5__2;

			private System.Runtime.CompilerServices.TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				try
				{
					System.Runtime.CompilerServices.TaskAwaiter awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(System.Runtime.CompilerServices.TaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0155;
					}
					_003C_003E8__1 = new ScryptoUtilsPad.Tools.CameraPage._003C_003Ec__DisplayClass48_0();
					if (!string.IsNullOrEmpty(_003C_003E4__this._recordingTempDir) && System.IO.Directory.Exists(_003C_003E4__this._recordingTempDir))
					{
						_003CvideosDir_003E5__2 = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos);
						_003C_003E8__1.outputDir = System.IO.Path.Combine(_003CvideosDir_003E5__2, "ScryptoRecordings");
						System.IO.Directory.CreateDirectory(_003C_003E8__1.outputDir);
						_003C_003E8__1.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
						_003C_003E8__1.outputFile = System.IO.Path.Combine(_003C_003E8__1.outputDir, string.Concat("recording_", _003C_003E8__1.timestamp, ".mp4"));
						_003C_003E8__1.tempDir = _003C_003E4__this._recordingTempDir;
						awaiter = System.Threading.Tasks.Task.Run(new System.Action(_003C_003E8__1._003CSaveRecordingAsync_003Eb__0)).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							ScryptoUtilsPad.Tools.CameraPage._003CSaveRecordingAsync_003Ed__48 stateMachine = this;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
							return;
						}
						goto IL_0155;
					}
					goto end_IL_0007;
					IL_0155:
					awaiter.GetResult();
					end_IL_0007:;
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003C_003E8__1 = null;
					_003CvideosDir_003E5__2 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003C_003E8__1 = null;
				_003CvideosDir_003E5__2 = null;
				_003C_003Et__builder.SetResult();
			}

			void System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext()
			{
				this.MoveNext();
			}

			[System.Diagnostics.DebuggerHidden]
			private void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
			{
			}

			void System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
			{
				this.SetStateMachine(stateMachine);
			}
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003CUpdateTimerRoutine_003Ed__45 : System.Collections.Generic.IEnumerator<object>, System.Collections.IEnumerator, System.IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public ScryptoUtilsPad.Tools.CameraPage _003C_003E4__this;

			private float _003Celapsed_003E5__1;

			private int _003Cminutes_003E5__2;

			private int _003Cseconds_003E5__3;

			private float _003Cuntil_003E5__4;

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
			public _003CUpdateTimerRoutine_003Ed__45(int _003C_003E1__state)
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
					goto IL_00c7;
				}
				_003C_003E1__state = -1;
				goto IL_00e7;
				IL_00c7:
				if (Time.time < _003Cuntil_003E5__4 && _003C_003E4__this._isRecording)
				{
					_003C_003E2__current = null;
					_003C_003E1__state = 1;
					return true;
				}
				goto IL_00e7;
				IL_00e7:
				if (_003C_003E4__this._isRecording)
				{
					_003Celapsed_003E5__1 = Time.time - _003C_003E4__this._recordingStartTime;
					_003Cminutes_003E5__2 = (int)(_003Celapsed_003E5__1 / 60f);
					_003Cseconds_003E5__3 = (int)(_003Celapsed_003E5__1 % 60f);
					ScryptoUtilsPad.Core.MenuButton recordBtn = _003C_003E4__this._recordBtn;
					if (recordBtn != null)
					{
						recordBtn.SetLabel(string.Format("{0:00}:{1:00}", _003Cminutes_003E5__2, _003Cseconds_003E5__3));
					}
					_003Cuntil_003E5__4 = Time.time + 1f;
					goto IL_00c7;
				}
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

		public static ScryptoUtilsPad.Tools.CameraPage Instance;

		private const float MinFov = 10f;

		private const float MaxFov = 190f;

		private const float FovStep = 10f;

		private static readonly Color DefaultMat1 = new Color(0f, 1f, 0.667f);

		private static readonly Color DefaultMat2 = new Color(0.169f, 0.169f, 0.169f);

		private GameObject _menu;

		private GameObject _cameraPrefab;

		private GameObject _spawnedCamera;

		private Camera _actualCamera;

		private TMP_Text _fovText;

		private float _fov = 90f;

		private bool _isGrabbed;

		private Transform _holdingHand;

		private Vector3 _grabLocalPos;

		private Quaternion _grabLocalRot;

		private float _lastMoveSend;

		private const float MoveSendInterval = 0.05f;

		private bool _isRecording = false;

		private Renderer _recordBtnRenderer;

		private Color _recordBtnDefaultColor;

		private ScryptoUtilsPad.Core.MenuButton _recordBtn;

		private static readonly Color RecordingColor = new Color(1f, 8f / 51f, 8f / 51f);

		private float _recordingStartTime;

		private string _recordingTempDir;

		private int _frameCount;

		private Coroutine _timerCoroutine;

		private Coroutine _captureCoroutine;

		private Color _currentMat1 = DefaultMat1;

		private Color _currentMat2 = DefaultMat2;

		private void Awake()
		{
			Instance = this;
		}

		public void Init(GameObject menu, GameObject cameraPrefab)
		{
			_menu = menu;
			_cameraPrefab = cameraPrefab;
		}

		public void Open()
		{
		}

		private void Open_Disabled()
		{
			_menu.SetActive(false);
			if ((Object)(object)_spawnedCamera == (Object)null)
			{
				if (!((Object)(object)_cameraPrefab == (Object)null))
				{
					_spawnedCamera = Object.Instantiate<GameObject>(_cameraPrefab);
					Object.DontDestroyOnLoad((Object)(object)_spawnedCamera);
					PositionNearPlayer();
					ApplyVisuals();
					SetupCameraComponents();
					ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
					if (instance != null)
					{
						instance.SendOpen(_spawnedCamera.transform.position, _spawnedCamera.transform.rotation);
					}
				}
			}
			else
			{
				_spawnedCamera.SetActive(true);
			}
		}

		public void Close()
		{
			if ((Object)(object)_spawnedCamera != (Object)null)
			{
				_spawnedCamera.SetActive(false);
			}
			_menu.SetActive(true);
			ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
			if (instance != null)
			{
				instance.SendClose();
			}
		}

		public void StartGrab(Transform hand)
		{
			if (!((Object)(object)_spawnedCamera == (Object)null))
			{
				_holdingHand = hand;
				_grabLocalPos = hand.InverseTransformPoint(_spawnedCamera.transform.position);
				_grabLocalRot = Quaternion.Inverse(hand.rotation) * _spawnedCamera.transform.rotation;
				_isGrabbed = true;
				ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
				if (instance != null)
				{
					instance.SendGrab(_spawnedCamera.transform.position, _spawnedCamera.transform.rotation);
				}
			}
		}

		public void ReleaseGrab()
		{
			if ((Object)(object)_spawnedCamera != (Object)null)
			{
				ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
				if (instance != null)
				{
					instance.SendRelease(_spawnedCamera.transform.position, _spawnedCamera.transform.rotation);
				}
			}
			_holdingHand = null;
			_isGrabbed = false;
		}

		public void ApplyTheme(Color mat1, Color mat2)
		{
			if ((Object)(object)_spawnedCamera == (Object)null)
			{
				return;
			}
			Transform obj = _spawnedCamera.transform.Find("ActualCamera/Screen");
			Renderer val = ((obj != null) ? ((Component)obj).GetComponent<Renderer>() : null);
			Renderer[] componentsInChildren = _spawnedCamera.GetComponentsInChildren<Renderer>(true);
			int num = 0;
			while (num < componentsInChildren.Length)
			{
				Renderer val2 = componentsInChildren[num];
				if (!((Object)(object)val2 == (Object)(object)val))
				{
					Material[] materials = val2.materials;
					int num2 = 0;
					while (num2 < materials.Length)
					{
						Material val3 = materials[num2];
						if (val3.HasProperty("_Color"))
						{
							if (ColorClose(val3.color, _currentMat1, 0.05f))
							{
								val3.color = mat1;
							}
							else if (ColorClose(val3.color, _currentMat2, 0.05f))
							{
								val3.color = mat2;
							}
						}
						num2++;
					}
				}
				num++;
			}
			_currentMat1 = mat1;
			_currentMat2 = mat2;
		}

		private void LateUpdate()
		{
			if (!_isGrabbed || (Object)(object)_holdingHand == (Object)null || (Object)(object)_spawnedCamera == (Object)null)
			{
				return;
			}
			_spawnedCamera.transform.position = _holdingHand.TransformPoint(_grabLocalPos);
			_spawnedCamera.transform.rotation = _holdingHand.rotation * _grabLocalRot;
			if (Time.time - _lastMoveSend >= 0.05f)
			{
				_lastMoveSend = Time.time;
				ScryptoUtilsPad.Core.NetworkManager instance = ScryptoUtilsPad.Core.NetworkManager.Instance;
				if (instance != null)
				{
					instance.SendMove(_spawnedCamera.transform.position, _spawnedCamera.transform.rotation);
				}
			}
		}

		private void PositionNearPlayer()
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
			Transform val = (Transform)obj;
			if (!((Object)(object)val == (Object)null))
			{
				float num = (((Object)(object)VRRig.LocalRig != (Object)null) ? VRRig.LocalRig.scaleFactor : 1f);
				_spawnedCamera.transform.position = val.position + val.forward * (0.5f * num) + Vector3.up * (-0.05f * num);
				_spawnedCamera.transform.rotation = Quaternion.LookRotation(val.forward, Vector3.up);
				_spawnedCamera.transform.localScale = Vector3.one * 1.3f;
			}
		}

		private void ApplyVisuals()
		{
			Shader val = Shader.Find("GorillaTag/UberShader");
			Transform obj = _spawnedCamera.transform.Find("ActualCamera/Screen");
			Renderer val2 = ((obj != null) ? ((Component)obj).GetComponent<Renderer>() : null);
			if ((Object)(object)val != (Object)null)
			{
				Renderer[] componentsInChildren = _spawnedCamera.GetComponentsInChildren<Renderer>(true);
				int num = 0;
				while (num < componentsInChildren.Length)
				{
					Renderer val3 = componentsInChildren[num];
					if (!((Object)(object)val3 == (Object)(object)val2))
					{
						Material[] materials = val3.materials;
						int num2 = 0;
						while (num2 < materials.Length)
						{
							Material val4 = materials[num2];
							if (((Object)val4.shader).name != "UI/Default")
							{
								val4.shader = val;
							}
							num2++;
						}
					}
					num++;
				}
			}
			ScryptoUtilsPad.Plugin instance = ScryptoUtilsPad.Plugin.Instance;
			if ((Object)(object)((instance != null) ? instance.Font : null) != (Object)null)
			{
				TMP_Text[] componentsInChildren2 = _spawnedCamera.GetComponentsInChildren<TMP_Text>(true);
				int num3 = 0;
				while (num3 < componentsInChildren2.Length)
				{
					TMP_Text val5 = componentsInChildren2[num3];
					val5.font = ScryptoUtilsPad.Plugin.Instance.Font;
					num3++;
				}
			}
		}

		private void SetupCameraComponents()
		{
			Transform transform = _spawnedCamera.transform;
			Transform val = transform.Find("ActualCamera");
			if ((Object)(object)val == (Object)null)
			{
				return;
			}
			_actualCamera = ((Component)val).GetComponent<Camera>();
			if ((Object)(object)_actualCamera != (Object)null)
			{
				((Behaviour)_actualCamera).enabled = true;
				_actualCamera.fieldOfView = _fov;
				Camera actualCamera = _actualCamera;
				Camera main = Camera.main;
				actualCamera.cullingMask = ((main != null) ? main.cullingMask : (-1));
				_actualCamera.clearFlags = (CameraClearFlags)2;
				_actualCamera.backgroundColor = Color.black;
				_actualCamera.nearClipPlane = 0.01f;
				_actualCamera.farClipPlane = 1000f;
				Camera mainDepth = Camera.main;
				_actualCamera.depth = ((mainDepth != null) ? (mainDepth.depth + 10f) : 10f);
				_actualCamera.targetTexture = (RenderTexture)null;
				_actualCamera.stereoTargetEye = StereoTargetEyeMask.Both;
				Rect rect = default(Rect);
				rect.x = 0.7f;
				rect.y = 0.04f;
				rect.width = 0.28f;
				rect.height = 0.28f;
				_actualCamera.rect = rect;
				Transform val2 = val.Find("Screen");
				if ((Object)(object)val2 != (Object)null)
				{
					((Component)val2).gameObject.SetActive(false);
				}
			}
			Transform val5 = val.Find("TopBar");
			if ((Object)(object)val5 != (Object)null)
			{
				Transform val6 = val5.Find("FOVText");
				if ((Object)(object)val6 != (Object)null)
				{
					_fovText = ((Component)val6).GetComponent<TMP_Text>();
				}
				SetupButton(val5.Find("FOVPlus"), new System.Action(IncreaseFov), "FOV +");
				SetupButton(val5.Find("FOVNegative"), new System.Action(DecreaseFov), "FOV -");
			}
			Transform val7 = val.Find("RecordButton");
			_recordBtnRenderer = ((val7 != null) ? ((Component)val7).GetComponent<Renderer>() : null);
			if ((Object)(object)_recordBtnRenderer != (Object)null)
			{
				_recordBtnDefaultColor = _recordBtnRenderer.material.color;
			}
			if ((Object)(object)val7 != (Object)null)
			{
				_recordBtn = ((Component)val7).GetComponent<ScryptoUtilsPad.Core.MenuButton>() ?? ((Component)val7).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
				_recordBtn.OnPress = new System.Action(ToggleRecording);
				_recordBtn.SetLabel("● REC");
			}
			Transform val8 = val.Find("Handles");
			if ((Object)(object)val8 == (Object)null)
			{
				GameObject val9 = new GameObject("Handles");
				val9.transform.SetParent(val, false);
				val8 = val9.transform;
			}
			if ((Object)(object)((Component)val8).GetComponent<ScryptoUtilsPad.Core.CameraObject>() == (Object)null)
			{
				((Component)val8).gameObject.AddComponent<ScryptoUtilsPad.Core.CameraObject>();
			}
			UpdateFovText();
		}

		private void IncreaseFov()
		{
			_fov = Mathf.Clamp(_fov + 10f, 10f, 190f);
			if ((Object)(object)_actualCamera != (Object)null)
			{
				_actualCamera.fieldOfView = _fov;
			}
			UpdateFovText();
		}

		private void DecreaseFov()
		{
			_fov = Mathf.Clamp(_fov - 10f, 10f, 190f);
			if ((Object)(object)_actualCamera != (Object)null)
			{
				_actualCamera.fieldOfView = _fov;
			}
			UpdateFovText();
		}

		private void UpdateFovText()
		{
			if ((Object)(object)_fovText != (Object)null)
			{
				_fovText.text = string.Format("FOV: {0:0}", _fov);
			}
		}

		private void ToggleRecording()
		{
			ScryptoUtilsPad.Core.MenuButton recordBtn = _recordBtn;
			if (recordBtn != null)
			{
				recordBtn.SetLabel("REC N/A");
			}
		}

		private void ToggleRecording_Disabled()
		{
			if ((Object)(object)_recordBtnRenderer != (Object)null)
			{
				_recordBtnRenderer.material.color = (_isRecording ? RecordingColor : _recordBtnDefaultColor);
			}
			if (_isRecording)
			{
				_recordingStartTime = Time.time;
				_frameCount = 0;
				_recordingTempDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), string.Format("ScryptoRec_{0:yyyyMMdd_HHmmss}", System.DateTime.Now));
				System.IO.Directory.CreateDirectory(_recordingTempDir);
				_timerCoroutine = ((MonoBehaviour)this).StartCoroutine(UpdateTimerRoutine());
				_captureCoroutine = ((MonoBehaviour)this).StartCoroutine(CaptureFramesRoutine());
				return;
			}
			if (_timerCoroutine != null)
			{
				((MonoBehaviour)this).StopCoroutine(_timerCoroutine);
				_timerCoroutine = null;
			}
			if (_captureCoroutine != null)
			{
				((MonoBehaviour)this).StopCoroutine(_captureCoroutine);
				_captureCoroutine = null;
			}
			ScryptoUtilsPad.Core.MenuButton recordBtn = _recordBtn;
			if (recordBtn != null)
			{
				recordBtn.SetLabel("● REC");
			}
			SaveRecordingAsync();
		}

		[System.Runtime.CompilerServices.IteratorStateMachine(typeof(ScryptoUtilsPad.Tools.CameraPage._003CUpdateTimerRoutine_003Ed__45))]
		private System.Collections.IEnumerator UpdateTimerRoutine()
		{
			ScryptoUtilsPad.Tools.CameraPage._003CUpdateTimerRoutine_003Ed__45 obj = new ScryptoUtilsPad.Tools.CameraPage._003CUpdateTimerRoutine_003Ed__45(0);
			obj._003C_003E4__this = this;
			return obj;
		}

		[System.Runtime.CompilerServices.IteratorStateMachine(typeof(ScryptoUtilsPad.Tools.CameraPage._003CCaptureFramesRoutine_003Ed__46))]
		private System.Collections.IEnumerator CaptureFramesRoutine()
		{
			ScryptoUtilsPad.Tools.CameraPage._003CCaptureFramesRoutine_003Ed__46 obj = new ScryptoUtilsPad.Tools.CameraPage._003CCaptureFramesRoutine_003Ed__46(0);
			obj._003C_003E4__this = this;
			return obj;
		}

		private void CaptureFrame()
		{
			if (!((Object)(object)_actualCamera == (Object)null) && !((Object)(object)_actualCamera.targetTexture == (Object)null))
			{
				RenderTexture targetTexture = _actualCamera.targetTexture;
				Texture2D val = new Texture2D(((Texture)targetTexture).width, ((Texture)targetTexture).height, (TextureFormat)3, false);
				RenderTexture active = RenderTexture.active;
				RenderTexture.active = targetTexture;
				val.ReadPixels(new Rect(0f, 0f, (float)((Texture)targetTexture).width, (float)((Texture)targetTexture).height), 0, 0);
				val.Apply();
				RenderTexture.active = active;
				byte[] bytes = ImageConversion.EncodeToJPG(val, 85);
				Object.Destroy((Object)(object)val);
				string path = System.IO.Path.Combine(_recordingTempDir, string.Format("frame_{0:D6}.jpg", _frameCount));
				System.IO.File.WriteAllBytes(path, bytes);
				_frameCount++;
			}
		}

		[System.Runtime.CompilerServices.AsyncStateMachine(typeof(ScryptoUtilsPad.Tools.CameraPage._003CSaveRecordingAsync_003Ed__48))]
		[System.Diagnostics.DebuggerStepThrough]
		private System.Threading.Tasks.Task SaveRecordingAsync()
		{
			ScryptoUtilsPad.Tools.CameraPage._003CSaveRecordingAsync_003Ed__48 stateMachine = new ScryptoUtilsPad.Tools.CameraPage._003CSaveRecordingAsync_003Ed__48();
			stateMachine._003C_003Et__builder = System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create();
			stateMachine._003C_003E4__this = this;
			stateMachine._003C_003E1__state = -1;
			stateMachine._003C_003Et__builder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		private static string FindFfmpeg()
		{
			string[] array = new string[4];
			array[0] = "ffmpeg";
			array[1] = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles), "ffmpeg", "bin", "ffmpeg.exe");
			array[2] = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86), "ffmpeg", "bin", "ffmpeg.exe");
			array[3] = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "ffmpeg", "bin", "ffmpeg.exe");
			string[] array2 = array;
			string[] array3 = array2;
			int num = 0;
			while (num < array3.Length)
			{
				string text = array3[num];
				try
				{
					System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
					processStartInfo.FileName = text;
					processStartInfo.Arguments = "-version";
					processStartInfo.UseShellExecute = false;
					processStartInfo.CreateNoWindow = true;
					processStartInfo.RedirectStandardOutput = true;
					processStartInfo.RedirectStandardError = true;
					System.Diagnostics.ProcessStartInfo startInfo = processStartInfo;
					System.Diagnostics.Process process = System.Diagnostics.Process.Start(startInfo);
					try
					{
						if (process != null)
						{
							process.WaitForExit(2000);
						}
						if (process != null && process.ExitCode == 0)
						{
							return text;
						}
					}
					finally
					{
						if (process != null)
						{
							((System.IDisposable)process).Dispose();
						}
					}
				}
				catch
				{
				}
				num++;
			}
			return null;
		}

		private static void SetupButton(Transform tr, System.Action onPress, string label = null)
		{
			if (!((Object)(object)tr == (Object)null))
			{
				if ((Object)(object)((Component)tr).GetComponent<Collider>() == (Object)null)
				{
					BoxCollider box = ((Component)tr).gameObject.AddComponent<BoxCollider>();
					((Collider)box).isTrigger = true;
					Renderer rend = ((Component)tr).GetComponent<Renderer>();
					if ((Object)(object)rend != (Object)null)
					{
						MeshFilter mfb = ((Component)tr).GetComponent<MeshFilter>();
						Mesh meshb = ((mfb != null) ? mfb.sharedMesh : null);
						if ((Object)(object)meshb != (Object)null)
						{
							box.center = meshb.bounds.center;
							box.size = meshb.bounds.size;
						}
						else
						{
							box.size = new Vector3(1f, 1f, 0.2f);
						}
					}
					else
					{
						box.size = new Vector3(1f, 1f, 1f);
					}
				}
				ScryptoUtilsPad.Core.MenuButton menuButton = ((Component)tr).GetComponent<ScryptoUtilsPad.Core.MenuButton>() ?? ((Component)tr).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
				menuButton.OnPress = onPress;
				if (label != null)
				{
					menuButton.SetLabel(label);
				}
			}
		}

		private static bool ColorClose(Color a, Color b, float tol = 0.05f)
		{
			return Mathf.Abs(a.r - b.r) < tol && Mathf.Abs(a.g - b.g) < tol && Mathf.Abs(a.b - b.b) < tol;
		}
	}
}
