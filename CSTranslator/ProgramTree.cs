﻿using System.Collections.Generic;
using SimpleLang.Visitors;

namespace ProgramTree
{
    public enum AssignType { Assign, AssignPlus, AssignMinus, AssignMult, AssignDivide };

    public abstract class Node // базовый класс для всех узлов    
    {
        public abstract void Visit(Visitor v);
    }

    public abstract class ExprNode : Node // базовый класс для всех выражений
    {
    }

    public class IdNode : ExprNode
    {
        public string Name { get; set; }
        public IdNode(string name) { Name = name; }
        public override void Visit(Visitor v)
        {
            v.VisitIdNode(this);
        }
    }

    public class IntNumNode : ExprNode
    {
        public int Num { get; set; }
        public IntNumNode(int num) { Num = num; }
        public override void Visit(Visitor v)
        {
            v.VisitIntNumNode(this);
        }
    }

    public class BinOpNode : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode Right { get; set; }
        public char Op { get; set; }
        public BinOpNode(ExprNode Left, ExprNode Right, char op) 
        {
            this.Left = Left;
            this.Right = Right;
            this.Op = op;
        }
        public override void Visit(Visitor v)
        {
            v.VisitBinOpNode(this);
        }

        public bool IsMultOrDiv => Op == '*' || Op == '/';
        public bool IsPlusOrMinus => Op == '+' || Op == '-';

        public bool ToBracketLeft =>
            IsMultOrDiv && (Left is BinOpNode) && (Left as BinOpNode).IsPlusOrMinus;
        public bool ToBracketRight =>
            IsMultOrDiv && (Right is BinOpNode) && (Right as BinOpNode).IsPlusOrMinus;
    }

    public abstract class StatementNode : Node // базовый класс для всех операторов
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
        public override void Visit(Visitor v)
        {
            v.VisitAssignNode(this);
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
        public override void Visit(Visitor v)
        {
            v.VisitCycleNode(this);
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
        public override void Visit(Visitor v)
        {
            v.VisitBlockNode(this);
        }
    }

    public class WriteNode : StatementNode
    {
        public ExprNode Expr { get; set; }
        public WriteNode(ExprNode Expr)
        {
            this.Expr = Expr;
        }
        public override void Visit(Visitor v)
        {
            v.VisitWriteNode(this);
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

        public override void Visit(Visitor v)
        {
            v.VisitIfNode(this);
        }
    }

    public class IfElseNode : IfNode
    {       
        public StatementNode ElseStat { get; set; }
        public IfElseNode(ExprNode expr, StatementNode stat, StatementNode elseStat) : base(expr, stat)
        {
            Expr = expr;
            Stat = stat;
            ElseStat = elseStat;
        }
        public override void Visit(Visitor v)
        {
            v.VisitIfElseNode(this);
        }
    }

    public class EmptyNode : StatementNode
    {
        public override void Visit(Visitor v)
        {
            v.VisitEmptyNode(this);
        }
    }

    public class VarDefNode : StatementNode
    {
        public List<IdNode> vars = new List<IdNode>();
        public VarDefNode(IdNode id)
        {
            Add(id);
        }

        public void Add(IdNode id)
        {
            vars.Add(id);
        }
        public override void Visit(Visitor v)
        {
            v.VisitVarDefNode(this);
        }
    }

    public class VarDefAssignNode : StatementNode
    {
        public List<AssignNode> vars = new List<AssignNode>();
        public void Add(AssignNode n)
        {
            vars.Add(n);
        }
        public override void Visit(Visitor v)
        {
            v.VisitVarDefAssignNode(this);
        }
    }
}