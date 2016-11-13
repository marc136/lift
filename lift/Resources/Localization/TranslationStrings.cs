using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lift.Resources.Localization
{
    using Texts = Dictionary<string, string>;

    public abstract class TranslationStrings
    {
        protected static Dictionary<CultureInfo, Texts> AllTexts = new Dictionary<CultureInfo, Texts>
        {
            {
                new CultureInfo("en"), new Texts {
                    { "MainPage.Buttons.Add", "Add" },
                    { "MainPage.Buttons.Edit", "Edit" },
                    { "MainPage.Buttons.Options", "Options" },
                    { "MainPage.ConfirmDelete", "Do you really want to delete '{0}'?" },
                    { "MainPage.ConfirmDeleteHeader", "Delete item" },
                    { "MainPage.ContextMenu.Edit", "Edit" },
                    { "MainPage.ContextMenu.Clone", "Clone" },
                    { "MainPage.ContextMenu.Delete", "Delete" },
                    { "MainPage.ContextMenu.Rename", "Rename" },
                    { "MainPage.EditCategory", "Rename category {0}" },
                    { "MainPage.EditCategoryHeader", "Rename" },
                    { "MainPage.Title", "Lift Starter" },
                    { "Options.Buttons.Back", "Back" },
                    { "Options.Buttons.Export", "export to file" },
                    { "Options.Buttons.Import", "import from file" },
                    { "Options.Language", "Language" },
                    { "Options.Other", "Other options" },
                    { "Options.PromptOnDelete", "Prompt before deleting an item" },
                    { "Options.Title", "Options" },
                }
            },
            // German texts
            {
                new CultureInfo("de"), new Texts {
                    { "MainPage.Buttons.Add", "Hinzufügen" },
                    { "MainPage.Buttons.Edit", "Bearbeiten" },
                    { "MainPage.Buttons.Options", "Einstellungen" },
                    { "MainPage.ConfirmDelete", "Soll '{0}' wirklich gelöscht werden?" },
                    { "MainPage.ConfirmDeleteHeader", "Eintrag löschen" },
                    { "MainPage.ContextMenu.Clone", "Duplizieren" },
                    { "MainPage.ContextMenu.Delete", "Löschen" },
                    { "MainPage.ContextMenu.Edit", "Bearbeiten" },
                    { "MainPage.ContextMenu.Rename", "Umbenennen" },
                    { "MainPage.EditCategory", "Kategorie {0} umbenennen" },
                    { "MainPage.EditCategoryHeader", "Kategorie umbenennen" },
                    { "MainPage.Title", "Lift Starter" },
                    { "Options.Buttons.Back", "Zurück" },
                    { "Options.Buttons.Export", "Einträge exportieren" },
                    { "Options.Buttons.Import", "Einträge importieren" },
                    { "Options.Language", "Sprache" },
                    { "Options.Other", "Sonstiges" },
                    { "Options.PromptOnDelete", "Vor Löschen nachfragen" },
                    { "Options.Title", "Einstellungen" },
                }
            },
            {
                new CultureInfo("fr"), new Texts
                {

                }
            }
        };
    }
}
