using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Lift.Data
{
    public class LiftItems : ObservableCollection<LiftItem>
    {
        internal void RenameCategory(string oldCategoryName, string newCategoryName)
        {
            foreach (var item in Items)
            {
                if (item.Category == oldCategoryName) item.Category = newCategoryName;
            }
        }

        internal void AddClone(LiftItem item)
        {
            var clone = item?.Clone() as Data.LiftItem;
            if (clone != null) Items.Add(clone);
        }

        internal void ClearAndImportFromFile(string filepath)
        {
            var importedItems = Persistence.DataStore.ImportFromFile(filepath);
            bool succesfulImport = true;
            if (importedItems == null || importedItems.Count == 0)
            {
                MessageBox.Show($"Could not read any Lift items from the file\n{filepath}", "Import error", MessageBoxButton.OK, MessageBoxImage.Warning);
                succesfulImport = false;
            }

            if (MessageBox.Show($"Do you want to keep the existing {this.Count} items?", "Keep existing items?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                // user wants to delete existing elements
                this.Clear();
            }

            if (succesfulImport)
            {
                foreach (var item in importedItems)
                {
                    Add(item);
                }
            }
        }
    }
}
