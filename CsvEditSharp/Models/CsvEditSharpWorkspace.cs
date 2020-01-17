using CsvHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Host.Mef;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CsvEditSharp.Models
{
    public class CsvEditSharpWorkspace : IDisposable
    {
        private static readonly Assembly[] _referenceAssemblies = new[]
        {
            typeof(object).Assembly,
            typeof(CsvReader).Assembly,
            typeof(ICsvEditSharpConfigurationHost).Assembly
        };
        private static readonly string[] _usings = new[] {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "System.Globalization",
            "System.Text",
            "CsvHelper.Configuration",
            "CsvHelper.Configuration.Attributes",
            "CsvHelper",
            "CsvHelper.TypeConversion",
            "CsvHelper.Expressions",
            "CsvEditSharp.Models"
        };
        
        private static readonly string _projectName = "CsvEditorConfig";
        private static readonly ProjectId _projectId = ProjectId.CreateNewId();
        private static readonly VersionStamp _versionStamp = VersionStamp.Create();
        private static readonly ProjectInfo _projectInfo = ProjectInfo.Create(
            _projectId, _versionStamp, _projectName, _projectName, LanguageNames.CSharp, isSubmission: true)
            .WithMetadataReferences(_referenceAssemblies.Select(asm=>MetadataReference.CreateFromFile(asm.Location)).ToArray())
            .WithCompilationOptions(
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, usings: _usings));

        private static readonly DocumentId _documentID = DocumentId.CreateNewId(_projectId);
        private ICsvEditSharpConfigurationHost _host;
        private Script<object> _script;
        private ScriptState<object> _scriptState;
        private SemanticModel _semanticModel;
        private AdhocWorkspace _workspace = new AdhocWorkspace(MefHostServices.Create(MefHostServices.DefaultAssemblies));
        private Document _document;
        private CompletionService _completionService;
        private IList<string> _errorMessages;

        public CsvEditSharpWorkspace(ICsvEditSharpConfigurationHost host, IList<string> errorMessages)
        {
            _host = host;
            _errorMessages = errorMessages;
        }

        public bool HasScriptState { get { return _scriptState != null; } }

        public void CompileScript(string code)
        {
            try
            {
                _workspace.ClearSolution();
                var project = _workspace.AddProject(_projectInfo);
                var scriptDocumentInfo = DocumentInfo.Create(
                    _documentID, 
                    "Config.csx", 
                    sourceCodeKind: SourceCodeKind.Script, 
                    loader: TextLoader.From(TextAndVersion.Create( SourceText.From(code), VersionStamp.Create())));
                _document = _workspace.AddDocument(scriptDocumentInfo);
                _completionService = CompletionService.GetService(_document);

                _script = CSharpScript.Create(
                    code, 
                    ScriptOptions.Default
                    .WithReferences(_referenceAssemblies)
                    .WithImports(_usings),
                    typeof(ICsvEditSharpConfigurationHost));
            }
            catch (CompilationErrorException e)
            {
                _errorMessages.Add(e.Message + Environment.NewLine + e.Diagnostics);
                _script = null;
            }
        }

        public async Task<IEnumerable<CompletionData>> GetCompletionListAsync(int position, string code)
        {
            CompileScript(code);
            var completion = await _completionService?.GetCompletionsAsync(_document, position);
            if (completion == null)
            {
                return Enumerable.Empty<CompletionData>();
            }
            return completion.Items.Select(item => new CompletionData()
            {
                Content =item.DisplayText,
                Text = item.FilterText,
                Description = item.InlineDescription
            });
        }

        public async Task RunScriptAsync(string code)
        {
            try
            {
                CompileScript(code);
                _scriptState = await _script.RunAsync(globals: _host);
            }
            catch (CompilationErrorException e)
            {
                _errorMessages.Add(e.Message + Environment.NewLine + e.Diagnostics);
                _script = null;
            }
        }

        public async Task ContinueScriptAsync(string code)
        {
            try
            {
                _scriptState = await _scriptState?.ContinueWithAsync(code);
            }
            catch (CompilationErrorException e)
            {
                _errorMessages.Add(e.Message + Environment.NewLine + e.Diagnostics);
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
                _workspace?.Dispose();
            }
        }
        ~ CsvEditSharpWorkspace()
        {
            Dispose(false);
        }
    }


}
