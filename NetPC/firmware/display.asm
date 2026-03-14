.data
    DISPLAY_USB_ADDR: .word 0
    DISPLAY_BUFFER: .space 10002400 # buffer maximo suportado pelo driver; 1MB (texto: max 400x320 x8 bytes por caractere)
    DISPLAY_BFF_LEN: .word 0 # tamanho sendo usado de acordo com a resolucao; modo texto 8bpc; deve ser calculado quando DISPLAY_INTR_DEV_CONNECT
    
    SCREEN_WIDTH: .word 0 # resolução X
    SCREEN_HEIGHT: .word 0 # resolução Y

    CURSOR_X: .word 0
    CURSOR_Y: .word 0

    CONNECTED: .byte 0 # 0x0 - desconectado, 0x1 - conectado
    RESOLUTION_REQUESTED: .byte 0 # 0x0 - não solicitado, 0x1 - solicitado
    RESOLUTION_DEFINED: .byte 0
.text
  
CONFIGURE_DISPLAY:
    # guardar o endereço da porta em uso
    la $t7, DISPLAY_USB_ADDR
    sw $a0, 0($t7)

    # atribuir as interrupções com as rotinas do display
    li $a0, 50 # USB_02_DATA_REC
    la $t0, .DISPLAY_INTR_DATA_REC
    jal .CONFIGURE_INTERRUPTION_HANDLER # definida em interruptions.asm
    # ===========================================
    li $a0, 51 # USB_02_DEV_CONNECT
    la $t0, .DISPLAY_INTR_DEV_CONNECT
    jal .CONFIGURE_INTERRUPTION_HANDLER # definida em interruptions.asm
    # ===========================================
    li $a0, 52 # USB_02_DEV_DISCONNECT
    la $t0, .DISPLAY_INTR_DEV_DISCONNECT
    jal .CONFIGURE_INTERRUPTION_HANDLER
    # ===========================================
    li $a0, 53 # USB_02_DEV_SEND_FAIL
    la $t0, .DISPLAY_INTR_SEND_FAIL
    jal .CONFIGURE_INTERRUPTION_HANDLER
    # ===========================================
    
    # colocar o endereço da função
    # PRINT_CHAR em $s2 para a rotina
    # que carregou este driver guardar 
    # como referencia; então ELE
    # chamará o PRINT_CHAR externamente
    # o caracter a ser enviado deverá estar em $v1
    la $s2, .PRINT_CHAR
   # la $s3, .PRINT_LINE

     # retornar para o programa que carregou
    # este driver
    jr $a1
    





WAIT_FOR_DISPLAY:
   # li $v0, 400
  #  syscall

    la $t5, RESOLUTION_DEFINED # address of resolution defined variable
    lb $t5, 0($t5)   # load resolution defined variable value
    li $t6, 1        # value 1 (defined)
    beq $t5, $t6, .END_WAIT_FOR_DISPLAY # wait until resolution defined

    la $t6, RESOLUTION_REQUESTED
    lb $t6, 0($t6)   # load resolution requested variable value
    li $t7, 1        # value 1 (requested)
    beq $t6, $t7, .WAIT_FOR_DISPLAY # wait until resolution requested

    la $t5, CONNECTED # address of connected variable
    lb $t5, 0($t5)   # load connected variable value
    li $t6, 1        # value 1 (connected)
    beq $t5, $t6, .REQUEST_RESOLUTION # wait until connected

    jal .END_WAIT_FOR_DISPLAY # wait until connected

END_WAIT_FOR_DISPLAY:
    jr $gp

REQUEST_RESOLUTION:


    la $s1, DISPLAY_USB_ADDR
    lw $s0, 0($s1)

    li $t0, 1    # OP = WRITE
    sb $t0, 5($s0) 

    li $t0, 0x1A    # MODE = TEXT
    sb $t0, 6($s0) 

    li $t0, 0x99    # ACTION = 0x99 (request resolution)
    sb $t0, 10($s0)

    li $t0, 1            # firing byte
    sb $t0, 0($s0)       # write byte (send)

    la $t1, RESOLUTION_REQUESTED # address of resolution requested variable
    li $t2, 1            # value 1 (requested)
    sb $t2, 0($t1)       # store value in RESOLUTION_REQUESTED

    jal .WAIT_FOR_DISPLAY

