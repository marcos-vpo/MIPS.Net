.link "C:\mOS\bin\mOS.dll"
.include "interruptions.asm"
.include "keyboard.asm"
.include "display.asm"
.include "mOS_calls.asm"

.data
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