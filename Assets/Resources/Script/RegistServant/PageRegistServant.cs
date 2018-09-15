using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KMUtility;
using KMUtility.Unity;
using KMUtility.Unity.UI;
using KMUtility.Unity.Page;

namespace FGOManager.Register
{
	public class PageRegistServant : PageBase
	{
		#region 宣言

		[Serializable]
		public class Setup_ic
		{
			public IntEvent No = null;
			public StringEvent Name = null;
			public IntEvent Class = null;
			public IntEvent Rare = null;
			public CommandCardEvent CommandCard = null;
			public IntEvent Sex = null;
			public IntEvent Policy = null;
			public IntEvent Personality = null;
			public BoolEvent IsInitialSR = null;
			public BoolEvent IsMagicType = null;
			public RankEvent RankPhysics = null;
			public RankEvent RankToughness = null;
			public RankEvent RankAgility = null;
			public RankEvent RankMagic = null;
			public RankEvent RankLuck = null;
			public RankEvent RankNoblePhantasm = null;
			public IntEvent StatusTrend = null;
			public IntEvent Attribute = null;

			public DecimalEvent ND = null;

			public StringEvent Illustrator = null;
			public StringEvent CV = null;
		}
		[Serializable]
		public class Change_ic
		{
			public IntEvent FirstHP = null;
			public IntEvent MaxHP = null;
			public IntEvent FirstATK = null;
			public IntEvent FirstATKMax = null;
			public IntEvent MaxATK = null;
			public IntEvent MaxATKMin = null;

			public DecimalEvent SR = null;
			public DecimalEvent SW = null;
			public DecimalEvent NA = null;
		}
		#endregion

		#region インスペクタ表示
		[SerializeField]
		private ServantNode m_PrefabServantNameNode = null;
		[SerializeField]
		private CustomUIScrollView m_ScrollList = null;
		[SerializeField]
		private CharacteristicSetter m_CharacteristicSetter = null;
		[SerializeField]
		private MaterialsSetter m_MaterialsSetter = null;
		[SerializeField]
		private Dropdown m_DdPolicy = null, m_DdPersonality = null, m_DdIllustrator = null, m_DdCV = null;

		public Setup_ic Setup = null;
		public Change_ic OnChange = null;
		#endregion

		public List<ServantNode> ServantNodeList { get; private set; } = new List<ServantNode>();
		public ServantNode EditServant { get; private set; } = null;
		private bool IsSetting { get; set; } = false;

		private void Start()
		{
			GameData.Instance.Servants.ForEach(servant => CreateNode(servant));
			OnSelect(ServantNodeList.Any() ? ServantNodeList.First() : null);
			SetPersonalityOption();
		}

		#region Menu系
		public ServantNode CreateNode(ServantBase _servant)
		{
			ServantNode node = Instantiate(m_PrefabServantNameNode);
			node.Servant = _servant;
			node.OnClick = () => OnSelect(node);
			ServantNodeList.Add(node);
			m_ScrollList?.AddContent(node);
			m_ScrollList?.OrderSort(c => c.GetComponent<ServantNode>().Servant.No);
			return node;
		}

		public void OnSelect(ServantNode _servant)
		{
			if (EditServant != null && EditServant == _servant) return;
			IsSetting = true;
			if (Servant?.Name == "" || Servant?.Name == NewServant)
				Delete();
			EditServant = _servant;
			Setup?.No?.Invoke(No);
			Setup?.Name?.Invoke(Name);
			Setup?.Class?.Invoke((int)Class);
			Setup?.Rare?.Invoke(Rare);
			Setup?.CommandCard?.Invoke(CommandCard);
			Setup?.Sex?.Invoke((int)Sex);
			Setup?.StatusTrend?.Invoke(StatusTrend == StatusTrend_e.Altria_Lily ? 0 : 3 - (int)StatusTrend);
			Setup?.Attribute?.Invoke((int)Attribute);

			SetPolicyOption();
			Setup?.Policy?.Invoke(m_DdPolicy.options.FindIndex(o => o.text == Policy.ToString()));
			SetPersonalityOption();
			Setup?.Personality?.Invoke(m_DdPersonality.options.FindIndex(o => o.text == Personality.ToString()));

			SetIllustratorOption();
			Setup?.Illustrator?.Invoke(Illustrator);
			SetCVOption();
			Setup?.CV?.Invoke(CV);

			Setup?.IsInitialSR?.Invoke(IsInitialSR);
			Setup?.IsMagicType?.Invoke(IsMagicType);

			Setup?.RankPhysics?.Invoke(ParaPhysics);
			Setup?.RankToughness?.Invoke(ParaToughness);
			Setup?.RankAgility?.Invoke(ParaAgility);
			Setup?.RankMagic?.Invoke(ParaMagic);
			Setup?.RankLuck?.Invoke(ParaLuck);
			Setup?.RankNoblePhantasm?.Invoke(ParaNoblePhantasm);

			m_CharacteristicSetter.Characteristic = Servant?.Characteristic;
			m_CharacteristicSetter.SetupNodes();
			m_MaterialsSetter.AllClear();
			m_MaterialsSetter.Setup(Servant?.SecondComingMaterials, Servant?.SkillMaterials);

			OnChangeHP();
			OnChangeATK();
			OnChangeSR();
			OnChangeSW();
			OnChangeNA();
			Setup?.ND?.Invoke(ND);
			IsSetting = false;
		}

