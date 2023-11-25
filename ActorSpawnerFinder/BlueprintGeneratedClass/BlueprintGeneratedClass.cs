using ActorSpawnerFinder.ActorComponent;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using ActorSpawnerFinder.UAssetAPIHelpers;

namespace ActorSpawnerFinder.BlueprintGeneratedClass
{
    public class UBlueprintGeneratedClass
    {
        public USimpleConstructionScript? SimpleConstructionScript { get; private set; }
        public UInheritableComponentHandler? InheritableComponentHandler { get; private set; }

        public UBlueprintGeneratedClass(ClassExport ClassExport)
        {
            /** SimpleConstructionScript */
            ObjectPropertyData? SCS_Object =
                ClassExport.FindPropertyByName<ObjectPropertyData>("SimpleConstructionScript");

            if (SCS_Object is not null)
                SimpleConstructionScript = new USimpleConstructionScript( (NormalExport)SCS_Object.ToExport(ClassExport.Asset) );

            /** InheritableComponentHandler */
            ObjectPropertyData? ICH_Object =
                ClassExport.FindPropertyByName<ObjectPropertyData>("InheritableComponentHandler");

            if (ICH_Object is not null)
                InheritableComponentHandler = new UInheritableComponentHandler( (NormalExport)ICH_Object.ToExport(ClassExport.Asset) );
        }

        public List<UActorComponent> GetAllActorComponents()
        {
            List<UActorComponent> Components = new List<UActorComponent>();

            if (SimpleConstructionScript is not null)
            {
                foreach (USCS_Node SCS_Node in SimpleConstructionScript.AllNodes)
                    if (SCS_Node.ComponentTemplate is not null)
                        Components.Add(new UActorComponent(SCS_Node.ComponentTemplate));
            }

            if (InheritableComponentHandler is not null)
            {
                foreach (FComponentOverrideRecord Record in InheritableComponentHandler.Records)
                    if (Record.ComponentTemplate is not null)
                        Components.Add(new UActorComponent(Record.ComponentTemplate));
            }

            return Components;
        }
    }
}
