using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Windows.Media;

namespace CsvEditSharp.Models
{
    public class CompletionData : ICompletionData
    {
        public object Content { get; set; }
        public object Description { get; set; }
        public ImageSource Image { get; set; } = null;
        public double Priority { get; set; } = 0;
        public string Text { get; set; }
        
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }

    }
}
