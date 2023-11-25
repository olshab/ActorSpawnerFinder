using ActorSpawnerFinder;
using ActorSpawnerFinder.ActorComponent;
using ActorSpawnerFinder.BlueprintGeneratedClass;
using ActorSpawnerFinder.Utils;
using UAssetAPI;
using UAssetAPI.UnrealTypes;

namespace BlueprintReferenceViewer
{
    public static class Settings
    {
        public static string AssetsDirectory = @"C:\Users\Oleg\Desktop\dumper";
        public static string ProjectDirectory = @"C:\Users\Oleg\Desktop\OldTiles";

        public static bool bVisualizationOnly = true;
        public static bool bInGameActorsOnly = false;

        public static EngineVersion UAssetAPI_GameVersion = EngineVersion.VER_UE4_27;

        public static bool bScanProjectForReferencedAssets = true;
        public static bool bIgnoreExistingAssetsAtPath = true;

        public static string[] IgnoreExistingAssetsAtPath = {
            "/Game/OriginalTiles",
            "/Game/NewTiles",
            //"/Game/MergedTiles",
        };
    }

    public class Program
    {
        public static List<string> ActorSpawners = new List<string>();

        public static Dictionary<string, string> AlreadyExistingAssets = new Dictionary<string, string>();

        static void Main(string[] args)
        {
            if (Settings.bScanProjectForReferencedAssets)
                GetProjectAssets(ref AlreadyExistingAssets);

            DirectoryInfo di = new DirectoryInfo(Settings.AssetsDirectory);
            FileInfo[] UassetFiles = di.GetFiles("*.uasset");

            foreach (FileInfo Uasset in UassetFiles)
            {
                UAsset Asset = new UAsset(Uasset.FullName, Settings.UAssetAPI_GameVersion);

                UBlueprintGeneratedClass BPGC = new UBlueprintGeneratedClass(Asset.GetClassExport());
                ProcessBlueprint(BPGC);
            }

            ActorSpawners = ActorSpawners.Distinct().ToList();
            ActorSpawners.Sort();

            foreach (string ActorSpawner in ActorSpawners)
            {
                string AssetName = ActorSpawner.GetAssetName();

                if (AlreadyExistingAssets.ContainsKey(AssetName))
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                Console.WriteLine(ActorSpawner);

                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }

        public static void ProcessBlueprint(UBlueprintGeneratedClass BPGC)
        {
            List<UActorComponent> ActorComponents = BPGC.GetAllActorComponents();

            List<UActorComponent> ActorSpawners = UActorComponent.SortByClassName(ActorComponents, "ActorSpawner");
            ActorSpawners.AddRange(UActorComponent.SortByClassName(ActorComponents, "HexSpawner"));

            foreach (UActorComponent ActorComponent in ActorSpawners)
                ProcessActorSpawner(new UActorSpawner( ActorComponent.GetComponentExport() ));
        }

        static void ProcessActorSpawner(UActorSpawner ActorSpawnerComponent)
        {
            if (Settings.bVisualizationOnly)
            {
                if (ActorSpawnerComponent.Visualization is not null)
                    ActorSpawners.Add(ActorSpawnerComponent.Visualization);

                return;
            }

            foreach (FActorSpawnerProperties ActivatedSceneElement in ActorSpawnerComponent.ActivatedSceneElement)
                ActorSpawners.Add(ActivatedSceneElement.SceneElement);

            foreach (FActorSpawnerProperties DeactivatedSceneElement in ActorSpawnerComponent.DeactivatedSceneElement)
                ActorSpawners.Add(DeactivatedSceneElement.SceneElement);

            if (!Settings.bInGameActorsOnly)
            {
                if (ActorSpawnerComponent.Visualization is not null)
                    ActorSpawners.Add(ActorSpawnerComponent.Visualization);
            }
        }

        static void GetProjectAssets(ref Dictionary<string, string> OutAlreadyExistingAssets)
        {
            if (!Directory.Exists(Settings.ProjectDirectory))
                throw new Exception("Project directory doesn't exist. Uncheck bScanProjectForReferencedAssets");

            string[] ProjectAssets = Directory.GetFiles($"{Settings.ProjectDirectory}\\Content", "*.uasset", SearchOption.AllDirectories);

            foreach (string projectAssetPath in ProjectAssets)
            {
                string AssetPath = "/Game" + projectAssetPath.SubstringAfter("Content").SubstringBeforeLast('.').Replace('\\', '/');

                bool bIncludeAsset = true;
                if (Settings.bIgnoreExistingAssetsAtPath)
                {
                    foreach (string IgnorePath in Settings.IgnoreExistingAssetsAtPath)
                        if (AssetPath.StartsWith(IgnorePath))
                        {
                            bIncludeAsset = false;
                            break;
                        }
                }

                if (bIncludeAsset)
                {
                    if (AlreadyExistingAssets.ContainsKey(projectAssetPath.GetAssetName()))
                        throw new Exception($"Two assets with the same name: {OutAlreadyExistingAssets[projectAssetPath.GetAssetName()]} and {AssetPath}");

                    OutAlreadyExistingAssets.Add(projectAssetPath.GetAssetName(), AssetPath);
                }
            }
        }
    }
}