		public void Save()
		{
			var servants = GameData.Instance.Servants;
			if (servants.Any())
			{
				List<string> filePaths = new List<string>();
				int count = (servants.Max(s => s.No) - 1) / 10 + 1;
				for (int i = 0; i < count; i++)
				{
					List<ServantBase> saveSev = servants.Where(s => s.No > i * 10 && s.No <= (i + 1) * 10).ToList();
					string filePath = $"Data/sev{i}.png";
					if (saveSev.Any())
					{
						SaveJsonPng.Save(filePath, saveSev);
						filePaths.Add(filePath);
					}
				}
				SaveJsonPng.Save(GameData.SavePath, filePaths);
			}
		}

		private static readonly string NewServant = "New Servant";

		public void NewRegist()
		{
			if (ServantNodeList.Any(sn => sn.Servant.Name == "" || sn.Servant.Name == NewServant))
				return;
			int no = 1;
			for (; ServantNodeList.Exists(sn => sn.Servant.No == no); no++) ;
			ServantBase newServant = new ServantBase { Name = NewServant, No = no };
			GameData.Instance.Servants.Add(newServant);
			OnSelect(CreateNode(newServant));
			OnChangeMaterials();
		}

		public void Copy()
		{
			if (!EditServant || Servant.Name == "" || Servant.Name == NewServant) return;
			var servant = Servant.DeepCopy();
			for (servant.No = 1; ServantNodeList.Exists(sn => sn.Servant.No == servant.No); servant.No++) ;
			OnSelect(CreateNode(servant));
		}

		public void Delete()
		{
			if (!EditServant) return;
			ServantNodeList.RemoveAll(sn => sn == EditServant);
			m_ScrollList?.RemoveContent(EditServant);
			EditServant = null;
		}
		#endregion

		public ServantBase Servant => EditServant?.Servant;

		public int No
		{
			get { return Servant?.No ?? 1; }
			set
			{
				if (!EditServant || Servant.No == value) return;
				int val = value;
				for (; ServantNodeList.RemoveRetern(EditServant).ToList().Exists(sn => sn.Servant.No == val); val++) ;
				Servant.No = val;
				EditServant.SetName();
				Setup?.No?.Invoke(No);
				m_ScrollList?.OrderSort(c => c.GetComponent<ServantNode>().Servant.No);
			}
		}

		public string Name
		{
			get { return Servant?.Name ?? ""; }
			set
			{
				if (!EditServant || Name == value) return;
				Servant.Name = value;
				Setup?.Name?.Invoke(Name);
				EditServant.SetName();
			}
		}

		public Class_e Class
		{
			get { return Servant?.Class ?? Class_e.Beast1; }
			set
			{
				if (!EditServant || Class == value) return;
				Servant.Class = value;
				Setup?.Class?.Invoke((int)Class);
				OnChangeHP();
				OnChangeATK();
				OnChangeSR();
				OnChangeSW();
				OnChangeNA();
				OnChangeMaterials();
			}
		}
		public void ChangeClass(int _value) => Class = (Class_e)_value;

		public int Rare
		{
			get { return Servant?.Rare ?? 0; }
			set
			{
				if (!EditServant) return;
				Servant.Rare = value;
				OnChangeHP();
				OnChangeATK();
				OnChangeMaterials();
			}
		}

