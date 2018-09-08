using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

namespace KMUtility.Unity
{
	public static class SaveJsonPng
	{
		[Serializable]
		class ListWrapper<T> { public List<T> List; }

		/// <summary> JSON文字列からリスト形式で取得する </summary>
		/// <param name="_json">変換したいJSON文字列</param>
		/// <returns>リスト</returns>
		public static List<T> ListFromJson<T>(string _json) => JsonUtility.FromJson<ListWrapper<T>>(_json).List;

		/// <summary> リスト形式からJSON文字列を取得する </summary>
		/// <param name="_list">変換したいリスト</param>
		/// <returns>JSON文字列</returns>
		public static string ListToJson<T>(List<T> _list) => JsonUtility.ToJson(new ListWrapper<T>() { List = _list });

		/// <summary> Png画像をの読み込み </summary>
		/// <param name="_path">読み込む画像のパス</param>
		/// <returns>取得画像のTexture2D</returns>
		public static Texture2D ReadPng(string _path)
		{
			FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read);
			BinaryReader bin = new BinaryReader(fileStream);
			byte[] readBinary = bin.ReadBytes((int)bin.BaseStream.Length);
			bin.Close();

			int pos = 16; // 16バイトから開始
			int width = 0, height = 0;
			for (int i = 0; i < 4; i++)
				width = width * 256 + readBinary[pos++];
			for (int i = 0; i < 4; i++)
				height = height * 256 + readBinary[pos++];

			Texture2D texture = new Texture2D(width, height);
			texture.LoadImage(readBinary);

			return texture;
		}

		/// <summary> Png画像の保存 </summary>
		/// <param name="_path">保存するパス</param>
		/// <param name="_texture">画像データ</param>
		public static void SavePng(string _path, Texture2D _texture) => File.WriteAllBytes(_path, _texture.EncodeToPNG());

		/// <summary> char型2つからColorに変換 </summary>
		public static Color GetColorFromChars(char _c0, char _c1)
		{
			float inv = 1.0f / 255.0f;
			float r = (((uint)_c0 >> 8) & 0xff) * inv;
			float g = (((uint)_c0) & 0xff) * inv;
			float b = (((uint)_c1 >> 8) & 0xff) * inv;
			float a = (((uint)_c1) & 0xff) * inv;
			return new Color(r, g, b, a);
		}

		/// <summary> Colorからchar型2つのタプルに変換 </summary>
		public static Tuple<char, char> GetCharFromColor(Color _color)
		{
			char c0 = (char)(((uint)(_color.r * 255) << 8) + (uint)(_color.g * 255));
			char c1 = (char)(((uint)(_color.b * 255) << 8) + (uint)(_color.a * 255));
			return new Tuple<char, char>(c0, c1);
		}

		/// <summary> JSONからTexture2D形式に変換する </summary>
		/// <param name="_json">JSON文字列</param>
		/// <returns>Texture2D</returns>
		public static Texture2D JsonToTexture(string _json)
		{
			int pixelNum = _json.Length / 2;
			int wh = 1;
			while (wh * wh < pixelNum) wh++;
			Texture2D texture = new Texture2D(wh, wh);
			for (int y = 0, pos = 0; y < wh; y++)
			{
				for (int x = 0; x < wh; x++)
				{
					char c0 = '\0', c1 = '\0';
					if (pos < _json.Length) c0 = _json[pos++];
					if (pos < _json.Length) c1 = _json[pos++];
					texture.SetPixel(x, y, GetColorFromChars(c0, c1));
				}
			}
			texture.Apply();
			return texture;
		}

		/// <summary> Texture2D形式からJSON文字列に変換する </summary>
		/// <param name="_json">Texture2D</param>
		/// <returns>JSON文字列</returns>
		public static string TextureToJson(Texture2D _texture)
		{
			string str = "";
			for (int y = 0; y < _texture.height; y++)
			{
				for (int x = 0; x < _texture.width; x++)
				{
					Tuple<char, char> charTuple = GetCharFromColor(_texture.GetPixel(x, y));
					if (charTuple.Item1 == '\0')
					{
						y = _texture.height;
						break;
					}
					str += charTuple.Item1;
					if (charTuple.Item2 == '\0')
					{
						y = _texture.height;
						break;
					}
					str += charTuple.Item2;
				}
			}
			return str;
		}

		/// <summary> オブジェクトをJSON形式に変換し、そのJSON文字列をPng画像として保存する </summary>
		/// <param name="_path">保存先のパス</param>
		/// <param name="_object">保存するオブジェクト</param>
		public static void Save(string _path, object _object) => SavePng(_path, JsonToTexture(JsonUtility.ToJson(_object)));

		/// <summary> リストオブジェクトをJSON形式に変換し、そのJSON文字列をPng画像として保存する </summary>
		/// <param name="_path">保存先のパス</param>
		/// <param name="_list">保存するリストオブジェクト</param>
		public static void SaveList<T>(string _path, List<T> _list) => SavePng(_path, JsonToTexture(ListToJson(_list)));

		/// <summary> Png画像画像からオブジェクトデータを読み出す </summary>
		/// <param name="_path">保存先のパス</param>
		public static T Load<T>(string _path) => JsonUtility.FromJson<T>(TextureToJson(ReadPng(_path)));

		/// <summary> Png画像画像からリストオブジェクトデータを読み出す </summary>
		/// <param name="_path">保存先のパス</param>
		public static List<T> LoadList<T>(string _path) => ListFromJson<T>(TextureToJson(ReadPng(_path)));
	}
}
