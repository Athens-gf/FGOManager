using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KMUtility;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FGOManager.Editor
{
	public class EditorServantRegist : EditorWindow
	{
		public class CheckChangeScope : IDisposable
		{
			Action OnChange { get; set; }

			public CheckChangeScope(Action _OnChange)
			{
				OnChange = _OnChange;
				EditorGUI.BeginChangeCheck();
			}

			public void Dispose()
			{
				if (EditorGUI.EndChangeCheck())
					OnChange();
			}
		}

		public class PopupItems<T> where T : struct
		{
			private Dictionary<int, T> NameDic { get; set; }
			private Dictionary<T, int> IndexDic { get; set; }
			private string[] Names { get; set; }

			public PopupItems(Func<T, string> _toStr, Func<T, int> _toValue, bool _isReverse = false)
			{
				var ie = Enum.GetValues(typeof(T)).Cast<T>().OrderBy(t => _toValue(t) * (_isReverse ? -1 : 1))
					.Select((t, i) => new { Type = t, Index = i, Value = _toValue(t) }).ToList();
				NameDic = ie.ToDictionary(x => x.Index, x => x.Type);
				IndexDic = ie.ToDictionary(x => x.Type, x => x.Index);
				Names = ie.Select(x => _toStr(x.Type)).ToArray();
			}

			public T Popup(string _label, T _select, int _width, params GUILayoutOption[] _options)
				=> NameDic[EditorGUILayout.Popup(_label, IndexDic[_select], Names, EditOptions(_options, _width))];
			public T Popup(T _select, int _width, params GUILayoutOption[] _options)
				=> NameDic[EditorGUILayout.Popup(IndexDic[_select], Names, EditOptions(_options, _width))];
		}

		readonly string[] Rares = new string[] { "_", "☆", "☆☆", "☆☆☆", "☆☆☆☆", "☆☆☆☆☆" };
		readonly string[] PlusStr = new string[] { "-", " ", "+", "++" };
		static PopupItems<Class_e> PopupClass = new PopupItems<Class_e>(x => x.GetShortName(), x => (int)x);
		static PopupItems<CommandCard.Type_e> PopupCommandCard = new PopupItems<CommandCard.Type_e>(x => x.ToString(), x => (int)x);
		static PopupItems<Attributes_e> PopupAttributes = new PopupItems<Attributes_e>(x => x.GetText(), x => (int)x);
		static PopupItems<Policy_e> PopupPolicy = new PopupItems<Policy_e>(x => x.GetText(), x => (int)x);
		static PopupItems<Personality.Type_e> PopupPersonality = new PopupItems<Personality.Type_e>(x => x.GetText(), x => (int)x);
		static PopupItems<Sex_e> PopupSex = new PopupItems<Sex_e>(x => x.GetText(), x => (int)x);
		static PopupItems<StatusTrend_e> PopupStatusTrend = new PopupItems<StatusTrend_e>(x => x.GetText(), x => (int)x, true);
		static PopupItems<Rank.Type_e> PopupRank = new PopupItems<Rank.Type_e>(x => x.ToString(), x => (int)x, true);

		List<ServantBase> servants = new List<ServantBase>();
		ServantBase editServant = null;
		Vector2 listScrollPos = Vector2.zero, editScrollPos = Vector2.zero;
		int selectID = 0;
		Action ChangeAtk = null, ChangePersonality = null, ChangeNA = null;

		string[] PersonalityOtherStrs;
		int otherPersonalityID = 0;
		int existCharacteristicID = 0;

		[MenuItem("Editor/Regist Servant")]
		private static void Create()
		{
			GetWindow<EditorServantRegist>("Regist Servant");
		}

		private void OnGUI()
		{
			if (ChangeAtk == null)
			{
				ChangeAtk = () =>
				{
					editServant.FirstATK = editServant.BaseFirstATK;
					editServant.MaxATK = editServant.BaseMaxATK;
				};
			}
			if (ChangePersonality == null)
				ChangePersonality = () => editServant.Personality.OtherStr = otherPersonalityID == 0 ? "" : PersonalityOtherStrs[otherPersonalityID];
			if (ChangeNA == null)
				ChangeNA = () => editServant.NA = (decimal)editServant.BaseNA;

			using (new GUILayout.HorizontalScope())
			{
				using (new GUILayout.VerticalScope(GUILayout.Width(300)))
				{
					using (new GUILayout.VerticalScope(GUI.skin.box))
					{
						using (new GUILayout.HorizontalScope())
						{
							Load();
							Save();
						}
						ScrollList();
					}
				}
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					using (new GUILayout.HorizontalScope())
					{
						NewRegister();
						Delete();
					}
					if (editServant != null)
						Register();
				}
			}
		}

		public void Load()
		{
			if (GUILayout.Button("Load"))
			{
			}
		}

		public void Save()
		{
			if (GUILayout.Button("Save"))
			{
			}
		}

		public void ScrollList()
		{
			using (var scrollViewScope = new GUILayout.ScrollViewScope(listScrollPos, GUI.skin.box))
			{
				listScrollPos = scrollViewScope.scrollPosition;

				EditorGUI.BeginChangeCheck();
				using (new CheckChangeScope(() => editServant = servants[selectID]))
					selectID = GUILayout.SelectionGrid(selectID, servants.Select(s => $"{s.No}: {s.Name}").ToArray(), 1, "PreferencesKeysElement");
			}
		}

		public void NewRegister()
		{
			if (GUILayout.Button("新規登録", GUILayout.MinHeight(30)))
			{
				editServant = new ServantBase();
				for (editServant.No = 1; servants.Exists(s => s.No == editServant.No); editServant.No++) ;
				servants.Add(editServant);
				servants = servants.OrderBy(s => s.No).ToList();
				selectID = servants.Select((s, i) => new { s.No, Index = i }).First(x => x.No == editServant.No).Index;
				ChangeAtk();
				ChangeNA();
			}
		}

		public void Delete()
		{
			if (GUILayout.Button("削除", GUILayout.MinHeight(30)))
			{
				if (servants.Count != 0)
				{
					servants.Remove(editServant);
					if (servants.Count == 0)
						editServant = null;
					else
					{
						if (selectID == servants.Count)
							selectID--;
						if (selectID < servants.Count)
							editServant = servants[selectID];
					}
				}
			}
		}

		private static GUILayoutOption[] EditOptions(GUILayoutOption[] _options, int _width) => _options.ReternAppend(GUILayout.Width(_width)).ToArray();

		public string TextField(string _label, string _text, int _width, params GUILayoutOption[] _options)
			=> EditorGUILayout.TextField(_label, _text, EditOptions(_options, _width));
		public string TextField(string _text, int _width, params GUILayoutOption[] _options)
			=> EditorGUILayout.TextField(_text, EditOptions(_options, _width));
		public int IntField(string _label, int _value, int _min, int _width, params GUILayoutOption[] _options)
			=> Mathf.Max(EditorGUILayout.IntField(_label, _value, EditOptions(_options, _width)), _min);
		public decimal DeciamlField(string _label, decimal _value, int _width, params GUILayoutOption[] _options)
			=> (decimal)EditorGUILayout.DoubleField(_label, (double)_value, EditOptions(_options, _width));
		public decimal DeciamlField(decimal _value, int _width, params GUILayoutOption[] _options)
			=> (decimal)EditorGUILayout.DoubleField((double)_value, EditOptions(_options, _width));

		public int Popup(string _label, int _select, string[] _display, int _width, params GUILayoutOption[] _options)
			=> EditorGUILayout.Popup(_label, _select, _display, EditOptions(_options, _width));
		public int Popup(int _select, string[] _display, int _width, params GUILayoutOption[] _options)
			=> EditorGUILayout.Popup(_select, _display, EditOptions(_options, _width));

		public void Register()
		{
			using (var scrollViewScope = new GUILayout.ScrollViewScope(editScrollPos, GUI.skin.box, GUILayout.Width(540)))
			{
				editScrollPos = scrollViewScope.scrollPosition;
				EditorGUIUtility.labelWidth = 100;
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					// No.、真名
					using (new GUILayout.HorizontalScope())
					{
						editServant.No = IntField("No:", editServant.No, 1, 150);
						editServant.Name = TextField("真名:", editServant.Name, 350);
					}
					// Class、Rare度
					using (new GUILayout.HorizontalScope())
					{
						using (new CheckChangeScope(ChangeNA.Add(ChangeAtk)))
							editServant.Class = PopupClass.Popup("Class:", editServant.Class, 250);
						using (new CheckChangeScope(ChangeAtk))
							editServant.Rare = (byte)Popup("レア度:", editServant.Rare, Rares, 250);
					}
					// コマンドカード配分、Cost
					using (new GUILayout.HorizontalScope())
					{
						using (new CheckChangeScope(ChangeNA))
							editServant.CommandCard.Type = PopupCommandCard.Popup("コマンドカード:", editServant.CommandCard.Type, 250);
						EditorGUILayout.LabelField("Cost:", editServant.Cost.ToString(), GUILayout.Width(250));
					}
				}
				// ステータス
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					// HP
					using (new GUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField("Lv.1 HP:", editServant.FirstHP.ToString(), GUILayout.Width(250));
						EditorGUILayout.LabelField($"Lv.{editServant.MaxLevel} HP:", editServant.MaxHP.ToString(), GUILayout.Width(250));
					}
					// Atk
					using (new GUILayout.HorizontalScope())
					{
						editServant.FirstATK = IntField("Lv.1 ATK:", editServant.FirstATK, 1, 250);
						editServant.MaxATK = IntField($"Lv.{editServant.MaxLevel} ATK:", editServant.MaxATK, editServant.FirstATK + 1, 250);
					}
				}
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					// 方針、性格
					using (new GUILayout.HorizontalScope())
					{
						editServant.Policy = PopupPolicy.Popup("方針:", editServant.Policy, 250);
						editServant.Personality.Type = PopupPersonality.Popup("性格:", editServant.Personality.Type, 250);
						if (editServant.Personality.Type == Personality.Type_e.Other)
						{
							var list = servants.Where(s => s != editServant && s.Personality.Type == Personality.Type_e.Other)
								.Select(s => s.Personality.OtherStr).ToList();
							list.Insert(0, "_");
							PersonalityOtherStrs = list.ToArray();
							if (PersonalityOtherStrs.Length > 1)
							{
								using (new CheckChangeScope(ChangePersonality))
									otherPersonalityID = Popup(otherPersonalityID, PersonalityOtherStrs, 250);
							}
							editServant.Personality.OtherStr = TextField(editServant.Personality.OtherStr, 250);
						}
					}
					// 性別、魔法タイプかどうか
					using (new GUILayout.HorizontalScope())
					{
						editServant.Sex = PopupSex.Popup("性別:", editServant.Sex, 250);
						using (new CheckChangeScope(ChangeAtk))
							editServant.IsMagicalType = EditorGUILayout.Toggle("魔法タイプ:", editServant.IsMagicalType, GUILayout.Width(250));
					}
				}
				// パラメーター
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					EditorGUILayout.LabelField("パラメータ:", GUILayout.Width(500));
					using (new GUILayout.HorizontalScope())
					{
						EditorGUIUtility.labelWidth = 40;
						Action<Parameter.Type_e> paramSet = (t) =>
						{
							const int allSize = 240;
							using (new GUILayout.HorizontalScope(GUILayout.Width(allSize)))
							{
								const int checkSize = 60;
								const int typeSize = 60;
								const int otherSize = allSize - checkSize - typeSize;
								var param = editServant.Parameter[t];
								param.IsUseDisplay = EditorGUILayout.Toggle($"{t.GetText()}:", param.IsUseDisplay, GUILayout.Width(checkSize));
								if (!param.IsUseDisplay)
								{
									param.Real.Type = PopupRank.Popup(param.Real.Type, typeSize);
									if (param.Real.Type != Rank.Type_e.Other)
										param.Real.Plus = (sbyte)(Popup(param.Real.Plus + 1, PlusStr, otherSize) - 1);
									else
										param.Real.Other = TextField(param.Real.Other, otherSize);
								}
								else
								{
									using (new GUILayout.VerticalScope())
									{
										using (new GUILayout.HorizontalScope())
										{
											param.Real.Type = PopupRank.Popup(param.Real.Type, typeSize);
											if (param.Real.Type != Rank.Type_e.Other)
												param.Real.Plus = (sbyte)(Popup(param.Real.Plus + 1, PlusStr, otherSize) - 1);
											else
												param.Real.Other = TextField(param.Real.Other, otherSize);
										}
										using (new GUILayout.HorizontalScope())
										{
											param.Display.Type = PopupRank.Popup(param.Display.Type, typeSize);
											if (param.Display.Type != Rank.Type_e.Other)
												param.Display.Plus = (sbyte)(Popup(param.Display.Plus + 1, PlusStr, otherSize) - 1);
											else
												param.Display.Other = TextField(param.Display.Other, otherSize);
										}
									}
								}
							}
						};
						using (new GUILayout.VerticalScope())
						{
							using (new GUILayout.HorizontalScope())
							{
								using (new CheckChangeScope(ChangeAtk))
									paramSet(Parameter.Type_e.Physics);
								paramSet(Parameter.Type_e.Toughness);
							}
							using (new GUILayout.HorizontalScope())
							{
								using (new CheckChangeScope(ChangeAtk))
									paramSet(Parameter.Type_e.Agility);
								using (new CheckChangeScope(ChangeNA.Add(ChangeAtk)))
									paramSet(Parameter.Type_e.Magic);
							}
							using (new GUILayout.HorizontalScope())
							{
								paramSet(Parameter.Type_e.Luck);
								paramSet(Parameter.Type_e.NoblePhantasm);
							}
						}
					}
				}

				EditorGUIUtility.labelWidth = 50;
				// Hit数
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					EditorGUILayout.LabelField("Hit:", GUILayout.Width(500));
					using (new GUILayout.HorizontalScope())
					{

						Action<CommandCard_e> setCC = (t) =>
						{
							var cc = editServant.CommandCard;
							cc[t].Hit = (byte)Mathf.Min(IntField($"{t.GetText()}:", cc[t].Hit, 1, 100), 20);
						};
						setCC(CommandCard_e.Quick);
						using (new CheckChangeScope(ChangeNA))
							setCC(CommandCard_e.Arts);
						setCC(CommandCard_e.Buster);
						setCC(CommandCard_e.Extra);
						setCC(CommandCard_e.NoblePhantasm);

					}
				}
				EditorGUIUtility.labelWidth = 100;
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					// ステータス傾向、相性
					using (new GUILayout.HorizontalScope())
					{
						using (new CheckChangeScope(ChangeAtk))
							editServant.StatusTrend = PopupStatusTrend.Popup("ステ傾向:", editServant.StatusTrend, 250);
						editServant.Attributes = PopupAttributes.Popup("相性:", editServant.Attributes, 250);
					}
					// SR補正初期、DR
					using (new GUILayout.HorizontalScope())
					{
						editServant.IsInitialSR = EditorGUILayout.Toggle("SR補正初期:", editServant.IsInitialSR, GUILayout.Width(250));
						EditorGUILayout.LabelField("即死率(DR):", editServant.DR.ToString(), GUILayout.Width(250));
					}
					// スター系
					using (new GUILayout.HorizontalScope())
					{
						EditorGUILayout.LabelField("スター発生率(SR):", editServant.SR.ToString(), GUILayout.Width(250));
						EditorGUILayout.LabelField("スター集中度(SW):", editServant.SW.ToString(), GUILayout.Width(250));
					}
					// N/A、N/D
					using (new GUILayout.HorizontalScope())
					{

						editServant.NA = Math.Max(DeciamlField("N/A:", editServant.NA, 250), 0);
						editServant.ND = Math.Max(DeciamlField("N/D:", editServant.ND, 250), 0);
					}
				}
				// 特性
				using (new GUILayout.HorizontalScope(GUI.skin.box))
				{
					using (new GUILayout.VerticalScope(GUILayout.Width(80)))
					{
						EditorGUILayout.LabelField("特性:", GUILayout.Width(80));
						if (GUILayout.Button("Add", GUILayout.Width(80)) && editServant.Characteristic.All(c => c != ""))
							editServant.Characteristic.Add("");
						if (GUILayout.Button("Remove", GUILayout.Width(80)) && editServant.Characteristic.Count != 0)
							editServant.Characteristic.RemoveAt(editServant.Characteristic.Count - 1);
					}
					using (new GUILayout.VerticalScope())
					{
						using (new GUILayout.HorizontalScope())
						{
							EditorGUILayout.LabelField("既存特性:", GUILayout.Width(120));
							var list = servants.SelectMany(s => s.Characteristic).Distinct().Except(editServant.Characteristic).ToArray();
							if (GUILayout.Button("Add", GUILayout.Width(80)) && list.Length != 0)
								editServant.Characteristic.Add(list[existCharacteristicID]);
							existCharacteristicID = Popup(existCharacteristicID, list, 220);
						}
						for (int i = 0; i < editServant.Characteristic.Count; i++)
						{
							var str = TextField(editServant.Characteristic[i], 420);
							if (!editServant.Characteristic.Where((_, j) => j < i).Contains(str))
								editServant.Characteristic[i] = str;
						}
					}
				}
				// 
				using (new GUILayout.HorizontalScope())
				{
				}
			}

		}
	}
}