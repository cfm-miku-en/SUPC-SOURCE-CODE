/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using UnityEngine;

namespace ScryptoUtilsPad.Core
{
	public class DesktopCamera : MonoBehaviour
	{
		public static ScryptoUtilsPad.Core.DesktopCamera Instance;

		private static readonly BepInEx.Logging.ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("DesktopCamera");

		public static bool FirstPerson;

		public static int Fov = 90;

		public static int Smooth = 5;

		private Camera _shoulder;

		private Camera _own;

		private GameObject _ownGo;

		private Vector3 _pos;

		private Quaternion _rot;

		private bool _init;

		private float _nextLog;

		private void Awake()
		{
			Instance = this;
		}

		public static void LoadPrefs()
		{
			FirstPerson = PlayerPrefs.GetInt("ScryptoUtilsPad.DesktopFirstPerson", 0) == 1;
			Fov = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.DesktopFov", 90), 30, 140);
			Smooth = Mathf.Clamp(PlayerPrefs.GetInt("ScryptoUtilsPad.DesktopSmooth", 5), 0, 10);
		}

		public static void Save()
		{
			PlayerPrefs.SetInt("ScryptoUtilsPad.DesktopFirstPerson", FirstPerson ? 1 : 0);
			PlayerPrefs.SetInt("ScryptoUtilsPad.DesktopFov", Fov);
			PlayerPrefs.SetInt("ScryptoUtilsPad.DesktopSmooth", Smooth);
			PlayerPrefs.Save();
		}

		public static void LogState(string where)
		{
			Log.LogInfo("[DesktopCamera] " + where + " FirstPerson=" + FirstPerson + " Fov=" + Fov + " Smooth=" + Smooth);
		}

		private bool FindShoulder()
		{
			if ((Object)(object)_shoulder != (Object)null)
			{
				return true;
			}
			if (!GorillaTagger.hasInstance)
			{
				return false;
			}
			GorillaTagger tagger = GorillaTagger.Instance;
			GameObject tpc = (((Object)(object)tagger != (Object)null) ? tagger.thirdPersonCamera : null);
			if ((Object)(object)tpc == (Object)null)
			{
				return false;
			}
			_shoulder = tpc.GetComponentInChildren<Camera>();
			return (Object)(object)_shoulder != (Object)null;
		}

		private void BuildOwnCamera()
		{
			if ((Object)(object)_ownGo != (Object)null)
			{
				return;
			}
			GorillaTagger tagger = GorillaTagger.Instance;
			GameObject head = (((Object)(object)tagger != (Object)null) ? tagger.mainCamera : null);
			if ((Object)(object)head == (Object)null || (Object)(object)_shoulder == (Object)null)
			{
				return;
			}
			_ownGo = new GameObject("SUPCDesktopCam");
			_own = _ownGo.AddComponent<Camera>();
			_own.CopyFrom(_shoulder);
			_own.stereoTargetEye = StereoTargetEyeMask.None;
			_own.targetTexture = (RenderTexture)null;
			_own.depth = _shoulder.depth + 5f;
			_own.fieldOfView = Fov;
			((Behaviour)_shoulder).enabled = false;
			Object.DontDestroyOnLoad(_ownGo);
			Log.LogInfo("[DesktopCamera] Built own desktop camera (depth " + _own.depth.ToString("0") + "); shoulder cam disabled.");
		}

		private void TearDownOwnCamera()
		{
			if ((Object)(object)_ownGo != (Object)null)
			{
				Object.Destroy(_ownGo);
				_ownGo = null;
				_own = null;
			}
			if ((Object)(object)_shoulder != (Object)null)
			{
				((Behaviour)_shoulder).enabled = true;
			}
			_init = false;
		}

		private void LateUpdate()
		{
			if (!FindShoulder())
			{
				return;
			}
			if (!FirstPerson)
			{
				if ((Object)(object)_ownGo != (Object)null)
				{
					TearDownOwnCamera();
					Log.LogInfo("[DesktopCamera] First person off; shoulder cam restored.");
				}
				return;
			}
			BuildOwnCamera();
			if ((Object)(object)_own == (Object)null)
			{
				return;
			}
			GorillaTagger tagger = GorillaTagger.Instance;
			GameObject head = (((Object)(object)tagger != (Object)null) ? tagger.mainCamera : null);
			Transform headTr = (((Object)(object)head != (Object)null) ? head.transform : null);
			if ((Object)(object)headTr == (Object)null)
			{
				return;
			}
			if (!_init)
			{
				_init = true;
				_pos = headTr.position;
				_rot = headTr.rotation;
			}
			if (Smooth <= 0)
			{
				_pos = headTr.position;
				_rot = headTr.rotation;
			}
			else
			{
				float t = Mathf.Clamp01(Time.deltaTime * (22f - (float)Smooth * 1.8f));
				_pos = Vector3.Lerp(_pos, headTr.position, t);
				_rot = Quaternion.Slerp(_rot, headTr.rotation, t);
			}
			_ownGo.transform.position = _pos;
			_ownGo.transform.rotation = _rot;
			_own.stereoTargetEye = StereoTargetEyeMask.None;
			_own.fieldOfView = (float)Fov;
			if (Time.time >= _nextLog)
			{
				_nextLog = Time.time + 5f;
				Log.LogInfo("[DesktopCamera] state fov=" + _own.fieldOfView.ToString("0")
					+ " wantFov=" + Fov
					+ " eye=" + _own.stereoTargetEye.ToString()
					+ " depth=" + _own.depth.ToString("0")
					+ " display=" + _own.targetDisplay
					+ " shoulderEnabled=" + ((Behaviour)_shoulder).enabled);
			}
		}

		private void OnDestroy()
		{
			TearDownOwnCamera();
		}
	}
}
