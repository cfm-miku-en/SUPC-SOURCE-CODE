namespace ScryptoUtilsPad.Core
{
	public class TimePage : MonoBehaviour
	{
		[System.Serializable]
		[System.Runtime.CompilerServices.CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly ScryptoUtilsPad.Core.TimePage._003C_003Ec _003C_003E9 = new ScryptoUtilsPad.Core.TimePage._003C_003Ec();

			public static System.Action _003C_003E9__7_0;

			public static System.Action _003C_003E9__7_1;

			public static System.Action _003C_003E9__7_2;

			public static System.Action _003C_003E9__7_3;

			internal void _003CInit_003Eb__7_0()
			{
				SetTime(1);
			}

			internal void _003CInit_003Eb__7_1()
			{
				SetTime(3);
			}

			internal void _003CInit_003Eb__7_2()
			{
				SetTime(7);
			}

			internal void _003CInit_003Eb__7_3()
			{
				SetTime(0);
			}
		}

		public static ScryptoUtilsPad.Core.TimePage Instance;

		private const int MorningTime = 1;

		private const int DayTime = 3;

		private const int EveningTime = 7;

		private const int NightTime = 0;

		private bool _isRaining;

		public bool IsRaining
		{
			get
			{
				return _isRaining;
			}
		}

		private void Awake()
		{
			Instance = this;
		}

		public void Init(Transform menuRoot)
		{
			Transform val = menuRoot.Find("TimePage");
			if (!((Object)(object)val == (Object)null))
			{
				SetupButton(val, "Morning", ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9__7_0 ?? (ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9__7_0 = new System.Action(ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9._003CInit_003Eb__7_0)));
				SetupButton(val, "Day", ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9__7_1 ?? (ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9__7_1 = new System.Action(ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9._003CInit_003Eb__7_1)));
				SetupButton(val, "Evening", ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9__7_2 ?? (ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9__7_2 = new System.Action(ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9._003CInit_003Eb__7_2)));
				SetupButton(val, "Night", ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9__7_3 ?? (ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9__7_3 = new System.Action(ScryptoUtilsPad.Core.TimePage._003C_003Ec._003C_003E9._003CInit_003Eb__7_3)));
				SetupButton(val, "Rain", new System.Action(ToggleRain));
			}
		}

		private static void SetupButton(Transform page, string childName, System.Action onPress)
		{
			Transform val = page.Find(childName);
			if (!((Object)(object)val == (Object)null))
			{
				EnsureButtonCollider(val);
				ScryptoUtilsPad.Core.MenuButton menuButton = ((Component)val).GetComponent<ScryptoUtilsPad.Core.MenuButton>() ?? ((Component)val).gameObject.AddComponent<ScryptoUtilsPad.Core.MenuButton>();
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


		public static void SetTime(int hour)
		{
			if (!((Object)(object)BetterDayNightManager.instance == (Object)null))
			{
				((BetterDayNightManager)BetterDayNightManager.instance).SetTimeOfDay(hour);
			}
		}

		public void ToggleRain()
		{
			BetterDayNightManager instance = BetterDayNightManager.instance;
			if (((instance != null) ? ((BetterDayNightManager)instance).weatherCycle : null) != null)
			{
				_isRaining = !_isRaining;
				int num = 0;
				while (num < ((BetterDayNightManager)BetterDayNightManager.instance).weatherCycle.Length)
				{
					((BetterDayNightManager)BetterDayNightManager.instance).weatherCycle[num] = (WeatherType)(_isRaining ? 1 : 0);
					num++;
				}
			}
		}
	}
}