		public CommandCard CommandCard
		{
			get { return Servant?.CommandCard ?? new CommandCard { Type = CommandCard.Type_e.Q1A1B3 }; }
			set
			{
				if (!EditServant || CommandCard.ToString() == value.ToString()) return;
				Servant.CommandCard = value;
				Setup?.CommandCard?.Invoke(CommandCard);
				OnChangeNA();
			}
		}

		public int FirstATK
		{
			get { return Servant?.FirstATK ?? 0; }
			set
			{
				if (!EditServant) return;
				Servant.FirstATK = value;
				OnChange?.MaxATKMin?.Invoke(Servant.FirstATK + 1);
			}
		}

		public int MaxATK
		{
			get { return Servant?.MaxATK ?? 0; }
			set
			{
				if (!EditServant) return;
				Servant.MaxATK = value;
				OnChange?.FirstATKMax?.Invoke(Servant.MaxATK - 1);
			}
		}

		public Sex_e Sex
		{
			get { return Servant?.Sex ?? Sex_e.Female; }
			set
			{
				if (!EditServant || Sex == value) return;
				Servant.Sex = value;
				Setup?.Sex?.Invoke((int)Sex);
			}
		}
		public void ChangeSex(int _value) => Sex = (Sex_e)_value;

		public StatusTrend_e StatusTrend
		{
			get { return Servant?.StatusTrend ?? StatusTrend_e.Balance; }
			set
			{
				if (!EditServant) return;
				Servant.StatusTrend = value;
				OnChangeHP();
				OnChangeATK();
			}
		}
		public void ChangeStatusTrend(int _value) => StatusTrend = _value == 0 ? StatusTrend_e.Altria_Lily : (StatusTrend_e)(3 - _value);

		public Attribute_e Attribute
		{
			get { return Servant?.Attribute ?? Attribute_e.Man; }
			set
			{
				if (!EditServant || Attribute == value) return;
				Servant.Attribute = value;
				Setup?.Attribute?.Invoke((int)Attribute);
			}
		}
		public void ChangeAttribute(int _value) => Attribute = (Attribute_e)_value;

		public void AddOtherOption(string _str, Dropdown _dropdown)
		{
			if (!_dropdown || _str == "" || _dropdown.options.Exists(op => op.text == _str)) return;
			_dropdown.AddOptions(_str.MakeCollection().ToList());
			_dropdown.value = m_DdPolicy.options.Count - 1;
		}
		public void SetOption(Dropdown _dropdown, string _current, List<string> _options)
		{
			if (!_dropdown) return;
			_dropdown.ClearOptions();
			_dropdown.AddOptions(_options);
			_dropdown.value = _options.Any() ? _options.IndexOf(_current) : 0;
		}
		public void SetOption<T>(Dropdown _dropdown, string _current, T _remove,
			Func<T, string> _change, Func<ServantNode, string> _exsit, params string[] defaultOptions) where T : struct
		{
			List<string> options = ExEnum.GetEnumIter<T>().RemoveRetern(_remove).Select(t => _change(t))
				.AddRetern(defaultOptions).Distinct().Union(ServantNodeList.Select(sn => _exsit(sn)).Distinct()).ToList();
			SetOption(_dropdown, _current, options);
		}

		public Policy Policy
		{
			get { return Servant?.Policy ?? new Policy { Type = Policy.Type_e.Neutral }; }
			set { if (EditServant) Servant.Policy = value; }
		}
		public void ChangePolicy(int _value)
		{
			Policy = (_value < (int)Policy.Type_e.Other) ? new Policy { Type = (Policy.Type_e)_value } :
				new Policy { Type = Policy.Type_e.Other, OtherStr = m_DdPolicy?.options[_value].text ?? "" };
			SetPolicyOption();
		}
		public void SetPolicyOption() => SetOption(m_DdPolicy, Policy.ToString(), Policy.Type_e.Other, t => t.GetText(), sn => sn.Servant.Policy.ToString());

		public void AddOtherPolicy(string _str) => AddOtherOption(_str, m_DdPolicy);

