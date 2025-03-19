using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Core.Services.Category
{
    public interface ICategoryDbRepository
    {
        IEnumerable<Category> ReadAll();

        Category FindOneById(int id);

        void Delete(long categoryId);

        void Save(Category layout);

        void Update(Category cat);
    }
}
