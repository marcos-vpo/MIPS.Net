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

	# proc_alloc
	li $a0, 3
    la $a1, ._mem_proc_alloc
    jal ._configure_code

    # mem_page_start
    li $a0, 4
    la $a1, ._mem_page_start
    jal ._configure_code

	# proc_addr
    li $a0, 9
    la $a1, ._sys_get_proc_addr
    jal ._configure_code

	# read_char
    li $a0, 11
    la $a1, ._console_read_char
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

_console_read_char:
	jal .console:read_char

_sys_program_exit:
	la $k1, ._kernel_loop
    jal .sys:p_exit
     
_sys_get_proc_addr:
    jal .sys:p_get_addr

_sys_get_pid:
    jal .sys:p_get_current_pid

_mem_proc_alloc:
    jal .mem:process_alloc

_mem_page_start:
    jal .mem:page_start_addr