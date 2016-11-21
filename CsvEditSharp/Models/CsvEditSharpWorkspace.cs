using CsvHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsvEditSharp.Models
{
    public class CsvEditSharpWorkspace : IDisposable
    {
        private static readonly string _projectName = "CsvEditorConfig";
        private static readonly ProjectId _projectId = ProjectId.CreateNewId();
        private static readonly VersionStamp _versionStamp = VersionStamp.Create();
        private static readonly ProjectInfo _projectInfo = ProjectInfo.Create(_projectId, _versionStamp, _projectName, _projectName, LanguageNames.CSharp);
        private ICsvEditSharpConfigurationHost _host;
        private Script<object> _script;
        private ScriptState<object> _scriptState;
        private SemanticModel _semanticModel;
        private AdhocWorkspace _workspace = new AdhocWorkspace();

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
                _script = CSharpScript.Create(code, ScriptOptions.Default
                    .WithReferences(
                        typeof(object).Assembly,
                        typeof(CsvReader).Assembly,
                        typeof(ICsvEditSharpConfigurationHost).Assembly)
                    .WithImports(
                        "System",
                        "System.Collections.Generic",
                        "System.Linq",
                        "System.Globalization",
                        "System.Text",
                        "CsvHelper.Configuration",
                        "CsvHelper",
                        "CsvEditSharp.Models"),
                    typeof(ICsvEditSharpConfigurationHost));

                _workspace.ClearSolution();
                var project = _workspace.AddProject(_projectInfo);
                var sourcetext = Microsoft.CodeAnalysis.Text.SourceText.From(code);
                _workspace.AddDocument(project.Id, "Config.csx", sourcetext);

                var compilation = _script.GetCompilation();
                var syntaxTree = compilation.SyntaxTrees.First();
                _semanticModel = compilation.GetSemanticModel(syntaxTree);

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
            if (_semanticModel == null || _workspace == null)
            {
                return Enumerable.Empty<CompletionData>();
            }

            var items = await Recommender.GetRecommendedSymbolsAtPositionAsync(_semanticModel, position, _workspace);
            return items.Select(symbol => new CompletionData()
            {
                Content = symbol.Name,
                Text = symbol.Name,
                Description = symbol.ToMinimalDisplayString(_semanticModel, position)
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
