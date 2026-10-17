.link "C:\mOS\bin\mShell.dll"
.data
.text
_main:
   li $v0, 410              #  enable ffi
   la $a0, ._ffi_wait_up 
   syscall
      
   jal .mOSShell:ShellMain

   # li $v0, 99 -- opcional -> o retorno de ShellMain já será colocado em $v0 automaticamente
   # porém em um cenário ideal o correto seria impor isso aqui, até por segurança

   syscall

_resume:
   jal .mOSShell:OnResume 

_ffi_wait_up:
  jal ._ffi_wait_up