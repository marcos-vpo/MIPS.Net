.data


.text
_init_display:
    move $s1, $ra

    li $a0, 60
    la $a1, ._display_data_rec_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    li $a0, 61
    la $a1, ._display_dev_connect_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    li $a0, 62
    la $a1, ._display_dev_disconnect_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    li $a0, 63
    la $a1, ._display_send_fail_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    li $a0, 64
    la $a1, ._display_rec_fail_handler
    jal .CONFIGURE_INTERRUPTION_HANDLER

    move $ra, $s1
    jr $ra

_display_data_rec_handler:
    jal .IOInterruptService:DeviceDataReceived

_display_dev_connect_handler:
    jal .IOInterruptService:DeviceConnected

_display_dev_disconnect_handler:
    jal .IOInterruptService:DeviceDisconnected

_display_send_fail_handler:
    jal .IOInterruptService:DeviceSendFail

_display_rec_fail_handler:
    jal .IOInterruptService:DeviceReceiveFail