		public Personality Personality
		{
			get { return Servant?.Personality ?? new Personality { Type = Personality.Type_e.Moderate }; }
			set { if (EditServant) Servant.Personality = value; }
		}
		public void ChangePersonality(int _value)
		{
			Personality = (_value < (int)Personality.Type_e.Other) ? new Personality { Type = (Personality.Type_e)_value } :
				new Personality { Type = Personality.Type_e.Other, OtherStr = m_DdPersonality?.options[_value].text ?? "" };
			SetPersonalityOption();
		}
		public void SetPersonalityOption()
			=> SetOption(m_DdPersonality, Personality.ToString(), Personality.Type_e.Other, t => t.GetText(), sn => sn.Servant.Personality.ToString(), "狂", "花嫁", "夏");
		public void AddOtherPersonality(string _str) => AddOtherOption(_str, m_DdPersonality);

		public bool IsInitialSR
		{
			get { return Servant?.IsInitialSR ?? false; }
			set
			{
				if (!EditServant) return;
				Servant.IsInitialSR = value;
				OnChangeSR();
			}
		}

		public bool IsMagicType
		{
			get { return Servant?.IsMagicalType ?? false; }
			set
			{
				if (!EditServant) return;
				Servant.IsMagicalType = value;
				OnChangeATK();
			}
		}

		#region パラメータ
		public Rank ParaPhysics
		{
			get { return Servant?.Parameter[Parameter.Type_e.Physics] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				Servant.Parameter[Parameter.Type_e.Physics] = value;
				OnChangeATK();
			}
		}
		public Rank ParaToughness
		{
			get { return Servant?.Parameter[Parameter.Type_e.Toughness] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				Servant.Parameter[Parameter.Type_e.Toughness] = value;
				OnChangeHP();
			}
		}
		public Rank ParaAgility
		{
			get { return Servant?.Parameter[Parameter.Type_e.Agility] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				Servant.Parameter[Parameter.Type_e.Agility] = value;
				OnChangeATK();
				OnChangeSR();
			}
		}
		public Rank ParaMagic
		{
			get { return Servant?.Parameter[Parameter.Type_e.Magic] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				Servant.Parameter[Parameter.Type_e.Magic] = value;
				OnChangeATK();
				OnChangeNA();
			}
		}
		public Rank ParaLuck
		{
			get { return Servant?.Parameter[Parameter.Type_e.Luck] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				Servant.Parameter[Parameter.Type_e.Luck] = value;
				OnChangeSW();
			}
		}
		public Rank ParaNoblePhantasm
		{
			get { return Servant?.Parameter[Parameter.Type_e.NoblePhantasm] ?? new Rank(); }
			set { if (EditServant) Servant.Parameter[Parameter.Type_e.NoblePhantasm] = value; }
		}
		#endregion

		public decimal NA
		{
			get { return Servant?.NA ?? 0; }
			set
			{
				if (!EditServant || NA == value) return;
				Servant.NA = value;
				Setup?.ND?.Invoke(ND);
			}
		}
		public decimal ND
		{
			get { return Servant?.ND ?? 0; }
			set
			{
				if (!EditServant || ND == value) return;
				Servant.ND = value;
				Setup?.ND?.Invoke(ND);
			}
		}

		public string Illustrator
		{
			get { return Servant?.Illustrator ?? ""; }
			set
			{
				if (!EditServant || Illustrator == value) return;
				Servant.Illustrator = value;
				SetIllustratorOption();
				SetIllustratorOption();
				Setup?.Illustrator?.Invoke(Illustrator);
			}
		}
		public void SetIllustratorOption()
			=> SetOption(m_DdIllustrator, Illustrator, ServantNodeList.Select(sn => sn.Servant.Illustrator).AddRetern("").Distinct().OrderBy(s => s).ToList());

		public string CV
		{
			get { return Servant?.CV ?? ""; }
			set
			{
				if (!EditServant || CV == value) return;
				Servant.CV = value;
				SetCVOption();
				SetCVOption();
				Setup?.CV?.Invoke(CV);
			}
		}
		public void SetCVOption() => SetOption(m_DdCV, CV, ServantNodeList.Select(sn => sn.Servant.CV).AddRetern("").Distinct().OrderBy(s => s).ToList());

		/// <summary> HP変更時処理、クラス・レア度・ステータス傾向・パラメータ「耐久」が変更されたとき呼び出し </summary>
		public void OnChangeHP()
		{
			OnChange?.FirstHP?.Invoke(Servant?.FirstHP ?? 0);
			OnChange?.MaxHP?.Invoke(Servant?.MaxHP ?? 0);
		}

