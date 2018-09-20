using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AthensUtility
{
	/// <summary> ノード管理基底クラス </summary>
	/// <typeparam name="TType">ノードの結合種別、Enum型</typeparam>
	/// <typeparam name="TValue">ノードの持つデータ</typeparam>
	/// <typeparam name="TNode">継承先のクラス自身</typeparam>
	[Serializable]
	public abstract class Node<TType, TValue, TNode>
		where TType : struct, IComparable
		where TNode : Node<TType, TValue, TNode>, new()
	{
		protected List<TNode> m_Childs = new List<TNode>();
		protected TValue m_Value;
		protected List<TNode> m_Children = new List<TNode>();

		/// <summary> 葉ノードとして扱うEnum種別 </summary>
		protected abstract TType LeafType { get; }
		/// <summary> 葉ノードのみがデータを持つかどうか </summary>
		protected virtual bool IsOnlyLeafHaveValue => false;
		/// <summary> 葉ノードのみがデータを持つ場合無効値として扱う値 </summary>
		public virtual TValue ErrorValue { get; }

		/// <summary> このノードの種別 </summary>
		public TType Type { get; protected set; }
		/// <summary> 親ノード </summary>
		public TNode Parent { get; protected set; } = null;
		/// <summary> 子ノードリスト </summary>
		public IList<TNode> Children { get { return m_Children.AsReadOnly(); } protected set { m_Children = new List<TNode>(value); } }
		/// <summary> 兄弟ノードリスト </summary>
		public IList<TNode> Sibling => (Parent?.Children.RemoveRetern((TNode)this).ToList() ?? new List<TNode>()).AsReadOnly();
		/// <summary> 根ノード </summary>
		public TNode Root => Parent?.Root ?? this;
		/// <summary> 根ノードかどうか </summary>
		public bool IsRoot => Parent == null;
		/// <summary> 葉ノードかどうか </summary>
		public bool IsLeaf => IsEqual(Type, LeafType);
		/// <summary> 深さ </summary>
		public int Depth => Parent?.Depth + 1 ?? 0;
		/// <summary> 高さ </summary>
		public int Height => IsLeaf ? 0 : Children.Select(c => c.Height).Max() + 1;
		/// <summary> 値 </summary>
		public TValue Value { get { return (!IsOnlyLeafHaveValue || IsLeaf) ? m_Value : ErrorValue; } set { if (!IsOnlyLeafHaveValue || IsLeaf) m_Value = value; } }

		public static implicit operator TNode(Node<TType, TValue, TNode> _node) => (TNode)_node;

		protected Node() { Type = LeafType; Value = ErrorValue; }
		public Node(TValue _value) { Type = LeafType; Value = _value; }

		protected static bool IsEqual(TType _type0, TType _type1) => _type0.CompareTo(_type1) == 0;

		/// <summary> ノード合成 </summary>
		/// <param name="_node0">ノード0</param>
		/// <param name="_node1">ノード1</param>
		/// <param name="_type">合成種別</param>
		/// <returns>合成後ノード</returns>
		protected virtual TNode Append(Node<TType, TValue, TNode> _node, TType _type)
		{
			if (IsEqual(_type, LeafType)) throw new Exception("葉ノードでは合成できません");
			Node<TType, TValue, TNode> node0 = this.DeepCopy(), node1 = _node.DeepCopy(), newNode = null;
			// node0が葉ノード かつ node1のタイプが_typeと等しい場合
			if (IsEqual(node0.Type, LeafType) && IsEqual(node1.Type, _type))
				return new TNode { Type = _type, Children = node1.Children.InsertRetern(0, (TNode)node0).ToList() };
			if (IsEqual(node0.Type, _type))
			{
				// node0のタイプが_typeと等しい かつ node1が葉ノードの場合
				if (IsEqual(node1.Type, LeafType))
				{
					newNode = new TNode { Type = _type, Children = node0.Children.AddRetern((TNode)node1).ToList() };
					node1.Parent = newNode;
					return newNode;
				}
				// 両ノードのタイプが_typeと等しい場合
				if (IsEqual(node0.Type, _type) && IsEqual(node1.Type, _type))
				{
					newNode = new TNode { Type = _type, Children = node0.Children.AddRetern(node1.Children).ToList() };
					node1.Children.ToList().ForEach(cn => cn.Parent = newNode);
					return newNode;
				}
			}
			newNode = new TNode { Type = _type, Children = new List<TNode> { node0, node1 } };
			node0.Parent = newNode;
			node1.Parent = newNode;
			return newNode;
		}
	}


	[DebuggerDisplay("AOA {ToString()}"), Serializable]
	public abstract class AndOrArrow<TValue, TNode> : Node<AndOrArrow<TValue, TNode>.Type_e, TValue, TNode>
		where TNode : Node<AndOrArrow<TValue, TNode>.Type_e, TValue, TNode>, new()
	{
		public enum Type_e { None, One, And, Or, Arrow, }
		/// <summary> 葉ノードとして扱うEnum種別 </summary>
		protected override Type_e LeafType => Type_e.One;
		/// <summary> 葉ノードのみがデータを持つかどうか </summary>
		protected override bool IsOnlyLeafHaveValue => true;
		/// <summary> 葉ノードのみがデータを持つ場合無効値として扱う値 </summary>
		public abstract override TValue ErrorValue { get; }

		public AndOrArrow() : base() { }
		public AndOrArrow(TValue _value) : base(_value) { }

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
					Children.ToList().ForEach(asa => str += (str == "" ? "( " : " & ") + asa.ToString());
					return str + " )";
				case Type_e.Or:
					Children.ToList().ForEach(asa => str += (str == "" ? "( " : " | ") + asa.ToString());
					return str + " )";
				case Type_e.Arrow:
					Children.ToList().ForEach(asa => str += (str == "" ? "( " : " -> ") + asa.ToString());
					return str + " )";
				default:
					return "";
			}
		}
	}
}
