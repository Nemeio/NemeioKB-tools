using System;
using System.Collections.Generic;
using Nemeio.Core.Exceptions;

namespace Nemeio.Core.DataModels.Configurator
{
    public class ConfiguratorLayout
    {
        private int _width;
        private int _height;

        public int Width
        {
            get => _width;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException(String.Format("{0} can't be equal or less than zero", nameof(Width)));
                }
                else
                {
                    _width = value;
                }
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException(String.Format("{0} can't be equal or less than zero", nameof(Height)));
                }
                else
                {
                    _height = value;
                }
            }
        }

        public BackgroundMode Background { get; set; }

        public IList<Key> Keys { get; set; }

        public ConfiguratorLayout(int width, int height, BackgroundMode backMode, IList<Key> keys)
        {
            Width = width;
            Height = height;
            Background = backMode;
            Keys = keys;
        }
    }
}
