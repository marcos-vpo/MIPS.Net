.data

.text
main:
    jal .init_btn_01
    
    jr $s0

# ===== Inicialização do hardware botão =====
init_btn_01:
    li $v0, 210 # retorna o primeiro endereço não-utilizado na IVT
    syscall

    # ===== posicionar o $sp para o endereço livre na IVT ========
    li $sp, 0
    addi $sp, $v0, 0

    # ===== layout um tupla da IVT =====
    # [  . . . .  . . . .  . . . .   ]
    #    Cód.     Hndlr.   Endr.Hw.

    # ===== adicionar uma entrada na IVT ========
    li $t0, 600    # definir o código da interrupção que vai ser associado ao sinal do botão
    sw $t0, 0($sp) # primeiros 4 bytes da entrada

    la $t1, .btn_01_click_handler # endereço da rotina que vai manipular a intr.
    sw $t1, 4($sp) # próximos 4 bytes

    li $t2, 0x51   # ID do hardware na MotherBoard; Em caso de porta/dispositivo de comunicação como USB, aqui vai o endereço na memoria do mesmo
    sw $t2, 8($sp) # ultimos 4 bytes - total 12

    # ===== chamada da syscall que vai resultar numa chamada na MB que inicializa o botão físico  =====
    addi $a0, $t2, 0 # arg1 - ID do hardware
    addi $a1, $t0, 0 # arg2 - Codigo de interrupção
    li $v0, 102      # função 102 - Inicializa um botão físico
    syscall

    # ===== limpar os registradores envolvidos =====
    li $t0, 0
    li $t1, 0
    li $t2, 0
    li $v0, 0
    
    # ===== retornar =====
    jr $ra 

# ===== Rotina da IVT que responde ao click =====
btn_01_click_handler:
    addi $s1, $s1, 10
    addi $s2, $s2, 20
    addi $s1, $s1, 10
    addi $s2, $s2, 20
    addi $s1, $s1, 10
    addi $s2, $s2, 20
    addi $s1, $s1, 10
    addi $s2, $s2, 20
    addi $s1, $s1, 10
    addi $s2, $s2, 20
    addi $s1, $s1, 10
    addi $s2, $s2, 20
    addi $s1, $s1, 10