using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lexer
{

    public class LexerException : System.Exception
    {
        public LexerException(string msg)
            : base(msg)
        {
        }

    }

    public class Lexer
    {

        protected int position;
        protected char currentCh; // ��������� ��������� ������
        protected int currentCharValue; // ����� �������� ���������� ���������� �������
        protected System.IO.StringReader inputReader;
        protected string inputString;

        public Lexer(string input)
        {
            inputReader = new System.IO.StringReader(input);
            inputString = input;
        }

        public void Error()
        {
            System.Text.StringBuilder o = new System.Text.StringBuilder();
            o.Append(inputString + '\n');
            o.Append(new System.String(' ', position - 1) + "^\n");
            o.AppendFormat("Error in symbol {0}", currentCh);
            throw new LexerException(o.ToString());
        }

        protected void NextCh()
        {
            this.currentCharValue = this.inputReader.Read();
            this.currentCh = (char) currentCharValue;
            this.position += 1;
        }

        public virtual bool Parse()
        {
            return true;
        }
    }

    public class IntLexer : Lexer
    {

        protected System.Text.StringBuilder intString;
        public int parseResult = 0;

        public IntLexer(string input)
            : base(input)
        {
            intString = new System.Text.StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {       
                intString.Append(currentCh);         
                NextCh();
            }
        
            if (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }
            parseResult = Int32.Parse(intString.ToString());

            if (currentCharValue != -1)
            {
                Error();
            }
            
            return true;

        }
    }
    
    public class IdentLexer : Lexer
    {
        private string parseResult;
        protected StringBuilder builder;
    
        public string ParseResult
        {
            get { return parseResult; }
        }
    
        public IdentLexer(string input) : base(input)
        {            
            builder = new StringBuilder();
        }

        public override bool Parse()
        { 
            NextCh();
            if (char.IsLetter(currentCh))
                builder.Append(currentCh);
            else
                Error();
            NextCh();

            while (char.IsLetter(currentCh) || char.IsDigit(currentCh)) {
                builder.Append(currentCh);
                NextCh();
            }

            if (currentCharValue != -1)
                Error();
            
            parseResult = builder.ToString();
            return true;
        }
       
    }

    public class IntNoZeroLexer : IntLexer
    {
        public IntNoZeroLexer(string input)
            : base(input)
        {
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '+' || currentCh == '-')
            {       
                intString.Append(currentCh);         
                NextCh();
            }            
        
            if (char.IsDigit(currentCh) && currentCh != '0')
            {
                intString.Append(currentCh);
                NextCh();
            }
            else
            {
                Error();
            }

            while (char.IsDigit(currentCh))
            {
                intString.Append(currentCh);
                NextCh();
            }
            parseResult = Int32.Parse(intString.ToString());

            if (currentCharValue != -1)
            {
                Error();
            }
            
            return true;   
            // throw new NotImplementedException();
        }
    }

    public class LetterDigitLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;
        protected bool digitLast;

        public string ParseResult
        {
            get { return parseResult; }
        }

        public LetterDigitLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();            
            if (char.IsLetter(currentCh))
                digitLast = false;
            else
                Error();

            NextCh();
            while (digitLast && char.IsLetter(currentCh) ||
                  !digitLast && char.IsDigit(currentCh)) {
                builder.Append(currentCh);
                digitLast = !digitLast;
                NextCh();
            }

            if (currentCharValue != -1)            
                Error();
            parseResult = builder.ToString();
            
            return true;   
        }
       
    }

    public class LetterListLexer : Lexer
    {
        protected List<char> parseResult;
        protected bool letterLast;

        public List<char> ParseResult
        {
            get { return parseResult; }
        }

        public LetterListLexer(string input)
            : base(input)
        {
            parseResult = new List<char>();
            letterLast = false;
        }

        public override bool Parse()
        {
            NextCh();
            while (currentCharValue != -1) {
                if (!letterLast) {
                    if (char.IsLetter(currentCh))
                        parseResult.Add(currentCh);
                    else
                        Error();                 
                }
                else if (currentCh != ',' && currentCh != ';')
                    Error();
                letterLast = !letterLast;
                NextCh();
            }

            if (!letterLast)
                Error();
            
            return true;
        }
    }

    public class DigitListLexer : Lexer
    {
        protected List<int> parseResult;
        protected bool digitLast;

        public List<int> ParseResult
        {
            get { return parseResult; }
        }

        public DigitListLexer(string input)
            : base(input)
        {
            parseResult = new List<int>();
        }

        public override bool Parse()
        {
            NextCh();
            if (char.IsDigit(currentCh))
                parseResult.Add(currentCharValue - '0');
            else
                Error();
            digitLast = true;
            
            NextCh();
            while (currentCharValue != -1) {
                if (char.IsDigit(currentCh)) {
                    if (!digitLast) {
                        parseResult.Add(currentCharValue - '0');
                        digitLast = true;
                    }
                    else
                        Error();
                }
                else if (currentCh == ' ')
                    digitLast = false;
                else
                    Error();
                NextCh();
            }

            if (!digitLast)
                Error();

            return true;
        }
    }

    public class LetterDigitGroupLexer : Lexer
    {
        protected StringBuilder builder;
        protected string parseResult;
        private bool digitLast;
        private bool canAdd;

        public string ParseResult
        {
            get { return parseResult; }
        }
        
        public LetterDigitGroupLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();            
            if (char.IsLetter(currentCh))
                digitLast = false;
            else
                Error();
            builder.Append(currentCh);
            canAdd = true;

            NextCh();
            while (currentCharValue != -1) {
                if (char.IsDigit(currentCh)) {
                    if (!digitLast)
                        canAdd = true;
                    else if (canAdd)
                        canAdd = false;
                    else Error();

                    digitLast = true;
                }
                else if (char.IsLetter(currentCh)) {
                    if (digitLast)
                        canAdd = true;
                    else if (canAdd)
                        canAdd = false;
                    else Error();

                    digitLast = false;
                }
                else
                    Error();
                builder.Append(currentCh);
                NextCh();
            }
            parseResult = builder.ToString();
            return true;
        }
       
    }

    public class DoubleLexer : Lexer
    {
        private StringBuilder builder;
        private double parseResult;
        private bool dotIn;
        private bool dotLast;

        public double ParseResult
        {
            get { return parseResult; }            
        }

        public DoubleLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
            dotIn = false;            
        }

        public override bool Parse()
        {
            NextCh();
            if (char.IsDigit(currentCh))
                builder.Append(currentCh);
            else
                Error();

            NextCh();
            while (currentCharValue != -1) {
                if (char.IsDigit(currentCh))
                    builder.Append(currentCh);
                else if (currentCh == '.' && !dotIn) {
                    builder.Append(currentCh);
                    dotIn = true;
                    NextCh();

                    if (currentCharValue != -1 &&
                        char.IsDigit(currentCh))
                        builder.Append(currentCh);
                    else
                        Error();
                }
                else 
                    Error();
                NextCh();
            }            

            parseResult = Double.Parse(builder.ToString());
            return true;
        }
       
    }

    public class StringLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;
        private bool canAdd;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public StringLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '\'')
                builder.Append(currentCh);
            else
                Error();

            canAdd = true;
            NextCh();
            while (currentCharValue != -1) {
                if (!canAdd)
                    Error();

                if (currentCh == '\'') {
                    canAdd = false;
                }
                builder.Append(currentCh);
                NextCh();
            }
            
            if (canAdd != false)
                Error();

            parseResult = builder.ToString();
            return true;
        }
    }

    public class CommentLexer : Lexer
    {
        private StringBuilder builder;
        private string parseResult;
        private bool canAdd;

        public string ParseResult
        {
            get { return parseResult; }

        }

        public CommentLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
        }

        public override bool Parse()
        {
            NextCh();
            if (currentCh == '/') {
                builder.Append(currentCh);
                NextCh();
                if (currentCh == '*')
                    builder.Append(currentCh);
                else
                    Error();
            }
            else
                Error();

            canAdd = true;
            NextCh();
            while (currentCharValue != -1) {
                if (!canAdd)
                    Error();
                if (currentCh == '*') {
                    builder.Append(currentCh);
                    NextCh();

                    if (currentCharValue == -1)
                        Error();
                    else if(currentCh == '/') {
                        builder.Append(currentCh);
                        canAdd = false;
                    }
                }
                NextCh();
            }

            if (canAdd != false)
                Error();
            parseResult = builder.ToString();
            return true;
        }
    }

    public class IdentChainLexer : Lexer
    {
        private StringBuilder builder;
        private List<string> parseResult;

        public List<string> ParseResult
        {
            get { return parseResult; }

        }

        public IdentChainLexer(string input)
            : base(input)
        {
            builder = new StringBuilder();
            parseResult = new List<string>();
        }

        public override bool Parse()
        {
            throw new NotImplementedException();
        }
    }

    public class Program
    {
        public static void Main()
        {
            string input = "154216";
            Lexer L = new IntLexer(input);
            try
            {
                L.Parse();
            }
            catch (LexerException e)
            {
                System.Console.WriteLine(e.Message);
            }

        }
    }
}