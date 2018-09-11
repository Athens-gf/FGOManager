using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace KMUtility.Unity
{
	public static class ExKeySet
	{
		public static KeySet ToKeySet(this AndOrArrow<KeyCode> _ks)
			=> new KeySet { Type = _ks.Type, Value = _ks.Value, Children = new List<AndOrArrow<KeyCode>>(_ks.Children) };
	}

	/// <summary>
	/// キーの組み合わせを記述できるクラス
	/// ・演算子
	/// 「 & 」：かつ、すべてのキーが押されているときに反応する
	/// 「 | |：または、いずれかのキーが押されているときに反応する
	/// 「 + 」：連続、GetKeyDown、GetKeyUpのときに一番最後に結合されたものが押された（離された）ときに他のすべてのキーが押されていれば反応する
	/// </summary>
	public class KeySet : AndOrArrow<KeyCode>
	{
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
		public static explicit operator KeySet(KeyCode _k) => CreateOne(_k).ToKeySet();

		public static KeySet operator &(KeySet _k0, KeySet _k1) => Append(_k0, _k1, Type_e.And).ToKeySet();
		public static KeySet operator &(KeySet _k0, KeyCode _k1) => Append(_k0, CreateOne(_k1), Type_e.And).ToKeySet();
		public static KeySet operator &(KeyCode _k0, KeySet _k1) => Append(CreateOne(_k0), _k1, Type_e.And).ToKeySet();

		public static KeySet operator |(KeySet _k0, KeySet _k1) => Append(_k0, _k1, Type_e.Or).ToKeySet();
		public static KeySet operator |(KeySet _k0, KeyCode _k1) => Append(_k0, CreateOne(_k1), Type_e.Or).ToKeySet();
		public static KeySet operator |(KeyCode _k0, KeySet _k1) => Append(CreateOne(_k0), _k1, Type_e.Or).ToKeySet();

		public static KeySet operator +(KeySet _k0, KeySet _k1) => Append(_k0, _k1, Type_e.Arrow).ToKeySet();
		public static KeySet operator +(KeySet _k0, KeyCode _k1) => Append(_k0, CreateOne(_k1), Type_e.Arrow).ToKeySet();
		public static KeySet operator +(KeyCode _k0, KeySet _k1) => Append(CreateOne(_k0), _k1, Type_e.Arrow).ToKeySet();
		#endregion

		public static KeySet GetAnd(params KeyCode[] _keys)
		{
			var ks = new AndOrArrow<KeyCode>();
			foreach (var key in _keys)
				ks &= key;
			return ks.ToKeySet();
		}

		public static KeySet GetOr(params KeyCode[] _keys)
		{
			var ks = new AndOrArrow<KeyCode>();
			foreach (var key in _keys)
				ks |= key;
			return ks.ToKeySet();
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
					foreach (var ks in Children)
						flag = flag && ks.ToKeySet().GetKey();
					return flag;
				case Type_e.Or:
					foreach (var ks in Children)
						if (ks.ToKeySet().GetKey())
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
						bool flag = ks0.ToKeySet().GetKeyDown();
						if (flag)
						{
							foreach (var ks1 in Children.Where(ks => ks != ks0))
								flag = flag && ks1.ToKeySet().GetKey();
							if (flag)
								return true;
						}
					}
					return false;
				case Type_e.Or:
					foreach (var ks in Children)
						if (ks.ToKeySet().GetKeyDown())
							return true;
					return false;
				case Type_e.Arrow:
					bool flag1 = true;
					foreach (var ks in Children.Where(ks => ks != Children.Last()))
						flag1 = flag1 && ks.ToKeySet().GetKey();
					return flag1 && Children.Last().ToKeySet().GetKeyDown();
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
						bool flag = ks0.ToKeySet().GetKeyUp();
						if (flag)
						{
							foreach (var ks1 in Children.Where(ks => ks != ks0))
								flag = flag && ks1.ToKeySet().GetKey();
							if (flag)
								return true;
						}
					}
					return false;
				case Type_e.Or:
					foreach (var ks in Children)
						if (ks.ToKeySet().GetKeyUp())
							return true;
					return false;
				case Type_e.Arrow:
					bool flag1 = true;
					foreach (var ks in Children.Where(ks => ks != Children.Last()))
						flag1 = flag1 && ks.ToKeySet().GetKey();
					return flag1 && Children.Last().ToKeySet().GetKeyUp();
				default:
					return false;
			}
		}

		/// <summary> 排他的なキーセットの中のどれが満たされたかをインデックスを返す
		/// どれも押されていない、あるいは複数押されている場合-1を返す </summary>
		public static int GetKeyExclusive(params KeySet[] _keysets)
		{
			List<bool> flags = new List<bool>();
			foreach (var ks in _keysets) flags.Add(ks.GetKey());
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
			if (Type == Type_e.Or) return Children.Select(ks => ks.ToKeySet()).ToArray();
			return new KeySet[] { this };
		}
	}
}
