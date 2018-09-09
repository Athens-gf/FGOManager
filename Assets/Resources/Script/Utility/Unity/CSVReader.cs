using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KMUtility.Unity
{
	class CSVReader
	{
		private List<string[]> data = new List<string[]>();
		public string[] this[int _row] { get { try { return data[_row]; } catch (Exception ex) { throw ex; } } }
		public string this[int _row, int _column] { get { try { return data[_row][_column]; } catch (Exception ex) { throw ex; } } }
		public int Row { get { return data.Count; } }
		public int MaxColumn { get { return data.Max(sl => sl.Length); } }
		public int MinColumn { get { return data.Min(sl => sl.Length); } }

		public CSVReader(string _filePath)
		{
			StringReader reader = new StringReader((Resources.Load(_filePath) as TextAsset).text);
			while (reader.Peek() > -1)
				data.Add(reader.ReadLine().Split(','));
		}

		public IEnumerable<string[]> Line()
		{
			foreach (var line in data)
				yield return line;
		}

		public delegate bool TryParse<T>(string _str, out T _result);

		public T Get<T>(int _row, int _column, T _def, TryParse<T> _fTryParce)
		{
			T r;
			if (_fTryParce(this[_row, _column], out r)) return r;
			return _def;
		}

		public int GetInt(int _row, int _column, int _def = 0) { return Get(_row, _column, _def, int.TryParse); }

		public uint GetUInt(int _row, int _column, uint _def = 0) { return Get(_row, _column, _def, uint.TryParse); }

		public long GetLong(int _row, int _column, long _def = 0) { return Get(_row, _column, _def, long.TryParse); }

		public ulong GetULong(int _row, int _column, ulong _def = 0) { return Get(_row, _column, _def, ulong.TryParse); }

		public float GetFloat(int _row, int _column, float _def = 0) { return Get(_row, _column, _def, float.TryParse); }

		public double GetDouble(int _row, int _column, double _def = 0) { return Get(_row, _column, _def, double.TryParse); }

		public decimal GetDecimal(int _row, int _column, decimal _def = 0) { return Get(_row, _column, _def, decimal.TryParse); }

		public bool GetBool(int _row, int _column, bool _def = false) { return Get(_row, _column, _def, bool.TryParse); }
	}
}