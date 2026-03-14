.data


.text
_init_keyboard:
    move $s1, $ra

    li $a0, 70
    la $a1, ._keyboard_data_rec_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    li $a0, 71
    la $a1, ._keyboard_dev_connect_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER
    
    li $a0, 72
    la $a1, ._keyboard_dev_disconnect_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    li $a0, 73
    la $a1, ._keyboard_send_fail_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    li $a0, 74
    la $a1, ._keyboard_rec_fail_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    move $ra, $s1
    jr $ra

_keyboard_data_rec_handler:
    jal .IOInterruptService:DeviceDataReceived

_keyboard_dev_connect_handler:
    jal .IOInterruptService:DeviceConnected

_keyboard_dev_disconnect_handler:
    jal .IOInterruptService:DeviceDisconnected

_keyboard_send_fail_handler:
    jal .IOInterruptService:DeviceSendFail

_keyboard_rec_fail_handler:
    jal .IOInterruptService:DeviceReceiveFail
