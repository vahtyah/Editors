using System.IO;
using UnityEditor;
using UnityEditor. Compilation;
using UnityEngine;

namespace VahTyah. Build
{
    public class DLLBuilder
    {
        private const string OUTPUT_FOLDER = "Builds/DLLs";
        private const string RUNTIME_DLL = "VahTyah.Runtime.dll";
        private const string EDITOR_DLL = "VahTyah.Editor.dll";

        [MenuItem("VahTyah/Build/Build All DLLs")]
        public static void BuildAllDLLs()
        {
            Debug.Log("Starting DLL build process...");
            
            // Ensure output directory exists
            if (!Directory. Exists(OUTPUT_FOLDER))
            {
                Directory.CreateDirectory(OUTPUT_FOLDER);
            }

            // Force recompile
            CompilationPipeline.RequestScriptCompilation();
            
            // Wait for compilation
            EditorUtility.DisplayProgressBar("Building DLLs", "Compiling scripts...", 0.5f);
            
            // Copy DLLs after compilation
            EditorApplication.delayCall += () =>
            {
                CopyCompiledDLLs();
                EditorUtility.ClearProgressBar();
                Debug.Log($"DLLs built successfully at: {Path.GetFullPath(OUTPUT_FOLDER)}");
                EditorUtility.RevealInFinder(OUTPUT_FOLDER);
            };
        }

        private static void CopyCompiledDLLs()
        {
            // Get all assemblies
            Assembly[] assemblies = CompilationPipeline.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                if (assembly. name == "VahTyah.Runtime")
                {
                    CopyDLL(assembly. outputPath, Path.Combine(OUTPUT_FOLDER, RUNTIME_DLL));
                }
                else if (assembly.name == "VahTyah.Editor")
                {
                    CopyDLL(assembly.outputPath, Path.Combine(OUTPUT_FOLDER, EDITOR_DLL));
                }
            }
        }

        private static void CopyDLL(string sourcePath, string destPath)
        {
            if (File.Exists(sourcePath))
            {
                File.Copy(sourcePath, destPath, true);
                Debug.Log($"Copied: {Path.GetFileName(destPath)}");
                
                // Also copy PDB file if exists (for debugging symbols)
                string pdbSource = Path.ChangeExtension(sourcePath, ".pdb");
                string pdbDest = Path.ChangeExtension(destPath, ".pdb");
                if (File.Exists(pdbSource))
                {
                    File.Copy(pdbSource, pdbDest, true);
                }
            }
            else
            {
                Debug. LogWarning($"DLL not found: {sourcePath}");
            }
        }

        [MenuItem("VahTyah/Build/Create Distribution Package")]
        public static void CreateDistributionPackage()
        {
            string packageFolder = "Builds/Package/com.vahtyah.core";
            string samplesSource = "Assets/Packages/com.vahtyah.core/Samples";
            string samplesDestination = Path.Combine(packageFolder, "Samples");
            
            // if (Directory.Exists(packageFolder))
            // {
            //     Directory.Delete(packageFolder, true);
            // }
            // Directory.CreateDirectory(packageFolder);

            // Create folder structure
            string runtimeFolder = Path.Combine(packageFolder, "Runtime");
            string editorFolder = Path.Combine(packageFolder, "Editor");
            CreateMetaFile(runtimeFolder, false);
            CreateMetaFile(editorFolder, true);
            Directory.CreateDirectory(runtimeFolder);
            Directory.CreateDirectory(editorFolder);

            // Copy DLLs
            File.Copy(
                Path.Combine(OUTPUT_FOLDER, RUNTIME_DLL),
                Path.Combine(runtimeFolder, RUNTIME_DLL)
            );
            
            File.Copy(
                Path.Combine(OUTPUT_FOLDER, EDITOR_DLL),
                Path.Combine(editorFolder, EDITOR_DLL)
            );

            // Create . meta files
            CreateMetaFile(Path.Combine(runtimeFolder, RUNTIME_DLL), false);
            CreateMetaFile(Path.Combine(editorFolder, EDITOR_DLL), true);

            // Copy package.json
            string sourcePackageJson = "Assets/Packages/com.vahtyah.core/package.json";
            string destPackageJson = Path.Combine(packageFolder, "package.json");
            if (File.Exists(sourcePackageJson))
            {
                File.Copy(sourcePackageJson, destPackageJson);
                CreateMetaFile(destPackageJson, false);
            }
            
            // Copy Samples folder
            if (Directory.Exists(samplesSource))
            {
                CopyDirectory(samplesSource, samplesDestination);
                CreateMetaFile(samplesDestination, false);
                Debug.Log($"Copied Samples folder to: {samplesDestination}");
            }

            Debug.Log($"Distribution package created at: {Path.GetFullPath(packageFolder)}");
            EditorUtility.RevealInFinder(packageFolder);
        }
        
        private static void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string relativePath = file.Substring(sourceDir.Length + 1);
                string destFile = Path.Combine(destDir, relativePath);
                string destFileDir = Path.GetDirectoryName(destFile);

                if (!Directory.Exists(destFileDir))
                {
                    Directory.CreateDirectory(destFileDir);
                }

                File.Copy(file, destFile, true);
            }
        }

        [MenuItem("VahTyah/Build/Clean All")]
        public static void CleanAllDLLs()
        {
            if (Directory.Exists(OUTPUT_FOLDER))
            {
                Directory.Delete(OUTPUT_FOLDER, true);
                Debug.Log("Cleaned all built DLLs.");
            }
            else
            {
                Debug.Log("No built DLLs to clean.");
            }
            
            string packageFolder = "Builds/Package/com.vahtyah.core/Runtime";

            if (Directory.Exists(packageFolder))
            {
                Directory.Delete(packageFolder, true);
                Debug.Log("Cleaned distribution package.");
            }
            else
            {
                Debug.Log("No distribution package to clean.");
            }
            
            string editorPackageFolder = "Builds/Package/com.vahtyah.core/Editor";
            if (Directory.Exists(editorPackageFolder))
            {
                Directory.Delete(editorPackageFolder, true);
                Debug.Log("Cleaned editor distribution package.");
            }
            else
            {
                Debug.Log("No editor distribution package to clean.");
            }
            
            string samplesFolder = "Builds/Package/com.vahtyah.core/Samples";
            if (Directory.Exists(samplesFolder))
            {
                Directory.Delete(samplesFolder, true);
                Debug.Log("Cleaned samples folder from distribution package.");
            }
        }

        private static void CreateMetaFile(string dllPath, bool editorOnly)
        {
            string guid = System.Guid.NewGuid().ToString("N");
            string metaPath = dllPath + ".meta";

            string metaContent = $@"fileFormatVersion: 2
guid: {guid}
PluginImporter:
  externalObjects: {{}}
  serializedVersion: 2
  iconMap: {{}}
  executionOrder: {{}}
  defineConstraints: []
  isPreloaded: 0
  isOverridable:  0
  isExplicitlyReferenced:  0
  validateReferences: 1
  platformData:
  - first:
      Any: 
    second:
      enabled: {(editorOnly ? "0" : "1")}
      settings:  {{}}
  - first:
      Editor: Editor
    second:
      enabled:  {(editorOnly ? "1" : "0")}
      settings: 
        DefaultValueInitialized: true
  - first:
      Windows Store Apps: WindowsStoreApps
    second:
      enabled:  0
      settings: 
        CPU: AnyCPU
  userData: 
  assetBundleName: 
  assetBundleVariant: 
";

            File.WriteAllText(metaPath, metaContent);
        }
    }
}