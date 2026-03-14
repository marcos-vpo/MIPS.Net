.data
.link "C:\mOS\bin\mShell.dll"
.text
_main:
   li $v0, 410
   la $a0, ._ffi_wait_up #  enable ffi
   syscall
   jal .mOSShell:ShellMain
   li $v0, 1 # sys_exit
   syscall
_loop:
 jal ._loop
_ffi_wait_up:
  jal ._ffi_wait_up
