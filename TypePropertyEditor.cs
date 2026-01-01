using DevExpress.Blazor;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using System.ComponentModel;

namespace GemTex.ExpressApp.Blazor.Editors;

public class TypePropertyEditor : DevExpress.ExpressApp.Blazor.Editors.TypePropertyEditor
{
    private readonly TypeConverter typeConverter;

    public TypePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
    {
        typeConverter = new CustomLocalizedClassInfoTypeConverter();
    }

    protected override IComponentAdapter CreateComponentAdapter()
    {
        var dxComboBoxModel = new DxComboBoxModel<DataItem<Type>, Type>();
        var list = new List<DataItem<Type>>();
        foreach (Type standardValue in typeConverter.GetStandardValues()!)
        {
            if (IsSuitableType(standardValue))
            {
                list.Add(new DataItem<Type>(standardValue, typeConverter.ConvertToString(standardValue)));
            }
        }

        dxComboBoxModel.Data = list;
        dxComboBoxModel.ValueFieldName = "Value";
        dxComboBoxModel.TextFieldName = "Text";
        dxComboBoxModel.SearchMode = ListSearchMode.AutoSearch;
        dxComboBoxModel.SearchFilterCondition = ListSearchFilterCondition.Contains;
        return new DxComboBoxAdapter<DataItem<Type>, Type>(dxComboBoxModel);
    }
}

public class CustomLocalizedClassInfoTypeConverter : LocalizedClassInfoTypeConverter
{
    protected override string GetClassCaption(string fullName)
    {
        return $"{base.GetClassCaption(fullName)} ({fullName})";
    }
}
