﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KMUtility
{
	public static class Extensions
	{
		/// <summary> 文字列から_removeStrを取り除いて返す </summary>
		/// <returns>取り除いた後の文字列</returns>
		/// <param name="_str">元の文字列</param>
		/// <param name="_removeStr">取り除く文字列</param>
		public static string Remove(this string _str, string _removeStr) { return _str.Replace(_removeStr, ""); }

		/// <summary> 文字列_str中の_cの数をカウントする </summary>
		/// <returns>カウント結果</returns>
		/// <param name="_str">元の文字列</param>
		/// <param name="_c">カウントする文字</param>
		public static int Count(this string _str, char _c) { return _str.Split(_c).Length - 1; }

		/// <summary> コンテナに1つ要素を追加したものを返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_iter">追加されるコンテナ</param>
		/// <param name="_t">追加する要素</param>
		public static IEnumerable<T> ReternAppend<T>(this IEnumerable<T> _iter, T _t)
		{
			var list = _iter.ToList(); list.Add(_t);
			return list;
		}

		/// <summary> コンテナに別のコンテナを連結し，その結果を返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_iter">追加されるコンテナ</param>
		/// <param name="_addIter">追加するコンテナ</param>
		public static IEnumerable<T> ReternAppend<T>(this IEnumerable<T> _iter, IEnumerable<T> _addIter)
		{
			var list = _iter.ToList(); list.AddRange(_addIter);
			return list;
		}

		/// <summary> 2つのコンテナが同じかどうかを返す </summary>
		public static bool Same<T>(this IEnumerable<T> _iter0, IEnumerable<T> _iter1) => !_iter0.Except(_iter1).Any() && !_iter1.Except(_iter0).Any();

		/// <summary> ある要素を初期要素としたコンテナを用意し，返す </summary>
		/// <returns>用意したコンテナ</returns>
		/// <param name="_t">初期要素</param>
		public static IEnumerable<T> MakeCollection<T>(this T _t) { return new List<T>() { _t }; }

		/// <summary> EnumをキーにとるDictionaryを初期化して返す </summary>
		/// <returns>初期化されたDictionary</returns>
		/// <param name="_dic">初期化するDictionary</param>
		/// <param name="_value">初期化要素</param>
		public static Dictionary<K, V> ResetEnumDictionary<K, V>(this Dictionary<K, V> _dic, V _value) where K : struct
		{
			_dic = new Dictionary<K, V>();
			foreach (K v in Enum.GetValues(typeof(K))) _dic[v] = _value;
			return _dic;
		}

		/// <summary> 2つのアクションを連続実行するアクションを作成する </summary>
		public static Action Add(this Action _action0, Action _action1) => () => { _action0(); _action1(); };

		/// <summary> DeapCopyを取得する </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public static T DeepCopy<T>(this T _src)
		{
			using (var memoryStream = new MemoryStream())
			{
				var binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, _src);
				memoryStream.Seek(0, SeekOrigin.Begin);
				return (T)binaryFormatter.Deserialize(memoryStream);
			}
		}
	}
}
