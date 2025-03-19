using Nemeio.LayoutGen.Models.Displayer;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;

namespace Nemeio.LayoutGen.Models
{
    public abstract class Component
    {
        private Point _position;

        protected Component(Size size, Point pos)
        {
            Size = size;
            Position = pos;
            Childrens = new List<Component>();
        }

        public Size Size { get; }

        public int Index { get; private set; }

        public IList<Component> Childrens { get; }

        public Point Position
        {
            get
            {
                if (Parent != null)
                {
                    return Parent.Position + _position;
                }
                else
                {
                    return _position;
                }
            }
            set => _position = value;
        }

        public IDisplayer Displayer { get; set; }

        public Component Parent { get; private set; }

        public void AddComponent(Component cpnt)
        {
            cpnt.Parent = this;
            cpnt.Index = Index + 1;

            Childrens.Add(cpnt);
        }

        public abstract void Render(SKCanvas canvas);
    }
}