		/// <summary> Atk変更時処理、クラス・レア度・ステータス傾向・パラメータ「筋力・魔力・敏捷」・攻撃タイプが変更されたとき呼び出し </summary>
		public void OnChangeATK()
		{
			OnChange?.FirstATK?.Invoke(Servant?.BaseFirstATK ?? 0);
			OnChange?.MaxATK?.Invoke(Servant?.BaseMaxATK ?? 0);
		}

		/// <summary> SR変更時処理、クラス・初期SRかどうか・パラメータ「敏捷」が変更されたとき呼び出し </summary>
		public void OnChangeSR() => OnChange?.SR?.Invoke(Servant?.SR ?? 0);

		/// <summary> SW変更時処理、クラス・パラメータ「幸運」が変更されたとき呼び出し </summary>
		public void OnChangeSW() => OnChange?.SW?.Invoke(Servant?.SW ?? 0);

		/// <summary> NA変更時処理、クラス・Artsカード枚数・Artsヒット数・パラメータ「魔力」が変更されたとき呼び出し </summary>
		public void OnChangeNA() => OnChange?.NA?.Invoke(Servant?.BaseNA ?? 0);


		/// <summary> 素材変更時処理 </summary>
		public void OnChangeMaterials()
		{
			if (IsSetting) return;
			m_MaterialsSetter.AllClear();
			for (int i = 0; i < 8; i++)
				foreach (var material in Servant.SkillMaterials[i].Keys)
					Servant.SkillMaterials[i][material] = 0;
			for (int i = 0; i < 4; i++)
				foreach (var material in Servant.SecondComingMaterials[i].Keys)
					Servant.SecondComingMaterials[i][material] = 0;
			switch (Class)
			{
				case Class_e.Saber:
				case Class_e.Archer:
				case Class_e.Lancer:
				case Class_e.Rider:
				case Class_e.Caster:
				case Class_e.Assassin:
				case Class_e.Berserker:
					for (int i = 0; i < 6; i++)
					{
						int count = (i % 2 == 0 ? Rare < 4 ? Rare + 1 : Rare : (Rare + 1) * 2);
						if (i < 4)
							m_MaterialsSetter.AddNode(true, i, Class.GetMaterial(i < 2 ? MaterialType_e.Piece : MaterialType_e.Monument), count);
						m_MaterialsSetter.AddNode(false, i, Class.GetMaterial(i < 2 ? MaterialType_e.Pyroxene : i < 3 ? MaterialType_e.Manastone : MaterialType_e.SecretStone), count);
					}
					break;
				default:
					break;
			}
		}


