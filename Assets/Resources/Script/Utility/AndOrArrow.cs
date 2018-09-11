using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KMUtility
{
	[DebuggerDisplay("AOA {ToString()}")]
	public class AndOrArrow<T>
	{
		public enum Type_e { None, One, And, Or, Arrow, }
		public Type_e Type = Type_e.None;
		public T Value;
		public List<AndOrArrow<T>> Children = new List<AndOrArrow<T>>();

		public bool IsEndNode { get { return Children.All(aoa => aoa.Type == Type_e.One); } }
		public List<T> EndNodeValues { get { return Children.Where(aoa => aoa.Type == Type_e.One).Select(aoa => aoa.Value).ToList(); } }

		protected static AndOrArrow<T> CreateOne(T _t) => new AndOrArrow<T> { Type = Type_e.One, Value = _t };

		protected static AndOrArrow<T> Append(AndOrArrow<T> _t0, AndOrArrow<T> _t1, Type_e _type)
		{
			if (_t0.Type == Type_e.None) return _t1;
			if (_t1.Type == Type_e.None) return _t0;
			if (_t0.Type == _type)
			{
				var aoa = new AndOrArrow<T> { Type = _t0.Type, Value = _t0.Value, Children = new List<AndOrArrow<T>>(_t0.Children) };
				aoa.Children.Add(_t1);
				return aoa;
			}
			return new AndOrArrow<T> { Type = _type, Children = new List<AndOrArrow<T>> { _t0, _t1 } };
		}

		public static explicit operator AndOrArrow<T>(T _t) => CreateOne(_t);

		public static AndOrArrow<T> operator &(AndOrArrow<T> _t0, AndOrArrow<T> _t1) => Append(_t0, _t1, Type_e.And);
		public static AndOrArrow<T> operator &(AndOrArrow<T> _t0, T _t1) => Append(_t0, CreateOne(_t1), Type_e.And);
		public static AndOrArrow<T> operator &(T _t0, AndOrArrow<T> _t1) => Append(CreateOne(_t0), _t1, Type_e.And);

		public static AndOrArrow<T> operator |(AndOrArrow<T> _t0, AndOrArrow<T> _t1) => Append(_t0, _t1, Type_e.Or);
		public static AndOrArrow<T> operator |(AndOrArrow<T> _t0, T _t1) => Append(_t0, CreateOne(_t1), Type_e.Or);
		public static AndOrArrow<T> operator |(T _t0, AndOrArrow<T> _t1) => Append(CreateOne(_t0), _t1, Type_e.Or);

		public static AndOrArrow<T> operator +(AndOrArrow<T> _t0, AndOrArrow<T> _t1) => Append(_t0, _t1, Type_e.Arrow);
		public static AndOrArrow<T> operator +(AndOrArrow<T> _t0, T _t1) => Append(_t0, CreateOne(_t1), Type_e.Arrow);
		public static AndOrArrow<T> operator +(T _t0, AndOrArrow<T> _t1) => Append(CreateOne(_t0), _t1, Type_e.Arrow);

		public override string ToString()
		{
			string str = "";
			switch (Type)
			{
				case Type_e.None:
					return "[None]";
				case Type_e.One:
					return Value.ToString();
				case Type_e.And:
					foreach (var asa in Children)
					{
						if (str == "")
							str = $"( {asa.ToString()}";
						else
							str += $" & {asa.ToString()}";
					}
					return str + " )";
				case Type_e.Or:
					foreach (var asa in Children)
					{
						if (str == "")
							str = $"( {asa.ToString()}";
						else
							str += $" | {asa.ToString()}";
					}
					return str + " )";
				case Type_e.Arrow:
					foreach (var asa in Children)
					{
						if (str == "")
							str = $"( {asa.ToString()}";
						else
							str += $" -> {asa.ToString()}";
					}
					return str + " )";
				default:
					return "";
			}
		}
	}
}
