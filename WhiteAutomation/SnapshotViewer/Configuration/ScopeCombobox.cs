using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using ConfigurationReader;

namespace SnapshotViewer.Configuration
{
    public class ScopeCombobox : ComboBox
    {
#region public
        public CompareScopes Scope
        {
            get
            {
                string id = SelectedValue.ToString();
                List<CompareScopes> keys = CompareScopersIds.Keys.ToList();
                CompareScopes currentKey = CompareScopes.OnlyControl;
                foreach (var key in keys)
                {
                    if (CompareScopersIds[key] == id)
                    {
                        currentKey = key;
                        break;
                    }
                }

                return currentKey;
            }
            set { SelectedValue = CompareScopersIds[value]; }
        }

        public object GetScopeWithBlankField()
        {
            string id = SelectedValue.ToString();
            if (string.IsNullOrEmpty(id))
                return null;

            List<CompareScopes> keys = CompareScopersIds.Keys.ToList();
            CompareScopes currentKey = CompareScopes.OnlyControl;
            foreach (var key in keys)
            {
                if (CompareScopersIds[key] == id)
                {
                    currentKey = key;
                    break;
                }
            }
            return currentKey;
        }

        public void SetScopeWithBlankField(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                SelectedValue = "";
                return;
            }
            CompareScopes scope;
            CompareScopes.TryParse(value, out scope);
            SelectedValue = CompareScopersIds[scope];
        }

        public void AddBlankField()
        {
            List<string> values = CompareScopersIds.Values.ToList();
            values.Add("");
            ItemsSource = values;
        }
        public ScopeCombobox() : base()
        {
            ItemsSource = CompareScopersIds.Values;
        }
#endregion
#region private
        private static readonly Dictionary<CompareScopes, string> CompareScopersIds = new Dictionary<CompareScopes, string>
        {
            {CompareScopes.OnlyControl, "Only control"},
            {CompareScopes.OnlyChildren, "Only descendants"},
            {CompareScopes.ChildrenWithControl, "Entire tree"}
        };

#endregion
    }
}