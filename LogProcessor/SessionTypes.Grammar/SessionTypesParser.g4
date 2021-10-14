parser grammar SessionTypesParser;

options { tokenVocab=SessionTypesLexer; }


protocol
	:	
		(localSession)+ EOF #localSessionType
	|	
		gtStart EOF #globalType
	;

globalRecursiveStart
	: MU OPEN_PARENS identifier CLOSE_PARENS (OPEN_PARENS gt CLOSE_PARENS | globalContinuation)
	;
globalSend
	: from=participantName TO to=participantName COLON messageLabel globalContinuation
	;
gtStart
	: globalRecursiveStart
	| globalSend
	| globalBranch
	;
globalBranch
	: from=participantName TO to=participantName COLON? OPEN_BRACE messageLabel globalContinuation (COMMA messageLabel globalContinuation)* CLOSE_BRACE
	;
gt
	: gtStart
	| endOrRec
	;
endMessage 
	: END
	;
recursiveLoop
	: identifier
	;
endOrRec
	: recursiveLoop
	| endMessage
	;

globalContinuation
	: DOT gt
	;

participantName
	: identifier (DOT identifier)?
	;
localSession
	: participantName COLON tStart
	;

tStart
	: recursiveStart
	| sendReceive
	| choiceMessage
	| offerMessage
	;
recursiveStart
	: MU OPEN_PARENS identifier CLOSE_PARENS continuation
	;
sendReceive
	: participantName op=(BANG | INTERR) messageLabel continuation
	;
choiceMessage
	: participantName XOR OPEN_BRACE messageLabel continuation (COMMA messageLabel continuation)* CLOSE_BRACE
	;
offerMessage
	: participantName AMP OPEN_BRACE messageLabel continuation (COMMA messageLabel continuation)* CLOSE_BRACE
	;
t
	: tStart
	| endOrRec
	;

continuation
	: DOT t
	;

messageLabel
	: identifier (OPEN_PARENS sort? CLOSE_PARENS)?
	;

sort
	: identifier // (COMMA identifier)*
	;

identifier
	: IDENTIFIER
	;