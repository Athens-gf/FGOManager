using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using KMUtility;
using KMUtility.Unity;
using KMUtility.Editor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FGOManager.Editor
{
	public class EditorServantRegist : EditorWindow
	{
		readonly string[] Rares = new string[] { "_", "☆", "☆☆", "☆☆☆", "☆☆☆☆", "☆☆☆☆☆" };
		readonly string[] PlusStr = new string[] { "-", " ", "+", "++" };
		readonly string[] SecondStr = new string[] { "第一", "第二", "第三", "最終" };
		private PopupItems<Class_e> PopupClass = new PopupItems<Class_e>(x => x.GetShortName(), x => (int)x);
		private PopupItems<CommandCard.Type_e> PopupCommandCard = new PopupItems<CommandCard.Type_e>(x => x.ToString(), x => (int)x);
		private PopupItems<Attributes_e> PopupAttributes = new PopupItems<Attributes_e>(x => x.GetText(), x => (int)x);
		private PopupItems<Policy_e> PopupPolicy = new PopupItems<Policy_e>(x => x.GetText(), x => (int)x);
		private PopupItems<Personality.Type_e> PopupPersonality = new PopupItems<Personality.Type_e>(x => x.GetText(), x => (int)x);
		private PopupItems<Sex_e> PopupSex = new PopupItems<Sex_e>(x => x.GetText(), x => (int)x);
		private PopupItems<StatusTrend_e> PopupStatusTrend = new PopupItems<StatusTrend_e>(x => x.GetText(), x => -(int)x);
		private PopupItems<Rank.Type_e> PopupRank = new PopupItems<Rank.Type_e>(x => x.ToString(), x => -(int)x);

		public class MultiPopupMaterial
		{
			public ExMultiPopupItems<Material_e> ExMPMaterial { get; private set; }
			public MultiPopupMaterial(List<Func<Material_e, bool>> _fillters)
			{
				ExMPMaterial = new ExMultiPopupItems<Material_e>(x => x.GetText(), x => (int)x, _fillters);
			}

			public void Popup(string _label, MaterialNumber _materialNumber)
			{
				using (new GUILayout.HorizontalScope(GUI.skin.box, GUILayout.Width(280)))
				{
					EditorGUIUtility.labelWidth = 100;
					using (new GUILayout.VerticalScope(GUILayout.Width(100)))
					{
						EditorGUILayout.LabelField(_label, GUILayout.Width(100));
						List<Material_e> lMat = _materialNumber.Table.Where(x => x.Value > 0).Select(x => x.Key).ToList();
						Dictionary<Material_e, int> table = new Dictionary<Material_e, int>();
						using (new CheckChangeScope(() => _materialNumber.Table = table))
							table = ExMPMaterial.Popup(lMat, false, 100).ToDictionary(m => m, m => _materialNumber[m] == 0 ? 1 : _materialNumber[m]);

					}
					EditorGUIUtility.labelWidth = 140;
					using (new GUILayout.VerticalScope(GUILayout.Width(170)))
					{
						Dictionary<Material_e, int> table = new Dictionary<Material_e, int>();
						using (new CheckChangeScope(() => _materialNumber.Table = table))
						{
							var buffer = new Dictionary<Material_e, int>();
							foreach (var key in _materialNumber.Keys)
							{
								if (_materialNumber[key] != 0)
									buffer[key] = IntField(key.GetText(), _materialNumber[key], 0, 170);
							}
							table = buffer.Where(d => d.Value != 0).ToDictionary(x => x.Key, x => x.Value);
						}
					}
				}
			}
		}

		private MultiPopupMaterial MPMaterialSC = new MultiPopupMaterial(new List<Func<Material_e, bool>>
				{ x => x.IsType(MaterialType_e.Copper), x => x.IsType(MaterialType_e.Silver), x => x.IsType(MaterialType_e.Gold),
					x => x.IsPieceMonument() || x == Material_e.DistributionMaterial});
		private MultiPopupMaterial MPMaterialSkill = new MultiPopupMaterial(new List<Func<Material_e, bool>>
				{ x => x.IsType(MaterialType_e.Copper), x => x.IsType(MaterialType_e.Silver), x => x.IsType(MaterialType_e.Gold),
					x => x.IsSkillStone() || x == Material_e.TraditionalCrystal });

		private PopupChangeList pclPersonality = new PopupChangeList();
		private PopupChangeList pclIllustrator = new PopupChangeList();
		private PopupChangeList pclCV = new PopupChangeList();

		List<ServantBase> servants = new List<ServantBase>();
		ServantBase editServant = null;
		int selectID = 0;
		Action ChangeAtk = null, ChangePersonality = null, ChangeNA = null, ChangeIllustrator = null, ChangeCV = null, ChangeMaterial = null;

		int existCharacteristicID = 0;

		//		[MenuItem("Editor/Regist Servant")]
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
			if (ChangeNA == null)
				ChangeNA = () => editServant.NA = editServant.BaseNA;

			if (ChangePersonality == null)
				ChangePersonality = () => editServant.Personality.OtherStr = pclPersonality.Str;
			if (ChangeIllustrator == null)
				ChangeIllustrator = () => editServant.Illustrator = pclIllustrator.Str;
			if (ChangeCV == null)
				ChangeCV = () => editServant.CV = pclCV.Str;
			if (ChangeMaterial == null)
			{
				ChangeMaterial = () =>
				{
					for (int i = 0; i < editServant.SecondComingMaterials.Length; i++)
					{
						var mat = editServant.SecondComingMaterials[i];
						foreach (Material_e m in Enum.GetValues(typeof(Material_e)).Cast<Material_e>().Where(m => m.IsPieceMonument()))
						{
							if (m.GetClass() == editServant.Class && (i < 2 ? m.IsType(MaterialType_e.Piece) : m.IsType(MaterialType_e.Monument)))
								mat[m] = i % 2 == 0 ? editServant.Rare <= 3 ? editServant.Rare + 1 : editServant.Rare : (editServant.Rare + 1) * 2;
							else
								mat[m] = 0;
						}
					}
					for (int i = 0; i < 6; i++)
					{
						var mat = editServant.SkillMaterials[i];
						foreach (Material_e m in Enum.GetValues(typeof(Material_e)).Cast<Material_e>().Where(m => m.IsSkillStone()))
						{
							if (m.GetClass() == editServant.Class &&
							(i < 2 ? m.IsType(MaterialType_e.Pyroxene) : (i < 4 ? m.IsType(MaterialType_e.Manastone) : m.IsType(MaterialType_e.SecretStone))))
								mat[m] = i % 2 == 0 ? editServant.Rare <= 3 ? editServant.Rare + 1 : editServant.Rare : (editServant.Rare + 1) * 2;
							else
								mat[m] = 0;
						}
					}
				};
			}

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
						using (new GUILayout.HorizontalScope())
						{
							Copy();
						}
						ScrollList();
					}
				}
				using (new GUILayout.VerticalScope(GUI.skin.box, GUILayout.Width(600)))
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
				string filePath = EditorUtility.OpenFilePanel("Load", "Data", "png");
				if (filePath != "")
				{
					var newSev = SaveJsonPng.LoadList<ServantBase>(filePath);
					servants = servants.Where(s => !newSev.Exists(ns => ns.No == s.No)).ToList();
					servants.AddRange(newSev);
					servants = servants.OrderBy(s => s.No).ToList();
				}
			}
		}

		public void Save()
		{
			if (GUILayout.Button("Save") && servants.Any())
			{
				List<string> filePaths = new List<string>();
				int count = (servants.Max(s => s.No) - 1) / 10 + 1;
				for (int i = 0; i < count; i++)
				{
					List<ServantBase> saveSev = servants.Where(s => s.No > i * 10 && s.No <= (i + 1) * 10).ToList();
					string filePath = $"Data/sev{i}.png";
					if (saveSev.Any())
						SaveJsonPng.SaveList(filePath, saveSev);
					filePaths.Add(filePath);
				}
				SaveJsonPng.SaveList("Data/sevPath.png", filePaths);
			}
		}

		public void AllReset()
		{
			if (GUILayout.Button("Reset"))
				servants.Clear();
		}

		public void Copy()
		{
			if (GUILayout.Button("Copy") && servants.Any())
			{
				editServant = servants[selectID].DeepCopy();
				for (editServant.No = 1; servants.Exists(s => s.No == editServant.No); editServant.No++) ;
				servants.Add(editServant);
				servants = servants.OrderBy(s => s.No).ToList();
				selectID = servants.Select((s, i) => new { s.No, Index = i }).First(x => x.No == editServant.No).Index;
				ChangeAtk();
				ChangeNA();
			}
		}

		public void ScrollList()
		{
			using (new ScrollView("List", GUI.skin.box))
			{
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
				ChangeMaterial();
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

		public static string TextField(string _label, string _text, int _width, params GUILayoutOption[] _options)
			=> EditorGUILayout.TextField(_label, _text, EditOptions(_options, _width));
		public static string TextField(string _text, int _width, params GUILayoutOption[] _options)
			=> EditorGUILayout.TextField(_text, EditOptions(_options, _width));
		public static int IntField(string _label, int _value, int _min, int _width, params GUILayoutOption[] _options)
			=> Mathf.Max(EditorGUILayout.IntField(_label, _value, EditOptions(_options, _width)), _min);
		public static decimal DeciamlField(string _label, decimal _value, int _width, params GUILayoutOption[] _options)
			=> (decimal)EditorGUILayout.DoubleField(_label, (double)_value, EditOptions(_options, _width));
		public static decimal DeciamlField(decimal _value, int _width, params GUILayoutOption[] _options)
			=> (decimal)EditorGUILayout.DoubleField((double)_value, EditOptions(_options, _width));

		public void Register()
		{
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
					using (new CheckChangeScope(ChangeNA.Add(ChangeAtk).Add(ChangeMaterial)))
						editServant.Class = PopupClass.Popup("Class:", editServant.Class, 250);
					using (new CheckChangeScope(ChangeAtk.Add(ChangeMaterial)))
						editServant.Rare = (byte)ExEditor.Popup("レア度:", editServant.Rare, Rares, 250);
				}
				// コマンドカード配分、Cost
				using (new GUILayout.HorizontalScope())
				{
					using (new CheckChangeScope(ChangeNA))
						editServant.CommandCard.Type = PopupCommandCard.Popup("コマンドカード:", editServant.CommandCard.Type, 250);
					EditorGUILayout.LabelField("Cost:", editServant.Cost.ToString(), GUILayout.Width(250));
				}
			}
			using (new ScrollView("Edit", GUI.skin.box, GUILayout.Width(600)))
			{
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
						editServant.Personality.Type = PopupPersonality.Popup("性格:", editServant.Personality.Type, 150);
						if (editServant.Personality.Type == Personality.Type_e.Other)
						{
							var list = servants.Where(s => s != editServant && s.Personality.Type == Personality.Type_e.Other)
								.Select(s => s.Personality.OtherStr).Where(s => s != "").OrderBy(s => s).Distinct();
							editServant.Personality.OtherStr = TextField(editServant.Personality.OtherStr, list.Any() ? 50 : 100);
							if (list.Any())
							{
								using (new CheckChangeScope(ChangePersonality))
									pclPersonality.Popup(list, 50);
							}
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
										param.Real.Plus = (sbyte)(ExEditor.Popup(param.Real.Plus + 1, PlusStr, otherSize) - 1);
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
												param.Real.Plus = (sbyte)(ExEditor.Popup(param.Real.Plus + 1, PlusStr, otherSize) - 1);
											else
												param.Real.Other = TextField(param.Real.Other, otherSize);
										}
										using (new GUILayout.HorizontalScope())
										{
											param.Display.Type = PopupRank.Popup(param.Display.Type, typeSize);
											if (param.Display.Type != Rank.Type_e.Other)
												param.Display.Plus = (sbyte)(ExEditor.Popup(param.Display.Plus + 1, PlusStr, otherSize) - 1);
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
							existCharacteristicID = ExEditor.Popup(existCharacteristicID, list, 220);
						}
						for (int i = 0; i < editServant.Characteristic.Count; i++)
						{
							var str = TextField(editServant.Characteristic[i], 420);
							if (!editServant.Characteristic.Where((_, j) => j < i).Contains(str))
								editServant.Characteristic[i] = str;
						}
					}
				}
				EditorGUIUtility.labelWidth = 100;
				// イラストレーター、CV
				using (new GUILayout.HorizontalScope(GUI.skin.box))
				{
					editServant.Illustrator = TextField("イラストレーター:", editServant.Illustrator, 200);
					using (new CheckChangeScope(ChangeIllustrator))
						pclIllustrator.Popup(servants.Where(s => s != editServant).Select(s => s.Illustrator).Where(s => s != "").OrderBy(s => s).Distinct(), 50);

					editServant.CV = TextField("CV:", editServant.CV, 200);
					using (new CheckChangeScope(ChangeCV))
						pclCV.Popup(servants.Where(s => s != editServant).Select(s => s.CV).Where(s => s != "").OrderBy(s => s).Distinct(), 50);
				}
				EditorGUIUtility.labelWidth = 100;
				// 再臨素材
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					EditorGUILayout.LabelField("再臨素材:", GUILayout.Width(500));
					for (int i = 0; i < 2; i++)
					{
						using (new GUILayout.HorizontalScope())
						{
							for (int j = 0; j < 2; j++)
								MPMaterialSC.Popup($"{SecondStr[i * 2 + j]}再臨:", editServant.SecondComingMaterials[i * 2 + j]);
						}
					}
				}
				// スキル強化素材
				using (new GUILayout.VerticalScope(GUI.skin.box))
				{
					EditorGUILayout.LabelField("スキル強化素材:", GUILayout.Width(500));
					for (int i = 0; i < 4; i++)
					{
						using (new GUILayout.HorizontalScope())
						{
							for (int j = 0; j < 2; j++)
							{
								int num = i * 2 + j;
								MPMaterialSkill.Popup($"{num + 1} -> {num + 2}:", editServant.SkillMaterials[num]);
							}
						}
					}
				}
			}
		}
	}
}