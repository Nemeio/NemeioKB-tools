using System;
using System.Collections.Generic;
using System.Linq;
using Nemeio.Core.DataModels.Configurator;

namespace Nemeio.Core.Services.Layouts
{
    public class LayoutRepository
    {
        private readonly ILayoutDbRepository _layoutDbRepository;
        private readonly ILayoutGenService _winLayoutGenService;
        private List<Layout> _layouts;

        public LayoutRepository(ILayoutDbRepository layoutDbRepository, ILayoutGenService winLayoutGenService)
        {
            _layoutDbRepository = layoutDbRepository;
            _winLayoutGenService = winLayoutGenService;

            _layouts = new List<Layout>();
            LoadLayoutsFromDatabase();
        }

        public void LoadLayoutsFromDatabase() => _layouts = _layoutDbRepository.ReadAll().ToList();

        public Layout GetLayoutById(LayoutId layoutId)
        {
            var targeted = _layouts.SingleOrDefault(l => l.LayoutId == layoutId);
            if (targeted == null)
            {
                return null;
            }
            return targeted.Equals(default(KeyValuePair<IntPtr, Layout>)) ? null : targeted;
        }

        public Layout GetLayoutByOsId(OsLayoutId osLayoutId) => _layouts.FirstOrDefault(x => x.LayoutInfo.OsLayoutId == osLayoutId.ToString());

        public IEnumerable<Layout> GetLayouts() => _layouts;

        public void UpdateCurrentLayouts(IEnumerable<OsLayoutId> osInstalledLayoutIds)
        {
            var index = 0;

            //  Remove only HID layout which isn't on system
            var removed = _layouts.Where(l => !osInstalledLayoutIds.Contains(l.LayoutInfo.OsLayoutId) && l.LayoutInfo.Hid).ToArray();
            removed.ForEach((l) => _layouts.Remove(l));

            osInstalledLayoutIds.ForEach(id =>
            {
                if (!_layouts.Any(x => x.LayoutInfo.OsLayoutId.ToString() == id.ToString()))
                {
                    var layout = CreateLayout(id, index);

                    _layoutDbRepository.Save(layout);
                    _layouts.Add(layout);

                    index += 1;
                }
            });
        }

        private Layout CreateLayout(OsLayoutId osLayoutId, int position)
        {
            var img = _winLayoutGenService.RenderLayout(osLayoutId);

            //  TODO: Must be change when configurator return all values
            var layout = new Layout(
                osLayoutId,
                img,
                NemeioConstants.DefaultCategoryId,
                position,
                osLayoutId,     //  Create real title
                DateTime.Now,
                DateTime.Now,
                false,
                true,           //  Necessary in Hid mode here
                new List<Key>() //  Hid mode so keys is empty
            );

            return layout;
        }

    }
}
