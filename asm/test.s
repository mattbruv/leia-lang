.const 0 100
.const 1 2
.const 2 3
.const 3 500

.main
PUSH 0 ; 100
PUSH 1 ; 2
PUSH 2 ; 3
ADD
MUL
STORE 0
LOAD 0 ; x
PRINT
LOAD 0 ; x
PUSH 3 ; 500
ADD
STORE 1
LOAD 0 ; x
LOAD 1 ; y
ADD
PRINT
HALT
