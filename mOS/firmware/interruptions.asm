.text

# vincula uma rotina de tratamento em uma interrupção
# da IVT.
# será feita a busca da entrada na IVT pelo código da syscal
# em seguida, será gravado na posição [4-8] da tupla, o endereço
# da rotina de tratamento a ser invocada

# parametros:
# $a0 - codigo da syscall desejada
# $a1 - o endereço da rotina de tratamento
CONFIGURE_INTERRUPTION_HANDLER:
    # $a0 deve estar com o codigo da syscall a ser localizada
    li $v0, 211 # $v0 - endereço da entrada com o codigo de $a0
    syscall

    li $sp, 0
    add $sp, $sp, $v0 #colocar em $sp a end. da entrada

    sw $a1, 4($sp) # gravar o endereço de tratamento de $a1 na pos [4-8] da tupla

    jr $ra