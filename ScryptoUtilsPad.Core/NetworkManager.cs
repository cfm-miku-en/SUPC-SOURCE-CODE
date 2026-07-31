namespace ScryptoUtilsPad.Core
{
	public class NetworkManager : MonoBehaviour, IOnEventCallback, IInRoomCallbacks, IMatchmakingCallbacks
	{
		public static ScryptoUtilsPad.Core.NetworkManager Instance;

		private const byte EvOpen = 42;

		private const byte EvClose = 43;

		private const byte EvMove = 44;

		private const byte EvGrab = 45;

		private const byte EvRelease = 46;

		private const byte EvTheme = 47;

		private readonly System.Collections.Generic.Dictionary<int, ScryptoUtilsPad.Core.CameraObject> _remoteMenus = new System.Collections.Generic.Dictionary<int, ScryptoUtilsPad.Core.CameraObject>();

		private GameObject _emptyMenuPrefab;

		public void Init(GameObject emptyMenuPrefab)
		{
			_emptyMenuPrefab = emptyMenuPrefab;
		}

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			PhotonNetwork.AddCallbackTarget((object)this);
		}

		private float _sigSweep;

		private void Update()
		{
			ScryptoUtilsPad.Core.CheatSigGetter.Tick();
			if (Time.time - _sigSweep < 5f || !PhotonNetwork.InRoom)
			{
				return;
			}
			_sigSweep = Time.time;
			Player[] players = PhotonNetwork.PlayerListOthers;
			if (players != null)
			{
				for (int i = 0; i < players.Length; i++)
				{
					if (players[i] != null)
					{
						ScryptoUtilsPad.Core.CheatSigGetter.Observe(players[i].CustomProperties);
						ScryptoUtilsPad.Core.CheatDetector.Check(players[i]);
					}
				}
			}
		}

		private void OnDestroy()
		{
			PhotonNetwork.RemoveCallbackTarget((object)this);
		}

		public void SendOpen(Vector3 pos, Quaternion rot)
		{
			if (PhotonNetwork.InRoom)
			{
				object[] array = Pack(pos, rot);
				RaiseEventOptions val = new RaiseEventOptions();
				val.Receivers = (ReceiverGroup)0;
				PhotonNetwork.RaiseEvent((byte)42, (object)array, val, SendOptions.SendReliable);
			}
		}

		public void SendClose()
		{
			if (PhotonNetwork.InRoom)
			{
				RaiseEventOptions val = new RaiseEventOptions();
				val.Receivers = (ReceiverGroup)0;
				PhotonNetwork.RaiseEvent((byte)43, (object)null, val, SendOptions.SendReliable);
			}
		}

		public void SendMove(Vector3 pos, Quaternion rot)
		{
			if (PhotonNetwork.InRoom)
			{
				object[] array = Pack(pos, rot);
				RaiseEventOptions val = new RaiseEventOptions();
				val.Receivers = (ReceiverGroup)0;
				PhotonNetwork.RaiseEvent((byte)44, (object)array, val, SendOptions.SendUnreliable);
			}
		}

		public void SendGrab(Vector3 pos, Quaternion rot)
		{
			if (PhotonNetwork.InRoom)
			{
				object[] array = Pack(pos, rot);
				RaiseEventOptions val = new RaiseEventOptions();
				val.Receivers = (ReceiverGroup)0;
				PhotonNetwork.RaiseEvent((byte)45, (object)array, val, SendOptions.SendReliable);
			}
		}

		public void SendRelease(Vector3 pos, Quaternion rot)
		{
			if (PhotonNetwork.InRoom)
			{
				object[] array = Pack(pos, rot);
				RaiseEventOptions val = new RaiseEventOptions();
				val.Receivers = (ReceiverGroup)0;
				PhotonNetwork.RaiseEvent((byte)46, (object)array, val, SendOptions.SendReliable);
			}
		}

		public void SendTheme(Color mat1, Color mat2)
		{
			if (PhotonNetwork.InRoom)
			{
				object[] array = new object[6];
				array[0] = mat1.r;
				array[1] = mat1.g;
				array[2] = mat1.b;
				array[3] = mat2.r;
				array[4] = mat2.g;
				array[5] = mat2.b;
				object[] array2 = array;
				RaiseEventOptions val = new RaiseEventOptions();
				val.Receivers = (ReceiverGroup)0;
				PhotonNetwork.RaiseEvent((byte)47, (object)array2, val, SendOptions.SendReliable);
			}
		}

		public void OnEvent(EventData ev)
		{
			int sender = ev.Sender;
			switch (ev.Code)
			{
			case 42:
				if ((Object)(object)_emptyMenuPrefab != (Object)null)
				{
					RemoveMenu(sender);
					Vector3 pos;
					Quaternion rot;
					Unpack(ev.CustomData, out pos, out rot);
					GameObject val = Object.Instantiate<GameObject>(_emptyMenuPrefab, pos, rot);
					Object.DontDestroyOnLoad((Object)(object)val);
					ApplyVisuals(val);
					ScryptoUtilsPad.Core.CameraObject cameraObject = val.AddComponent<ScryptoUtilsPad.Core.CameraObject>();
					cameraObject.IsRemote = true;
					val.SetActive(true);
					_remoteMenus[sender] = cameraObject;
				}
				break;
			case 44:
			{
				ScryptoUtilsPad.Core.CameraObject value4;
				if (_remoteMenus.TryGetValue(sender, out value4) && (Object)(object)value4 != (Object)null)
				{
					Vector3 pos4;
					Quaternion rot4;
					Unpack(ev.CustomData, out pos4, out rot4);
					value4.SetTarget(pos4, rot4);
				}
				break;
			}
			case 45:
			{
				ScryptoUtilsPad.Core.CameraObject value2;
				if (_remoteMenus.TryGetValue(sender, out value2) && (Object)(object)value2 != (Object)null)
				{
					Vector3 pos2;
					Quaternion rot2;
					Unpack(ev.CustomData, out pos2, out rot2);
					value2.OnGrab(pos2, rot2);
				}
				break;
			}
			case 46:
			{
				ScryptoUtilsPad.Core.CameraObject value3;
				if (_remoteMenus.TryGetValue(sender, out value3) && (Object)(object)value3 != (Object)null)
				{
					Vector3 pos3;
					Quaternion rot3;
					Unpack(ev.CustomData, out pos3, out rot3);
					value3.OnRelease(pos3, rot3);
				}
				break;
			}
			case 47:
			{
				ScryptoUtilsPad.Core.CameraObject value;
				if (_remoteMenus.TryGetValue(sender, out value) && (Object)(object)value != (Object)null)
				{
					object[] array = (object[])ev.CustomData;
					Color mat = default(Color);
					mat = new Color((float)array[0], (float)array[1], (float)array[2]);
					Color mat2 = default(Color);
					mat2 = new Color((float)array[3], (float)array[4], (float)array[5]);
					value.ApplyTheme(mat, mat2);
				}
				break;
			}
			case 43:
				RemoveMenu(sender);
				break;
			}
		}

		public void OnPlayerLeftRoom(Player other)
		{
			RemoveMenu(other.ActorNumber);
		}

		public void OnLeftRoom()
		{
			RemoveAllMenus();
			ScryptoUtilsPad.Core.CheatDetector.Reset();
			ScryptoUtilsPad.Core.OwnerNametags.Reset();
			ScryptoUtilsPad.Core.DiscordRPC.SetPresence("In Menu", "SUPC");
		}

		public void OnPlayerEnteredRoom(Player newPlayer)
		{
		}

		public void OnRoomPropertiesUpdate(Hashtable props)
		{
		}

		public void OnPlayerPropertiesUpdate(Player p, Hashtable c)
		{
			ScryptoUtilsPad.Core.CheatSigGetter.Observe(c);
			ScryptoUtilsPad.Core.CheatDetector.Check(p);
		}

		public void OnMasterClientSwitched(Player newMaster)
		{
		}

		public void OnFriendListUpdate(System.Collections.Generic.List<FriendInfo> friendList)
		{
		}

		public void OnCreatedRoom()
		{
		}

		public void OnCreateRoomFailed(short code, string msg)
		{
		}

		public void OnJoinedRoom()
		{
			ScryptoUtilsPad.Core.CheatDetector.Reset();
			ScryptoUtilsPad.Core.OwnerNametags.Reset();
			ScryptoUtilsPad.Core.DiscordRPC.SetPresence("In a Lobby", "SUPC");
		}

		public void OnJoinRoomFailed(short code, string msg)
		{
		}

		public void OnJoinRandomFailed(short code, string msg)
		{
		}

		public void OnPreLeavingRoom()
		{
		}

		private void RemoveMenu(int actorNumber)
		{
			ScryptoUtilsPad.Core.CameraObject value;
			if (_remoteMenus.TryGetValue(actorNumber, out value))
			{
				if ((Object)(object)value != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)value).gameObject);
				}
				_remoteMenus.Remove(actorNumber);
			}
		}

		private void RemoveAllMenus()
		{
			System.Collections.Generic.Dictionary<int, ScryptoUtilsPad.Core.CameraObject>.ValueCollection.Enumerator enumerator = _remoteMenus.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ScryptoUtilsPad.Core.CameraObject current = enumerator.Current;
					if ((Object)(object)current != (Object)null)
					{
						Object.Destroy((Object)(object)((Component)current).gameObject);
					}
				}
			}
			finally
			{
				((System.IDisposable)enumerator).Dispose();
			}
			_remoteMenus.Clear();
		}

		private static void ApplyVisuals(GameObject go)
		{
			Shader val = Shader.Find("GorillaTag/UberShader");
			if ((Object)(object)val != (Object)null)
			{
				Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>(true);
				int num = 0;
				while (num < componentsInChildren.Length)
				{
					Renderer val2 = componentsInChildren[num];
					Material[] materials = val2.materials;
					int num2 = 0;
					while (num2 < materials.Length)
					{
						Material val3 = materials[num2];
						if (((Object)val3.shader).name != "UI/Default")
						{
							val3.shader = val;
						}
						num2++;
					}
					num++;
				}
			}
			ScryptoUtilsPad.Plugin instance = ScryptoUtilsPad.Plugin.Instance;
			TMP_FontAsset val4 = ((instance != null) ? instance.Font : null);
			if ((Object)(object)val4 != (Object)null)
			{
				TMP_Text[] componentsInChildren2 = go.GetComponentsInChildren<TMP_Text>(true);
				int num3 = 0;
				while (num3 < componentsInChildren2.Length)
				{
					TMP_Text val5 = componentsInChildren2[num3];
					val5.font = val4;
					num3++;
				}
			}
		}

		private static object[] Pack(Vector3 pos, Quaternion rot)
		{
			object[] array = new object[7];
			array[0] = pos.x;
			array[1] = pos.y;
			array[2] = pos.z;
			array[3] = rot.x;
			array[4] = rot.y;
			array[5] = rot.z;
			array[6] = rot.w;
			return array;
		}

		private static void Unpack(object data, out Vector3 pos, out Quaternion rot)
		{
			object[] array = (object[])data;
			pos = new Vector3((float)array[0], (float)array[1], (float)array[2]);
			rot = new Quaternion((float)array[3], (float)array[4], (float)array[5], (float)array[6]);
		}
	}
}
