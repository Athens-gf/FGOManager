using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AthensUtility
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


		/// <summary> 2つのActionを連続実行するActionを作成する </summary>
		public static Action Add(this Action _action0, Action _action1) => () => { _action0(); _action1(); };
		/// <summary> 2つのActionを連続実行するActionを作成する </summary>
		public static Action<T> Add<T>(this Action<T> _action0, Action<T> _action1) => x => { _action0(x); _action1(x); };
		/// <summary> 2つのActionを連続実行するActionを作成する </summary>
		public static Action<T1, T2> Add<T1, T2>(this Action<T1, T2> _action0, Action<T1, T2> _action1)
			=> (x1, x2) => { _action0(x1, x2); _action1(x1, x2); };
		/// <summary> 2つのActionを連続実行するActionを作成する </summary>
		public static Action<T1, T2, T3> Add<T1, T2, T3>(this Action<T1, T2, T3> _action0, Action<T1, T2, T3> _action1)
			=> (x1, x2, x3) => { _action0(x1, x2, x3); _action1(x1, x2, x3); };
		/// <summary> 2つのActionを連続実行するActionを作成する </summary>
		public static Action<T1, T2, T3, T4> Add<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> _action0, Action<T1, T2, T3, T4> _action1)
			=> (x1, x2, x3, x4) => { _action0(x1, x2, x3, x4); _action1(x1, x2, x3, x4); };

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

	public static class ExIEnumerable
	{
		/// <summary> _startから始まり_stepずつ増える_count個のint型配列を返す </summary>
		/// <param name="_count">要素数</param>
		/// <param name="_start">初期値</param>
		/// <param name="_step">増えていく数</param>
		/// <returns></returns>
		public static IEnumerable<int> Range(int _count, int _start = 0, int _step = 1) => Enumerable.Range(0, _count).Select(i => _start + (i * _step));

		/// <summary> コンテナに1つ要素を追加したものを返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_iter">追加されるコンテナ</param>
		/// <param name="_t">追加する要素</param>
		public static IEnumerable<T> AddRetern<T>(this IEnumerable<T> _iter, T _t)
		{
			var list = _iter.ToList(); list.Add(_t);
			return list;
		}

		/// <summary> コンテナに別のコンテナを連結し，その結果を返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_iter">追加されるコンテナ</param>
		/// <param name="_addIter">追加するコンテナ</param>
		public static IEnumerable<T> AddRetern<T>(this IEnumerable<T> _iter, IEnumerable<T> _addIter)
		{
			var list = _iter.ToList(); list.AddRange(_addIter);
			return list;
		}

		/// <summary> コンテナに1つ要素を挿入したものを返す </summary>
		/// <returns>挿入後のコンテナ</returns>
		/// <param name="_iter">挿入されるコンテナ</param>
		/// <param name="_index">挿入する位置</param>
		/// <param name="_t">挿入する要素</param>
		public static IEnumerable<T> InsertRetern<T>(this IEnumerable<T> _iter, int _index, T _t)
		{
			var list = _iter.ToList(); list.Insert(_index, _t);
			return list;
		}

		/// <summary> コンテナに別のコンテナを挿入したものを返す </summary>
		/// <returns>挿入後のコンテナ</returns>
		/// <param name="_iter">挿入されるコンテナ</param>
		/// <param name="_index">挿入する位置</param>
		/// <param name="_insertIter">挿入するコンテナ</param>
		public static IEnumerable<T> InsertRetern<T>(this IEnumerable<T> _iter, int _index, IEnumerable<T> _insertIter)
		{
			var list = _iter.ToList(); list.InsertRange(_index, _insertIter);
			return list;
		}

		/// <summary> コンテナから要素を削除したものを返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_iter">追加されるコンテナ</param>
		/// <param name="_t">削除する要素</param>
		public static IEnumerable<T> RemoveRetern<T>(this IEnumerable<T> _iter, T _t) => _iter.Where(x => !x.Equals(_t));

		/// <summary> コンテナから別のコンテナ要素をすべて削除したものを返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_iter">追加されるコンテナ</param>
		/// <param name="_removeIter">削除する要素</param>
		public static IEnumerable<T> RemoveRetern<T>(this IEnumerable<T> _iter, IEnumerable<T> _removeIter)
			=> _iter.Where(x => !_removeIter.Any(rx => x.Equals(rx)));

		/// <summary> コンテナから特定の位置の要素を削除したものを返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_iter">追加されるコンテナ</param>
		/// <param name="_t">追加する要素</param>
		public static IEnumerable<T> RemoveAtRetern<T>(this IEnumerable<T> _iter, int _index) => _iter.Where((_, i) => i != _index);

		/// <summary> コンテナ内で条件に一致したすべての要素のインデックスを返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_check">確認用関数</param>
		/// <returns>一致したインデックスのリスト</returns>
		public static List<int> AllIndex<T>(this IEnumerable<T> _iter, Func<T, bool> _check)
			=> _iter.Select((t, i) => new { Value = t, Index = i }).Where(x => _check(x.Value)).Select(x => x.Index).ToList();

		/// <summary> コンテナ内で条件に最初に一致した要素のインデックスを返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_check">確認用関数</param>
		/// <returns>一致したインデックス、存在しなかった場合「-1」</returns>
		public static int FirstIndex<T>(this IEnumerable<T> _iter, Func<T, bool> _check)
			=> _iter.Any(_check) ? _iter.AllIndex(_check).First() : -1;

		/// <summary> コンテナ内で条件に最初に一致した要素のインデックスを返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_check">確認用関数</param>
		/// <param name="_default">デフォルトインデックス</param>
		/// <returns>一致したインデックス、存在しなかった場合「_default」</returns>
		public static int FirstIndex<T>(this IEnumerable<T> _iter, Func<T, bool> _check, int _default)
			=> _iter.Any(_check) ? _iter.AllIndex(_check).First() : _default;

		/// <summary> コンテナ内で条件に最後に一致した要素のインデックスを返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_check">確認用関数</param>
		/// <returns>一致したインデックス、存在しなかった場合「-1」</returns>
		public static int LastIndex<T>(this IEnumerable<T> _iter, Func<T, bool> _check)
			=> _iter.Any(_check) ? _iter.AllIndex(_check).Last() : -1;

		/// <summary> コンテナ内で条件に最初に一致した要素のインデックスを返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_check">確認用関数</param>
		/// <param name="_default">デフォルトインデックス</param>
		/// <returns>一致したインデックス、存在しなかった場合「_default」</returns>
		public static int LastIndex<T>(this IEnumerable<T> _iter, Func<T, bool> _check, int _default)
			=> _iter.Any(_check) ? _iter.AllIndex(_check).Last() : _default;

		/// <summary> コンテナ内の最初の要素を返す、コンテナが空の場合指定したデフォルト要素を返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_default">デフォルト要素</param>
		/// <returns>一致した要素、存在しなかった場合「-1」</returns>
		public static T FirstOrDefault<T>(this IEnumerable<T> _iter, T _default) => _iter.Any() ? _iter.First() : _default;

		/// <summary> コンテナ内で条件に最初に一致した要素を返す、一致する要素がない場合指定したデフォルト要素を返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_check">確認用関数</param>
		/// <param name="_default">デフォルト要素</param>
		/// <returns>一致した要素、存在しなかった場合「-1」</returns>
		public static T FirstOrDefault<T>(this IEnumerable<T> _iter, Func<T, bool> _check, T _default) => _iter.Any(_check) ? _iter.First(_check) : _default;

		/// <summary> コンテナ内の最後の要素を返す、コンテナが空の場合指定したデフォルト要素を返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_default">デフォルト要素</param>
		/// <returns>一致した要素、存在しなかった場合「-1」</returns>
		public static T LastOrDefault<T>(this IEnumerable<T> _iter, T _default) => _iter.Any() ? _iter.Last() : _default;

		/// <summary> コンテナ内で条件に最後に一致した要素を返す、一致する要素がない場合指定したデフォルト要素を返す </summary>
		/// <param name="_iter">コンテナ</param>
		/// <param name="_check">確認用関数</param>
		/// <param name="_default">デフォルト要素</param>
		/// <returns>一致した要素、存在しなかった場合「-1」</returns>
		public static T LastOrDefault<T>(this IEnumerable<T> _iter, Func<T, bool> _check, T _default) => _iter.Any(_check) ? _iter.Last(_check) : _default;


		/// <summary> 2つのコンテナが同じかどうかを返す </summary>
		public static bool Same<T>(this IEnumerable<T> _iter0, IEnumerable<T> _iter1) => !_iter0.Except(_iter1).Any() && !_iter1.Except(_iter0).Any();

		/// <summary> ある要素を初期要素としたコンテナを用意し，返す </summary>
		/// <returns>用意したコンテナ</returns>
		/// <param name="_t">初期要素</param>
		public static IEnumerable<T> MakeCollection<T>(this T _t) { return new List<T>() { _t }; }


		/// <summary> ディクショナリに1つ要素を追加し，その結果を返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_dic">追加されるディクショナリ</param>
		/// <param name="_t">追加する要素</param>
		public static Dictionary<TKey, Tvalue> AppendDictionary<TKey, Tvalue>(this IEnumerable<KeyValuePair<TKey, Tvalue>> _dic, KeyValuePair<TKey, Tvalue> _t)
			=> _dic.AddRetern(_t).ToDictionary(kv => kv.Key, kv => kv.Value);

		/// <summary> ディクショナリに別のディクショナリを連結し，その結果を返す </summary>
		/// <returns>追加後のコンテナ</returns>
		/// <param name="_dic">追加されるディクショナリ</param>
		/// <param name="_addDic">追加するディクショナリ</param>
		public static Dictionary<TKey, Tvalue> AppendDictionary<TKey, Tvalue>(this IEnumerable<KeyValuePair<TKey, Tvalue>> _dic, IEnumerable<KeyValuePair<TKey, Tvalue>> _addDic)
			=> _dic.AddRetern(_addDic).ToDictionary(kv => kv.Key, kv => kv.Value);

		/// <summary> EnumをキーにとるDictionaryを初期化して返す </summary>
		/// <returns>初期化されたDictionary</returns>
		/// <param name="_dic">初期化するDictionary</param>
		/// <param name="_value">初期化要素</param>
		public static Dictionary<K, V> ResetEnumDictionary<K, V>(this Dictionary<K, V> _dic, V _value) where K : struct
			=> ExEnum.GetEnumIter<K>().Select(k => new { Key = k, Value = _value }).ToDictionary(x => x.Key, x => x.Value);

		/// <summary> 例外要素の場合例外を返す </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="_value">チェックする値</param>
		/// <param name="_predicate">例外チェック関数</param>
		/// <param name="_exception">例外</param>
		public static T ThrowIf<T>(this T _value, Func<T, bool> _predicate, Exception _exception)
		{
			if (_predicate(_value)) throw _exception;
			else return _value;
		}

		/// <summary> コンテナ内に例外要素が存在する場合例外を返す </summary>
		/// <param name="_value">チェックする値</param>
		/// <param name="_predicate">例外チェック関数</param>
		/// <param name="_exception">例外</param>
		public static IEnumerable<T> ThrowIf<T>(this IEnumerable<T> _iter, Func<T, bool> _predicate, Exception _exception)
		{
			try
			{
				return _iter.Select(x => x.ThrowIf(_predicate, _exception));
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary> 三項演算子のチェーンを崩さないため </summary>
		/// <param name="_exception"></param>
		/// <returns></returns>
		static T Throw<T>(Exception _exception)
		{
			throw _exception;
		}
	}
}
