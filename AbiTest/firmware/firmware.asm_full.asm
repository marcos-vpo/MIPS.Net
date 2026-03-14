.data
#.include "interruptions.asm"
#.include "usb.asm"
    usb_01_addr: .word 10000
    usb_01_buffer: .word 10000   # ~1MB buffer
    usb_02_addr: .word 20000      # 10000 + 1000000
    usb_02_buffer: .word 10000
    usb_03_addr: .word 30000      # 1010000 + 1000000
    usb_03_buffer: .word 10000
    usb_04_addr: .word 40000      # 2010000 + 1000000
    usb_04_buffer: .word 10000
    kernel_file: .asciiz "/boot/mOS.mex"
    kernel_addr: .word 60000      # load kernel at 4MB
    kernel_params_addr: .word 70000
.text
_firmware_main:
   # jal ._loop
    la $gp, ._firmware_main
    addi $gp, $gp, 12
    jal ._init_usb_01
    la $gp, ._firmware_main
    addi $gp, $gp, 24
    jal ._init_usb_02
    la $gp, ._firmware_main
    addi $gp, $gp, 36
    jal ._init_usb_03
    la $gp, ._firmware_main
    addi $gp, $gp, 48
    jal ._init_usb_04
    jal ._activate_usb_all
_load_kernel:
      # routine to requests data
      # to external storage device
      la $s1, usb_01_addr
      lw $sp, 0($s1)  # device memory start
      li $t1, 0
      sb $t1, 5($sp) # 0x00 - read operation
      sb $t1, 6($sp) # 0x00 - sincronous operation
      la $s2, usb_01_buffer
      # clear device data-area
      lw $a0, 0($s1) # usb01 addr
      addi $a0, $a0, 10 # usb01 buffer start
      lw $a1, 0($s2) # length to clear; destination address
      li $v0, 115 # syscall to clear data area
      syscall
      # transfer file name from .data address
      # to device memory address
      la $a0, kernel_file # source address
      lw $a1, 0($s1)  # usb01 addr
      addi $a1, $a1, 10  # usb01 buffer start
      li $a2, 13 # b length
      li $v0, 110 # syscall to transfer byte-block with DMA 
      syscall
      li $t1, 1 # firing-byte
      lb $t1, 0($sp)  # write firing-byte; in this moment, external device will be action
      lw $a0, 0($s1)  # usb01 addr
      addi $a0, $a0, 10  # usb01 buffer start
      la $a1, kernel_addr
      lw $a1, 0($a1) # destination
      lw $a2, 0($s2) # bytes
      li $v0, 110 # syscall to transfer byte-block with DMA 
      syscall
      la $t1, kernel_params_addr
      lw $t1, 0($t1) # load kernel params addr
      la $t2, usb_01_addr
      lw $t2, 0($t2) 
      sw $t2, 0($t1) 
      la $t2, usb_02_addr
      lw $t2, 0($t2) 
      sw $t2, 4($t1) 
      la $t2, usb_03_addr
      lw $t2, 0($t2) 
      sw $t2, 8($t1) 
      la $t2, usb_04_addr
      lw $t2, 0($t2) 
      sw $t2, 12($t1) 
      la $t0, kernel_addr
      lw $t0, 0($t0) # load kernel addr
      jr $t0
_activate_usb_all:
    li $s0, 6000
    li $s1, 0x01
    la $s2, usb_01_addr
    lw $s2, 0($s2)
    jal .USB_MAP_BIOS 
    li $s0, 6010
    li $s1, 0x02
    la $s2, usb_02_addr
    lw $s2, 0($s2)
    jal .USB_MAP_BIOS 
    li $s0, 6020
    li $s1, 0x03
    la $s2, usb_03_addr
    lw $s2, 0($s2)
    jal .USB_MAP_BIOS 
    li $s0, 6030
    li $s1, 0x04
    la $s2, usb_04_addr
    lw $s2, 0($s2)
    jal .USB_MAP_BIOS 
    jal ._load_kernel
