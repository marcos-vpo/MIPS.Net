.include "interruptions.asm"
.include "usb.asm"

.data 

      usb_01_addr:    .word 10000
      usb_01_buffer:  .word 1000000       # ~1 MB
      usb_02_addr:    .word 1010010       # 10000 + 10 + 1_000_000
      usb_02_buffer:  .word 1000000
      usb_03_addr:    .word 2020020       # usb_02 + 10 + 1_000_000
      usb_03_buffer:  .word 1000000
      usb_04_addr:    .word 3030030       # usb_03 + 10 + 1_000_000
      usb_04_buffer:  .word 1000000
      kernel_file:    .asciiz "/boot/mOS.mex"
      # 2 portas reservadas após USB_04
      kernel_addr:    .word 6100000
      # parâmetros do kernel (~8 MB)
      kernel_args:    .word 8000000

.text
_firmware_main:
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


_init_usb_01:
      # header registers
      move $v1, $gp
      li $s0, 0x01   # ID
      la $s1, usb_01_addr
      lw $s1, 0($s1) # load base addr
      li $s2, 0x2D   # class
      
      # buffer size = ~1mb
      la $t0, usb_01_buffer
      lw $t0, 0($t0) # load buffer size
      move $s3, $t0  # ~ 1mb in $s3 ( dev buffer size )

      li $s4, 6000   # socket port number
      
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
      la $s1, usb_02_addr
      lw $s1, 0($s1) # load base addr
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
      la $s1, usb_03_addr
      lw $s1, 0($s1) # load base addr
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
      la $s1, usb_04_addr
      lw $s1, 0($s1) # load base addr
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

_activate_usb_all:
    
    li $s0, 6000
    li $s1, 0x01
    la $s2, usb_01_addr
    lw $s2, 0($s2) # load base addr
    jal .USB_MAP_BIOS 
    
    li $s0, 6010
    li $s1, 0x02
    la $s2, usb_02_addr
    lw $s2, 0($s2) # load base addr
    jal .USB_MAP_BIOS 

    li $s0, 6020
    li $s1, 0x03
    la $s2, usb_03_addr
    lw $s2, 0($s2) # load base addr
    jal .USB_MAP_BIOS 

    li $s0, 6030
    li $s1, 0x04
    la $s2, usb_04_addr
    lw $s2, 0($s2) # load base addr
 #   jal .USB_MAP_BIOS 

    jal ._load_kernel

_load_kernel: 
   
   # routine to requests data
   # to external storage device
   la $s1, usb_01_addr
   lw $sp, 0($s1)  # device memory start

   li $t1, 0
   sb $t1, 4($sp) # 0x00 - read operation
   sb $t1, 5($sp) # 0x00 - sincronous operation

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

   li $t3, 0xF0 # command: read file
   sb $t3, 5($sp)  # command

   li $t1, 1 # firing-byte
   lb $t1, 0($sp)  # write firing-byte; in this moment, external device will be action
  
   lw $a0, 0($s1)  # usb01 addr
   addi $a0, $a0, 10  # usb01 buffer start

   lw $t8, 0($a0) # kernel length
   addi $a0, $a0, 4 # skip kernel length

   la $t5, kernel_args
   lw $t5, 0($t5) # load kernel args addr
   
   # [start] fill kernel args
   la $t4, usb_01_addr
   lw $t4, 0($t4) # load usb01 base addr
   sw $t4, 0($t5) # store usb01 base addr as first arg

   la $t4, usb_02_addr
   lw $t4, 0($t4) # load usb02 base addr
   sw $t4, 4($t5) # store usb02 base addr as second arg

   la $t4, usb_03_addr
   lw $t4, 0($t4) # load usb03 base addr
   sw $t4, 8($t5) # store usb03 base addr as third arg

   la $t4, usb_04_addr
   lw $t4, 0($t4) # load usb04 base addr
   sw $t4, 12($t5) # store usb04 base addr as fourth arg

   la $t4, kernel_addr
   lw $t4, 0($t4) # load kernel addr
   sw $t4, 16($t5) # store kernel addr as fifth arg

   sw $t8, 20($t5) # store kernel length as six arg
   # [end] fill kernel args

   la $a1, kernel_addr # destination addr (data)
   lw $a1, 0($a1) # destination
   move $a2, $t8 # bytes
   li $v0, 110 # syscall to transfer byte-block with DMA 
   syscall

   la $t0, kernel_addr
   lw $t0, 0($t0) # load kernel addr
   jr $t0


_loop:
    li $v0, 200      # idle syscall
    li $t0, 100
    add $t2, $v0, $t0
    jal ._loop