namespace Sitecore.Support.Shell.Applications.ContentManager.ReturnFieldEditorValues
{
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Reflection;
    using Sitecore.Shell.Applications.ContentEditor;
    using Sitecore.Shell.Applications.ContentManager;
    using Sitecore.Shell.Applications.ContentManager.ReturnFieldEditorValues;
    using System.Collections.Generic;
    using System.Web.UI;

    public class SetValues
    {
        public void Process(ReturnFieldEditorValuesArgs args)
        {
            #region patch 414532
            List<FieldDescriptor> unchangedFields = new List<FieldDescriptor>();
            #endregion
            foreach (FieldInfo info in args.FieldInfo.Values)
            {
                Control control = Context.ClientPage.FindSubControl(info.ID);
                if (control != null)
                {
                    string str;
                    if (control is IContentField)
                    {
                        string[] values = new string[] { (control as IContentField).GetValue() };
                        str = StringUtil.GetString(values);
                    }
                    else
                    {
                        str = StringUtil.GetString(ReflectionUtil.GetProperty(control, "Value"));
                    }
                    if (str != "__#!$No value$!#__")
                    {
                        string str2 = info.Type.ToLowerInvariant();
                        if ((str2 == "rich text") || (str2 == "html"))
                        {
                            char[] trimChars = new char[] { ' ' };
                            str = str.TrimEnd(trimChars);
                        }
                        foreach (FieldDescriptor descriptor in args.Options.Fields)
                        {
                            if (descriptor.FieldID == info.FieldID)
                            {
                                ItemUri uri = new ItemUri(info.ItemID, info.Language, info.Version, Factory.GetDatabase(descriptor.ItemUri.DatabaseName));
                                if (descriptor.ItemUri == uri)
                                {
                                    #region patch 414532
                                    if (descriptor.ContainsStandardValue)
                                    {
                                        var standardValuesItem = Factory.GetDatabase(descriptor.ItemUri.DatabaseName)?.GetItem(descriptor.ItemUri.ItemID);
                                        if (standardValuesItem != null)
                                        {
                                            var standardValue = standardValuesItem.Fields?[descriptor.FieldID]?.Value ?? string.Empty;
                                            if (str == standardValue)
                                            {
                                                unchangedFields.Add(descriptor);
                                                continue;
                                            }
                                        }
                                    }
                                    #endregion
                                    descriptor.Value = str;
                                }
                            }
                        }
                    }
                }
            }
            #region patch 414532
            foreach (var id in unchangedFields)
                args.Options.Fields.Remove(id);
#endregion
        }
    }
}