_init_usb_01:
      move $v1, $gp
      li $s0, 0x01   # ID
      la $s1, usb_01_addr  # base addr
      lw $s1, 0($s1)
      li $s2, 0x2D   # class
      # buffer size = 10mb
      la $t0, usb_01_buffer
      lw $t0, 0($t0) # load buffer size
      move $s3, $t0  # 10mb in $s3 ( dev buffer size )
      li $s4, 6000   # port number
      # get first-free IVT entry address
      li $v0, 210
      syscall
      # then move to $s5
      move $s5, $v0 
      # interruption codes
      li $t4, 50 # data rec intr. code
      li $t5, 51 # dev connect intr. code
      li $t6, 52 # dev disconnect intr. code
      li $t7, 53 # send fail intr. code
      li $t8, 54 # rec fail intr. code
      jal .CONFIGURE_USB
_init_usb_02:
      move $v1, $gp
      li $s0, 0x02   # ID
      la $s1, usb_02_addr  # base addr
      lw $s1, 0($s1)
      li $s2, 0x3D   # class
      # buffer size = 10mb
      la $t0, usb_02_buffer
      lw $t0, 0($t0) # load buffer size
      move $s3, $t0  # 10mb in $s3 ( dev buffer size )
      li $s4, 6010   # port number
      # get first-free IVT entry address
      li $v0, 210
      syscall
      # then move to $s5
      move $s5, $v0 
      # interruption codes
      li $t4, 60 # data rec intr. code
      li $t5, 61 # dev connect intr. code
      li $t6, 62 # dev disconnect intr. code
      li $t7, 63 # send fail intr. code
      li $t8, 64 # rec fail intr. code
      jal .CONFIGURE_USB
_init_usb_03:
      move $v1, $gp
      li $s0, 0x03   # ID
      la $s1, usb_03_addr  # base addr
      lw $s1, 0($s1)
      li $s2, 0x7D   # class
      # buffer size = 10mb
      la $t0, usb_03_buffer
      lw $t0, 0($t0) # load buffer size
      move $s3, $t0  # 10mb in $s3 ( dev buffer size )
      li $s4, 6020   # port number
      # get first-free IVT entry address
      li $v0, 210
      syscall
      # then move to $s5
      move $s5, $v0 
      # interruption codes
      li $t4, 70 # data rec intr. code
      li $t5, 71 # dev connect intr. code
      li $t6, 72 # dev disconnect intr. code
      li $t7, 73 # send fail intr. code
      li $t8, 74 # rec fail intr. code
      jal .CONFIGURE_USB
_init_usb_04:
      move $v1, $gp
      li $s0, 0x04   # ID
      la $s1, usb_04_addr  # base addr
      lw $s1, 0($s1)
      li $s2, 0x8D   # class
      # buffer size = 10mb
      la $t0, usb_04_buffer
      lw $t0, 0($t0) # load buffer size
      move $s3, $t0  # 10mb in $s3 ( dev buffer size )
      li $s4, 6030   # port number
      # get first-free IVT entry address
      li $v0, 210
      syscall
      # then move to $s5
      move $s5, $v0 
      # interruption codes
      li $t4, 80 # data rec intr. code
      li $t5, 81 # dev connect intr. code
      li $t6, 82 # dev disconnect intr. code
      li $t7, 83 # send fail intr. code
      li $t8, 84 # rec fail intr. code
      jal .CONFIGURE_USB
_loop:
    li $v0, 200      # idle syscall
    li $t0, 100
    add $t2, $v0, $t0
    jal ._loop
# vincula uma rotina de tratamento em uma interrupção
# da IVT.
# será feita a busca da entrada na IVT pelo código da syscal
# em seguida, será gravado na posição [4-8] da tupla, o endereço
# da rotina de tratamento a ser invocada
# parametros:
# $a0 - codigo da syscall desejada
# $t0 - o endereço da rotina de tratamento
CONFIGURE_INTERRUPTION_HANDLER:
    # $a0 deve estar com o codigo da syscall a ser localizada
    li $v0, 211 # $v0 - endereço da entrada com o codigo de $a0
    syscall
    li $sp, 0
    add $sp, $sp, $v0 #colocar em $sp a end. da entrada
    sw $t0, 4($sp) # gravar o endereço de tratamento de $a1 na pos [4-8] da tupla
    jr $ra
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
