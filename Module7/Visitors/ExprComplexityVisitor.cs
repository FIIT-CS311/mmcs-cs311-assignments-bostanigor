using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class ExprComplexityVisitor : AutoVisitor
    {
        private List<int> Complexities = new List<int>();
        private int CurrentComplexity;
        // список должен содержать сложность каждого выражения, встреченного при обычном порядке обхода AST        
        public List<int> getComplexityList()
        {
            return Complexities;
        }

        public override void VisitAssignNode(AssignNode a)
        {
            // для каких-то визиторов порядок может быть обратный - вначале обойти выражение, потом - идентификатор            
            a.Id.Visit(this);
            CurrentComplexity = 0;
            a.Expr.Visit(this);
            Complexities.Add(CurrentComplexity);
        }

        public override void VisitCycleNode(CycleNode c)
        {
            CurrentComplexity = 0;
            c.Expr.Visit(this);
            Complexities.Add(CurrentComplexity);
            c.Stat.Visit(this);
        }

        public override void VisitWriteNode(WriteNode w)
        {
            CurrentComplexity = 0;
            w.Expr.Visit(this);
            Complexities.Add(CurrentComplexity);
        }

        public override void VisitBinOpNode(BinOpNode binop)
        {
            binop.Left.Visit(this);
            binop.Right.Visit(this);
            switch (binop.Op)
            {
                case '*': case '/': CurrentComplexity += 3; break;
                case '+': case '-': CurrentComplexity += 1; break;
            }
        }
    }
}
