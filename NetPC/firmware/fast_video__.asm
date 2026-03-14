.include "usb.asm"
.link "abitest.dll"

.data

usb_01_addr: .word 10000
usb_01_buffer: .word 1000000   # ~1MB buffer

usb_02_addr: .word 1010000      # 10000 + 1000000
usb_02_buffer: .word 1000000

usb_03_addr: .word 2010000      # 1010000 + 1000000
usb_03_buffer: .word 1000000

usb_04_addr: .word 3010000      # 2010000 + 1000000
usb_04_buffer: .word 1000000



.text
_init:

  #  jal .MyABIClass:WriteAnyWhere
 
    li $v0, 410
    la $a0, ._ffi_wait_up
    syscall

    jal .MyABIClass:WriteAnyWhere

    jal ._main_loop
  #  jal ._activate_usb_all

_ffi_wait_up:
   li $v0, 400
   syscall

   jal ._ffi_wait_up

_init_usb_01:
      move $v1, $gp
      li $s0, 0x01   # ID
      li $s1, 10000  # base addr
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
      li $s1, 1010000  # base addr
      li $s2, 0x2D   # class
      
      # buffer size = 10mb
      la $t0, usb_02_buffer
      lw $t0, 0($t0) # load buffer size
      move $s3, $t0  # 10mb in $s3 ( dev buffer size )

      li $s4, 6002   # port number
      
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
      li $s1, 2010000  # base addr
      li $s2, 0x2D   # class
      
      # buffer size = 10mb
      la $t0, usb_03_buffer
      lw $t0, 0($t0) # load buffer size
      move $s3, $t0  # 10mb in $s3 ( dev buffer size )

      li $s4, 6003   # port number
      
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
      li $s1, 3010000  # base addr
      li $s2, 0x2D   # class
      
      # buffer size = 10mb
      la $t0, usb_04_buffer
      lw $t0, 0($t0) # load buffer size
      move $s3, $t0  # 10mb in $s3 ( dev buffer size )

      li $s4, 6010   # port number
      
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
    li $s2, 10000
    jal .USB_MAP_BIOS 
    
    li $s0, 6002
    li $s1, 0x02
    li $s2, 1010000
    jal .USB_MAP_BIOS

    li $s0, 6003
    li $s1, 0x03
    li $s2, 2010000
   # jal .USB_MAP_BIOS 

    li $s0, 6004
    li $s1, 0x04
    li $s2, 3010000
   # jal .USB_MAP_BIOS

    jal ._main_loop 

_main_loop:

    la $s1, usb_01_addr
    lw $s0, 0($s1)

    li $t0, 1    # OP = WRITE
    sb $t0, 5($s0) 

    li $t0, 0x1A    # MODE = TEXT
    sb $t0, 6($s0) 

    li $t0, 0x99    # ACTION = 0x99 (request resolution)
    sb $t0, 10($s0)

    li $t0, 1            # firing byte
    sb $t0, 0($s0)       # write byte (send)

    li $v0, 400
 #   syscall

    li $t0, 0 #cursor x
    sw $t0, 10($s0)

    li $t0, 0 #cursor y
    sw $t0, 14($s0)

 #   lw $s0, 0($s1)

    li $t0, 1    # OP = WRITE
    sb $t0, 5($s0) 

    li $t0, 0x1A    # MODE = TEXT
    sb $t0, 6($s0) 

    li $t0, 1    # ACTION = 0x99 (request resolution)
    sb $t0, 10($s0)

    li $t0, 'A'    # ACTION = 0x99 (request resolution)
    sb $t0, 18($s0)

    li $t0, 1            # firing byte
   # sb $t0, 0($s0)       # write byte (send)


    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    addi $t5, $zero, 10
    li $t1, 10
    addi $t2, $zero, 20
    add $t3, $t1, $t2
    jal ._main_loop