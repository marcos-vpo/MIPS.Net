
.include "usb.asm"
.include "display.asm"
.include "interruptions.asm"
.include "utils.asm"
.include "keyboard.asm"

.data
    kernel_bin: .space 0x100000 # 1mb
    buffer: .byte 68
    
    usb_01_addr: .word 10000
    usb_01_buffer: .word 10002400 # 1mb buffer

    usb_02_addr: .word 10012500
    usb_02_buffer: .word 8 # 8b buffer

    usb_03_addr: .word 0
    usb_04_addr: .word 0

    init_msg: .asciiz "========== [NetPC System Firmware] =========="
    msg_version: .asciiz "========== [    version 0.0.6    ] =========="
    msg_init_kbd: .asciiz "Initializing keyboard..."
.text
_INIT_USB_PORTS:
      la $gp, ._INIT_USB_PORTS
      addi $gp, $gp, 12
      jal ._init_usb_01

      la $gp, ._INIT_USB_PORTS
      addi $gp, $gp, 24
      jal ._init_usb_02

      jal .CLEAR_REGISTERS
      jal ._init_display_driver
      jal .CLEAR_REGISTERS

      la $sp, msg_init_kbd
      jal ._print_msg

      jal ._init_keyboard_driver
      jal .CLEAR_REGISTERS
      jal ._print_wellcome

_init_usb_01:
      move $v1, $gp
      li $s0, 0x01   # ID
      li $s1, 10000  # base addr
      li $s2, 0x2D   # class
      
      # buffer size = 1mb (store in $s3)
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

      la $s1, usb_02_addr
      lw $s1, 0($s1) # load usb_02_addr

      li $s2, 0x2D   # class
      
      # buffer size = 1mb (store in $s3)
      li $s3, 8

      li $s4, 6010   # port number
      
      # get first-free IVT entry address
      li $v0, 210
      syscall

      # then move to $s5
      move $s5, $v0 

      # interruption codes
      li $t4, 30 # data rec intr. code
      li $t5, 31 # dev connect intr. code
      li $t6, 32 # dev disconnect intr. code
      li $t7, 33 # send fail intr. code
      li $t8, 34 # rec fail intr. code


      jal .CONFIGURE_USB


_init_display_driver:
      li $a0, 10000
      la $a1, ._post_init_display_driver
      jal .CONFIGURE_DISPLAY
   
_init_keyboard_driver:
      move $k1, $ra # save return address
      addi $k1, $k1, 4 # increment to next instruction

      li $s0, 6010
      li $s1, 0x02
      la $s2, usb_02_addr
      lw $s2, 0($s2) # load usb_02_addr
      jal .USB_MAP_BIOS 

      la $a0, usb_02_addr
      lw $a0, 0($a0) # load usb_02_addr
      la $a1, ._keyboard_input
      jal .INIT_KEYBOARD


      jr $k1
      
_post_init_display_driver:

      li $s0, 6000
      li $s1, 0x01
      li $s2, 10000
      jal .USB_MAP_BIOS 

      la $gp, ._post_init_display_driver
      addi $gp, $gp, 28 # increment to next instruction

      jal .WAIT_FOR_DISPLAY # wait until connected

      la $ra, ._INIT_USB_PORTS
      addi $ra, $ra, 28 # increment to next instruction
      jr $ra
_print_wellcome:
      
      la $sp, init_msg
      jal ._print_msg

      la $sp, msg_version
      jal ._print_msg


      jal ._main_loop

_print_msg:
      move $k1, $ra
      addi $k1, $k1, 4 # increment to next instruction

      li $s1, 15   # fore color
      li $s2, 0   # back color
      li $s3, 0   # font style
      li $s4, 9   # font size
      li $s5, 0   # padding
      li $s6, 0   # padding
      li $s7, 0   # padding
      jal .PRINT_LINE

      jr $k1


_main_loop:
   li $t6, 50
   jal ._main_loop

_keyboard_input:
      


_halt:
      li $ra, 0
      jr $ra