		public void TextSetting(string _str)
		{
			if (EditServant == null) return;
			var splitBuff = _str.Replace("\t", " ").Remove("\r").Split('\n').Select(x => x.Split(' ').RemoveRetern("").ToList()).ToList();
			if (_str.StartsWith("No."))
			{
				int buff;
				if (int.TryParse(splitBuff[0][0].Remove("No."), out buff))
					No = buff;

				Name = splitBuff[1][1];

				Class = ExEnum.GetEnumIter<Class_e>().Where(c => c.GetText() == splitBuff[2][1]).FirstOrDefault();

				buff = 5;
				if (int.TryParse(splitBuff[2][3], out buff))
					Rare = buff;

				int q = 0, a = 0, b = 0;
				int.TryParse(splitBuff[5][0], out q);
				int.TryParse(splitBuff[5][1], out a);
				int.TryParse(splitBuff[5][2], out b);
				CommandCard.Type = (CommandCard.Type_e)((q > 1 ? CommandCard_e.Quick : 0) | (a > 1 ? CommandCard_e.Arts : 0) | (b > 1 ? CommandCard_e.Buster : 0));
				Setup?.CommandCard?.Invoke(CommandCard);
			}
			else if (_str.StartsWith("隠しステータス") || _str.StartsWith("相性"))
			{
				if (_str.StartsWith("隠しステータス"))
					splitBuff = splitBuff.Skip(1).ToList();
				Attribute = ExEnum.GetEnumIter<Attribute_e>().Where(a => a.GetText() == splitBuff[0][1]).FirstOrDefault();

				Policy.Type = ExEnum.GetEnumIter<Policy.Type_e>().Where(a => a.GetText() == splitBuff[1][2]).FirstOrDefault(Policy.Type_e.Other);
				if (Policy.Type == Policy.Type_e.Other) Policy.OtherStr = splitBuff[1][2];
				SetPolicyOption();
				Setup?.Policy?.Invoke(m_DdPolicy.options.FindIndex(o => o.text == Policy.ToString()));

				Personality.Type = ExEnum.GetEnumIter<Personality.Type_e>().Where(a => a.GetText() == splitBuff[1][3]).FirstOrDefault(Personality.Type_e.Other);
				if (Personality.Type == Personality.Type_e.Other) Personality.OtherStr = splitBuff[1][3];
				SetPersonalityOption();
				Setup?.Personality?.Invoke(m_DdPersonality.options.FindIndex(o => o.text == Personality.ToString()));

				Sex = ExEnum.GetEnumIter<Sex_e>().Where(a => a.GetText().Remove("性") == splitBuff[1][4]).FirstOrDefault(Sex_e.Other);

				for (int i = 0; i < 5; i++)
				{
					sbyte buff;
					if (sbyte.TryParse(splitBuff[3][i], out buff))
						CommandCard[(CommandCard_e)(1 << i)].Hit = buff;
				}
				Setup?.CommandCard?.Invoke(CommandCard);

				decimal buffD;
				if (decimal.TryParse(splitBuff[4].Last(), out buffD))
					ND = buffD;

				Servant.Characteristic.Clear();
				splitBuff[5].Skip(3).RemoveRetern("/").ToList().ForEach(s => Servant.Characteristic.Add(s));
				if (!Servant.Characteristic.Remove("ｴﾇﾏ特攻無効"))
					Servant.Characteristic.Add("エヌマ・エリシュ");
				m_CharacteristicSetter.SetupNodes();
			}
			else if (_str.StartsWith("ILLUST"))
			{
				Illustrator = splitBuff[0][1];
				CV = splitBuff[1][1];
			}
			else if (_str.StartsWith("筋力"))
			{
				(new[] { Setup?.RankPhysics, Setup?.RankToughness, Setup?.RankAgility, Setup?.RankMagic, Setup?.RankLuck, Setup?.RankNoblePhantasm })
					.Select((setup, i) => new { Setup = setup, Rank = Rank.FromString(splitBuff[i / 2][(i % 2) * 2 + 1]) })
					.ToList()
					.ForEach(x => x.Setup?.Invoke(x.Rank));
			}
			else if (_str.StartsWith("霊基再臨"))
			{
				splitBuff = _str.Replace("\t", " ").Remove("\r").Split('\n').Select(x => x.Split(' ').ToList()).ToList();
				m_MaterialsSetter.AllClear();
				Servant.SecondComingMaterials.ToList().ForEach(m => m.Clear());
				int line = 0;
				for (int i = 0; i < 2; i++)
				{
					for (; splitBuff[line][0] != "素材"; line++) ;
					line++;
					for (; splitBuff[line][0] != "QP"; line++)
					{
						for (int j = 0; j < 2; j++)
						{
							string strMat = splitBuff[line][j * 2];
							if (strMat != "")
								Servant.SecondComingMaterials[(i * 2) + j][strMat.GetEnumByText<Material_e>()] = int.Parse(splitBuff[line][j * 2 + 1]);
						}
					}
				}
				m_MaterialsSetter?.Setup(Servant?.SecondComingMaterials, Servant?.SkillMaterials);
			}
			else if (_str.StartsWith("レベル"))
			{

				splitBuff = _str.Replace("\t", " ").Remove("\r").Split('\n').Select(x => x.Split(' ').ToList()).ToList();
				m_MaterialsSetter.AllClear();
				Servant.SkillMaterials.ToList().ForEach(m => m.Clear());
				int line = 0;
				for (; splitBuff[line][0] != "1"; line++) ;
				line++;
				for (int step = -1; step < 8; line++)
				{
					string str = "";
					if (char.IsNumber(splitBuff[line][0][0]))
					{
						step++;
						str = splitBuff[line][2];
					}
					else
						str = splitBuff[line][0];
					var spStr = str.Split('×');
					Servant.SkillMaterials[step][spStr[0].GetEnumByText<Material_e>()] = int.Parse(spStr[1]);
				}
				m_MaterialsSetter?.Setup(Servant?.SecondComingMaterials, Servant?.SkillMaterials);
			}
		}
	}
}