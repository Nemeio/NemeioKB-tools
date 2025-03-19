using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.LayoutGen.Models.Displayer;
using SkiaSharp;
using CAction = Nemeio.Core.DataModels.Configurator.Action;
using CKey = Nemeio.Core.DataModels.Configurator.Key;
using CFont = Nemeio.Core.DataModels.Configurator.Font;

namespace Nemeio.LayoutGen.Models.Renderer
{
    public class JsonRenderer : BaseRenderer<ConfiguratorLayout>
    {
        public override SKBitmap Render(ConfiguratorLayout configLyt)
        {
            Layout lyt = new Layout(
                new Size(configLyt.Width, configLyt.Height),
                Point.Zero,
                configLyt.Background == BackgroundMode.Dark
            );

            foreach (var key in configLyt.Keys)
            {
                var gKey = CreateKey(key);

                OrderEachAction(key.Actions, action =>
                {
                    var subKey = CreateSubKey(gKey, action);

                    gKey.AddComponent(subKey);
                });

                lyt.AddComponent(gKey);
            }

            var lytRenderer = new LayoutRenderer();

            return lytRenderer.Render(lyt);
        }

        private void OrderEachAction(IList<CAction> actions, Action<CAction> action)
        {
            var orderedActions = new List<CAction>()
            {
                GetActionForModifier(actions, KeyModifier.Shift),
                GetActionForModifier(actions, KeyModifier.Both),
                GetActionForModifier(actions, KeyModifier.None),
                GetActionForModifier(actions, KeyModifier.AltGr),
            };

            foreach (var act in orderedActions)
            {
                if (act != null)
                {
                    action(act);
                }
            }
        }

        private CAction GetActionForModifier(IList<CAction> actions, KeyModifier modifier)
        {
            var res = actions.FirstOrDefault(x => x.Modifier == modifier);
            if (res == default(CAction))
            {
                var action = new CAction();
                action.Display = string.Empty;
                action.Modifier = modifier;

                return action;
            }
            
            return res;
        }

        private Key CreateKey(CKey key)
        {
            var gKey = new Key(
                new Size(key.Width, key.Height),
                new Point(key.X, key.Y),
                key.Background == BackgroundMode.Dark,
                GetKeyDisposition(key)
            );

            return gKey;
        }

        private Key CreateSubKey(Key parentKey, CAction action)
        {
            var subKey = new Key(
                Size.Zero,
                Point.Zero,
                parentKey.IsDarkBackground,
                Disposition.None,
                CreateFont(action.Font)
            );

            subKey.Modifier = ConvertKeyModifierToModifier(action.Modifier);
            subKey.Displayer = GetKeyDisplayer(subKey, action);

            return subKey;
        }

        private Font CreateFont(CFont fnt)
        {
            if (fnt == null)
            {
                return Font.Default;
            }

            return new Font(
                fnt.Name,
                fnt.Bold,
                fnt.Italic,
                fnt.Underline,
                fnt.Size
            );
        }

        private Disposition GetKeyDisposition(CKey key)
        {
            if (key == null)
            {
                return Disposition.None;
            }

            if (key.Actions.Count == 0)
            {
                return Disposition.None;
            }

            switch (key.Actions.Count)
            {
                case 1:
                    return Disposition.OnlyOneSymbol;
                case 2:
                    return Disposition.TwoSymbolVertical;
                case 3:
                case 4:
                    return Disposition.FourSymbol;
                default:
                    return Disposition.None;
            }
        }

        private IDisplayer GetKeyDisplayer(Key subkey, CAction action)
        {
            var item = action.Display;

            if (IsFileRef(item))
            {
                if (IsEmbeddedResource(item))
                {
                    var embLength = CAction.EMBEDDED_PREFIX.Length;
                    var fileName = item.Substring(embLength, item.Length - embLength);
                    var imgFile = LoadEmbeddedResource(fileName);

                    return new ImageDisplayer(subkey, fileName, imgFile);
                }
                else
                {
                    return new ImageDisplayer(subkey, item);
                }
            }
            else
            {
                return new TextDisplayer(subkey, item);
            }
        }

        private bool IsFileRef(string val)
        {
            try
            {
                return Path.HasExtension(val);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private bool IsEmbeddedResource(string val) => CAction.IsEmbeddedResource(val);

        public Stream LoadEmbeddedResource(string name)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            return asm.GetManifestResourceStream("Nemeio.LayoutGen.Resources.Images." + name);
        }

        public Modifier ConvertKeyModifierToModifier(KeyModifier modifier)
        {
            return (Modifier) modifier;
        }
    }
}
