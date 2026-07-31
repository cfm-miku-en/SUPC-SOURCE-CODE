using System.Collections;
using System.Collections.Generic;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
namespace ScryptoUtilsPad.Core
{
	public class NotificationManager : MonoBehaviour
	{
		public static ScryptoUtilsPad.Core.NotificationManager Instance;
		private GameObject _prefab;
		private GameObject _instance;

		private TMP_Text _text;

		private struct Entry
		{
			public string Message;
			public Color Color;
		}

		private readonly Queue<Entry> _queue = new Queue<Entry>();

		private bool _showing;

		private Vector3 _baseScale = Vector3.one;

		
		private static readonly Vector3 ViewOffset = new Vector3(-0.22f, -0.4f, 1f);

		
		private const float SizeScale = 0.65f;

		private const float DisplaySeconds = 5f;

		private const float GapSeconds = 0.35f;

		private static readonly ManualLogSource Log = Logger.CreateLogSource("NotificationManager");

		private void Awake()
		{
			Instance = this;
		}
		public void Init(GameObject notificationPrefab)
		{
			_prefab = notificationPrefab;
			if ((Object)(object)_prefab == (Object)null)
			{
				Log.LogWarning("[NotificationManager] No notification prefab in bundle — building a fallback.");
				BuildFallback();
				return;
			}
			_instance = Object.Instantiate<GameObject>(_prefab);
			Object.DontDestroyOnLoad(_instance);
			_baseScale = _instance.transform.localScale;
			Transform tr = _instance.transform.Find("NotificationText");
			_text = ((((Object)(object)tr != (Object)null) ? ((Component)tr).GetComponent<TMP_Text>() : null) ?? _instance.GetComponentInChildren<TMP_Text>(true));
			if ((Object)(object)_text == (Object)null)
			{
				Log.LogWarning("[NotificationManager] No 'NotificationText' TMP found in prefab.");
			}
			_instance.SetActive(false);
		}
		private void BuildFallback()
		{
			_instance = new GameObject("SUPCNotification");
			Object.DontDestroyOnLoad(_instance);
			TextMeshPro tmp = _instance.AddComponent<TextMeshPro>();
			_text = (TMP_Text)(object)tmp;
			_text.alignment = (TextAlignmentOptions)257;
			_text.fontSize = 1.1f;
			_text.enableAutoSizing = false;
			_text.richText = true;
			((Graphic)_text).color = Color.white;
			ScryptoUtilsPad.Plugin plugin = ScryptoUtilsPad.Plugin.Instance;
			if ((Object)(object)plugin != (Object)null && (Object)(object)plugin.Font != (Object)null)
			{
				_text.font = plugin.Font;
			}
			_text.rectTransform.sizeDelta = new Vector2(6f, 1.2f);
			_instance.SetActive(false);
		}
		public static void Notify(string message)
		{
			Notify(message, Color.white);
		}
		public static void Notify(string message, Color color)
		{
			ScryptoUtilsPad.Core.NotificationManager inst = Instance;
			if ((Object)(object)inst == (Object)null || string.IsNullOrEmpty(message))
			{
				return;
			}
			inst._queue.Enqueue(new Entry { Message = message, Color = color });
		}
		private void Update()
		{
			if (!_showing && _queue.Count > 0 && (Object)(object)_instance != (Object)null)
			{
				StartCoroutine(ShowNext());
			}
		}
		private IEnumerator ShowNext()
		{
			_showing = true;
			while (_queue.Count > 0)
			{
				Entry entry = _queue.Dequeue();
				if ((Object)(object)_text != (Object)null)
				{
					_text.text = entry.Message;
					((Graphic)_text).color = entry.Color;
					_text.alignment = (TextAlignmentOptions)514;
				}
				AttachToView();
				_instance.SetActive(true);
				yield return new WaitForSeconds(5f);
				_instance.SetActive(false);
				yield return new WaitForSeconds(0.35f);
			}
			_showing = false;
		}
		private void AttachToView()
		{
			GorillaTagger tagger = GorillaTagger.Instance;
			GameObject cam = ((tagger != null) ? tagger.mainCamera : null);
			Transform view = (((Object)(object)cam != (Object)null) ? cam.transform : null);
			if ((Object)(object)view == (Object)null)
			{
				Camera main = Camera.main;
				view = (((Object)(object)main != (Object)null) ? ((Component)main).transform : null);
			}
			if ((Object)(object)view == (Object)null)
			{
				return;
			}
			_instance.transform.SetParent(view, false);
			_instance.transform.localPosition = ViewOffset;
			_instance.transform.localRotation = Quaternion.identity;
			_instance.transform.localScale = _baseScale * SizeScale;
		}
	}
}
