using System;
using System.Collections.Generic;
using System.Text;
using Nemeio.Core.Exceptions;

namespace Nemeio.Core.DataModels.Configurator
{
    public enum BackgroundMode
    {
        Light = 0,
        Dark = 1
    }

    public class Key
    {
        public const int MAX_NUMBER_ACTION = 4;

        private IList<Action> _actions;

        public int Index { get; set; }

        public float Width { get; set; }

        public float Height { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public BackgroundMode Background { get; set; }

        public IList<Action> Actions
        {
            get => _actions;
            set
            {
                if (value.Count > MAX_NUMBER_ACTION)
                {
                    throw new TooManyActionException();
                }
                else
                {
                    _actions = value;
                }
            }
        }
    }
}
