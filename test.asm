.const 0 "hello from foo()!"


.fn_foo
    PUSH_CONST 0 ; "hello from foo()!"
    PRINT
    RET

.fn_main
    CALL fn_foo
    CALL fn_foo
    HALT