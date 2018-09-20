using System;
using System.Collections.Generic;
using System.Linq;

namespace AthensUtility
{
	/// <summary> Enumに文字情報を付与したいときに記述する． </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class EnumTextAttribute : Attribute
	{
		public string Text { get; set; }
		public EnumTextAttribute(string _text) { Text = _text; }
	}

	/// <summary> Enum型拡張クラス </summary>
	public static class ExEnum
	{
		private static Dictionary<Enum, string> m_textCache = new Dictionary<Enum, string>();

		/// <summary>
		/// Enum型からその文字列情報を取得する．
		/// EnumAttributeが設定されていない場，そのEnumの名前を返す．
		/// </summary>
		/// <returns>取得文字列</returns>
		public static string GetText(this Enum _instance)
		{
			lock (m_textCache)
			{
				if (m_textCache.ContainsKey(_instance))
					return m_textCache[_instance];

				var instanceType = _instance.GetType();

				Func<Enum, string> enumToText = delegate (Enum enumElement)
				{
					if (m_textCache.ContainsKey(enumElement))
						return m_textCache[enumElement];

					var attributes = instanceType.GetField(enumElement.ToString())
												 .GetCustomAttributes(typeof(EnumTextAttribute), true);
					if (attributes.Length == 0)
						return _instance.ToString();

					var enumText = ((EnumTextAttribute)attributes[0]).Text;
					m_textCache.Add(enumElement, enumText);

					return enumText;
				};

				if (Enum.IsDefined(instanceType, _instance))
					return enumToText(_instance);
				else if (instanceType.GetCustomAttributes(typeof(FlagsAttribute), true).Length > 0)
				{
					var instanceValue = Convert.ToInt64(_instance);

					var enumes =
						from Enum value in Enum.GetValues(instanceType)
						where (instanceValue & Convert.ToInt64(value)) != 0
						select value;

					var enumSumValue =
						enumes.Sum(value => Convert.ToInt64(value));

					if (enumSumValue != instanceValue)
						return _instance.ToString();

					var enumText = string.Join(", ", (from Enum value in enumes
													  select enumToText(value)).ToArray());

					if (!m_textCache.ContainsKey(_instance))
						m_textCache.Add(_instance, enumText);

					return enumText;
				}
				else
					return _instance.ToString();
			}
		}

		/// <summary> EnumTextを持っているかどうか </summary>
		public static bool HasText(this Enum _instance) => _instance.GetText() != _instance.ToString();

		/// <summary>
		/// 文字列からEnumに変換したものを取得する．
		/// </summary>
		/// <returns>変換したEnum</returns>
		public static T GetEnumByText<T>(this string _str) where T : struct
		{
			foreach (Enum e in Enum.GetValues(typeof(T)))
				if (e.GetText() == _str)
					return (T)Enum.Parse(typeof(T), Enum.GetName(typeof(T), e), false);
			return (T)Enum.ToObject(typeof(T), 0);
		}

		public static IEnumerable<T> GetEnumIter<T>() where T : struct => Enum.GetValues(typeof(T)).Cast<T>();
		public static IEnumerable<T> GetEnumIter<T>(this T _) where T : struct => Enum.GetValues(typeof(T)).Cast<T>();
	}
}