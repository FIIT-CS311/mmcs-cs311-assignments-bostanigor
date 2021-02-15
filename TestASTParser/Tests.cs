using System;
using NUnit.Framework;
using SimpleScanner;
using SimpleParser;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestASTParser
{
    public class ASTParserTests
    {
        public static JObject Parse(string text)
        {
            Scanner scanner = new Scanner();
            scanner.SetSource(text, 0);

            Parser parser = new Parser(scanner);

            var b = parser.Parse();
            if (!b)
                Assert.Fail("программа не распознана");
            else
            {
                JsonSerializerSettings jsonSettings = new JsonSerializerSettings();
                jsonSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonSettings.TypeNameHandling = TypeNameHandling.All;
                string output = JsonConvert.SerializeObject(parser.root, jsonSettings);
                return JObject.Parse(output);
            }

            return null;

        }
    }

    [TestFixture]
    public class WhileTests
    {

        [Test]
        public void TestWhile()
        {
            var tree = ASTParserTests.Parse("begin while 2 do a:=2 end");
            Assert.AreEqual("ProgramTree.WhileNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);
            Assert.AreEqual("ProgramTree.IntNumNode, SimpleLang", (string)tree["StList"]["$values"][0]["Expr"]["$type"]);
            Assert.AreEqual("2", ((string)tree["StList"]["$values"][0]["Expr"]["Num"]).Trim());
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)tree["StList"]["$values"][0]["Stat"]["$type"]);
        }
    }

    [TestFixture]
    public class RepeatTests
    {

        [Test]
        public void TestRepeat()
        {
            var tree = ASTParserTests.Parse("begin repeat a:=2 until 2 end");
            Assert.AreEqual("ProgramTree.RepeatNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);
            // TODO: проверить узлы содержимого repeat
            var assignNode = tree["StList"]["$values"][0]["Block"]["StList"]["$values"][0];
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)assignNode["$type"]);
            Assert.AreEqual("2", ((string)assignNode["Expr"]["Num"]).Trim());
            Assert.AreEqual("0", ((string)assignNode["AssOp"]).Trim());
            Assert.AreEqual("a", ((string)assignNode["Id"]["Name"]).Trim());
            Assert.AreEqual("2", (string)tree["StList"]["$values"][0]["Expr"]["Num"]);
        }
    }

    [TestFixture]
    public class ForTests
    {

        [Test]
        public void TestFor()
        {
            var tree = ASTParserTests.Parse("begin for i:=2 to 10 do a:=2 end");
            Assert.AreEqual("ProgramTree.ForNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);
            // TODO: проверить узлы содержимого for
            var assigmentNode = tree["StList"]["$values"][0]["AssignNode"];
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)assigmentNode["$type"]);
            Assert.AreEqual("2", ((string)assigmentNode["Expr"]["Num"]).Trim());
            Assert.AreEqual("0", ((string)assigmentNode["AssOp"]).Trim());
            Assert.AreEqual("i", ((string)assigmentNode["Id"]["Name"]).Trim());

            Assert.AreEqual("10", (string)tree["StList"]["$values"][0]["Expr"]["Num"]);

            var statementNode = tree["StList"]["$values"][0]["Stat"];
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)statementNode["$type"]);
            Assert.AreEqual("2", ((string)statementNode["Expr"]["Num"]).Trim());
            Assert.AreEqual("0", ((string)statementNode["AssOp"]).Trim());
            Assert.AreEqual("a", ((string)statementNode["Id"]["Name"]).Trim());
        }
    }

    [TestFixture]
    public class WriteTests
    {

        [Test]
        public void TestWrite()
        {
            var tree = ASTParserTests.Parse("begin write(2) end");
            Assert.AreEqual("ProgramTree.WriteNode, SimpleLang", (string)tree["StList"]["$values"][0]["$type"]);
            // TODO: проверить содержимое write
            Assert.AreEqual("2", (string)tree["StList"]["$values"][0]["Expr"]["Num"]);
        }
    }

    [TestFixture]
    public class ExtraTests
    {

        [Test]
        public void TestIf()
        {
            var tree = ASTParserTests.Parse("begin if 2 then a:=3 else c:=8; if 3 then z:=10 end");


            var firstIf = tree["StList"]["$values"][0];
            Assert.AreEqual("ProgramTree.IfElseNode, SimpleLang", (string)firstIf["$type"]);
            Assert.AreEqual("2", (string)firstIf["Expr"]["Num"]);

            var trueAssign = firstIf["Stat"];
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)trueAssign["$type"]);
            Assert.AreEqual("3", ((string)trueAssign["Expr"]["Num"]).Trim());
            Assert.AreEqual("0", ((string)trueAssign["AssOp"]).Trim());
            Assert.AreEqual("a", ((string)trueAssign["Id"]["Name"]).Trim());

            var falseAssign = firstIf["ElseStat"];
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)falseAssign["$type"]);
            Assert.AreEqual("8", ((string)falseAssign["Expr"]["Num"]).Trim());
            Assert.AreEqual("0", ((string)falseAssign["AssOp"]).Trim());
            Assert.AreEqual("c", ((string)falseAssign["Id"]["Name"]).Trim());

            var secondIf = tree["StList"]["$values"][1];
            Assert.AreEqual("ProgramTree.IfNode, SimpleLang", (string)secondIf["$type"]);
            Assert.AreEqual("3", (string)secondIf["Expr"]["Num"]);

            var secondIfAssign = secondIf["Stat"];
            Assert.AreEqual("ProgramTree.AssignNode, SimpleLang", (string)secondIfAssign["$type"]);
            Assert.AreEqual("10", ((string)secondIfAssign["Expr"]["Num"]).Trim());
            Assert.AreEqual("0", ((string)secondIfAssign["AssOp"]).Trim());
            Assert.AreEqual("z", ((string)secondIfAssign["Id"]["Name"]).Trim());
        }

        [Test]
        public void TestVarDef()
        {
            var tree = ASTParserTests.Parse("begin var a,b,d end");

            var varNode = tree["StList"]["$values"][0];

            Assert.AreEqual("ProgramTree.VarDefNode, SimpleLang", (string)varNode["$type"]);
            Assert.AreEqual(3, ((JArray)varNode["IdentList"]["$values"]).Count);
            Assert.AreEqual("a", (string)varNode["IdentList"]["$values"][0]["Name"]);
            Assert.AreEqual("b", (string)varNode["IdentList"]["$values"][1]["Name"]);
            Assert.AreEqual("d", (string)varNode["IdentList"]["$values"][2]["Name"]);
        }

        [Test]
        public void TestBinary()
        {
            var tree = ASTParserTests.Parse("begin a:=x-z*3/(c+3-(ddz)+2) end");

            var expr = tree["StList"]["$values"][0]["Expr"];

            Assert.AreEqual("a", (string)tree["StList"]["$values"][0]["Id"]["Name"]);
            Assert.AreEqual("x", (string)expr["Left"]["Name"]);
            Assert.AreEqual("-", (string)expr["Operation"]);
            
            Assert.AreEqual("ProgramTree.BinaryNode, SimpleLang", (string)expr["Right"]["$type"]);
            Assert.AreEqual("*", (string)expr["Right"]["Operation"]);
            Assert.AreEqual("3", (string)expr["Right"]["Left"]["Right"]["Num"]);
            Assert.AreEqual("/", (string)expr["Right"]["Left"]["Operation"]);
            Assert.AreEqual("z", (string)expr["Right"]["Left"]["Left"]["Name"]);
            Assert.AreEqual("+", (string)expr["Right"]["Right"]["Operation"]);
            Assert.AreEqual("c", (string)expr["Right"]["Right"]["Left"]["Left"]["Left"]["Name"]);
            Assert.AreEqual("-", (string)expr["Right"]["Right"]["Left"]["Operation"]);
            Assert.AreEqual("3", (string)expr["Right"]["Right"]["Left"]["Left"]["Right"]["Num"]);
            Assert.AreEqual("ddz", (string)expr["Right"]["Right"]["Left"]["Right"]["Name"]);
            Assert.AreEqual("+", (string)expr["Right"]["Right"]["Left"]["Left"]["Operation"]);
            Assert.AreEqual("2", (string)expr["Right"]["Right"]["Right"]["Num"]);
        }
    }
}