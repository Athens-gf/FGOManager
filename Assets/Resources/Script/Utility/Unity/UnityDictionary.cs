using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Collections;

namespace KMUtility.Unity
{
	/// <summary>
	/// Unity用テーブルの管理クラス
	/// </summary>
	[Serializable]
	[DataContract]
	public class UnityDictionary<TKey, TValue, Type> : IEnumerable<KeyValuePair<TKey, TValue>>
		where TKey : IComparable
		where Type : KeyAndValue<TKey, TValue>, new()
	{
		#region Inspector
		[SerializeField, DataMember]
		private TValue m_Default;
		[SerializeField, DataMember]
		private List<Type> m_List = new List<Type>();

		#endregion

		#region Property
		/// <summary> インスペクタ対応辞書 </summary>
		public Dictionary<TKey, TValue> Table
		{
			get { return m_List?.GroupBy(x => x.Key).ToDictionary(x => x.Last().Key, x => x.Last().Value) ?? new Dictionary<TKey, TValue>(); }
			set { m_List = value?.Select(d => new Type { Key = d.Key, Value = d.Value }).ToList(); }
		}

		/// <summary> 要素数 </summary>
		public int Count { get { return m_List?.Count ?? 0; } }

		/// <summary> インデクサ </summary>
		public TValue this[TKey _key]
		{
			get { return ContainsKey(_key) ? Table[_key] : Default; }
			set { Add(_key, value); }
		}

		/// <summary> 辞書内のキーのコレクション </summary>
		public Dictionary<TKey, TValue>.KeyCollection Keys { get { return Table.Keys; } }

		/// <summary> 辞書内の要素のコレクション </summary>
		public Dictionary<TKey, TValue>.ValueCollection Values { get { return Table.Values; } }

		/// <summary> デフォルト要素 </summary>
		public TValue Default { get { return m_Default; } set { m_Default = value; } }

		#endregion

		/// <summary> 要素が変更されたときに呼び出されるイベントハンドラ </summary>
		public event EventHandler OnChanged;

		#region Constructor
		public UnityDictionary() { }

		public UnityDictionary(TValue _default) { Default = _default; }

		public UnityDictionary(UnityDictionary<TKey, TValue, Type> _dictionary)
		{ m_List = new List<Type>(_dictionary.m_List); Default = _dictionary.Default; }

		#endregion

		#region Method
		/// <summary> 要素の追加 </summary>
		/// <param name="_key">追加するキー</param>
		/// <param name="_value">キーに対応する要素</param>
		public void Add(TKey _key, TValue _value)
		{
			if (m_List == null) m_List = new List<Type>();
			m_List.Add(new Type { Key = _key, Value = _value });
			m_List = m_List.GroupBy(x => x.Key).Select(x => x.Last()).ToList();
			OnChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary> 要素の削除 </summary>
		/// <param name="_key">削除するキー</param>
		public bool Remove(TKey _key)
		{
			if (ContainsKey(_key))
			{
				m_List.RemoveAll(kv => kv.Key.CompareTo(_key) == 0);
				return true;
			}
			return false;
		}

		/// <summary> 要素の全削除 </summary>
		public void Clear() => m_List?.Clear();

		/// <summary> キーが辞書に含まれているかどうか </summary>
		/// <param name="_key">確認するキー</param>
		public bool ContainsKey(TKey _key) => m_List?.Any(pair => pair.Key.CompareTo(_key) == 0) == true;

		/// <summary> 要素が辞書に含まれているかどうか </summary>
		/// <param name="_key">確認する要素</param>
		public bool ContainsValue(TValue _value) => Table.ContainsValue(_value);

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)Table).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)Table).GetEnumerator();

		#endregion
	}

	/// <summary>
	/// シリアル化できる、KeyValuePair
	/// </summary>
	[Serializable]
	public class KeyAndValue<TKey, TValue> where TKey : IComparable
	{
		public TKey Key;
		public TValue Value;

		public KeyAndValue() { }
		public KeyAndValue(TKey _key, TValue _value) { Key = _key; Value = _value; }
		public KeyAndValue(KeyValuePair<TKey, TValue> _pair) { Key = _pair.Key; Value = _pair.Value; }
	}
}