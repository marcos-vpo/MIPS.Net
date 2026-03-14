using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mOS.IOKit.Devices
{
    namespace mOS.IOKit.Devices
    {
        internal sealed class IOKeyPressedEvent
        {
            /// <summary>
            /// True if Ctrl was pressed
            /// </summary>
            public bool Ctrl { get; init; }

            /// <summary>
            /// True if Shift was pressed
            /// </summary>
            public bool Shift { get; init; }

            /// <summary>
            /// True if Alt was pressed
            /// </summary>
            public bool Alt { get; init; }

            /// <summary>
            /// Raw keycode coming from the keyboard device (WinForms origin)
            /// </summary>
            public byte KeyCode { get; init; }

            /// <summary>
            /// Interpreted control key, if applicable
            /// </summary>
           public KeyboardControlKey ControlKey { get; init; }

            /// <summary>
            /// True when this key represents a printable character
            /// </summary>
            public bool IsPrintable =>
                ControlKey == KeyboardControlKey.None;
        }
    }

}
