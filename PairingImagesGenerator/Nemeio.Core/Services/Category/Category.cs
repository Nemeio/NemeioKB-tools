using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Services.Category
{
    public class Category
    {
        public int Id { get; }
        public int Index { get; }
        public string Title { get; }
        public IList<Layout> Layouts { get; }

        public Category(int index, string title, IEnumerable<Layout> layouts = null) : this(0, index, title, layouts) { }

        public Category(int id, int index, string title, IEnumerable<Layout> layouts = null)
        {
            Id = id;
            Index = index;
            Title = title;
            Layouts = layouts != null ? layouts.ToList() : null;
        }
    }
}
