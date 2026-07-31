using UnityEngine;
namespace ScryptoUtilsPad.Core
{
	public enum SelectMode
	{
		Leaderboard,
		Gun
	}
	public static class SelectionSettings
	{
		public static readonly string[] Names = new string[2] { "Leaderboard", "Gun" };

		private static int _index;
		public static SelectMode Mode
		{
			get
			{
				return (SelectMode)_index;
			}
		}
		public static int Index
		{
			get
			{
				return _index;
			}
			set
			{
				_index = ((value % Names.Length) + Names.Length) % Names.Length;
				PlayerPrefs.SetInt("ScryptoUtilsPad.SelectMode", _index);
			}
		}
		public static string CurrentName
		{
			get
			{
				return Names[_index];
			}
		}
		public static void Load()
		{
			_index = PlayerPrefs.GetInt("ScryptoUtilsPad.SelectMode", 0);
			if (_index < 0 || _index >= Names.Length)
			{
				_index = 0;
			}
		}
	}
	public class GunSelector : MonoBehaviour
	{
		private static readonly RaycastHit[] _rayBuffer = new RaycastHit[64];

		private static readonly BepInEx.Logging.ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("GunSelector");
		private LineRenderer _laser;

		private float _prevTrigger;

		private bool _loggedOnce;

		private GameObject _highlighter;

		private Material _highlightMat;

		private VRRig _selected;

