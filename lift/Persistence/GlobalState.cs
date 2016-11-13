using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lift.Persistence
{
    public class GlobalState
    {
        public Data.Options Options { get; internal set; }

        public Resources.Localization.Translations Translations { get; internal set; }

        public Data.LiftItems LiftItems { get; internal set; }

        public static GlobalState Load()
        {
            Data.Options options = Persistence.OptionsStore.Load();
            Data.LiftItems liftItems = Persistence.LiftItemsStore.Load();
            Resources.Localization.Translations translations = new Lift.Resources.Localization.Translations();

            return new GlobalState { Options = options, LiftItems = liftItems, Translations = translations };
        }
    }
}
