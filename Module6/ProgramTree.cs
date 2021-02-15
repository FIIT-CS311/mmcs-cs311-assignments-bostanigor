using System.Collections.Generic;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public class Node // базовый класс для всех узлов    
    {
    }

    public class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
    }

    public class BinaryNode : ExprNode
    {
        public ExprNode Left { get; set; }

        public ExprNode Right { get; set; }
        public char Operation { get; set; }

        public BinaryNode(ExprNode left, ExprNode right, char operation)
        {
            Left = left;
            Right = right;
            Operation = operation;
        }
    }

    public class StatementNode : Node // базовый класс для всех операторов
    {
    }

    public class AssignNode : StatementNode
    {
        public IdNode Id { get; set; }
        public ExprNode Expr { get; set; }
        public AssignType AssOp { get; set; }
        public AssignNode(IdNode id, ExprNode expr, AssignType assop = AssignType.Assign)
        {
            Id = id;
            Expr = expr;
            AssOp = assop;
        }
    }

    public class CycleNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public CycleNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class WhileNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public WhileNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class RepeatNode : StatementNode
    {
        public BlockNode Block { get; set; }
        public ExprNode Expr { get; set; }
        public RepeatNode(BlockNode block, ExprNode expr)
        {
            Block = block;
            Expr = expr;
        }
    }

    public class ForNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public StatementNode AssignNode { get; set; }
        public ForNode(StatementNode assignNode, ExprNode expr, StatementNode stat)
        {
            AssignNode = assignNode;
            Expr = expr;
            Stat = stat;
        }
    }

    public class IfNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public IfNode(ExprNode expr, StatementNode stat)
        {
            Expr = expr;
            Stat = stat;
        }
    }

    public class IfElseNode : IfNode
    {
        public ExprNode Expr { get; set; }
        public StatementNode Stat { get; set; }
        public StatementNode ElseStat { get; set; }
        public IfElseNode(ExprNode expr, StatementNode stat, StatementNode elseStat) : base(expr, stat)
        {
            Expr = expr;
            Stat = stat;
            ElseStat = elseStat;
        }
    }

    public class WriteNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public WriteNode(ExprNode expr)
        {
            Expr = expr;
        }
    }

    public class VarDefNode : StatementNode
    {
        public List<ExprNode> IdentList = new List<ExprNode>();
        public VarDefNode(ExprNode ident)
        {
            Add(ident);
        }
        public void Add(ExprNode stat)
        {
            IdentList.Add(stat);
        }
    }


    public class BlockNode : StatementNode
    {
        public List<StatementNode> StList = new List<StatementNode>();
        public BlockNode(StatementNode stat)
        {
            Add(stat);
        }
        public void Add(StatementNode stat)
        {
            StList.Add(stat);
        }
    }

}