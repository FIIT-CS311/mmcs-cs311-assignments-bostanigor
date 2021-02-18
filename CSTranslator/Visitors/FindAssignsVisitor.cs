using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProgramTree;

namespace SimpleLang.Visitors
{
    class FindAssignVisitor : AutoVisitor
    {
        private Dictionary<string, bool> taintedVars = new Dictionary<string, bool>();

        public override void VisitBlockNode(BlockNode bl)
        {            
            var Count = bl.StList.Count;
            var varDefAssignNodes = new Dictionary<string, VarDefAssignNode>();
            var varsInBlock = new List<string>();
            var indiciesToRemove = new List<int>();

            /*if (Count > 0)
                bl.StList[0].Visit(this);*/
            for (var i = 0; i < bl.StList.Count; i++)
            {
                var statement = bl.StList[i];
                if (statement is VarDefNode) {
                    var varDefAssignNode = new VarDefAssignNode();                    
                    foreach (var id in (statement as VarDefNode).vars)
                    {
                        taintedVars[id.Name] = false; // Mark that ID wasn't modified or used
                        varDefAssignNodes[id.Name] = varDefAssignNode; // So we can find the new node by Id name
                        varDefAssignNode.Add(new AssignNode(id, new IntNumNode(0)));
                        varsInBlock.Add(id.Name);
                    }
                    bl.StList[i] = varDefAssignNode; // Swap old node for new                    
                }
                else if (statement is AssignNode)
                {
                    var asNode = statement as AssignNode;
                    if (taintedVars[asNode.Id.Name] == false) // ID was not modified or used before in block
                    {
                        if (varDefAssignNodes.ContainsKey(asNode.Id.Name))
                        {
                            var varDefAssignNode = varDefAssignNodes[asNode.Id.Name];
                            foreach (var assign in varDefAssignNode.vars)
                            {
                                if (assign.Id.Name == asNode.Id.Name)
                                    assign.Expr = asNode.Expr;
                            }
                            indiciesToRemove.Add(i - indiciesToRemove.Count); // Remove unnecessary assign
                        }
                        else
                            taintedVars[asNode.Id.Name] = true;
                    }                        
                }
                else 
                    statement.Visit(this);                
            }
            foreach (var i in indiciesToRemove)
                bl.StList.RemoveAt(i);
            foreach (var name in varsInBlock)
                taintedVars.Remove(name);
        }
        public override void VisitIdNode(IdNode id)
        {
            if (taintedVars.ContainsKey(id.Name))            
                taintedVars[id.Name] = true; // ID is modified or used in block           
        }
    }
}
