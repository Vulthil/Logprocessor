lexer grammar SessionTypesLexer;



WHITESPACES:   (Whitespace | NewLine)+            -> channel(HIDDEN);

//REC:					'rec';
END:					'end';
KEYWORD:			 Keyword;
IDENTIFIER:          Identifier;


OPEN_BRACKET:             '[';
CLOSE_BRACKET:            ']';
OPEN_PARENS:              '(';
CLOSE_PARENS:             ')';
OPEN_BRACE:				  '{';
CLOSE_BRACE:			  '}';

DOT:                      '.';
COMMA:                    ',';
COLON:                    ':';
SEMICOLON:                ';';
AMP:                      '&';
BANG:                     '!';
INTERR:                   '?';
XOR:					  XorSigns;
TO:                     '->';
MU:						  RecSigns;

fragment XorSigns
	: '@'
	| '\u2295'
	| '\u2A01'
	| '(+)'
	;
fragment RecSigns
	: '\u03BC'
	| '\u00B5'
	;
fragment NewLine
	: '\r\n' | '\r' | '\n'
	| '\u0085' // <Next Line CHARACTER (U+0085)>'
	| '\u2028' //'<Line Separator CHARACTER (U+2028)>'
	| '\u2029' //'<Paragraph Separator CHARACTER (U+2029)>'
	;

fragment Whitespace
	: UnicodeClassZS //'<Any Character With Unicode Class Zs>'
	| '\u0009' //'<Horizontal Tab Character (U+0009)>'
	| '\u000B' //'<Vertical Tab Character (U+000B)>'
	| '\u000C' //'<Form Feed Character (U+000C)>'
	;

fragment UnicodeClassZS
	: '\u0020' // SPACE
	| '\u00A0' // NO_BREAK SPACE
	| '\u1680' // OGHAM SPACE MARK
	| '\u180E' // MONGOLIAN VOWEL SEPARATOR
	| '\u2000' // EN QUAD
	| '\u2001' // EM QUAD
	| '\u2002' // EN SPACE
	| '\u2003' // EM SPACE
	| '\u2004' // THREE_PER_EM SPACE
	| '\u2005' // FOUR_PER_EM SPACE
	| '\u2006' // SIX_PER_EM SPACE
	| '\u2008' // PUNCTUATION SPACE
	| '\u2009' // THIN SPACE
	| '\u200A' // HAIR SPACE
	| '\u202F' // NARROW NO_BREAK SPACE
	| '\u3000' // IDEOGRAPHIC SPACE
	| '\u205F' // MEDIUM MATHEMATICAL SPACE
	;

fragment Keyword
	: END
	//| REC
	;

fragment Identifier
	:	IdentifierStartCharacter IdentifierPartCharacter*
	;

fragment IdentifierStartCharacter
	: LetterCharacter
	;

fragment IdentifierPartCharacter
	: LetterCharacter
	| DecimalDigitCharacter
	;

fragment LetterCharacter
	: [A-Za-z]
	;
fragment DecimalDigitCharacter
	: [0-9]
	;