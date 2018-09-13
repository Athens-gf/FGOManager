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
			if (EditServant?.Servant.Name == "" || EditServant?.Servant.Name == NewServant)
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
			Setup?.Personality?.Invoke(m_DdPolicy.options.FindIndex(o => o.text == Policy.ToString()));

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

			m_CharacteristicSetter.Characteristic = EditServant?.Servant.Characteristic;
			m_CharacteristicSetter.SetupNodes();
			m_MaterialsSetter.Setup(EditServant?.Servant.SecondComingMaterials, EditServant?.Servant.SkillMaterials);

			OnChangeHP();
			OnChangeATK();
			OnChangeSR();
			OnChangeSW();
			OnChangeNA();
			Setup?.ND?.Invoke(ND);
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
						SaveJsonPng.SaveList(filePath, saveSev);
					filePaths.Add(filePath);
				}
				SaveJsonPng.SaveList(GameData.SavePath, filePaths);
			}
		}

		private static readonly string NewServant = "New Servant";

		public void NewRegist()
		{
			if (ServantNodeList.Any(sn => sn.Servant.Name == "" || sn.Servant.Name == NewServant))
				return;
			int no = 1;
			for (; ServantNodeList.Exists(sn => sn.Servant.No == no); no++) ;
			OnSelect(CreateNode(new ServantBase { Name = NewServant, No = no }));
		}

		public void Copy()
		{
			if (!EditServant || EditServant.Servant.Name == "" || EditServant.Servant.Name == NewServant) return;
			var servant = EditServant.Servant.DeepCopy();
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

		public int No
		{
			get { return EditServant?.Servant.No ?? 1; }
			set
			{
				if (!EditServant) return;
				int val = value;
				for (; ServantNodeList.ReternRemove(EditServant).ToList().Exists(sn => sn.Servant.No == val); val++) ;
				EditServant.Servant.No = val;
				EditServant.SetName();
				if (val != value)
					Setup?.No?.Invoke(No);
				m_ScrollList?.OrderSort(c => c.GetComponent<ServantNode>().Servant.No);
			}
		}

		public string Name
		{
			get { return EditServant?.Servant.Name ?? ""; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Name = value;
				EditServant.SetName();
			}
		}

		public Class_e Class
		{
			get { return EditServant?.Servant.Class ?? Class_e.Beast1; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Class = value;
				OnChangeHP();
				OnChangeATK();
				OnChangeSR();
				OnChangeSW();
				OnChangeNA();
			}
		}
		public void ChangeClass(int _value) => Class = (Class_e)_value;

		public int Rare
		{
			get { return EditServant?.Servant.Rare ?? 0; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Rare = value;
				OnChangeHP();
				OnChangeATK();
			}
		}

		public CommandCard CommandCard
		{
			get { return EditServant?.Servant.CommandCard ?? new CommandCard { Type = CommandCard.Type_e.Q1A1B3 }; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.CommandCard = value;
				OnChangeNA();
			}
		}

		public int FirstATK
		{
			get { return EditServant?.Servant.FirstATK ?? 0; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.FirstATK = value;
				OnChange?.MaxATKMin?.Invoke(EditServant.Servant.FirstATK + 1);
			}
		}

		public int MaxATK
		{
			get { return EditServant?.Servant.MaxATK ?? 0; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.MaxATK = value;
				OnChange?.FirstATKMax?.Invoke(EditServant.Servant.MaxATK - 1);
			}
		}

		public Sex_e Sex { get { return EditServant?.Servant.Sex ?? Sex_e.Female; } set { if (EditServant) EditServant.Servant.Sex = value; } }
		public void ChangeSex(int _value) => Sex = (Sex_e)_value;

		public StatusTrend_e StatusTrend
		{
			get { return EditServant?.Servant.StatusTrend ?? StatusTrend_e.Balance; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.StatusTrend = value;
				OnChangeHP();
				OnChangeATK();
			}
		}
		public void ChangeStatusTrend(int _value) => StatusTrend = _value == 0 ? StatusTrend_e.Altria_Lily : (StatusTrend_e)(3 - _value);

		public Attribute_e Attribute { get { return EditServant?.Servant.Attribute ?? Attribute_e.Man; } set { if (EditServant) EditServant.Servant.Attribute = value; } }
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
			List<string> options = ExEnum.GetEnumIter<T>().ReternRemove(_remove).Select(t => _change(t))
				.ReternAppend(defaultOptions).Distinct().Union(ServantNodeList.Select(sn => _exsit(sn)).Distinct()).ToList();
			SetOption(_dropdown, _current, options);
		}

		public Policy Policy
		{
			get { return EditServant?.Servant.Policy ?? new Policy { Type = Policy.Type_e.Neutral }; }
			set { if (EditServant) EditServant.Servant.Policy = value; }
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
			get { return EditServant?.Servant.Personality ?? new Personality { Type = Personality.Type_e.Moderate }; }
			set { if (EditServant) EditServant.Servant.Personality = value; }
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
			get { return EditServant?.Servant.IsInitialSR ?? false; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.IsInitialSR = value;
				OnChangeSR();
			}
		}

		public bool IsMagicType
		{
			get { return EditServant?.Servant.IsMagicalType ?? false; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.IsMagicalType = value;
				OnChangeATK();
			}
		}

		#region パラメータ
		public Rank ParaPhysics
		{
			get { return EditServant?.Servant.Parameter[Parameter.Type_e.Physics] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Parameter[Parameter.Type_e.Physics] = value;
				OnChangeATK();
			}
		}
		public Rank ParaToughness
		{
			get { return EditServant?.Servant.Parameter[Parameter.Type_e.Toughness] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Parameter[Parameter.Type_e.Toughness] = value;
				OnChangeHP();
			}
		}
		public Rank ParaAgility
		{
			get { return EditServant?.Servant.Parameter[Parameter.Type_e.Agility] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Parameter[Parameter.Type_e.Agility] = value;
				OnChangeATK();
				OnChangeSR();
			}
		}
		public Rank ParaMagic
		{
			get { return EditServant?.Servant.Parameter[Parameter.Type_e.Magic] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Parameter[Parameter.Type_e.Magic] = value;
				OnChangeATK();
				OnChangeNA();
			}
		}
		public Rank ParaLuck
		{
			get { return EditServant?.Servant.Parameter[Parameter.Type_e.Luck] ?? new Rank(); }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Parameter[Parameter.Type_e.Luck] = value;
				OnChangeSW();
			}
		}
		public Rank ParaNoblePhantasm
		{
			get { return EditServant?.Servant.Parameter[Parameter.Type_e.NoblePhantasm] ?? new Rank(); }
			set { if (EditServant) EditServant.Servant.Parameter[Parameter.Type_e.NoblePhantasm] = value; }
		}
		#endregion

		public decimal NA { get { return EditServant?.Servant.NA ?? 0; } set { if (EditServant) EditServant.Servant.NA = value; } }
		public decimal ND { get { return EditServant?.Servant.ND ?? 0; } set { if (EditServant) EditServant.Servant.ND = value; } }

		public string Illustrator
		{
			get { return EditServant?.Servant.Illustrator ?? ""; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.Illustrator = value;
				SetIllustratorOption();
			}
		}
		public void SetIllustratorOption()
			=> SetOption(m_DdIllustrator, Illustrator, ServantNodeList.Select(sn => sn.Servant.Illustrator).ReternAppend("").Distinct().OrderBy(s => s).ToList());

		public string CV
		{
			get { return EditServant?.Servant.CV ?? ""; }
			set
			{
				if (!EditServant) return;
				EditServant.Servant.CV = value;
				SetCVOption();
			}
		}
		public void SetCVOption() => SetOption(m_DdCV, CV, ServantNodeList.Select(sn => sn.Servant.CV).ReternAppend("").Distinct().OrderBy(s => s).ToList());

		/// <summary> HP変更時処理、クラス・レア度・ステータス傾向・パラメータ「耐久」が変更されたとき呼び出し </summary>
		public void OnChangeHP()
		{
			OnChange?.FirstHP?.Invoke(EditServant?.Servant.FirstHP ?? 0);
			OnChange?.MaxHP?.Invoke(EditServant?.Servant.MaxHP ?? 0);
		}

		/// <summary> Atk変更時処理、クラス・レア度・ステータス傾向・パラメータ「筋力・魔力・敏捷」・攻撃タイプが変更されたとき呼び出し </summary>
		public void OnChangeATK()
		{
			OnChange?.FirstATK?.Invoke(EditServant?.Servant.BaseFirstATK ?? 0);
			OnChange?.MaxATK?.Invoke(EditServant?.Servant.BaseMaxATK ?? 0);
		}

		/// <summary> SR変更時処理、クラス・初期SRかどうか・パラメータ「敏捷」が変更されたとき呼び出し </summary>
		public void OnChangeSR() => OnChange?.SR?.Invoke(EditServant?.Servant.SR ?? 0);

		/// <summary> SW変更時処理、クラス・パラメータ「幸運」が変更されたとき呼び出し </summary>
		public void OnChangeSW() => OnChange?.SW?.Invoke(EditServant?.Servant.SW ?? 0);

		/// <summary> NA変更時処理、クラス・Artsカード枚数・Artsヒット数・パラメータ「魔力」が変更されたとき呼び出し </summary>
		public void OnChangeNA() => OnChange?.NA?.Invoke(EditServant?.Servant.BaseNA ?? 0);

	}
}