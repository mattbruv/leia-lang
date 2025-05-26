.const 0 1
.const 1 2
.const 2 3
.const 3 4
.const 4 0

.main
    PUSH_CONST 0 ; 1
    JUMPZ L0
    POP
    PUSH_CONST 1 ; 2
.L0
    JUMPZ L1
    POP
    PUSH_CONST 2 ; 3
.L1
    JUMPZ L2
    POP
    PUSH_CONST 3 ; 4
.L2
    PRINT
    PUSH_CONST 0 ; 1
    JUMPZ L3
    POP
    PUSH_CONST 1 ; 2
.L3
    JUMPZ L4
    POP
    PUSH_CONST 4 ; 0
.L4
    JUMPZ L5
    POP
    PUSH_CONST 3 ; 4
.L5
    PRINT
    PUSH_CONST 4 ; 0
    JUMPNZ L7
    POP
    PUSH_CONST 4 ; 0
    JUMPZ L6
    POP
    PUSH_CONST 3 ; 4
.L6
.L7
    PRINT
    PUSH_CONST 4 ; 0
    JUMPNZ L8
    POP
    PUSH_CONST 0 ; 1
.L8
    JUMPNZ L9
    POP
    PUSH_CONST 1 ; 2
.L9
    JUMPNZ L10
    POP
    PUSH_CONST 2 ; 3
.L10
    PRINT
    PUSH_CONST 1 ; 2
    JUMPNZ L11
    POP
    PUSH_CONST 1 ; 2
.L11
    PRINT
    PUSH_CONST 4 ; 0
    JUMPNZ L12
    POP
    PUSH_CONST 4 ; 0
.L12
    PRINT
    HALT