SET_RESOLUTION:
    lw $t0, 4($s1) # value X
    lw $t1, 8($s1) # value Y

    la $t2, SCREEN_WIDTH # address X
    sw $t0, 0($t2) # store value X in SCREEN_WIDTH

    la $t2, SCREEN_HEIGHT # address Y
    sw $t1, 0($t2) # store value Y in SCREEN_HEIGHT

    la $s1, DISPLAY_BFF_LEN

    li $t4, 8      # 8 bytes por caracter; modo inicial = texto (default)
    mul $s2, $t0, $t1 # multiplica X * Y
    mul $s2, $s2, $t4 # multiplica por 8 bytes por caractere
    sw $s2, 0($s1)

    la $t0, RESOLUTION_DEFINED
    li $t1, 1        # value 1 (defined)
    sb $t1, 0($t0)   # store value in RESOLUTION_DEFINED

    li $s3, 0x0
    sb $s3, 10($s0)
    sb $s3, 11($s0)
    sb $s3, 12($s0)
    sb $s3, 13($s0)
    sb $s3, 14($s0)
    sb $s3, 15($s0)
    sb $s3, 16($s0)
    sb $s3, 17($s0)
    sb $s3, 18($s0)
    sb $s3, 19($s0)
    sb $s3, 20($s0)

    jr $ra

DISPLAY_INTR_DATA_REC:

    # tupla [0x92, DADOS...]
    # 0x92 - ação (0x92 = mudar resolução)

    # ler os dados da memoria da porta USB
    la $s1, DISPLAY_USB_ADDR  # endereço da variavel
    lw $s1, 0($s1)    # valor da variavel (endereço do buffer USB)
    addi $s1, $s1, 10 # buffer start

    lw $t5, 0($s1)

    li $t6, 92

    beq $t5, $t6, .SET_RESOLUTION # action = 0 (disconnect)

DISPLAY_INTR_DEV_CONNECT:



  la $s1, DISPLAY_USB_ADDR
  lw $s0, 0($s1)

  li $t0, 1    # OP = WRITE
  sb $t0, 5($s0) 

  li $t0, 0x1A    # MODE = TEXT
  sb $t0, 6($s0) 

  li $t1, 1
  sb $t1, 0($s0) # action

  la $t2, CONNECTED # address of connected variable
  li $t3, 1        # value 1 (connected)
  sb $t3, 0($t2)   # store value in CONNECTED

  #jr $ra

DISPLAY_INTR_DEV_DISCONNECT:
    li $s6, 999

DISPLAY_INTR_SEND_FAIL:
    li $s6, 999

