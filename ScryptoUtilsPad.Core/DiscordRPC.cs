/*
 * SUPC - Scrypto Utils Pad Continued
 * Copyright (C) 2026 cfm-miku-en. Based on Scrypto Utils Pad (C) low, used with permission.
 * Licensed under the GNU General Public License v3.0 or later. See LICENSE.
 */

using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using BepInEx.Logging;
using UnityEngine;

namespace ScryptoUtilsPad.Core
{
	public class DiscordRPC : MonoBehaviour
	{
		private const string ClientId = "1527098470136152174";

		private static readonly ManualLogSource Log = Logger.CreateLogSource("DiscordRPC");

		public static DiscordRPC Instance;

		private NamedPipeClientStream _pipe;
		private Thread _thread;
		private volatile bool _running;
		private volatile bool _connected;
		private volatile bool _dirty;
		private string _pendingDetails = "In Menu";
		private string _pendingState = "SUPC";
		private readonly object _lock = new object();
		private readonly long _startTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

		private void Awake()
		{
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			_running = true;
			_thread = new Thread(RunLoop);
			_thread.IsBackground = true;
			_thread.Start();
		}

		private void OnDestroy()
		{
			_running = false;
			try
			{
				if (_thread != null)
				{
					_thread.Interrupt();
				}
			}
			catch
			{
			}
			Disconnect();
		}

		public static void SetPresence(string details, string state = null)
		{
			if (Instance == null)
			{
				return;
			}
			lock (Instance._lock)
			{
				Instance._pendingDetails = details;
				Instance._pendingState = string.IsNullOrEmpty(state) ? "SUPC" : state;
				Instance._dirty = true;
			}
		}

		private void RunLoop()
		{
			while (_running)
			{
				try
				{
					if (!_connected)
					{
						Connect();
						SendActivity();
					}
					if (_dirty)
					{
						SendActivity();
					}
					Thread.Sleep(2000);
				}
				catch (ThreadInterruptedException)
				{
					break;
				}
				catch (Exception ex)
				{
					_connected = false;
					Log.LogWarning("[DiscordRPC] " + ex.Message + " — retrying in 10s.");
					try
					{
						Thread.Sleep(10000);
					}
					catch
					{
						break;
					}
				}
			}
		}

		private void Connect()
		{
			for (int i = 0; i < 10; i++)
			{
				try
				{
					NamedPipeClientStream pipe = new NamedPipeClientStream(".", "discord-ipc-" + i, PipeDirection.InOut, PipeOptions.Asynchronous);
					pipe.Connect(2000);
					_pipe = pipe;
					Handshake();
					_connected = true;
					Log.LogInfo("[DiscordRPC] Connected (discord-ipc-" + i + ").");
					return;
				}
				catch
				{
				}
			}
			throw new IOException("No Discord IPC pipe available.");
		}

		private void Handshake()
		{
			string json = "{\"v\":1,\"client_id\":\"" + ClientId + "\"}";
			WriteFrame(0, json);
			ReadFrame();
		}

		private void SendActivity()
		{
			if (_pipe == null || !_pipe.IsConnected)
			{
				_connected = false;
				return;
			}
			string details;
			string state;
			lock (_lock)
			{
				details = _pendingDetails;
				state = _pendingState;
				_dirty = false;
			}
			string payload =
				"{\"cmd\":\"SET_ACTIVITY\",\"args\":{\"pid\":" + System.Diagnostics.Process.GetCurrentProcess().Id +
				",\"activity\":{" +
				"\"details\":\"" + Escape(details) + "\"," +
				"\"state\":\"" + Escape(state) + "\"," +
				"\"timestamps\":{\"start\":" + _startTime + "}," +
				"\"assets\":{\"large_image\":\"supc\",\"large_text\":\"SUPC\"}," +
				"\"buttons\":[{\"label\":\"Get SUPC\",\"url\":\"https://discord.gg/CUFYZmqXGH\"}]" +
				"}},\"nonce\":\"" + Guid.NewGuid().ToString() + "\"}";
			WriteFrame(1, payload);
			ReadFrame();
		}

		private void WriteFrame(int opcode, string data)
		{
			byte[] body = Encoding.UTF8.GetBytes(data);
			byte[] frame = new byte[8 + body.Length];
			BitConverter.GetBytes(opcode).CopyTo(frame, 0);
			BitConverter.GetBytes(body.Length).CopyTo(frame, 4);
			body.CopyTo(frame, 8);
			_pipe.Write(frame, 0, frame.Length);
			_pipe.Flush();
		}

		private void ReadFrame()
		{
			byte[] header = new byte[8];
			int read = _pipe.Read(header, 0, 8);
			if (read < 8)
			{
				return;
			}
			int len = BitConverter.ToInt32(header, 4);
			if (len <= 0)
			{
				return;
			}
			byte[] body = new byte[len];
			int total = 0;
			while (total < len)
			{
				int r = _pipe.Read(body, total, len - total);
				if (r <= 0)
				{
					break;
				}
				total += r;
			}
		}

		private void Disconnect()
		{
			_connected = false;
			try
			{
				if (_pipe != null)
				{
					_pipe.Dispose();
					_pipe = null;
				}
			}
			catch
			{
			}
		}

		private static string Escape(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return string.Empty;
			}
			return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
		}
	}
}
