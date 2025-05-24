.const 0 3

.main
PUSH 0 ; 3
STORE 0 ; z
LOAD 0 ; need to pop this before assigning
STORE 1 ; y
LOAD 1 ; need to pop this before assigning
STORE 2 ; x
LOAD 2 ; x
LOAD 1 ; y
ADD
LOAD 0 ; z
ADD
PRINT
HALT

