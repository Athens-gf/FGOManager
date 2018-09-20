using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace AthensUtility.Json
{
	public class KMJson
	{
		public static string ToJson<T>(T _t)
		{
			using (var ms = new MemoryStream())
			{
				new DataContractJsonSerializer(typeof(T)).WriteObject(ms, _t);
				return Encoding.UTF8.GetString(ms.ToArray());
			}
		}

		public static T FromJson<T>(string _json)
		{
			using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(_json)))
				return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
		}
	}
}
