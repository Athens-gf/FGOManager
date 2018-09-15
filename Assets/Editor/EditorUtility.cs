using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace KMUtility.Editor
{
	public static class ExEditor
	{
		public static GUILayoutOption[] AddOption(this GUILayoutOption[] _options, GUILayoutOption _addOption) => _options.AddRetern(_addOption).ToArray();

		public static int Popup(string _label, int _select, string[] _display, int _width, params GUILayoutOption[] _options)
			=> EditorGUILayout.Popup(_label, _select, _display, _options.AddOption(GUILayout.Width(_width)));

		public static int Popup(int _select, string[] _display, int _width, params GUILayoutOption[] _options)
			=> EditorGUILayout.Popup(_select, _display, _options.AddOption(GUILayout.Width(_width)));

	}

	/// <summary> using範囲を抜けたときに変更されていたらOnChangeを呼び出す </summary>
	public class CheckChangeScope : IDisposable
	{
		Action OnChange { get; set; }

		public CheckChangeScope(Action _OnChange)
		{
			OnChange = _OnChange;
			EditorGUI.BeginChangeCheck();
		}

		/// <summary> using範囲を抜けたときに呼び出される </summary>
		public void Dispose()
		{
			if (EditorGUI.EndChangeCheck())
				OnChange();
		}
	}

	/// <summary> 汎用管理スクロールビュー </summary>
	public class ScrollView : IDisposable
	{
		public static Dictionary<string, Vector2> ScrollPosDic { get; private set; } = new Dictionary<string, Vector2>();
		public string ID { get; private set; }
		public Vector2 Pos { get { return ScrollPosDic[ID]; } set { ScrollPosDic[ID] = value; } }

		private void Setup(string _id)
		{
			ID = _id;
			if (!ScrollPosDic.ContainsKey(_id)) Pos = Vector2.zero;
		}

		public ScrollView(string _id, params GUILayoutOption[] _options)
		{
			Setup(_id);
			Pos = EditorGUILayout.BeginScrollView(Pos, _options);
		}

		public ScrollView(string _id, bool _alwaysShowHorizontal, bool _alwaysShowVertical, params GUILayoutOption[] _options)
		{
			Setup(_id);
			Pos = EditorGUILayout.BeginScrollView(Pos, _alwaysShowHorizontal, _alwaysShowVertical, _options);
		}

		public ScrollView(string _id, GUIStyle _horizontalScrollbar, GUIStyle _verticalScrollbar, params GUILayoutOption[] _options)
		{
			Setup(_id);
			Pos = EditorGUILayout.BeginScrollView(Pos, _horizontalScrollbar, _verticalScrollbar, _options);
		}

		public ScrollView(string _id, GUIStyle _style, params GUILayoutOption[] _options)
		{
			Setup(_id);
			Pos = EditorGUILayout.BeginScrollView(Pos, _style, _options);
		}

		/// <summary> using範囲を抜けたときに呼び出される </summary>
		public void Dispose() => EditorGUILayout.EndScrollView();
	}

	public class PopupChangeList
	{
		public string[] Options { get; private set; }
		public int SelectID { get; private set; } = 0;
		public string Str { get { return Options[SelectID]; } }

		private void SetOption(IEnumerable<string> _display)
		{
			var list = _display.ToList();
			list.Insert(0, " ");
			Options = list.ToArray();
		}

		public string Popup(IEnumerable<string> _display, params GUILayoutOption[] _options)
		{
			SetOption(_display);
			SelectID = EditorGUILayout.Popup(SelectID, Options, _options);
			return Str;
		}

		public string Popup(IEnumerable<string> _display, int _width, params GUILayoutOption[] _options)
		{
			SetOption(_display);
			SelectID = ExEditor.Popup(SelectID, Options, _width, _options);
			return Str;
		}
	}

	/// <summary> EnumをPopup表示するクラス </summary>
	public class PopupItems<T> where T : struct
	{
		public Dictionary<int, T> ResultDic { get; private set; }
		public Dictionary<T, int> IndexDic { get; private set; }
		public string[] Options { get; private set; }

		/// <summary> コンストラクタ </summary>
		/// <param name="_toStr">Enumを文字列に変更する関数</param>
		/// <param name="_toValue">Enumを順番の番号に変換する関数</param>
		public PopupItems(Func<T, string> _toStr, Func<T, int> _toValue, Func<T, bool> _fillter = null)
		{
			if (_fillter == null) _fillter = t => true;
			var ie = ExEnum.GetEnumIter<T>().Where(t => _fillter(t)).OrderBy(t => _toValue(t))
				.Select((t, i) => new { Type = t, Index = i, Value = _toValue(t) }).ToList();
			ResultDic = ie.ToDictionary(x => x.Index, x => x.Type);
			IndexDic = ie.ToDictionary(x => x.Type, x => x.Index);
			Options = ie.Select(x => _toStr(x.Type)).ToArray();
		}

		public T Popup(string _label, T _select, params GUILayoutOption[] _options) => ResultDic[EditorGUILayout.Popup(_label, IndexDic[_select], Options, _options)];

		public T Popup(T _select, params GUILayoutOption[] _options) => ResultDic[EditorGUILayout.Popup(IndexDic[_select], Options, _options)];

		public T Popup(string _label, T _select, int _width, params GUILayoutOption[] _options) => Popup(_label, _select, _options.AddOption(GUILayout.Width(_width)));

		public T Popup(T _select, int _width, params GUILayoutOption[] _options) => Popup(_select, _options.AddOption(GUILayout.Width(_width)));
	}

	public class MultiPopupItems<T> where T : struct
	{
		public Dictionary<int, List<T>> ResultDic { get; private set; } = new Dictionary<int, List<T>>();
		public Dictionary<List<T>, int> IndexDic { get; private set; } = new Dictionary<List<T>, int>();
		public string[] Options { get; private set; }

		public MultiPopupItems(Func<T, string> _toStr, Func<T, int> _toValue, Func<T, bool> _fillter = null)
		{
			if (_fillter == null) _fillter = t => true;
			var enums = ExEnum.GetEnumIter<T>().Where(t => _fillter(t)).OrderBy(t => _toValue(t));
			var enumsBuff = enums.Select((t, i) => new { Type = t, Index = i });
			Options = enumsBuff.Select(x => _toStr(x.Type)).ToArray();
			var shiftIDs = enumsBuff.ToDictionary(x => x.Type, x => 1 << x.Index);
			int max = 0;
			foreach (int i in shiftIDs.Values)
				max |= i;
			for (int i = 0; i <= max; i++)
			{
				List<T> tList = new List<T>();
				foreach (var shiftID in shiftIDs)
				{
					if ((i & shiftID.Value) != 0)
						tList.Add(shiftID.Key);
				}
				ResultDic.Add(i, tList);
				IndexDic.Add(tList, i);
			}
			ResultDic[-1] = ResultDic[max];
		}

		public List<T> GetValue(List<T> _list) => ResultDic.Values.First(l => l.Same(_list));

		public List<T> Popup(string _label, List<T> _select, params GUILayoutOption[] _options)
			=> ResultDic[EditorGUILayout.MaskField(_label, IndexDic[GetValue(_select)], Options, _options)];

		public List<T> Popup(List<T> _select, params GUILayoutOption[] _options)
			=> ResultDic[EditorGUILayout.MaskField(IndexDic[GetValue(_select)], Options, _options)];

		public List<T> Popup(string _label, List<T> _select, int _width, params GUILayoutOption[] _options)
			=> Popup(_label, _select, _options.AddOption(GUILayout.Width(_width)));

		public List<T> Popup(List<T> _select, int _width, params GUILayoutOption[] _options)
			=> Popup(_select, _options.AddOption(GUILayout.Width(_width)));
	}

	public class ExMultiPopupItems<T> where T : struct
	{
		public Dictionary<MultiPopupItems<T>, Func<T, bool>> MultiPopupItems { get; private set; } = new Dictionary<MultiPopupItems<T>, Func<T, bool>>();

		public ExMultiPopupItems(Func<T, string> _toStr, Func<T, int> _toValue, List<Func<T, bool>> _fillters)
		{
			foreach (var filter in _fillters)
				MultiPopupItems[new MultiPopupItems<T>(_toStr, _toValue, filter)] = filter;
		}

		public List<T> Popup(string _label, List<T> _select, bool _isHorizontal, params GUILayoutOption[] _options)
		{
			List<T> result = new List<T>();
			if (_isHorizontal)
			{
				using (new GUILayout.HorizontalScope())
				{
					foreach (var mpi in MultiPopupItems)
						result.AddRange(mpi.Key.Popup(_label, _select.Where(t => mpi.Value(t)).ToList(), _options));
				}
			}
			else
			{
				using (new GUILayout.VerticalScope())
				{
					foreach (var mpi in MultiPopupItems)
						result.AddRange(mpi.Key.Popup(_label, _select.Where(t => mpi.Value(t)).ToList(), _options));
				}
			}
			return result;
		}

		public List<T> Popup(List<T> _select, bool _isHorizontal, params GUILayoutOption[] _options)
		{
			List<T> result = new List<T>();
			if (_isHorizontal)
			{
				using (new GUILayout.HorizontalScope())
				{
					foreach (var mpi in MultiPopupItems)
						result.AddRange(mpi.Key.Popup(_select.Where(t => mpi.Value(t)).ToList(), _options));
				}
			}
			else
			{
				using (new GUILayout.VerticalScope())
				{
					foreach (var mpi in MultiPopupItems)
						result.AddRange(mpi.Key.Popup(_select.Where(t => mpi.Value(t)).ToList(), _options));
				}
			}
			return result;
		}

		public List<T> Popup(string _label, List<T> _select, bool _isHorizontal, int _width, params GUILayoutOption[] _options)
			=> Popup(_label, _select, _isHorizontal, _options.AddOption(GUILayout.Width(_width)));

		public List<T> Popup(List<T> _select, bool _isHorizontal, int _width, params GUILayoutOption[] _options)
			=> Popup(_select, _isHorizontal, _options.AddOption(GUILayout.Width(_width)));
	}
}