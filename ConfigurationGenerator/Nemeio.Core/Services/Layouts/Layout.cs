using Nemeio.Core.DataModels;
using Nemeio.Core.DataModels.Configurator;
using System;
using System.Collections.Generic;
using System.Linq;
using Action = Nemeio.Core.DataModels.Configurator.Action;

namespace Nemeio.Core.Services.Layouts
{
    public class Layout
    {
        public const int WinSASLength = 3;
        private List<Key> _keys;

        public LayoutId LayoutId { get; set; }
        public LayoutInfo LayoutInfo { get; set; }
        public byte[] LayoutImage { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateUpdate { get; set; }
        public bool Enable { get; set; }    
        public SpecialSequences SpecialSequences { get; private set; }

        public List<Key> Keys
        {
            get => _keys;
            set
            {
                _keys = value;
                ComputeSpecialSequences();
            }
        }

        public Layout(LayoutId layoutId, OsLayoutId osLayoutId, byte[] layoutImage, int categoryId, int index, string title, DateTime creation, DateTime update, bool isDefault, bool isHid, List<Key> keys,bool enable = true)
            : this(osLayoutId, layoutImage, categoryId, index, title, creation, update, isDefault, isHid, keys, enable)
        {
            LayoutId = layoutId;
        }

        public Layout(OsLayoutId osLayoutId, byte[] layoutImage, int categoryId, int index, string title, DateTime creation, DateTime update, bool isDefault, bool isHid, List<Key> keys, bool enable = true)
        {
            LayoutInfo = new LayoutInfo(osLayoutId, isDefault, isHid);
            LayoutImage = layoutImage;
            LayoutId = LayoutId.Compute(LayoutInfo, LayoutImage);
            CategoryId = categoryId;
            Index = index;
            Title = title;
            DateCreation = creation;
            DateUpdate = update;
            Enable = enable;
            Keys = keys;
        }

        /// <summary>
        /// Manage case :
        /// - Ctrl, Alt and Suppr on individual keys (only on None position)
        /// - All in one keys
        /// </summary>
        private void ComputeSpecialSequences()
        {
            if (Keys.Count == 0)
            {
                SpecialSequences = GetSAS(LayoutInfo);

                return;
            }

            try
            {
                var result = new Permutation<int>();

                var delKeys = SelectOnly(KeyboardLiterals.Suppr);
                if (delKeys != null && delKeys.Count() > 0)
                {
                    var ctrlKeys = SelectOnly(KeyboardLiterals.Ctrl);
                    var altKeys = SelectOnly(KeyboardLiterals.Alt);
                    var altGrKeys = SelectOnly(KeyboardLiterals.AltGr);

                    var globalMerge = new List<List<int>>();
                    foreach (var delKey in delKeys)
                    {
                        var mergeCtrlAlt = GetPermutations(
                            ctrlKeys.Concat(altKeys),
                            2
                        );

                        foreach (var permutation in mergeCtrlAlt.Values)
                        {
                            permutation.Add(delKey);
                        }

                        globalMerge = globalMerge.Concat(mergeCtrlAlt.Values).ToList();

                        var mergeCtrlAltGr = GetPermutations(
                            ctrlKeys.Concat(altGrKeys),
                            2
                        );

                        foreach (var permutation in mergeCtrlAltGr.Values)
                        {
                            permutation.Add(delKey);
                        }

                        globalMerge = globalMerge.Concat(mergeCtrlAltGr.Values).ToList();
                    }

                    result = result.Concat(globalMerge).ToPermutation();
                }

                var ctrlAltSuppr = SelectOnly(
                    new List<string>()
                    {
                        KeyboardLiterals.Ctrl,
                        KeyboardLiterals.Alt,
                        KeyboardLiterals.Suppr
                    }
                );

                var ctrlAltGrSuppr = SelectOnly(
                    new List<string>()
                    {
                        KeyboardLiterals.Ctrl,
                        KeyboardLiterals.AltGr,
                        KeyboardLiterals.Suppr
                    }
                );

                var altCtrlSuppr = SelectOnly(
                    new List<string>()
                    {
                        KeyboardLiterals.Alt,
                        KeyboardLiterals.Ctrl,
                        KeyboardLiterals.Suppr
                    }
                );

                var altGrCtrlSuppr = SelectOnly(
                    new List<string>()
                    {
                        KeyboardLiterals.AltGr,
                        KeyboardLiterals.Ctrl,
                        KeyboardLiterals.Suppr
                    }
                );

                //  List of only one keys
                var merge = ctrlAltSuppr
                    .Concat(ctrlAltGrSuppr)
                    .Concat(altCtrlSuppr)
                    .Concat(altGrCtrlSuppr)
                    .ToList();

                if (merge.Count > 0)
                {
                    result.Add(merge);
                }

                if (!result.Any())
                {
                    SpecialSequences = GetSAS(LayoutInfo);

                    return;
                }

                result.ForEach((items) =>
                {
                    items.ForEach((val) => { val += 1; });
                });

                SpecialSequences = new SpecialSequences(result);
            }
            catch (ArgumentNullException e)
            {
                //  If JSON is malformed by configurator, some data can by corrupted

                SpecialSequences = GetSAS(LayoutInfo);
            }
        }

        private SpecialSequences GetSAS(LayoutInfo infos) => infos != null && infos.Hid ? SpecialSequences.Default : SpecialSequences.Empty;

        private IEnumerable<int> SelectOnly(string keyToFind) => Keys
            .Where(x => GetActionWithNoneAsModifier(x.Actions).Any(y => SubactionHasKey(y, keyToFind)))
            .Select(x => x.Index);

        private IEnumerable<Action> GetActionWithNoneAsModifier(IEnumerable<Action> actions)
        {
            if (actions == null)
            {
                return Enumerable.Empty<Action>();
            }

            return actions.Where((action) =>
            {
                if (action != null)
                {
                    return action.Modifier == KeyModifier.None;
                }

                return false;
            });
        }

        private bool SubactionHasKey(Action action, string key)
        {
            if (action == null || key == null)
            {
                return false;
            }

            if (action.Subactions == null)
            {
                return false;
            }

            return action.Subactions.Any((z) =>
            {
                var data = z.Data;

                if (string.IsNullOrEmpty(data))
                {
                    return false;
                }

                return key == data;
            });
        }

        private IEnumerable<int> SelectOnly(List<string> keyToFound)
        {
            var validActions = Keys
                .SelectMany(x => GetActionWithNoneAsModifier(x.Actions))
                .Where(action => isMatching(action.Subactions.ToList(), keyToFound));

            return Keys.Where(x => x.Actions.All(s => validActions.Contains(s))).Select(x => x.Index);
        }

        private bool isMatching(List<Subaction> subactions, List<string> keys)
        {
            if (subactions == null || keys == null)
            {
                return false;
            }

            var subsStr = subactions.Select(x => x.Data).Aggregate((i, j) => i + ";" + j);
            var keysStr = keys.Aggregate((i, j) => i + ";" + j);

            return subsStr.Contains(keysStr);
        }

        public static Permutation<int> GetPermutations(IEnumerable<int> list, int length)
        {
            if (length == 1)
            {
                return list.Select(t => new int[] { t }.ToList()).ToPermutation();
            }

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new int[] { t2 }).ToList()).ToPermutation();
        }
    }

    public class LayoutComparer : IEqualityComparer<Layout>
    {
        public static LayoutComparer Instance { get; } = new LayoutComparer();

        public bool Equals(Layout x, Layout y) => x.LayoutId == y.LayoutId;

        public int GetHashCode(Layout obj) => obj.LayoutId.GetHashCode();
    }
}
