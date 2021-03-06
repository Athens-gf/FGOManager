﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace AthensUtility.Unity
{
	/// <summary>
	/// キーの組み合わせを記述できるクラス
	/// ・演算子
	/// 「 & 」：かつ、すべてのキーが押されているときに反応する
	/// 「 | |：または、いずれかのキーが押されているときに反応する
	/// 「 + 」：連続、GetKeyDown、GetKeyUpのときに一番最後に結合されたものが押された（離された）ときに他のすべてのキーが押されていれば反応する
	/// </summary>
	[DebuggerDisplay("KeySet {ToString()}"), Serializable]
	public class KeySet : AndOrArrow<KeyCode, KeySet>
	{
		private static Dictionary<string, float> TimeStamp { get; } = new Dictionary<string, float>();
		public override KeyCode ErrorValue => KeyCode.None;

		#region 予約
		public static KeySet None => (KeySet)KeyCode.None;
		public static KeySet Backspace => (KeySet)KeyCode.Backspace;
		public static KeySet Tab => (KeySet)KeyCode.Tab;
		public static KeySet Return => (KeySet)KeyCode.Return;
		public static KeySet Escape => (KeySet)KeyCode.Escape;
		public static KeySet Space => (KeySet)KeyCode.Space;
		public static KeySet Delete => (KeySet)KeyCode.Delete;
		public static KeySet Insert => (KeySet)KeyCode.Insert;
		public static KeySet Home => (KeySet)KeyCode.Home;
		public static KeySet End => (KeySet)KeyCode.End;
		public static KeySet PageUp => (KeySet)KeyCode.PageUp;
		public static KeySet PageDown => (KeySet)KeyCode.PageDown;
		public static KeySet Arrows => (KeySet)KeyCode.UpArrow | KeyCode.DownArrow | KeyCode.LeftArrow | KeyCode.RightArrow;
		public static KeySet Shift => (KeySet)KeyCode.LeftShift | KeyCode.RightShift;
		public static KeySet Control => (KeySet)KeyCode.LeftControl | KeyCode.RightControl;
		public static KeySet Alt => (KeySet)KeyCode.LeftAlt | KeyCode.RightAlt;
		public static KeySet Command => (KeySet)KeyCode.LeftCommand | KeyCode.RightCommand;
		public static KeySet Apple => (KeySet)KeyCode.LeftApple | KeyCode.RightApple;
		public static KeySet Windows => (KeySet)KeyCode.LeftWindows | KeyCode.RightWindows;
		public static KeySet Alphabet => (KeySet)KeyCode.A | KeyCode.B | KeyCode.C | KeyCode.D | KeyCode.E
			| KeyCode.F | KeyCode.G | KeyCode.H | KeyCode.I | KeyCode.J
			| KeyCode.K | KeyCode.L | KeyCode.M | KeyCode.N | KeyCode.O
			| KeyCode.P | KeyCode.Q | KeyCode.R | KeyCode.S | KeyCode.T
			| KeyCode.U | KeyCode.V | KeyCode.W | KeyCode.X | KeyCode.Y | KeyCode.Z;
		public static KeySet Keypad => (KeySet)KeyCode.Keypad0 | KeyCode.Keypad1 | KeyCode.Keypad2 | KeyCode.Keypad3 | KeyCode.Keypad4 | KeyCode.Keypad5
			| KeyCode.Keypad6 | KeyCode.Keypad7 | KeyCode.Keypad8 | KeyCode.Keypad9;
		public static KeySet Function => (KeySet)KeyCode.F1 | KeyCode.F2 | KeyCode.F3 | KeyCode.F4 | KeyCode.F5
			| KeyCode.F6 | KeyCode.F7 | KeyCode.F8 | KeyCode.F9 | KeyCode.F10
			| KeyCode.F11 | KeyCode.F12 | KeyCode.F13 | KeyCode.F14 | KeyCode.F15;
		public static KeySet MouseButton => (KeySet)KeyCode.Mouse0 | KeyCode.Mouse1 | KeyCode.Mouse2 | KeyCode.Mouse3 | KeyCode.Mouse4 | KeyCode.Mouse5 | KeyCode.Mouse6;

		#endregion

		#region operator
		public static explicit operator KeySet(KeyCode _code) => new KeySet(_code);

		public static KeySet operator &(KeySet _k0, KeySet _k1) => _k0.Append(_k1, Type_e.And);
		public static KeySet operator &(KeySet _k0, KeyCode _k1) => _k0 & (KeySet)_k1;
		public static KeySet operator &(KeyCode _k0, KeySet _k1) => (KeySet)_k0 & _k1;

		public static KeySet operator |(KeySet _k0, KeySet _k1) => _k0.Append(_k1, Type_e.Or);
		public static KeySet operator |(KeySet _k0, KeyCode _k1) => _k0 | (KeySet)_k1;
		public static KeySet operator |(KeyCode _k0, KeySet _k1) => (KeySet)_k0 | _k1;

		public static KeySet operator +(KeySet _k0, KeySet _k1) => _k0.Append(_k1, Type_e.Arrow);
		public static KeySet operator +(KeySet _k0, KeyCode _k1) => _k0 + (KeySet)_k1;
		public static KeySet operator +(KeyCode _k0, KeySet _k1) => (KeySet)_k0 + _k1;
		#endregion

		public KeySet() : base() { }
		public KeySet(KeyCode _value) : base(_value) { }

		public static KeySet GetAnd(params KeyCode[] _keys)
		{
			var ks = new KeySet();
			_keys.ToList().ForEach(key => ks &= key);
			return ks;
		}

		public static KeySet GetOr(params KeyCode[] _keys)
		{
			var ks = new KeySet();
			_keys.ToList().ForEach(key => ks |= key);
			return ks;
		}

		/// <summary> キーセットが押されている間Trueを返す </summary>
		public bool GetKey()
		{
			switch (Type)
			{
				case Type_e.One:
					return Input.GetKey(Value);
				case Type_e.And:
				case Type_e.Arrow:
					bool flag = true;
					Children.ToList().ForEach(ks => flag = flag && ks.GetKey());
					return flag;
				case Type_e.Or:
					foreach (var ks in Children)
						if (ks.GetKey())
							return true;
					return false;
				default:
					return false;
			}
		}

		/// <summary> キーセットが押されたときTrueを返す </summary>
		public bool GetKeyDown()
		{
			switch (Type)
			{
				case Type_e.One:
					return Input.GetKeyDown(Value);
				case Type_e.And:
					foreach (var ks0 in Children)
					{
						bool flag = ks0.GetKeyDown();
						if (flag)
						{
							Children.Where(ks => ks != ks0).ToList().ForEach(ks1 => flag = flag && ks1.GetKey());
							if (flag) return true;
						}
					}
					return false;
				case Type_e.Or:
					foreach (var ks in Children)
						if (ks.GetKeyDown())
							return true;
					return false;
				case Type_e.Arrow:
					bool flag1 = true;
					Children.Where(ks => ks != Children.Last()).ToList().ForEach(ks => flag1 = flag1 && ks.GetKey());
					return flag1 && Children.Last().GetKeyDown();
				default:
					return false;
			}
		}

		/// <summary> キーセットが放されたときTrueを返す </summary>
		public bool GetKeyUp()
		{
			switch (Type)
			{
				case Type_e.One:
					return Input.GetKeyUp(Value);
				case Type_e.And:
					foreach (var ks0 in Children)
					{
						bool flag = ks0.GetKeyUp();
						if (flag)
						{
							Children.Where(ks => ks != ks0).ToList().ForEach(ks => flag = flag && ks.GetKey());
							if (flag) return true;
						}
					}
					return false;
				case Type_e.Or:
					foreach (var ks in Children)
						if (ks.GetKeyUp())
							return true;
					return false;
				case Type_e.Arrow:
					bool flag1 = true;
					Children.Where(ks => ks != Children.Last()).ToList().ForEach(ks => flag1 = flag1 && ks.GetKey());
					return flag1 && Children.Last().GetKeyUp();
				default:
					return false;
			}
		}

		public bool GetKeyInterval(float _time)
		{
			string key = ToString();
			if (!TimeStamp.ContainsKey(key)) TimeStamp[key] = 0;
			if (GetKey())
			{
				TimeStamp[key] += Time.deltaTime;
				if (TimeStamp[key] > _time)
				{
					TimeStamp[key] = 0;
					return true;
				}
				return false;
			}
			TimeStamp[key] = 0;
			return false;
		}

		/// <summary> 排他的なキーセットの中のどれが満たされたかをインデックスを返す
		/// どれも押されていない、あるいは複数押されている場合-1を返す </summary>
		public static int GetKeyExclusive(params KeySet[] _keysets)
		{
			List<bool> flags = new List<bool>();
			_keysets.ToList().ForEach(ks => flags.Add(ks.GetKey()));
			if (flags.Count(b => b) != 1) return -1;
			return flags.Select((b, i) => new { Flag = b, Index = i }).First(x => x.Flag).Index;
		}

		/// <summary> 排他的なキーセットの中のどれが満たされたかをインデックスを返す
		/// どれも押されていない、あるいは複数押されている場合-1を返す </summary>
		public static int GetKeyDownExclusive(params KeySet[] _keysets)
		{
			List<bool> flags = new List<bool>();
			foreach (var ks in _keysets) flags.Add(ks.GetKeyDown());
			if (flags.Count(b => b) != 1) return -1;
			return flags.Select((b, i) => new { Flag = b, Index = i }).First(x => x.Flag).Index;
		}

		/// <summary> Orをキーセットの配列に変更する </summary>
		public KeySet[] DivideOr()
		{
			if (Type == Type_e.Or) return Children.Select(ks => ks).ToArray();
			return new KeySet[] { this };
		}
	}
}
