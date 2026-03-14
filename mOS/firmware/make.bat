
mpc -c firmware.asm -d
mpc -c mOS.asm -d

move .\firmware.asm_full.asm .\compiled\firmware.asm_full.asm
move .\mOS.asm_full.asm .\compiled\mOS.asm_full.asm
move .\firmware.mex .\compiled\firmware.mex
move .\mOS.mex .\compiled\mOS.mex
move .\firmware.asm_full.mdbx .\compiled\firmware.asm_full.mdbx
move .\mOS.asm_full.mdbx .\compiled\mOS.asm_full.mdbx

copy .\compiled\firmware.mex C:\mOS\firmware\firmware.mex
copy .\compiled\mOS.mex C:\mOS\boot\mOS.mex
copy .\compiled\firmware.asm_full.mdbx C:\mOS\debug\firmware.asm_full.mdbx
copy .\compiled\mOS.asm_full.mdbx C:\mOS\debug\mOS.asm_full.mdbx