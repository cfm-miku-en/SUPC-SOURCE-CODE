namespace ScryptoUtilsPad.Core
{
	public class RoomPage : MonoBehaviour
	{
		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003COnJoinPublic_003Ed__11 : System.Runtime.CompilerServices.IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public System.Runtime.CompilerServices.AsyncVoidMethodBuilder _003C_003Et__builder;

			private string _003Czone_003E5__1;

			private GorillaNetworkJoinTrigger _003Ctrigger_003E5__2;

			private System.Reflection.MethodInfo _003Cmethod_003E5__3;

			private System.Reflection.ParameterInfo[] _003Cparms_003E5__4;

			private object[] _003Cargs_003E5__5;

			private System.Reflection.MethodInfo[] _003C_003Es__6;

			private int _003C_003Es__7;

			private System.Reflection.MethodInfo _003Cm_003E5__8;

			private System.Reflection.ParameterInfo[] _003Cp_003E5__9;

			private int _003Ci_003E5__10;

			private System.Type _003Cpt_003E5__11;

			private System.Exception _003Ce_003E5__12;

			private System.Runtime.CompilerServices.TaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				try
				{
					if (num != 0)
					{
					}
					try
					{
						System.Runtime.CompilerServices.TaskAwaiter awaiter;
						if (num != 0)
						{
							if (!NetworkSystem.Instance.InRoom)
							{
								goto IL_008a;
							}
							awaiter = NetworkSystem.Instance.ReturnToSinglePlayer().GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								ScryptoUtilsPad.Core.RoomPage._003COnJoinPublic_003Ed__11 stateMachine = this;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
								return;
							}
						}
						else
						{
							awaiter = _003C_003Eu__1;
							_003C_003Eu__1 = default(System.Runtime.CompilerServices.TaskAwaiter);
							num = (_003C_003E1__state = -1);
						}
						awaiter.GetResult();
						goto IL_008a;
						IL_008a:
						GorillaNetworkJoinTrigger currentJoinTrigger = ((PhotonNetworkController)PhotonNetworkController.Instance).currentJoinTrigger;
						_003Czone_003E5__1 = ((currentJoinTrigger != null) ? currentJoinTrigger.networkZone : null) ?? "forest";
						_003Ctrigger_003E5__2 = ((GorillaComputer)GorillaComputer.instance).GetJoinTriggerForZone(_003Czone_003E5__1);
						_003Cmethod_003E5__3 = null;
						_003C_003Es__6 = typeof(PhotonNetworkController).GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
						_003C_003Es__7 = 0;
						while (_003C_003Es__7 < _003C_003Es__6.Length)
						{
							_003Cm_003E5__8 = _003C_003Es__6[_003C_003Es__7];
							if (!(_003Cm_003E5__8.Name != "AttemptToJoinPublicRoom"))
							{
								_003Cp_003E5__9 = _003Cm_003E5__8.GetParameters();
								if (_003Cp_003E5__9.Length != 0 && _003Cp_003E5__9[0].ParameterType == typeof(GorillaNetworkJoinTrigger))
								{
									_003Cmethod_003E5__3 = _003Cm_003E5__8;
									break;
								}
								_003Cp_003E5__9 = null;
								_003Cm_003E5__8 = null;
							}
							_003C_003Es__7++;
						}
						_003C_003Es__6 = null;
						if (_003Cmethod_003E5__3 == null)
						{
							Log.LogWarning((object)"[RoomPage] AttemptToJoinPublicRoom not found");
						}
						else
						{
							_003Cparms_003E5__4 = _003Cmethod_003E5__3.GetParameters();
							_003Cargs_003E5__5 = new object[_003Cparms_003E5__4.Length];
							_003Cargs_003E5__5[0] = _003Ctrigger_003E5__2;
							_003Ci_003E5__10 = 1;
							while (_003Ci_003E5__10 < _003Cparms_003E5__4.Length)
							{
								_003Cpt_003E5__11 = _003Cparms_003E5__4[_003Ci_003E5__10].ParameterType;
								_003Cargs_003E5__5[_003Ci_003E5__10] = (_003Cparms_003E5__4[_003Ci_003E5__10].HasDefaultValue ? _003Cparms_003E5__4[_003Ci_003E5__10].DefaultValue : (_003Cpt_003E5__11.IsEnum ? System.Enum.GetValues(_003Cpt_003E5__11).GetValue(0) : System.Activator.CreateInstance(_003Cpt_003E5__11)));
								_003Cpt_003E5__11 = null;
								_003Ci_003E5__10++;
							}
							_003Cmethod_003E5__3.Invoke(PhotonNetworkController.Instance, _003Cargs_003E5__5);
							_003Czone_003E5__1 = null;
							_003Ctrigger_003E5__2 = null;
							_003Cmethod_003E5__3 = null;
							_003Cparms_003E5__4 = null;
							_003Cargs_003E5__5 = null;
						}
					}
					catch (System.Exception ex)
					{
						_003Ce_003E5__12 = ex;
						Debug.LogException(_003Ce_003E5__12);
					}
				}
				catch (System.Exception ex)
				{
					_003C_003E1__state = -2;
					_003C_003Et__builder.SetException(ex);
					return;
				}
				_003C_003E1__state = -2;
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

		public static ScryptoUtilsPad.Core.RoomPage Instance;

		private Transform _pageTr;

		private TMP_Text _infoText;

		private bool _showRoomCode = true;

		private float _infoRefreshTimer;

		private string _lastInfoText;

		private static readonly ManualLogSource Log = Logger.CreateLogSource("RoomPage");

		private void Awake()
		{
			Instance = this;
		}

		public void Init(Transform menuRoot)
		{
			_pageTr = menuRoot.Find("RoomPage");
			if ((Object)(object)_pageTr == (Object)null)
			{
				Log.LogWarning((object)"[RoomPage] RoomPage not found in menu hierarchy");
				return;
			}
			Transform obj = _pageTr.Find("Info");
			_infoText = ((obj != null) ? ((Component)obj).GetComponent<TMP_Text>() : null);
			SetupButton(_pageTr.Find("Disconnect"), new System.Action(OnDisconnect));
			SetupButton(_pageTr.Find("JoinPublic"), new System.Action(OnJoinPublic));
			SetupButton(_pageTr.Find("ShowCode"), new System.Action(ToggleRoomCode));
		}

		private void Update()
		{
			if ((Object)(object)_pageTr == (Object)null || !((Component)_pageTr).gameObject.activeInHierarchy || (Object)(object)_infoText == (Object)null)
			{
				return;
			}
			_infoRefreshTimer -= Time.deltaTime;
			if (!(_infoRefreshTimer > 0f))
			{
				_infoRefreshTimer = 1f;
				object obj;
				if (!NetworkSystem.Instance.InRoom)
				{
					obj = "NOT IN A ROOM";
				}
				else
				{
					string[] array = new string[8];
					array[0] = "CODE: ";
					array[1] = (_showRoomCode ? NetworkSystem.Instance.RoomName : "------");
					array[2] = "\n";
					array[3] = string.Format("PLAYERS: {0}\n", NetworkSystem.Instance.RoomPlayerCount);
					array[4] = "GAMEMODE: ";
					array[5] = FormatGamemode(NetworkSystem.Instance.GameModeString);
					array[6] = "\nQUEUE: ";
					array[7] = FormatQueue(NetworkSystem.Instance.GameModeString);
					obj = string.Concat(array);
				}
				string text = (string)obj;
				if (text != _lastInfoText)
				{
					_lastInfoText = text;
					_infoText.text = text;
				}
			}
		}

		private static void OnDisconnect()
		{
			if (NetworkSystem.Instance.InRoom)
			{
				NetworkSystem.Instance.ReturnToSinglePlayer();
			}
		}

		[System.Runtime.CompilerServices.AsyncStateMachine(typeof(ScryptoUtilsPad.Core.RoomPage._003COnJoinPublic_003Ed__11))]
		[System.Diagnostics.DebuggerStepThrough]
		private static void OnJoinPublic()
		{
			ScryptoUtilsPad.Core.RoomPage._003COnJoinPublic_003Ed__11 stateMachine = new ScryptoUtilsPad.Core.RoomPage._003COnJoinPublic_003Ed__11();
			stateMachine._003C_003Et__builder = System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Create();
			stateMachine._003C_003E1__state = -1;
			stateMachine._003C_003Et__builder.Start(ref stateMachine);
		}

		private void ToggleRoomCode()
		{
			_showRoomCode = !_showRoomCode;
			_infoRefreshTimer = 0f;
		}

		private static string FormatGamemode(string gamemode)
		{
			string text = gamemode.ToLower();
			if (text.Contains("super"))
			{
				return "SUPER INFECTION";
			}
			if (text.Contains("infection"))
			{
				return "INFECTION";
			}
			if (text.Contains("casual"))
			{
				return "CASUAL";
			}
			if (text.Contains("paint"))
			{
				return "PAINT-BRAWL";
			}
			if (text.Contains("guard"))
			{
				return "GUARDIAN";
			}
			if (text.Contains("hunt"))
			{
				return "HUNT";
			}
			if (text.Contains("ghost"))
			{
				return "GHOST TAG";
			}
			if (text.Contains("freeze"))
			{
				return "FREEZE TAG";
			}
			return gamemode;
		}

		private static string FormatQueue(string gamemode)
		{
			string text = gamemode.ToLower();
			if (text.Contains("default"))
			{
				return "DEFAULT";
			}
			if (text.Contains("minig"))
			{
				return "MINIGAMES";
			}
			if (text.Contains("comp"))
			{
				return "COMPETITIVE";
			}
			return gamemode;
		}

		private static void SetupButton(Transform tr, System.Action onPress)
		{
			if (!((Object)(object)tr == (Object)null))
			{
				EnsureButtonCollider(tr);
				ScryptoUtilsPad.Core.MenuButton menuButton = ((Component)tr).GetComponent<ScryptoUtilsPad.Core.MenuButton>() ?? ((Component)tr).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
				menuButton.OnPress = onPress;
			}
		}

		private static void EnsureButtonCollider(Transform tr)
		{
			if ((Object)(object)tr == (Object)null || (Object)(object)((Component)tr).GetComponent<Collider>() != (Object)null)
			{
				return;
			}
			BoxCollider box = ((Component)tr).gameObject.AddComponent<BoxCollider>();
			((Collider)box).isTrigger = true;
			MeshFilter mf = ((Component)tr).GetComponent<MeshFilter>();
			Mesh mesh = ((mf != null) ? mf.sharedMesh : null);
			if ((Object)(object)mesh != (Object)null)
			{
				Bounds lb = mesh.bounds;
				box.center = lb.center;
				Vector3 sz = lb.size;
				box.size = new Vector3(Mathf.Abs(sz.x), Mathf.Abs(sz.y), Mathf.Max(Mathf.Abs(sz.z), 0.05f));
			}
			else
			{
				box.size = new Vector3(1f, 1f, 0.2f);
			}
		}
	}
}
