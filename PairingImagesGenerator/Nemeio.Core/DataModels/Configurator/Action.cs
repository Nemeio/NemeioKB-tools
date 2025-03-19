using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nemeio.Core.Exceptions;

namespace Nemeio.Core.DataModels.Configurator
{
    public enum KeyActionType
    {
        Unicode = 1,
        Special = 2,
        Application = 3,
        Url = 4
    }

    public enum KeyModifier
    {
        None = 0,
        Shift = 1,
        AltGr = 2,
        Both = 3
    }

    public class Action
    {
        public const string EMBEDDED_PREFIX = "emb://";
        public const int MAX_ITEM_LENGTH = 6;

        private string _display;

        public KeyModifier Modifier { get; set; }

        public Font Font { get; set; }

        public string Display
        {
            get => _display;
            set
            {
                if (Action.IsEmbeddedResource(value))
                {
                    _display = value;
                }
                else
                {
                    if (value.Length > MAX_ITEM_LENGTH)
                    {
                        throw new TooLargeItemException();
                    }
                    else
                    {
                        _display = value;
                    }
                }
            }
        }

        public IList<Subaction> Subactions { get; set; }

        public static bool IsEmbeddedResource(string val) => val.StartsWith(EMBEDDED_PREFIX);
    }
}