PRINT_CHAR:
   
    # args (provided by external previous rotine):
    # s0: char code
    # s1: fore color
    # s2: back color
    # s3: font style
    # s4: font size
    # s5: padding
    # s6: padding
    # s7: padding
    
   

    move $t9, $ra  # save return address

    # 1 - get X value
    la $t1, CURSOR_X # CURSOR_X address
    lw $t1, 0($t1)     # X value
    # $t1 = CURSOR X

    la $t2, CURSOR_Y # CURSOR_Y address
    lw $t2, 0($t2)     # Y value
    # $t2 = CURSOR Y


    # 2 - check if cursor_x ends of line
    la $t3, SCREEN_WIDTH
    lw $t3, 0($t3)

    li $t5, 8 # 8 bytes por caractere (modo texto)
    move $t4, $t1 # $t4 = CURSOR_X
    mul $t4, $t4, $t5 # $t4 = CURSOR_X * 8 (bytes por caractere)

    div  $t3, $t5
    mflo $k0
    beq $t1, $k0,  .NEW_LINE # if cursor_x == screen_width, go to new line

    # refresh cursor_x and cursor_y
    la $t1, CURSOR_X # CURSOR_X address
    lw $t1, 0($t1)     # X value
    la $t2, CURSOR_Y # CURSOR_Y address
    lw $t2, 0($t2)     # Y value

    # 2 - get display buffer addr
    la $t6, DISPLAY_BUFFER # display buffer address

    mul $t7, $t2, $t3 # $t7 = Y * SCREEN_WIDTH
    add $t7, $t7, $t1 # $t7 = Y * SCREEN_WIDTH + X (bytes por caractere)
    mul $t7, $t7, $t5 # $t7 = Y * SCREEN_WIDTH * 8 (bytes por caractere)
    add $t7, $t7, $t6 # $t7 = display buffer address + offset (Y * SCREEN_WIDTH * 8 + X * 8)    
    # $t7 = buffer address offset

    # 3 - store char code in display buffer
    sb $s0, 0($t7) # char code
    sb $s1, 1($t7) # fore color
    sb $s2, 2($t7) # back color
    sb $s3, 3($t7) # font style
    sb $s4, 4($t7) # font size
    sb $s5, 5($t7) # padding 
    sb $s6, 6($t7) # padding
    sb $s7, 7($t7) # padding

    # transfer data from display buffer to USB buffer
    la $a0, DISPLAY_BUFFER # source address
    la $t1, DISPLAY_USB_ADDR     # usb01 addr data
    lw $a1, 0($t1)       # usb01 addr value
    addi $a1, $a1, 10    # usb01 buffer start

    # first 8 bytes is reserved for value X and Y of cursor
    la $t5, CURSOR_X
    lw $t5, 0($t5)     # X value
    sw $t5, 0($a1) # X value in USB buffer

    la $t6, CURSOR_Y # CURSOR_Y address
    lw $t6, 0($t6)     # Y value
    sw $t6, 4($a1) # Y value in USB buffer

    addi $a1, $a1, 8    # usb01 buffer start
    # after 8 bytes, the data is stored in display buffer
    la $t2, DISPLAY_BFF_LEN # display buffer length
    lw $a2, 0($t2)          # size of data to transfer 
    li $v0, 110             # syscall to transfer byte-block with DMA 
    syscall

    la $t3, DISPLAY_USB_ADDR  # usb address
    lw $t4, 0($t3)

    li $t0, 1    # OP = WRITE
    sb $t0, 5($t4) 

    li $t0, 0x1A    # MODE = TEXT
    sb $t0, 6($t4) 

    li $t0, 1            # firing byte
    sb $t0, 0($t4)       # write byte (send)



    # 4 - update X value in CURSOR_X
    la $t1, CURSOR_X # CURSOR_X address
    lw $t1, 0($t1)     # X value
    addi $t1, $t1, 1 # increases value of X
    la $t2, CURSOR_X # CURSOR_X address
    sw $t1, 0($t2)   # update X value (value in $t1, address in $t2) 

    addi $t9, $t9, 4 # increment to next instruction (return address)
    jr $t9 # return to the next instruction after this function



PRINT_LINE:
    move $gp, $ra # save return address
    addi $gp, $gp, 4 # increment to next instruction

    jal .PRINT_LOOP
    

PRINT_LOOP:
    # args (provided by external previous rotine):
    # t0: text buffer address
    # s1: fore color
    # s2: back color
    # s3: font style
    # s4: font size
    # s5: padding
    # s6: padding
    # s7: padding

    lb $s0, 0($sp) # load first char from buffer

    li $t4, 0 # null terminator

    beq $s0, $t4, .END_PRINT_LINE # if char is null terminator, go to end of print line

    jal .PRINT_CHAR # print char
    addi $sp, $sp, 1 # increment buffer address

    jal  .PRINT_LOOP

PRINT_NO_LINE:

    # args (provided by external previous rotine):
    # t0: text buffer address
    # s1: fore color
    # s2: back color
    # s3: font style
    # s4: font size
    # s5: padding
    # s6: padding
    # s7: padding
    # t0: counter; (need be reset to 0)

    lb $s0, 0($sp) # load first char from buffer

    li $t4, 0 # null terminator

    beq $s0, $t4, .END_PRINT # if char is null terminator, go to end of print line


    jal .PRINT_CHAR # print char
    addi $sp, $sp, 1 # increment buffer address

    jal  .PRINT_NO_LINE

END_PRINT:
   jr $gp

END_PRINT_LINE:
    jal .NEW_LINE # go to new lin
 
    jr $gp

NEW_LINE:
    # 0 X
    la $t5, CURSOR_X
    li $t6, 0
    sw $t6, 0($t5)

    # +1 Y
    la $t5, CURSOR_Y
    lw $t6, 0($t5)
    addi $t6, $t6, 1
    sw $t6, 0($t5)

    li $t5, 0
    li $t5, 0

    jr $ra





    

