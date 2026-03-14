.link "C:\mOS\bin\mOS.dll"

.data
.text
_mos_start:
  li $v0, 410
  la $a0, ._ffi_wait_up #  enable ffi
  syscall



  jal .mos_kernel:start

  jal ._kernel_loop

_kernel_loop:
  jal ._kernel_loop

_ffi_wait_up:
  jal ._ffi_wait_up

