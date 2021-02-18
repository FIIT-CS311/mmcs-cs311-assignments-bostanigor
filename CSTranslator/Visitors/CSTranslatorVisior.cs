using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class CSTranslatorVisitor : AutoVisitor
    {
        private StringBuilder Text = new StringBuilder();
        private int Indent = 4;

        public string Result => $@"using System;
namespace default {{

  public class MainClass {{

    public static void main(string[] args)
{Text}
  }}
}}";

        private string IndentStr()
        {
            return new string(' ', Indent);
        }
        private void IndentPlus()
        {
            Indent += 2;
        }
        private void IndentMinus()
        {
            Indent -= 2;
        }
        public override void VisitIdNode(IdNode id)
        {
            Text.Append(id.Name);
        }
        public override void VisitIntNumNode(IntNumNode num)
        {
            Text.Append(num.Num.ToString());
        }
        public override void VisitBinOpNode(BinOpNode binop)
        {
            var toBrL = binop.ToBracketLeft;
            var toBrR = binop.ToBracketRight;
            if (toBrL)
                Text.Append("( ");
            binop.Left.Visit(this);
            if (toBrL)
                Text.Append(" )");
            Text.Append($" {binop.Op} ");
            if (toBrR)
                Text.Append("( ");
            binop.Right.Visit(this);
            if (toBrR)
                Text.Append(" )");            
        }
        public override void VisitAssignNode(AssignNode a)
        {
            Text.Append(IndentStr());
            a.Id.Visit(this);
            Text.Append(" = ");
            a.Expr.Visit(this);
            Text.Append(";");
        }
        public override void VisitCycleNode(CycleNode c)
        {
            Text.Append(IndentStr() + "for (var i = 0; i < ");
            c.Expr.Visit(this);
            Text.Append("; i++)\n");
            c.Stat.Visit(this);
        }
        public override void VisitBlockNode(BlockNode bl)
        {
            Text.Append(IndentStr() + "{" + Environment.NewLine);
            IndentPlus();

            var Count = bl.StList.Count;

            if (Count > 0)
                bl.StList[0].Visit(this);
            for (var i = 1; i < Count; i++)
            {
                //Text += ';';
                if (!(bl.StList[i] is EmptyNode))
                    Text.Append(Environment.NewLine);
                bl.StList[i].Visit(this);
            }
            IndentMinus();
            Text.Append(Environment.NewLine + IndentStr() + "}");
        }
        public override void VisitWriteNode(WriteNode w)
        {
            Text.Append(IndentStr() + "Console.WriteLine(");
            w.Expr.Visit(this);
            Text.Append(");");
        }
        public override void VisitVarDefNode(VarDefNode w)
        {
            Text.Append(IndentStr() + "var " + w.vars[0].Name);
            for (int i = 1; i < w.vars.Count; i++)
                Text.Append(',' + w.vars[i].Name);
            Text.Append(";");
        }

        public override void VisitVarDefAssignNode(VarDefAssignNode w)
        {
            Text.Append(IndentStr() + "var ");
            for (var i = 0; i < w.vars.Count - 1; i++) {
                var assign = w.vars[i];
                Text.Append($"{assign.Id.Name} = ");
                assign.Expr.Visit(this);
                Text.Append(", ");
            }
            var last = w.vars.Last();
            Text.Append($"{last.Id.Name} = ");
            last.Expr.Visit(this);
            Text.Append(";");
        }

        public override void VisitIfNode(IfNode i)
        {
            Text.Append(IndentStr() + "if (");
            i.Expr.Visit(this);
            Text.Append(" != 0)\n");
            if (!(i.Stat is BlockNode))
                IndentPlus();
            i.Stat.Visit(this);
            if (!(i.Stat is BlockNode))
                IndentMinus();
        }

        public override void VisitIfElseNode(IfElseNode i)
        {
            Text.Append(IndentStr() + "if (");
            i.Expr.Visit(this);
            Text.Append(" != 0)\n");
            if (!(i.Stat is BlockNode))
                IndentPlus();
            i.Stat.Visit(this);
            if (!(i.Stat is BlockNode))
                IndentMinus();
            Text.Append(Environment.NewLine + IndentStr() + "else" + Environment.NewLine);
            if (!(i.ElseStat is BlockNode))
                IndentPlus();
            i.ElseStat.Visit(this);
            if (!(i.ElseStat is BlockNode))
                IndentMinus();
        }
    }
}
