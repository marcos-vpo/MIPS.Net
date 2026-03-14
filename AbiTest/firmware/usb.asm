
.text

# Parametros
# $v1 - endereço de retorno apos completar CONFIGURE_USB
# $s0 - ID do device
# $s1 - endereço-base do device
# $s2 - classe do device
# $s3 - tamanho do buffer do device
# $s4 - numero da porta (socket) de comunicacao do devic
# $s5 - inicio da IVT para mapeamento das interrupções
# $t4 - interrupcao DATA_REC
# $t5 - interrupcao DEV_CONNECT
# $t6 - interrupcao DEV_DISCONNECT
# $t7 - interrupcao SEND_FAIL
# $t8 - interrupcao REC_FAIL
CONFIGURE_USB:

    jal .USB_MAP_DEV_MEMORY        # chama a rotina de mapeamento do dispositivo
 
    #----- MAP INTERRUPTIONS ---- 
    jal .USB_MAP_DATA_REC           # chama a rotina de recebimento de dados do dispositivo
    jal .USB_MAP_DEV_CONNECT        # chama a rotina de conexão do dispositivo
    jal .USB_MAP_DEV_DISCONNECT     # chama a rotina de desconexão do dispositivo
    jal .USB_MAP_DEV_SEND_FAIL      # chama a rotina de falha de envio do dispositivo
    jal .USB_MAP_DEV_REC_FAIL       # chama a rotina de falha de recebimento do dispositivo
    #----- MAP INTERRUPTIONS ---- 

   # jal .USB_MAP_BIOS               # chama a rotina de mapeamento do BIOS

    jr $v1

USB_MAP_DEV_MEMORY:
    move $sp, $s1  # Ajusta o stack pointer para o início da região de mapeamento

    #---------------------------------------------------------------------
    #  LAYOUT DA MEMÓRIA DO DISPOSITIVO
    #---------------------------------------------------------------------
    # [ 0  0  0  0  0  0   0000    ~~~~~ ]
    #  [0][1][2][3][4][5][  6  ] [  ~~~  ]
    #   |--|--|--|--|--|----|--------|--- ACAO                          [1 byte ]
    #      |--|--|--|--|----|--------|--- ID DO DISPOSITIVO             [1 byte ]
    #         |--|--|--|----|--------|--- CLASS DO DISPOSITIVO          [1 byte ]
    #            |--|--|----|--------|--- STATUS DO DISPOSITIVO         [1 byte ]
    #               |--|----|--------|--- OPERATION DO DISPOSITIVO      [1 byte ]
    #                  |----|--------|--- MODO DO DISPOSITIVO           [1 byte ] 
    #                       |--------|--- TAMANHO DO BUFFER DE DADOS    [4 bytes]
    #                                |--- DADOS BRUTOS .............    [N bytes]
    #---------------------------------------------------------------------

    li $t1, 0
    sb $t1, 0($sp)  # Grava o registrador de ação no stack

    move $t1, $s0
    sb $t1, 1($sp)  # Grava o ID do dispositivo no stack

    move $t1, $s2
    sb $t1, 2($sp)  # Grava a classe do dispositivo no stack

    li $t1, 0x0
    sb $t1, 3($sp)  # Grava o registrador de status no stack

    li $t1, 0x0
    sb $t1, 4($sp)  # Grava o registrador de operação no stack

    li $t1, 0x0
    sb $t1, 5($sp)  # Grava o registrador de modo no stack

    move $t2, $s3
    sw $t2, 6($sp)  # Grava o tamanho do buffer de dados no stack

    jr $ra


USB_MAP_BIOS:



    #----- MAP BIOS ------
    move $a0, $s0         # numero da porta socket-usb
    move $a1, $s1         # ID do device
    move $a2, $s2         # endereço base do device
    li $v0, 101          # função 101 do BIOS - mapeia a porta socket na porta USB_PORT
    syscall              # call BIOS
    jr $ra

USB_MAP_DATA_REC:
    move $sp, $s5 # inicio da IVT 

    # USB_DATA_REC code
    move $t1, $t4   # syscall code
    sw $t1, 0($sp)   # write in pos [0-3]

    # USB_DATA_REC handler
    li $t1, 0        # endereço de tratamento 
    sw $t1, 4($sp)   # in pos [4-7]

    # device (if is) address
    move $t1, $s1     # em caso de dispositivo, o endereço da memoria do dispositivo
    sw $t1, 8($sp)  # in pos [8-11]

    jr $ra

USB_MAP_DEV_CONNECT:
    move $sp, $s5 # inicio da IVT 

    # USB_DEV_CONNECT code
    move $t1, $t5   # dados brutos
    sw $t1, 12($sp)   # in pos [12-15]

    # USB_DEV_CONNECT handler 
    li $t1, 0         # endereço de tratamento 
    sw $t1, 16($sp)  # in pos [16-19]

    # device (if is) address
    move $t1, $s1     # em caso de dispositivo, o endereço da memoria do dispositivo
    sw $t1, 20($sp)  # in pos [20-23]

    jr $ra

USB_MAP_DEV_DISCONNECT:
    move $sp, $s5 # inicio da IVT 

    # USB_DEV_DISCONNECT code
    move $t1, $t6   # dados brutos
    sw $t1, 24($sp)   # in pos [24-27]

    # USB_DEV_DISCONNECT handler 
    li $t1, 0         # endereço de tratamento 
    sw $t1, 28($sp)  # in pos [28-31]

    # device (if is) address
    move $t1, $s1     # em caso de dispositivo, o endereço da memoria do dispositivo
    sw $t1, 32($sp)  # in pos [32-35]

    jr $ra



USB_MAP_DEV_SEND_FAIL:
    move $sp, $s5 # inicio da IVT 
    # USB_DEV_SEND_FAIL code
    move $t1, $t7   # dados brutos
    sw $t1, 36($sp)     # in pos [36-39]

    # USB_DEV_SEND_FAIL handler 
    li $t1, 0         # endereço de tratamento 
    sw $t1, 40($sp)    # in pos [40-43]

    # device (if is) address
    move $t1, $s1     # em caso de dispositivo, o endereço da memoria do dispositivo
    sw $t1, 44($sp)  # in pos [44-47]

    jr $ra


USB_MAP_DEV_REC_FAIL:
    move $sp, $s5 # inicio da IVT 
    # USB_DEV_REC_FAIL code
    move $t1, $t8   # dados brutos
    sw $t1, 48($sp)    # in pos [48-51]

    # USB_DEV_REC_FAIL handler 
    li $t1, 0         # endereço de tratamento 
    sw $t1, 52($sp)   # in pos [52-55]

    # device (if is) address
    move $t1, $s1     # em caso de dispositivo, o endereço da memoria do dispositivo
    sw $t1, 56($sp)  # in pos [56-59]

    jr $ra
