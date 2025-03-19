using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services.Layouts;
using System.Collections.Generic;
using System.Linq;

namespace Nemeio.Core.Managers
{
    public class KeystrokeInterpreter
    {
        public List<Subaction> GetActions(Layout currentConfiguration, NemeioIndexKeystroke[] keystrokes)
        {
            var keys = KeystrokeFromIndexs(currentConfiguration, keystrokes);
            var actions = ActionFromKeystroke(keys);
            return actions;
        }

        private IEnumerable<NemeioKeystroke> KeystrokeFromIndexs(Layout currentCnfiguration, NemeioIndexKeystroke[] keystrokes)
            => keystrokes.Join(currentCnfiguration.Keys,
                keyStroke => keyStroke.Index,
                keyModel => keyModel.Index,
                (keyStroke, keyModel) => new NemeioKeystroke { IndexKeystroke = keyStroke, Key = keyModel });

        // TODO Très grande complexité cognitive et cyclomatique, à réécrire
        private List<Subaction> ActionFromKeystroke(IEnumerable<NemeioKeystroke> keystroke)
        {
            var result = new List<Subaction>();

            if (keystroke == null)
            {
                return result;
            }

            bool shiftIsPressed = false;
            bool altGrIsPressed = false;
            bool combinaisonFound = false;

            foreach (var key in keystroke)
            {
                KeyModifier modifier = KeyModifier.None;

                if (shiftIsPressed && !altGrIsPressed)
                {
                    modifier = KeyModifier.Shift;
                }
                else if (!shiftIsPressed && altGrIsPressed)
                {
                    modifier = KeyModifier.AltGr;
                }
                else if (shiftIsPressed && altGrIsPressed)
                {
                    modifier = KeyModifier.Both;
                }

                var customActions = key.Key.Actions;

                if (customActions.Count > 0)
                {
                    var action = customActions.FirstOrDefault(x => x.Modifier == modifier);
                    if (action != null)
                    {
                        var subactions = action.Subactions;

                        if (subactions == null || subactions.Count <= 0)
                        {
                            modifier = KeyModifier.None;
                        }
                        else
                        {
                            combinaisonFound = modifier != KeyModifier.None;

                            foreach (var subaction in subactions)
                            {
                                shiftIsPressed = subaction.IsShift();
                                altGrIsPressed = subaction.IsAltGr();
                                
                                result.Add(subaction);
                            }
                        }
                    }
                }
            }

            if (combinaisonFound)
            {
                bool shiftDeleted = false;
                bool altGrDeleted = false;

                foreach (var action in result.ToList())
                {
                    if ((action.IsShift() && !shiftDeleted) ||
                        (action.IsAltGr() && !altGrDeleted))
                    {
                        shiftDeleted = action.IsShift();
                        altGrDeleted = action.IsAltGr();

                        result.Remove(action);
                    }
                }
            }

            if (result.Count == 0)
            {
                if (shiftIsPressed)
                    result.Add(Subaction.CreateModifierAction(KeyboardLiterals.Shift));

                if (altGrIsPressed)
                    result.Add(Subaction.CreateModifierAction(KeyboardLiterals.AltGr));
            }

            return result;
        }
    }
}
