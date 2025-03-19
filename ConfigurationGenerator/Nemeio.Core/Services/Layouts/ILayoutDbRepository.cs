using System.Collections.Generic;

namespace Nemeio.Core.Services.Layouts
{
    public interface ILayoutDbRepository
    {
        IEnumerable<Layout> ReadAll();

        IEnumerable<Layout> ReadAllWhereCategoryId(long id);

        Layout ReadLayoutWithOsId(OsLayoutId osLayoutId);

        Layout FindOneById(LayoutId id);

        int CountLayoutsForCategory(long categoryId);

        void Update(Layout layout);

        void Save(Layout layout);

        void Delete(Layout layout);
    }
}
