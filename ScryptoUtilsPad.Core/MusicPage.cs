namespace ScryptoUtilsPad.Core
{
	public class MusicPage : MonoBehaviour
	{
		private Transform _pageTr;

		private TMP_Text _songTitle;

		private TMP_Text _songArtist;

		private GameObject _pausedIcon;

		private Material _albumMat;

		private bool _albumColorSet;

		public void Init(Transform menuRoot)
		{
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Expected O, but got Unknown
			_pageTr = menuRoot.Find("MusicPage");
			if (!((Object)(object)_pageTr == (Object)null))
			{
				SetupButton(_pageTr, "Back", new System.Action(ScryptoUtilsPad.Tools.MediaManager.PreviousTrack));
				SetupButton(_pageTr, "Next", new System.Action(ScryptoUtilsPad.Tools.MediaManager.NextTrack));
				Transform val = FindChildByName(_pageTr, "Pause/Play");
				if ((Object)(object)val != (Object)null)
				{
					AddButtonAction(val, new System.Action(ScryptoUtilsPad.Tools.MediaManager.PlayPause));
				}
				Transform obj = _pageTr.Find("SongTitle");
				_songTitle = ((obj != null) ? ((Component)obj).GetComponent<TMP_Text>() : null);
				Transform obj2 = _pageTr.Find("ArtistName");
				_songArtist = ((obj2 != null) ? ((Component)obj2).GetComponent<TMP_Text>() : null);
				Transform obj3 = _pageTr.Find("PausedIcon");
				_pausedIcon = ((obj3 != null) ? ((Component)obj3).gameObject : null);
				Transform val2 = _pageTr.Find("Album");
				if ((Object)(object)val2 != (Object)null)
				{
					val2.localRotation = Quaternion.Euler(0f, 0f, 0f);
				}
				Renderer val3 = ((val2 != null) ? ((Component)val2).GetComponent<Renderer>() : null);
				if ((Object)(object)val3 != (Object)null)
				{
					_albumMat = new Material(val3.material);
					val3.material = _albumMat;
					_albumMat.shader = Shader.Find("Unlit/Texture");
					_albumMat.mainTexture = (Texture)(object)ScryptoUtilsPad.Tools.MediaManager.Icon;
				}
			}
		}

		private void Update()
		{
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)_pageTr == (Object)null) && ((Component)_pageTr).gameObject.activeInHierarchy)
			{
				if ((Object)(object)_songTitle != (Object)null && _songTitle.text != ScryptoUtilsPad.Tools.MediaManager.Title)
				{
					_songTitle.text = ScryptoUtilsPad.Tools.MediaManager.Title;
				}
				if ((Object)(object)_songArtist != (Object)null && _songArtist.text != ScryptoUtilsPad.Tools.MediaManager.Artist)
				{
					_songArtist.text = ScryptoUtilsPad.Tools.MediaManager.Artist;
				}
				if ((Object)(object)_pausedIcon != (Object)null)
				{
					_pausedIcon.SetActive(ScryptoUtilsPad.Tools.MediaManager.ValidData && ScryptoUtilsPad.Tools.MediaManager.Paused);
				}
				if ((Object)(object)_albumMat != (Object)null && ScryptoUtilsPad.Tools.MediaManager.ValidData && ((Texture)ScryptoUtilsPad.Tools.MediaManager.Icon).width > 2 && !_albumColorSet)
				{
					_albumMat.color = Color.white * 1.1f;
					_albumColorSet = true;
				}
			}
		}

		private static void SetupButton(Transform parent, string childName, System.Action action)
		{
			Transform val = parent.Find(childName);
			if ((Object)(object)val != (Object)null)
			{
				AddButtonAction(val, action);
			}
		}

		private static void AddButtonAction(Transform tr, System.Action action)
		{
			EnsureButtonCollider(tr);
			ScryptoUtilsPad.Core.MenuButton menuButton = ((Component)tr).GetComponent<ScryptoUtilsPad.Core.MenuButton>() ?? ((Component)tr).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
			menuButton.OnPress = action;
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


		private static Transform FindChildByName(Transform parent, string name)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			System.Collections.IEnumerator enumerator = parent.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform val = (Transform)enumerator.Current;
					if (((Object)val).name == name)
					{
						return val;
					}
				}
			}
			finally
			{
				System.IDisposable disposable = enumerator as System.IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			return null;
		}
	}
}
