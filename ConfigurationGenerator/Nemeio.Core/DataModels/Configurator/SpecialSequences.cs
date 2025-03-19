using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nemeio.Core.Extensions;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.DataModels.Configurator
{
    public class SpecialSequences
    {
        private const int CTRL_POSITION = 71 + 1;   //  TODO : "+1" Wait Eloise refactor on keyboard
        private const int CTRL_RIGHT_POSITION = 77 + 1;
        private const int ALT_POSITION = 74 + 1;
        private const int ALTGR_POSITION = 76 + 1;
        private const int DELETE_POSITION = 15 + 1;

        public Permutation<int> WinSAS { get; }

        public SpecialSequences(Permutation<int> sas)
        {
            WinSAS = sas;
        }

        public static SpecialSequences Empty => new SpecialSequences(
            new Permutation<int>()
        );

        public static SpecialSequences Default => ObjectExtensions.IsOSXPlatform() ? MacDefault : WindowsDefault;

        private static SpecialSequences MacDefault = new SpecialSequences(new Permutation<int>());

        private static SpecialSequences WindowsDefault = new SpecialSequences(
            Layout.GetPermutations(new List<int>() { CTRL_POSITION, ALT_POSITION, DELETE_POSITION }, Layout.WinSASLength)
                .Concat(Layout.GetPermutations(new List<int>() { CTRL_POSITION, ALTGR_POSITION, DELETE_POSITION }, Layout.WinSASLength))
                .Concat(Layout.GetPermutations(new List<int>() { CTRL_RIGHT_POSITION, ALT_POSITION, DELETE_POSITION }, Layout.WinSASLength))
                .Concat(Layout.GetPermutations(new List<int>() { CTRL_RIGHT_POSITION, ALTGR_POSITION, DELETE_POSITION }, Layout.WinSASLength))
        );
    }
}
