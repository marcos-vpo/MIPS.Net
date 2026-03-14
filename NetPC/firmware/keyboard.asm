.data
    KBD_USB_ADDR: .word 0     # usb address
    KBD_DATA: .space 8    # keyboard buffer
    KBD_DATA_LEN: .word 8 # sinal USB (8 bytes)
    HANDLER_ADDR: .word 0 # custom keyboard handler address


.text

# args
# $a0 = usb address
# $a1 = custom handler address
INIT_KEYBOARD:
    move $gp, $ra # save return address

    # guardar o endereço da usb usada pelo keyboard
    la $t7, KBD_USB_ADDR
    sw $a0, 0($t7)
    xor
    # atribuir as interrupções com as rotinas do keyboard
    li $a0, 30 # USB_02_DATA_REC
    la $t0, .INTR_DATA_RECEIVED
    jal .CONFIGURE_INTR
    # ===========================================
    li $a0, 31 # USB_02_DEV_CONNECT
    la $t0, .INTR_DEV_CONNECT
    jal .CONFIGURE_INTR
    # ===========================================
    li $a0, 32 # USB_02_DEV_DISCONNECT
    la $t0, .INTR_DEV_DISCONNECT
    jal .CONFIGURE_INTR
    # ===========================================
    li $a0, 34 # USB_02_DEV_REC_FAIL
    la $t0, .INTR_REC_FAIL
    jal .CONFIGURE_INTR
    
    # put on t0 the address of routine 
    # that defines a custom handler addr
    la $t0, HANDLER_ADDR
    sw $a1, 0($t0) # store custom handler address


    addi $gp, $gp, 4 # increment to next instruction
    jr $gp # return to caller (another) program


CONFIGURE_INTR:
    li $v0, 211
    syscall
    li $sp, 0
    add $sp, $sp, $v0
    sw $t0, 4($sp)
    jr $ra

INTR_DEV_CONNECT:
    li $s6, 999
    
INTR_DEV_DISCONNECT:
    li $s6, 999

INTR_REC_FAIL:
    li $s6, 999

INTR_DATA_RECEIVED:
        li $v0, 400
      syscall

    # Read USB data then put on keyboard buffer
    la $t0, KBD_USB_ADDR # source addr: base usb03 addr
    lw $a0, 0($t0)
    addi $a0, $a0, 10 # usb buffer start
    la $a1, KBD_DATA # destination addr
    la $t0, KBD_DATA_LEN 
    lw $a2, 0($t0)

    li $v0, 110 # BLOCK TRANSFER TO KBD_DATA
    syscall

    # go to keyboard custom handler
    la $v0, HANDLER_ADDR # handler data addr
    lw $v0, 0($v0)       # handler addr value
    la $a1, KBD_DATA     # keyboard buffer address
    jr $v0               # go to HANDLER_ADDR

