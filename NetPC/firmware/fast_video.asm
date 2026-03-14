.link "abi-test\\abitest.dll" # VINCULAR ASSEMBLY .NET
.text
_init:
    li $v0, 410     # HABILITAR FFI
    la $a0, ._ffi_wait_up # ENDEREÇO DA FUNÇÃO DE WAIT-UP QUE VAI RODAR ENQUANTO ESPERA O METODO .NET TERMINAR
    syscall
    jal .MyABIClass:WriteAnyWhere # CHAMAR MÉTODO .NET

_ffi_wait_up:
    li $v0, 400     # CHAMAR DEBUGGER PRA TESTAR SE O FFI ESTÁ FUNCIONANDO
   # syscall
    jal ._ffi_wait_up