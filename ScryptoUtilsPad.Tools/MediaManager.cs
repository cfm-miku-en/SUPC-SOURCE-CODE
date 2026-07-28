namespace ScryptoUtilsPad.Tools
{
	public class MediaManager : MonoBehaviour
	{
		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003CUpdateDataAsync_003Ed__40 : System.Runtime.CompilerServices.IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public System.Runtime.CompilerServices.AsyncTaskMethodBuilder _003C_003Et__builder;

			private System.Diagnostics.ProcessStartInfo _003Cpsi_003E5__1;

			private System.Diagnostics.Process _003Cproc_003E5__2;

			private string _003Coutput_003E5__3;

			private string _003CprevAlbumArt_003E5__4;

			private string _003C_003Es__5;

			private System.Collections.Generic.Dictionary<string, object> _003Cdata_003E5__6;

			private byte[] _003CthumbBytes_003E5__7;

			private System.Security.Cryptography.MD5 _003Cmd5_003E5__8;

			private byte[] _003Chash_003E5__9;

			private string _003CnewHash_003E5__10;

			private System.Runtime.CompilerServices.TaskAwaiter<string> _003C_003Eu__1;

			private System.Runtime.CompilerServices.TaskAwaiter _003C_003Eu__2;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				try
				{
					if ((uint)num > 1u)
					{
						System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo();
						processStartInfo.FileName = QuickSongPath;
						processStartInfo.Arguments = "-all";
						processStartInfo.UseShellExecute = false;
						processStartInfo.RedirectStandardOutput = true;
						processStartInfo.CreateNoWindow = true;
						_003Cpsi_003E5__1 = processStartInfo;
						_003Cproc_003E5__2 = new System.Diagnostics.Process();
					}
					try
					{
						System.Runtime.CompilerServices.TaskAwaiter awaiter;
						System.Runtime.CompilerServices.TaskAwaiter<string> awaiter2;
						if (num != 0)
						{
							if (num == 1)
							{
								awaiter = _003C_003Eu__2;
								_003C_003Eu__2 = default(System.Runtime.CompilerServices.TaskAwaiter);
								num = (_003C_003E1__state = -1);
								goto IL_0171;
							}
							_003Cproc_003E5__2.StartInfo = _003Cpsi_003E5__1;
							_003Cproc_003E5__2.Start();
							awaiter2 = _003Cproc_003E5__2.StandardOutput.ReadToEndAsync().GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter2;
								ScryptoUtilsPad.Tools.MediaManager._003CUpdateDataAsync_003Ed__40 stateMachine = this;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
								return;
							}
						}
						else
						{
							awaiter2 = _003C_003Eu__1;
							_003C_003Eu__1 = default(System.Runtime.CompilerServices.TaskAwaiter<string>);
							num = (_003C_003E1__state = -1);
						}
						_003C_003Es__5 = awaiter2.GetResult();
						_003Coutput_003E5__3 = _003C_003Es__5;
						_003C_003Es__5 = null;
						awaiter = System.Threading.Tasks.Task.Run(new System.Action(_003Cproc_003E5__2.WaitForExit)).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__2 = awaiter;
							ScryptoUtilsPad.Tools.MediaManager._003CUpdateDataAsync_003Ed__40 stateMachine = this;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
							return;
						}
						goto IL_0171;
						IL_0171:
						awaiter.GetResult();
						_003CprevAlbumArt_003E5__4 = AlbumArt;
						ValidData = false;
						Paused = true;
						Title = "No Media";
						Artist = "Detected";
						AlbumArt = "InvalidArtHash";
						try
						{
							_003Cdata_003E5__6 = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.Dictionary<string, object>>(_003Coutput_003E5__3);
							Title = (string)_003Cdata_003E5__6["Title"];
							Artist = (string)_003Cdata_003E5__6["Artist"];
							Paused = (string)_003Cdata_003E5__6["Status"] != "Playing";
							if (_003Cdata_003E5__6.ContainsKey("ThumbnailBase64") && !string.IsNullOrEmpty((string)_003Cdata_003E5__6["ThumbnailBase64"]))
							{
								_003CthumbBytes_003E5__7 = System.Convert.FromBase64String((string)_003Cdata_003E5__6["ThumbnailBase64"]);
								_003Cmd5_003E5__8 = System.Security.Cryptography.MD5.Create();
								try
								{
									_003Chash_003E5__9 = _003Cmd5_003E5__8.ComputeHash(_003CthumbBytes_003E5__7);
									_003CnewHash_003E5__10 = System.BitConverter.ToString(_003Chash_003E5__9).Replace("-", "").ToLower();
									if (_003CnewHash_003E5__10 != _003CprevAlbumArt_003E5__4)
									{
										AlbumArt = _003CnewHash_003E5__10;
										ImageConversion.LoadImage(Icon, _003CthumbBytes_003E5__7);
									}
								}
								finally
								{
									if (num < 0 && _003Cmd5_003E5__8 != null)
									{
										((System.IDisposable)_003Cmd5_003E5__8).Dispose();
									}
								}
								_003CthumbBytes_003E5__7 = null;
								_003Cmd5_003E5__8 = null;
								_003Chash_003E5__9 = null;
								_003CnewHash_003E5__10 = null;
							}
							else
							{
								AlbumArt = "CouldNotGetArtHash [Err -01]";
							}
							ValidData = true;
							_003Cdata_003E5__6 = null;
						}
						catch
						{
						}
					}
					finally
					{
						if (num < 0 && _003Cproc_003E5__2 != null)
						{
							((System.IDisposable)_003Cproc_003E5__2).Dispose();
						}
					}
				}
				catch (System.Exception exception)
				{
					_003C_003E1__state = -2;
					_003Cpsi_003E5__1 = null;
					_003Cproc_003E5__2 = null;
					_003Coutput_003E5__3 = null;
					_003CprevAlbumArt_003E5__4 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003Cpsi_003E5__1 = null;
				_003Cproc_003E5__2 = null;
				_003Coutput_003E5__3 = null;
				_003CprevAlbumArt_003E5__4 = null;
				_003C_003Et__builder.SetResult();
			}

			void System.Runtime.CompilerServices.IAsyncStateMachine.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				this.MoveNext();
			}

			[System.Diagnostics.DebuggerHidden]
			private void SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
			{
			}

			void System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine(System.Runtime.CompilerServices.IAsyncStateMachine stateMachine)
			{
				//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
				this.SetStateMachine(stateMachine);
			}
		}

		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003CUpdateDataCoroutine_003Ed__41 : System.Collections.Generic.IEnumerator<object>, System.Collections.IEnumerator, System.IDisposable
		{
			private int _003C_003E1__state;

			private object _003C_003E2__current;

			public float delay;

			public ScryptoUtilsPad.Tools.MediaManager _003C_003E4__this;

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
			public _003CUpdateDataCoroutine_003Ed__41(int _003C_003E1__state)
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
				//IL_0032: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Expected O, but got Unknown
				switch (_003C_003E1__state)
				{
				default:
					return false;
				case 0:
					_003C_003E1__state = -1;
					_003C_003E2__current = (object)new WaitForSeconds(delay);
					_003C_003E1__state = 1;
					return true;
				case 1:
					_003C_003E1__state = -1;
					UpdateDataAsync();
					_003C_003E2__current = null;
					_003C_003E1__state = 2;
					return true;
				case 2:
					_003C_003E1__state = -1;
					return false;
				}
			}

			bool System.Collections.IEnumerator.MoveNext()
			{
				//ILSpy generated this explicit interface implementation from .override directive in MoveNext
				return this.MoveNext();
			}

			[System.Diagnostics.DebuggerHidden]
			void System.Collections.IEnumerator.Reset()
			{
				throw new System.NotSupportedException();
			}
		}

		private static float nextFullUpdate;

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static string _003CTitle_003Ek__BackingField = "Loading...";

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static string _003CArtist_003Ek__BackingField = "Loading...";

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static bool _003CPaused_003Ek__BackingField = true;

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static string _003CAlbumArt_003Ek__BackingField = "InvalidArtHash";

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static readonly Texture2D _003CIcon_003Ek__BackingField = new Texture2D(2, 2);

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static bool _003CValidData_003Ek__BackingField;

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static string _003CQuickSongPath_003Ek__BackingField;

		[System.Runtime.CompilerServices.CompilerGenerated]
		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		private static ScryptoUtilsPad.Tools.MediaManager _003CInstance_003Ek__BackingField;

		private const byte VK_MEDIA_NEXT_TRACK = 176;

		private const byte VK_MEDIA_PREV_TRACK = 177;

		private const byte VK_MEDIA_PLAY_PAUSE = 179;

		private const uint KEYEVENTF_KEYUP = 2u;

		public static string Title
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CTitle_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CTitle_003Ek__BackingField = value;
			}
		}

		public static string Artist
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CArtist_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CArtist_003Ek__BackingField = value;
			}
		}

		public static bool Paused
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CPaused_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CPaused_003Ek__BackingField = value;
			}
		}

		public static string AlbumArt
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CAlbumArt_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CAlbumArt_003Ek__BackingField = value;
			}
		}

		public static Texture2D Icon
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CIcon_003Ek__BackingField;
			}
		}

		public static bool ValidData
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CValidData_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CValidData_003Ek__BackingField = value;
			}
		}

		public static string QuickSongPath
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CQuickSongPath_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CQuickSongPath_003Ek__BackingField = value;
			}
		}

		public static ScryptoUtilsPad.Tools.MediaManager Instance
		{
			[System.Runtime.CompilerServices.CompilerGenerated]
			get
			{
				return _003CInstance_003Ek__BackingField;
			}
			[System.Runtime.CompilerServices.CompilerGenerated]
			private set
			{
				_003CInstance_003Ek__BackingField = value;
			}
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

		public void Awake()
		{
			Instance = this;
			QuickSongPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "QuickSong.exe");
			if (System.IO.File.Exists(QuickSongPath))
			{
				System.IO.File.Delete(QuickSongPath);
			}
			System.IO.Stream manifestResourceStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("ScryptoUtilsPad.Resources.QuickSong.exe");
			try
			{
				System.IO.FileStream fileStream = new System.IO.FileStream(QuickSongPath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
				try
				{
					manifestResourceStream.CopyTo(fileStream);
				}
				finally
				{
					if (fileStream != null)
					{
						((System.IDisposable)fileStream).Dispose();
					}
				}
			}
			finally
			{
				if (manifestResourceStream != null)
				{
					((System.IDisposable)manifestResourceStream).Dispose();
				}
			}
		}

		public void RequestNewRefresh(float delay)
		{
			((MonoBehaviour)this).StartCoroutine(UpdateDataCoroutine(delay));
		}

		public void Update()
		{
			if (!(Time.time < nextFullUpdate))
			{
				nextFullUpdate = Time.time + 3f;
				((MonoBehaviour)this).StartCoroutine(UpdateDataCoroutine(0f));
			}
		}

		[System.Runtime.CompilerServices.AsyncStateMachine(typeof(ScryptoUtilsPad.Tools.MediaManager._003CUpdateDataAsync_003Ed__40))]
		[System.Diagnostics.DebuggerStepThrough]
		private static System.Threading.Tasks.Task UpdateDataAsync()
		{
			ScryptoUtilsPad.Tools.MediaManager._003CUpdateDataAsync_003Ed__40 stateMachine = new ScryptoUtilsPad.Tools.MediaManager._003CUpdateDataAsync_003Ed__40();
			stateMachine._003C_003Et__builder = System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Create();
			stateMachine._003C_003E1__state = -1;
			stateMachine._003C_003Et__builder.Start(ref stateMachine);
			return stateMachine._003C_003Et__builder.Task;
		}

		[System.Runtime.CompilerServices.IteratorStateMachine(typeof(ScryptoUtilsPad.Tools.MediaManager._003CUpdateDataCoroutine_003Ed__41))]
		private System.Collections.IEnumerator UpdateDataCoroutine(float delay = 0f)
		{
			ScryptoUtilsPad.Tools.MediaManager._003CUpdateDataCoroutine_003Ed__41 obj = new ScryptoUtilsPad.Tools.MediaManager._003CUpdateDataCoroutine_003Ed__41(0);
			obj._003C_003E4__this = this;
			obj.delay = delay;
			return obj;
		}

		public static void PlayPause()
		{
			SendKey(179);
		}

		public static void NextTrack()
		{
			SendKey(176);
		}

		public static void PreviousTrack()
		{
			SendKey(177);
		}

		private static void SendKey(byte vk)
		{
			keybd_event(vk, 0, 0u, 0u);
			keybd_event(vk, 0, 2u, 0u);
		}
	}
}
