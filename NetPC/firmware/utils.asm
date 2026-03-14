.text
CLEAR_REGISTERS:
     li $zero, 0      # $zero é sempre zero, mas aqui para clareza

      move $t0, $zero   # $t0 = 0
      move $t1, $zero   # $t1 = 0
      move $t2, $zero   # $t2 = 0
      move $t3, $zero   # $t3 = 0
      move $t4, $zero   # $t4 = 0
      move $t5, $zero   # $t5 = 0
      move $t6, $zero   # $t6 = 0
      move $t7, $zero   # $t7 = 0

      move $s0, $zero   # $s0 = 0
      move $s1, $zero   # $s1 = 0
      move $s2, $zero   # $s2 = 0
      move $s3, $zero   # $s3 = 0
      move $s4, $zero   # $s4 = 0
      move $s5, $zero   # $s5 = 0
      move $s6, $zero   # $s6 = 0
      move $s7, $zero   # $s7 = 0

      move $a0, $zero   # $a0 = 0
      move $a1, $zero   # $a1 = 0
      move $a2, $zero   # $a2 = 0
      move $a3, $zero   # $a3 = 0

      move $v0, $zero   # $v0 = 0
      move $v1, $zero   # $v1 = 0

      move $k0, $zero   # $k0 = 0
      move $k1, $zero   # $k1 = 0

      move $gp, $zero   # $gp = 0
      move $sp, $zero   # $sp = 0

    jr $ra