		private void Update()
		{
			if (ScryptoUtilsPad.Core.SelectionSettings.Mode != ScryptoUtilsPad.Core.SelectMode.Gun || (!ScryptoUtilsPad.Core.PlayersPage.IsOpen && !ScryptoUtilsPad.Core.ModsPage.IsOpen))
			{
				SetLaserVisible(false);
				Highlight(null);
				_prevTrigger = 0f;
				_loggedOnce = false;
				return;
			}
			GorillaLocomotion.GTPlayer player = GorillaLocomotion.GTPlayer.Instance;
			if ((Object)(object)player == (Object)null)
			{
				SetLaserVisible(false);
				return;
			}
			Transform ctrl = player.RightHand.controllerTransform;
			if ((Object)(object)ctrl == (Object)null)
			{
				SetLaserVisible(false);
				return;
			}
			Vector3 origin = ctrl.TransformPoint(player.RightHand.handOffset);
			Quaternion aimRot = ctrl.rotation * player.RightHand.handRotOffset;
			Vector3 aimDir = aimRot * Vector3.forward;
			Vector3 endPoint = origin + aimDir * 200f;
			VRRig target = null;
			float bestDist = float.MaxValue;
			int worldMask = (int)player.locomotionEnabledLayers;
			int hitCount = Physics.RaycastNonAlloc(origin, aimDir, _rayBuffer, 200f);
			int i = 0;
			while (i < hitCount)
			{
				RaycastHit hit = _rayBuffer[i];
				i++;
				if (hit.distance >= bestDist)
				{
					continue;
				}
				Collider col = hit.collider;
				if ((Object)(object)col == (Object)null)
				{
					continue;
				}
				if ((worldMask & (1 << ((Component)col).gameObject.layer)) != 0)
				{
					bestDist = hit.distance;
					endPoint = hit.point;
					target = null;
					continue;
				}
				VRRig rig = ((Component)col).GetComponentInParent<VRRig>();
				if ((Object)(object)rig != (Object)null && !rig.isLocal && rig.creator != null)
				{
					bestDist = hit.distance;
					endPoint = hit.point;
					target = rig;
				}
			}
			if ((Object)(object)target == (Object)null)
			{
				target = FindByAngle(origin, aimDir);
			}
			DrawLaser(origin, endPoint, (Object)(object)target != (Object)null);
			Highlight((Object)(object)target != (Object)null ? target : _selected);
			if (!_loggedOnce)
			{
				_loggedOnce = true;
				Log.LogInfo("[GunSelector] Gun mode active — laser drawn, raycast running.");
			}
			ControllerInputPoller poller = ControllerInputPoller.instance;
			if ((Object)(object)poller == (Object)null)
			{
				return;
			}
			float trigger = poller.rightControllerIndexFloat;
			bool held = trigger > 0.5f;
			bool pressed = held && _prevTrigger <= 0.5f;
			_prevTrigger = trigger;
			if ((Object)(object)target == (Object)null)
			{
				return;
			}
			bool changed = (Object)(object)target != (Object)(object)_selected;
			if (!pressed && !(held && changed))
			{
				return;
			}
			ScryptoUtilsPad.Core.PlayersPage page = ScryptoUtilsPad.Core.PlayersPage.Instance;
			if ((Object)(object)page == (Object)null)
			{
				return;
			}
			page.SelectRig(target);
			_selected = target;
			ScryptoUtilsPad.Core.MenuManager menu = ScryptoUtilsPad.Core.MenuManager.Instance;
			if ((Object)(object)menu != (Object)null)
			{
				menu.OpenModsPage();
			}
			NetPlayer sel = target.creator;
			Log.LogInfo("[GunSelector] Selected " + ((sel != null) ? sel.SanitizedNickName : "?"));
		}
		private void EnsureLaser()
		{
			if ((Object)(object)_laser != (Object)null)
			{
				return;
			}
			GameObject go = new GameObject("SUPCGunLaser");
			Object.DontDestroyOnLoad(go);
			_laser = go.AddComponent<LineRenderer>();
			_laser.positionCount = 2;
			_laser.useWorldSpace = true;
			_laser.startWidth = 0.0125f;
			_laser.endWidth = 0.0125f;
			Material mat = ((Renderer)_laser).material;
			Shader uber = Shader.Find("GorillaTag/UberShader");
			if ((Object)(object)uber != (Object)null && (Object)(object)mat != (Object)null)
			{
				mat.shader = uber;
				mat.SetInt("_SrcBlend", 5);
				mat.SetInt("_DstBlend", 10);
				mat.SetInt("_SrcBlendAlpha", 1);
				mat.SetInt("_DstBlendAlpha", 10);
				mat.SetInt("_ZWrite", 0);
				mat.SetInt("_AlphaToMask", 0);
				mat.renderQueue = 3000;
			}
			if ((Object)(object)mat != (Object)null)
			{
				mat.color = ThemeTint(0.7f, 0.6f);
			}
		}
		private static Color ThemeTint(float alpha, float dim)
		{
			Color c = ScryptoUtilsPad.Core.SettingsPage.ThemeMat1;
			float max = Mathf.Max(c.r, Mathf.Max(c.g, c.b));
			if (max < 0.001f)
			{
				c = Color.white;
			}
			else if (max < 0.5f)
			{
				float boost = 0.5f / max;
				c = new Color(Mathf.Clamp01(c.r * boost), Mathf.Clamp01(c.g * boost), Mathf.Clamp01(c.b * boost));
			}
			return new Color(c.r * dim, c.g * dim, c.b * dim, alpha);
		}
		private void DrawLaser(Vector3 from, Vector3 to, bool onTarget)
		{
			EnsureLaser();
			if ((Object)(object)_laser == (Object)null)
			{
				return;
			}
			((Renderer)_laser).enabled = true;
			_laser.SetPosition(0, from);
			_laser.SetPosition(1, to);
			Material mat = ((Renderer)_laser).material;
			if ((Object)(object)mat != (Object)null)
			{
				mat.color = (onTarget ? ThemeTint(0.95f, 1f) : ThemeTint(0.7f, 0.6f));
			}
		}
		private void Highlight(VRRig rig)
		{
			if ((Object)(object)rig == (Object)null)
			{
				if ((Object)(object)_highlighter != (Object)null)
				{
					_highlighter.transform.SetParent((Transform)null);
					_highlighter.SetActive(false);
				}
				return;
			}
			if ((Object)(object)_highlighter == (Object)null)
			{
				_highlighter = GameObject.CreatePrimitive((PrimitiveType)0);
				Object.DontDestroyOnLoad(_highlighter);
				Collider hc = _highlighter.GetComponent<Collider>();
				if ((Object)(object)hc != (Object)null)
				{
					Object.Destroy((Object)(object)hc);
				}
				Renderer hr = _highlighter.GetComponent<Renderer>();
				if ((Object)(object)hr != (Object)null)
				{
					Material hm = hr.material;
					Shader uber = Shader.Find("GorillaTag/UberShader");
					if ((Object)(object)uber != (Object)null && (Object)(object)hm != (Object)null)
					{
						hm.shader = uber;
						hm.SetInt("_SrcBlend", 5);
						hm.SetInt("_DstBlend", 10);
						hm.SetInt("_SrcBlendAlpha", 1);
						hm.SetInt("_DstBlendAlpha", 10);
						hm.SetInt("_ZWrite", 0);
						hm.SetInt("_AlphaToMask", 0);
						hm.renderQueue = 3000;
					}
					_highlightMat = hm;
				}
				_highlighter.transform.localScale = Vector3.one * 0.5f;
			}
			if ((Object)(object)_highlightMat != (Object)null)
			{
				_highlightMat.color = ThemeTint(0.35f, 1f);
			}
			_highlighter.transform.SetParent(((Component)rig).transform);
			_highlighter.transform.localPosition = Vector3.zero;
			_highlighter.transform.localRotation = Quaternion.identity;
			_highlighter.SetActive(true);
		}
		private void SetLaserVisible(bool visible)
		{
			if ((Object)(object)_laser != (Object)null)
			{
				((Renderer)_laser).enabled = visible;
			}
		}
		private void OnDestroy()
		{
			if ((Object)(object)_laser != (Object)null)
			{
				Object.Destroy(((Component)_laser).gameObject);
			}
			if ((Object)(object)_highlighter != (Object)null)
			{
				Object.Destroy((Object)(object)_highlighter);
			}
		}
		private static VRRig FindByAngle(Vector3 origin, Vector3 dir)
		{
			VRRig best = null;
			float bestAngle = 12f;
			System.Collections.Generic.IReadOnlyList<VRRig> rigs = VRRigCache.ActiveRigs;
			if (rigs == null)
			{
				return null;
			}
			int i = 0;
			while (i < rigs.Count)
			{
				VRRig rig = rigs[i];
				i++;
				if ((Object)(object)rig == (Object)null || rig.isLocal || rig.creator == null)
				{
					continue;
				}
				GameObject headMesh = rig.headMesh;
				Transform t = (((Object)(object)headMesh != (Object)null) ? headMesh.transform : ((Component)rig).transform);
				Vector3 to = t.position - origin;
				float dist = to.magnitude;
				if (dist < 0.01f || dist > 200f)
				{
					continue;
				}
				float angle = Vector3.Angle(dir, to);
				if (angle < bestAngle)
				{
					bestAngle = angle;
					best = rig;
				}
			}
			return best;
		}
	}
}
