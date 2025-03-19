using Nemeio.Core.JsonModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Core.DataModels.Configurator
{
    public class Subaction
    {
        public string Data { get; set; }

        public KeyActionType Type { get; set; }

        public bool IsShift() => Data == KeyboardLiterals.Shift;

        public bool IsAltGr() => Data == KeyboardLiterals.AltGr;

        public bool IsCtrl() => Data == KeyboardLiterals.Ctrl;

        public bool IsAlt() => Data == KeyboardLiterals.Alt;

        public bool IsAnyModifier() => IsShift() || IsAltGr();

        public static Subaction CreateModifierAction(string modifier)
        {
            return new Subaction()
            {
                Data = modifier,
                Type = KeyActionType.Special
            };
        }
    }
}
