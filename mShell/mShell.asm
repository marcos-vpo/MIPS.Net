.link "C:\mOS\bin\mShell.dll"
.data
.text
_main:
   li $v0, 410              #  enable ffi
   la $a0, ._ffi_wait_up 
   syscall
    
   li $v0, 415              #  register on_resume
   la $a0, ._resume
   syscall

   jal .mOSShell:ShellMain

_resume:
   jal .mOSShell:OnResume 

_ffi_wait_up:
  jal ._ffi_wait_up