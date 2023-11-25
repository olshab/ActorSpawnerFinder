using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI;

namespace ActorSpawnerFinder.UAssetAPIHelpers
{
    public static class UAssetAPIHelpers
    {
        public static T? FindPropertyByName<T>(this NormalExport ExportToSearchIn, string PropertyName) where T : PropertyData
        {
            foreach (PropertyData data in ExportToSearchIn.Data)
                if (data.Name.ToString() == PropertyName)
                    return (T)data;

            return null;
        }

        public static T FindPropertyByNameChecked<T>(this NormalExport ExportToSearchIn, string PropertyName) where T : PropertyData
        {
            foreach (PropertyData data in ExportToSearchIn.Data)
                if (data.Name.ToString() == PropertyName)
                    return (T)data;

            throw new Exception($"Failed to find property {PropertyName} in export {ExportToSearchIn.ObjectName.ToString()}");
        }

        public static T? FindPropertyByName<T>(this StructPropertyData StructToSearchIn, string PropertyName) where T : PropertyData
        {
            foreach (PropertyData data in StructToSearchIn.Value)
                if (data.Name.ToString() == PropertyName)
                    return (T)data;

            return null;
        }

        public static T FindPropertyByNameChecked<T>(this StructPropertyData StructToSearchIn, string PropertyName) where T : PropertyData
        {
            foreach (PropertyData data in StructToSearchIn.Value)
                if (data.Name.ToString() == PropertyName)
                    return (T)data;

            throw new Exception($"Failed to find property {PropertyName} in export {StructToSearchIn.Name.ToString()}");
        }

        public static NormalExport? FindExportByClassName(this UAsset Asset, String ClassName)
        {
            for (int i = 0; i < Asset.Exports.Count; i++)
            {
                Export export = Asset.Exports[i];

                if (!(export is NormalExport))
                    continue;

                if (!export.ClassIndex.IsImport())
                    continue;

                Import Class = export.ClassIndex.ToImport(Asset);
                if (Class.ObjectName.ToString() == ClassName)
                    return (NormalExport)export;
            }

            return null;
        }
    }
}
