using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CsvEditSharp.Models
{
    
    public class CsvEditSharpWorkspace : IDisposable
    {
        private static readonly Type[] referenceTypes = new[]
        {
            typeof(object),
            typeof(Enumerable),
            typeof(IEnumerable),
            typeof(Console),
            typeof(Assembly),
            typeof(List<>),
            typeof(Type),
            typeof(System.Runtime.InteropServices.Marshal),
            typeof(Microsoft.CSharp.RuntimeBinder.Binder),
            typeof(System.Text.Encoding),
            typeof(System.Globalization.CultureInfo),
            typeof(CsvHelper.CsvReader),
            typeof(CsvHelper.Configuration.ClassMap),
            typeof(CsvHelper.Configuration.Attributes.NameAttribute),
            typeof(CsvHelper.TypeConversion.TypeConverter),
            typeof(ICsvEditSharpApi)

        };
        private readonly ICsvEditSharpApi host;
        private Script<object> script;
        private ScriptState<object> scriptState;

        private readonly DocumentId documentId;
        private readonly AdhocWorkspace workspace;
        private ProjectInfo projectInfo;
        private Document document;
        private CompletionService completionService;

        private readonly IList<string> errorMessages;
       

        public CsvEditSharpWorkspace(ICsvEditSharpApi host, IList<string> errorMessages)
        {
            this.host = host;
            this.errorMessages = errorMessages;

            workspace = new AdhocWorkspace(MefHostServices.Create(MefHostServices.DefaultAssemblies));

            var projectId = ProjectId.CreateNewId();
            documentId = DocumentId.CreateNewId(projectId);

            projectInfo = ProjectInfo.Create(
                projectId, VersionStamp.Create(), "CsvEditorConfig", "CsvEditorConfig", LanguageNames.CSharp, isSubmission: true)
                .WithMetadataReferences(
                    referenceTypes
                        .Select(t => t.Assembly.Location)
                        .Distinct()
                .Select(file => MetadataReference.CreateFromFile(file)).ToArray())
                .WithCompilationOptions(
                    new CSharpCompilationOptions(
                        OutputKind.DynamicallyLinkedLibrary,
                        usings: referenceTypes.Select(t => t.Namespace).Distinct()));
            
    }

        public bool HasScriptState { get { return scriptState != null; } }

        public void CompileScript(string code)
        {
            try
            {
                workspace.ClearSolution();
                workspace.AddProject(projectInfo);
                var scriptDocumentInfo = DocumentInfo.Create(
                    documentId, 
                    "Config.csx", 
                    sourceCodeKind: SourceCodeKind.Script, 
                    loader: TextLoader.From(TextAndVersion.Create( SourceText.From(code), VersionStamp.Create())));
                document = workspace.AddDocument(scriptDocumentInfo);
                completionService = CompletionService.GetService(document);
                
                script = CSharpScript.Create(
                    code, 
                    ScriptOptions.Default
                    .WithReferences(referenceTypes.Select(t=>t.Assembly).Distinct())
                    .WithImports(referenceTypes.Select(t=>t.Namespace).Distinct()),
                    typeof(ICsvEditSharpApi));
            }
            catch (CompilationErrorException e)
            {
                errorMessages.Add(e.Message + Environment.NewLine + e.Diagnostics);
                script = null;
            }
        }

        public async Task<IEnumerable<CompletionData>> GetCompletionListAsync(int position, string code)
        {
            CompileScript(code);

            var semanticModel = await document.GetSemanticModelAsync();
            var items = await Recommender.GetRecommendedSymbolsAtPositionAsync(
                semanticModel, position, workspace);

            return items.Select(symbol => new CompletionData()
            {
                Content = symbol.Name.EndsWith("Attribute") ? symbol.Name.Substring(0,symbol.Name.Length-"Attribute".Length) : symbol.Name,
                Text = symbol.Name.EndsWith("Attribute") ? symbol.Name.Substring(0, symbol.Name.Length - "Attribute".Length) : symbol.Name,
                Description = symbol.ToMinimalDisplayString(semanticModel, position)
            });

        }

        public async Task RunScriptAsync(string code)
        {
            try
            {
                CompileScript(code);
                scriptState = await script.RunAsync(globals: host);
            }
            catch (CompilationErrorException e)
            {
                errorMessages.Add(e.Message + Environment.NewLine + e.Diagnostics);
                script = null;
            }
        }

        public async Task ContinueScriptAsync(string code)
        {
            try
            {
                scriptState = await scriptState?.ContinueWithAsync(code);
            }
            catch (CompilationErrorException e)
            {
                errorMessages.Add(e.Message + Environment.NewLine + e.Diagnostics);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                workspace?.Dispose();
            }
        }
        ~ CsvEditSharpWorkspace()
        {
            Dispose(false);
        }
    }


}
