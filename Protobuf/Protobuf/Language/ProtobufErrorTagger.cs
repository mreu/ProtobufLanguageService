#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufErrorTagger.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.Collections.Generic;

  using MichaelReukauff.Lexer;

  using Microsoft.VisualStudio.Shell;
  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Tagging;

  /// <summary>
  /// Translate Token into ErrorTags and Error List items
  /// </summary>
  internal sealed class ErrorTagger : ITagger<ErrorTag>, IDisposable
  {
    readonly ITagAggregator<ProtobufTokenTag> _aggregator;

    readonly ITextBuffer _buffer;

    readonly ErrorListProvider _errorProvider;

    readonly ITextDocument _document;

    /// <summary>
    /// The constructor
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="aggregatorFactory"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="textDocumentFactory"></param>
    internal ErrorTagger(ITextBuffer buffer, IBufferTagAggregatorFactoryService aggregatorFactory, IServiceProvider serviceProvider, ITextDocumentFactoryService textDocumentFactory)
    {
      _buffer = buffer;

      _aggregator = aggregatorFactory.CreateTagAggregator<ProtobufTokenTag>(buffer);

      if (!textDocumentFactory.TryGetTextDocument(_buffer, out _document))
        _document = null;

      _errorProvider = new ErrorListProvider(serviceProvider);

      ReparseFile(null, EventArgs.Empty);

      BufferIdleEventUtil.AddBufferIdleEventListener(_buffer, ReparseFile);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose()
    {
      if (_errorProvider != null)
      {
        _errorProvider.Tasks.Clear();
        _errorProvider.Dispose();
      }

      BufferIdleEventUtil.RemoveBufferIdleEventListener(_buffer, ReparseFile);
    }

    /// <summary>
    /// Find the Error tokens in the set of all tokens and create an ErrorTag for each
    /// </summary>
    public IEnumerable<ITagSpan<ErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
      foreach (var tagSpan in _aggregator.GetTags(spans))
      {
        if (tagSpan.Tag._type == CodeType.Error)
        {
          var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
          ProtobufErrorTag tag = tagSpan.Tag as ProtobufErrorTag;
          if (tag != null)
            yield return new TagSpan<ErrorTag>(tagSpans[0], new ErrorTag("error", tag._message));
        }
      }
    }

#pragma warning disable 67
    // the Classifier tagger is translating buffer change events into TagsChanged events, so we don't have to
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore

    /// <summary>
    /// Updates the Error List by clearing our items and adding any errors found in the current set of tags
    /// </summary>
    void ReparseFile(object sender, EventArgs args)
    {
      ITextSnapshot snapshot = _buffer.CurrentSnapshot;
      NormalizedSnapshotSpanCollection spans = new NormalizedSnapshotSpanCollection(new SnapshotSpan(snapshot, 0, snapshot.Length));

      _errorProvider.Tasks.Clear();

      foreach (var tagSpan in _aggregator.GetTags(spans))
      {
        if (tagSpan.Tag._type == CodeType.Error)
        {
          var tagSpans = tagSpan.Span.GetSpans(spans[0].Snapshot);
          ProtobufErrorTag tag = tagSpan.Tag as ProtobufErrorTag;
          AddErrorTask(tagSpans[0], tag);
        }
      }
    }

    /// <summary>
    /// Add one task to the Error List based on the given tag
    /// </summary>
    private void AddErrorTask(SnapshotSpan span, ProtobufErrorTag tag)
    {
      if (_errorProvider != null)
      {
        ErrorTask task = new ErrorTask { CanDelete = true };
        if (_document != null)
          task.Document = _document.FilePath;
        task.ErrorCategory = TaskErrorCategory.Error;
        task.Text = tag._message;
        task.Line = span.Start.GetContainingLine().LineNumber;
        task.Column = span.Start.Position - span.Start.GetContainingLine().Start.Position;

        task.Navigate += task_Navigate;

        _errorProvider.Tasks.Add(task);
      }
    }

    /// <summary>
    /// Callback method attached to each of our tasks in the Error List
    /// </summary>
    void task_Navigate(object sender, EventArgs e)
    {
      ErrorTask error = sender as ErrorTask;

      if (error != null)
      {

        error.Line += 1;
        error.Column += 1;
        _errorProvider.Navigate(error, new Guid(EnvDTE.Constants.vsViewKindCode));
        error.Column -= 1;
        error.Line -= 1;
      }
    }
  }
}