using UnityEngine.InputSystem;

namespace ScryptoUtilsPad.Core
{
	[DefaultExecutionOrder(1000)]
	public class DesktopMenu : MonoBehaviour
	{
		private bool _pcOpen;

		private void Update()
		{
			if (QPressed())
			{
				ScryptoUtilsPad.Core.PositionHandler instance = ScryptoUtilsPad.Core.PositionHandler.Instance;
				if (instance != null)
				{
					instance.SetMode(ScryptoUtilsPad.Core.PositionHandler.MenuMode.Float);
					instance.PcToggle();
					GameObject checker = ScryptoUtilsPad.Core.PositionHandler.Checker;
					_pcOpen = (Object)(object)checker != (Object)null && checker.activeSelf;
				}
			}
			if (ClickPressed(out Vector2 pos))
			{
				TryClick(pos);
			}
		}

		private void LateUpdate()
		{
			if (!_pcOpen)
			{
				return;
			}
			GameObject checker = ScryptoUtilsPad.Core.PositionHandler.Checker;
			if ((Object)(object)checker == (Object)null || !checker.activeSelf)
			{
				_pcOpen = false;
				return;
			}
			Camera cam = GetScreenCamera();
			if ((Object)(object)cam == (Object)null)
			{
				return;
			}
			Transform ct = ((Component)cam).transform;
			Vector3 fwd = ct.forward;
			checker.transform.position = ct.position + fwd * 0.5f;
			checker.transform.rotation = Quaternion.LookRotation(fwd, ct.up);
		}

		private static Camera GetScreenCamera()
		{
			Camera best = null;
			Camera[] cams = Camera.allCameras;
			int i = 0;
			while (i < cams.Length)
			{
				Camera c = cams[i];
				if ((Object)(object)c != (Object)null && c.isActiveAndEnabled && (Object)(object)c.targetTexture == (Object)null && c.targetDisplay == 0)
				{
					if ((Object)(object)best == (Object)null || c.depth > best.depth)
					{
						best = c;
					}
				}
				i++;
			}
			if ((Object)(object)best != (Object)null)
			{
				return best;
			}
			if ((Object)(object)Camera.main != (Object)null)
			{
				return Camera.main;
			}
			GorillaTagger tagger = (GorillaTagger.hasInstance ? GorillaTagger.Instance : null);
			GameObject mc = (((Object)(object)tagger != (Object)null) ? tagger.mainCamera : null);
			return ((Object)(object)mc != (Object)null) ? mc.GetComponent<Camera>() : null;
		}

		private static bool QPressed()
		{
			Keyboard kb = Keyboard.current;
			if (kb != null)
			{
				return kb.qKey.wasPressedThisFrame;
			}
			return Input.GetKeyDown(KeyCode.Q);
		}

		private static bool ClickPressed(out Vector2 pos)
		{
			Mouse mouse = Mouse.current;
			if (mouse != null)
			{
				pos = mouse.position.ReadValue();
				return mouse.leftButton.wasPressedThisFrame;
			}
			pos = Input.mousePosition;
			return Input.GetMouseButtonDown(0);
		}

		private static void TryClick(Vector2 screenPos)
		{
			Camera cam = GetScreenCamera();
			if ((Object)(object)cam == (Object)null)
			{
				return;
			}
			Ray ray = cam.ScreenPointToRay(new Vector3(screenPos.x, screenPos.y, 0f));
			RaycastHit[] hits = Physics.RaycastAll(ray, 200f, -1, (QueryTriggerInteraction)2);
			if (hits.Length == 0)
			{
				return;
			}
			System.Array.Sort(hits, (RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance));
			int i = 0;
			while (i < hits.Length)
			{
				ScryptoUtilsPad.Core.MenuButton button = ((Component)hits[i].collider).GetComponentInParent<ScryptoUtilsPad.Core.MenuButton>();
				if ((Object)(object)button != (Object)null && button.OnPress != null)
				{
					button.OnPress();
					PlayClick();
					return;
				}
				i++;
			}
		}

		private static void PlayClick()
		{
			ScryptoUtilsPad.Plugin plugin = ScryptoUtilsPad.Plugin.Instance;
			if (plugin == null || (Object)(object)plugin.ButtonClickSound == (Object)null || !GorillaTagger.hasInstance)
			{
				return;
			}
			GorillaTagger tagger = GorillaTagger.Instance;
			if ((Object)(object)tagger == (Object)null || (Object)(object)tagger.offlineVRRig == (Object)null)
			{
				return;
			}
			AudioSource src = tagger.offlineVRRig.leftHandPlayer;
			if ((Object)(object)src != (Object)null)
			{
				GTAudioSourceExtensions.GTPlayOneShot(src, plugin.ButtonClickSound, 2f);
			}
		}
	}
}
