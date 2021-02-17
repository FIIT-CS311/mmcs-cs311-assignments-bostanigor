using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    public class CountCyclesOpVisitor : AutoVisitor
    {
        private int CycleDepth = 0;
        private int CycleCount = 0;
        private int OpCount = 0;
        public int MidCount()
        {
            return CycleCount == 0 ? 0 : OpCount / CycleCount;
        }

        public override void VisitCycleNode(CycleNode c)
        {
            CycleCount++;
            CycleDepth++;
            c.Expr.Visit(this);            
            c.Stat.Visit(this);
            CycleDepth--;
        }

        public override void VisitBinOpNode(BinOpNode binop)
        {
            if (CycleDepth > 0)
                OpCount++;
            binop.Left.Visit(this);
            binop.Right.Visit(this);
        }

        public override void VisitAssignNode(AssignNode a)
        {
            if (CycleDepth > 0)
                OpCount++;
            a.Id.Visit(this);         
            a.Expr.Visit(this);
        }
    }
}
