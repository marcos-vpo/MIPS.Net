.data

.text
_configure_syscalls:
    move $s1, $ra

    # sys_exit
    li $a0, 1
    la $a1, ._sys_program_exit
    jal ._configure_code

    # sys_get_pid
    li $a0, 2
    la $a1, ._sys_get_pid
    jal ._configure_code

    move $ra, $s1
    jr $ra

_configure_code:
    # 210 - get first-free IVT entry address
    li $v0, 210
    syscall

    move $sp, $v0   # IVT entry address

    move $t1, $a0
    sw $t1, 0($sp)  # syscall code

    move $t1, $a1           
    sw $t1, 4($sp)  # handler address

    jr $ra

_sys_program_exit:
    jal .sys:p_exit
    la $k1, ._kernel_loop

_sys_get_pid:
    jal .sys:p_get_current_pid