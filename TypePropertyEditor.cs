using DevExpress.Blazor;
using DevExpress.Data.Internal;
using DevExpress.ExpressApp.Blazor.Components.Models;
using DevExpress.ExpressApp.Blazor.Editors.Adapters;
using DevExpress.ExpressApp.Model;
using System.ComponentModel;
using DevExpress.ExpressApp.Utils.Reflection;

namespace GemTex.ExpressApp.Blazor.Editors;

public class TypePropertyEditor : DevExpress.ExpressApp.Blazor.Editors.TypePropertyEditor
{
    private readonly TypeConverter typeConverter;

    public TypePropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
    {
        TypeConverterAttribute typeConverterAttribute = base.MemberInfo.FindAttribute<TypeConverterAttribute>();
        if (typeConverterAttribute != null)
        {
            Type knownUserType = SafeTypeResolver.GetKnownUserType(typeConverterAttribute.ConverterTypeName);
            if (knownUserType == typeof(DevExpress.Persistent.Base.Security.SecurityTargetTypeConverter))
            {
                knownUserType = typeof(SecurityTargetTypeConverter);
            }
            else if (knownUserType == typeof(DevExpress.Persistent.Base.ReportDataTypeConverter))
            {
                knownUserType = typeof(ReportDataTypeConverter);
            }
            typeConverter = (TypeConverter)TypeHelper.CreateInstance(knownUserType);
        }
        else
        {
            typeConverter = new LocalizedClassInfoTypeConverter();
        }
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

public class LocalizedClassInfoTypeConverter : DevExpress.Persistent.Base.LocalizedClassInfoTypeConverter
{
    protected override string GetClassCaption(string fullName)
    {
        return $"{base.GetClassCaption(fullName)} ({fullName[(fullName.LastIndexOf('.') + 1)..]})";
    }
}

public class SecurityTargetTypeConverter : DevExpress.Persistent.Base.Security.SecurityTargetTypeConverter
{
    protected override string GetClassCaption(string fullName)
    {
        return $"{base.GetClassCaption(fullName)} ({fullName[(fullName.LastIndexOf('.') + 1)..]})";
    }
}

public class ReportDataTypeConverter : DevExpress.Persistent.Base.ReportDataTypeConverter
{
    protected override string GetClassCaption(string fullName)
    {
        return $"{base.GetClassCaption(fullName)} ({fullName[(fullName.LastIndexOf('.') + 1)..]})";
    }
}
