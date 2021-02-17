using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{    
    public class MaxNestCyclesVisitor : AutoVisitor
    {
        public int CurrentDepth = 0;
        public int MaxNest = 0;

        public override void VisitCycleNode(CycleNode c)
        {
            CurrentDepth++;
            c.Expr.Visit(this);
            c.Stat.Visit(this);
            if (CurrentDepth > MaxNest)
                MaxNest = CurrentDepth;
            CurrentDepth--;
        }
    }
}
