using System;
using UnityEngine;
using System.Collections.Generic;

namespace KMUtility.Unity
{
    /// <summary>
    /// Unity用テーブルの管理クラス
    /// </summary>
    [Serializable]
    public class UnityDictionary<TKey, TValue, Type> where TKey : IComparable
        where Type : KeyAndValue<TKey, TValue>, new()
    {
        #region Inspector
        [SerializeField]
        private List<Type> m_List = null;

        #endregion

        #region PrivateMember
        private Dictionary<TKey, TValue> m_Table = null;

        #endregion

        #region Property
        /// <summary>
        /// インスペクタ対応辞書
        /// </summary>
        public Dictionary<TKey, TValue> Table
        {
            get
            {
                if (m_Table == null) m_Table = ConvertListToDictionary(m_List);
                return m_Table;
            }
        }

        /// <summary>
        /// 要素数
        /// </summary>
        public int Count { get { return Table.Count; } }

		/// <summary>
		/// インデクサ
		/// </summary>
		public TValue this[TKey key]
		{
			get
			{
				if (m_Table == null) m_Table = ConvertListToDictionary(m_List);
				return Table[key];
			}
			set { Add(key, value); }
		}

		/// <summary>
		/// 辞書内のキーのコレクション
		/// </summary>
		public Dictionary<TKey, TValue>.KeyCollection Keys { get { return Table.Keys; } }

        /// <summary>
        /// 辞書内の要素のコレクション
        /// </summary>
        public Dictionary<TKey, TValue>.ValueCollection Values { get { return Table.Values; } }
        #endregion

        #region Constructor
        public UnityDictionary() { }

        public UnityDictionary(UnityDictionary<TKey, TValue, Type> _dictionary)
        { m_List = new List<Type>(_dictionary.m_List); }

        #endregion

        #region Method
        /// <summary>
        /// 内部管理用リストから辞書を作成する
        /// </summary>
        private static Dictionary<TKey, TValue> ConvertListToDictionary(List<Type> _list)
        {
            Dictionary<TKey, TValue> dic = new Dictionary<TKey, TValue>();
			if (_list == null) _list = new List<Type>();
			foreach (Type pair in _list)
				dic.Add(pair.Key, pair.Value);
			return dic;
        }

        /// <summary>
        /// 要素の追加
        /// </summary>
        /// <param name="_key">追加するキー</param>
        /// <param name="_value">キーに対応する要素</param>
        public void Add(TKey _key, TValue _value)
        {
            if (ContainsKey(_key))
            {
                m_List.Find(kv => kv.Key.CompareTo(_key) == 0).Value = _value;
                Table[_key] = _value;
            }
            else
            {
                var kav = new Type();
                kav.Key = _key;
                kav.Value = _value;
				if (m_List == null) m_List = new List<Type>();
                m_List.Add(kav);
            }
            Table[_key] = _value;
        }

        /// <summary>
        /// 要素の全削除
        /// </summary>
        public void Clear()
        {
            m_List.Clear();
            m_Table = null;
        }

        /// <summary>
        /// キーが辞書に含まれているかどうか
        /// </summary>
        /// <param name="_key">確認するキー</param>
        public bool ContainsKey(TKey _key) => Table.ContainsKey(_key);

        /// <summary>
        /// 要素が辞書に含まれているかどうか
        /// </summary>
        /// <param name="_key">確認する要素</param>
        public bool ContainsValue(TValue _value) => Table.ContainsValue(_value);
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