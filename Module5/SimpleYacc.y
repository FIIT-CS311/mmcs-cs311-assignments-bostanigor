%{
// Ёти объ€влени€ добавл€ютс€ в класс GPPGParser, представл€ющий собой парсер, генерируемый системой gppg
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token BEGIN END CYCLE INUM RNUM ID ASSIGN SEMICOLON COMMA WHILE DO REPEAT UNTIL FOR TO WRITE LEFT_BRACKET RIGHT_BRACKET IF THEN ELSE VAR PLUS MINUS MUL DIVISION

%%

progr   : block
		;

stlist	: statement 
		| stlist SEMICOLON statement 
		;

statement: assign
		| block  
		| cycle
		| while
		| repeat
		| for
		| if
		| write
		| var
		;
		
identlist	: ident 
		    | identlist COMMA ident
		    ;

ident 	: ID 
		;
	
assign 	: ident ASSIGN expr 
		;

expr : T
     | expr PLUS T
     | expr MINUS T
     ;

T    : F
     | T MUL F
     | T DIVISION F
     ;

F    : ident
     | INUM 
     | LEFT_BRACKET expr RIGHT_BRACKET
     ;

block	: BEGIN stlist END 
		;

cycle	: CYCLE expr statement 
		;
	
while	: WHILE expr DO statement 
		;
		
repeat	: REPEAT stlist UNTIL expr 
		;

for	    : FOR assign TO expr DO statement
		;

write	: WRITE LEFT_BRACKET expr RIGHT_BRACKET
		;	
		
if	    : IF expr THEN statement
        | IF expr THEN statement ELSE statement
		;
		
var     : VAR identlist
        ;

%%