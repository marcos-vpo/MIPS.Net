.data
.link "C:\mOS\bin\mOS.dll"
#.include "interruptions.asm"
#.include "keyboard.asm"
#.include "display.asm"
#.include "mOS_calls.asm"
  process_table_map: .space 2048 # space for process table info
  process_heap_map: .space 8182 # space for process heap info
  kernel_heap_map: .space 8182 # space for kernel heap info
  kernel_loop_args: .space 64 # space for kernel loop args
  kernel_params_addr: .word 8000000 # address to load kernel params
.text
_mos_start:
    jal ._init_keyboard
    jal ._init_display
    jal ._configure_syscalls
    li $v0, 210
    syscall
    move $sp, $v0
    # definir uma entrada na IVT que responde pelo syscall 300
    # e aponta para ._kernel_entry
    li $s1, 300
    sw $s1, 0($sp)
    la $s1, ._kernel_entry
    sw $s1, 4($sp)
    li $s1, 0
    sw $s1, 8($sp)
    #fim interrupcao 300
    la $t0, kernel_params_addr
    lw $t0, 0($t0) # load kernel params address
    addi $t0, $t0, 24 # skip 24 bytes
    la $t1, process_table_map
    sw $t1, 0($t0) #  store process table area addr in params
    addi $t0, $t0, 4 # skip 4 bytes
    la $t1, process_heap_map
    sw $t1, 0($t0) #  store process heap area addr in params
    addi $t0, $t0, 4 # skip 4 bytes
    la $t1, kernel_heap_map
    sw $t1, 0($t0) #  store kernel heap area addr in params
    addi $t0, $t0, 4 # skip 4 bytes
    la $t1, kernel_loop_args
    sw $t1, 0($t0) #  store kernel loop args area addr in params
    li $v0, 410
    la $a0, ._ffi_wait_up #  enable ffi
    syscall
    li $gp, 0    
	jal .mos_kernel:start
    jal ._kernel_loop
_kernel_loop:
    # t0 = &kernel_loop_args
    la   $t0, kernel_loop_args
    # t1 = intenção
    lw   $t1, 0($t0)
    beq  $t1, $zero, ._kernel_idle   # se não há intenção, fica no kernel
    # t2 = endereço de retorno / entrypoint
    lw   $t2, 4($t0)
    # carregar argumentos
    lw   $a0, 8($t0)
    lw   $a1, 12($t0)
    lw   $a2, 16($t0)
    lw   $a3, 20($t0)
    # limpa intenção (kernel retoma controle)
    sw   $zero, 0($t0)
    # retorno do processo sempre volta pro kernel
    la   $ra, ._kernel_loop
    # pula para o processo
    jr   $t2
_kernel_idle:
    jal .sys:mk_shell
    jal ._kernel_loop
_ffi_wait_up:
  jal ._ffi_wait_up
_kernel_entry:
 # jal .mos_kernel:interruption_test
  li $s1, 100
  jr $ra
_rebind_ffi:
  move $gp, $ra
  li $v0, 410
  la $a0, ._ffi_wait_up #  enable ffi
  syscall
  addi $gp, $gp, 4
  jr $gp
# vincula uma rotina de tratamento em uma interrupção
# da IVT.
# será feita a busca da entrada na IVT pelo código da syscall desejada, e
# em seguida, será gravado na posição [4-8] da tupla, o endereço
# da rotina de tratamento a ser invocada
# parametros:
# $a0 - codigo da syscall desejada
# $a1 - o endereço da rotina de tratamento
CONFIGURE_INTERRUPTION_HANDLER:
    # $a0 deve estar com o codigo da syscall a ser localizada
    li $v0, 211 # $v0 - endereço da entrada com o codigo de $a0
    syscall
    li $sp, 0
    add $sp, $sp, $v0 #colocar em $sp a end. da entrada
    sw $a1, 4($sp) # gravar o endereço de tratamento de $a1 na pos [4-8] da tupla
    jr $ra
_init_keyboard:
    move $s1, $ra
    li $a0, 70
    la $a1, ._keyboard_data_rec_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    li $a0, 71
    la $a1, ._keyboard_dev_connect_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    li $a0, 72
    la $a1, ._keyboard_dev_disconnect_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    li $a0, 73
    la $a1, ._keyboard_send_fail_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    li $a0, 74
    la $a1, ._keyboard_rec_fail_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    move $ra, $s1
    jr $ra
_keyboard_data_rec_handler:
    jal .IOInterruptService:DeviceDataReceived
_keyboard_dev_connect_handler:
    jal .IOInterruptService:DeviceConnected
_keyboard_dev_disconnect_handler:
    jal .IOInterruptService:DeviceDisconnected
_keyboard_send_fail_handler:
    jal .IOInterruptService:DeviceSendFail
_keyboard_rec_fail_handler:
    jal .IOInterruptService:DeviceReceiveFail
_init_display:
    move $s1, $ra
    li $a0, 60
    la $a1, ._display_data_rec_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    li $a0, 61
    la $a1, ._display_dev_connect_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    li $a0, 62
    la $a1, ._display_dev_disconnect_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    li $a0, 63
    la $a1, ._display_send_fail_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    li $a0, 64
    la $a1, ._display_rec_fail_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    move $ra, $s1
    jr $ra
_display_data_rec_handler:
    jal .IOInterruptService:DeviceDataReceived
_display_dev_connect_handler:
    jal .IOInterruptService:DeviceConnected
_display_dev_disconnect_handler:
    jal .IOInterruptService:DeviceDisconnected
_display_send_fail_handler:
    jal .IOInterruptService:DeviceSendFail
_display_rec_fail_handler:
    jal .IOInterruptService:DeviceReceiveFail
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
	# proc_free
	li $a0, 8
    la $a1, ._mem_proc_free
    jal ._configure_code
	# proc_addr
    li $a0, 9
    la $a1, ._sys_get_proc_addr
    jal ._configure_code
	# read_char
    li $a0, 11
    la $a1, ._console_read_char
    jal ._configure_code
	# read_line
    li $a0, 12
    la $a1, ._console_read_line
    jal ._configure_code
	# proc_pause
    li $a0, 99
    la $a1, ._sys_program_pause
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
_console_read_line:
	jal .console:read_line
_sys_program_pause:
	la $k1, ._kernel_loop
    jal .sys:p_pause
_sys_program_exit:
	la $k1, ._kernel_loop
    jal .sys:p_exit
_sys_get_proc_addr:
    jal .sys:p_get_addr
_sys_get_pid:
    jal .sys:p_get_current_pid
_mem_proc_alloc:
    jal .mem:process_alloc
_mem_proc_free:
    jal .mem:process_free
_mem_page_start:
    jal .mem:page_start_addr
