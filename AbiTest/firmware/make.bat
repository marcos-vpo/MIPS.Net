mpc -c firmware.asm -d
mpc -c mOS.asm -d
copy .\firmware.mex C:\mOS\firmware\firmware.mex
copy .\mOS.mex C:\mOS\storage\boot\mOS.mex
copy .\firmware.asm_full.mdbx C:\mOS\firmware\firmware.asm_full.mdbx
copy .\mOS.asm_full.mdbx C:\mOS\storage\boot\mOS.asm_full.mdbx