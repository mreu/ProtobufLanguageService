#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufLanguageTagger.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;

  using MichaelReukauff.Lexer;

  using Microsoft.VisualStudio.Text;
  using Microsoft.VisualStudio.Text.Tagging;

  /// <summary>
  /// ProtobufLanguageTagger is the core parser for the protobuf 'language'. It handles continuation lines
  /// and finds both language elements and errors within the given SnapshotSpan(s)
  /// </summary>
  internal sealed class ProtobufLanguageTagger : ITagger<ProtobufTokenTag>
  {
    readonly ITextBuffer _buffer;

    private Task _task;

    private Lexer _lexer;

    private readonly List<ITagSpan<ProtobufTokenTag>> _tagList = new List<ITagSpan<ProtobufTokenTag>>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="buffer">The text buffer</param>
    internal ProtobufLanguageTagger(ITextBuffer buffer)
    {
      _buffer = buffer;

      Parse();

      _buffer.Changed += buffer_Changed;
    }

    /// <summary>
    /// Task to reparse the whole text and fires a TagsChanged event
    /// </summary>
    /// <param name="snapshot"></param>
    private void Parse(ITextSnapshot snapshot)
    {
      Parse();

      SnapshotPoint startPoint = new SnapshotPoint(snapshot, 0);
      SnapshotPoint endPoint = new SnapshotPoint(snapshot, snapshot.Length);
      SnapshotSpan expandedSpan = new SnapshotSpan(startPoint, endPoint);
      SnapshotSpanEventArgs args = new SnapshotSpanEventArgs(expandedSpan);
      TagsChanged(this, args);
    }

    /// <summary>
    /// Parse the text and build error and token lists
    /// </summary>
    private void Parse()
    {
      _lexer = new Lexer(_buffer.CurrentSnapshot.GetText());
      _lexer.Analyze();

      _tagList.Clear();

      foreach (var message in _lexer.Errors)
      {
        // check the length of the new span, should not be longer than the current text
        int length = message.Length;
        if (message.Position + message.Length > _buffer.CurrentSnapshot.Length)
        {
          length = _buffer.CurrentSnapshot.Length - message.Position;
        }

        if (length > 0)
        {
          var newSpan = new Span(message.Position, length);
          _tagList.Add(new TagSpan<ProtobufTokenTag>(new SnapshotSpan(_buffer.CurrentSnapshot, newSpan), new ProtobufErrorTag(message.Message)));
        }
      }

      foreach (var token in _lexer.Tokens)
      {
        // check the length of the new span, should not be longer than the current text
        int length = token.Length;
        if (token.Position + token.Length > _buffer.CurrentSnapshot.Length)
        {
          length = _buffer.CurrentSnapshot.Length - token.Position;
        }

        if (length > 0)
        {
          var newSpan = new Span(token.Position, length);
          _tagList.Add(new TagSpan<ProtobufTokenTag>(new SnapshotSpan(_buffer.CurrentSnapshot, newSpan), new ProtobufTokenTag(token.CodeType)));
        }
      }
    }

    /// <summary>
    /// When the buffer changes, we reparse all in a separate task
    /// </summary>
    private void buffer_Changed(object sender, TextContentChangedEventArgs e)
    {
      System.Diagnostics.Debug.WriteLine("buffer_changed start");
      var temp = TagsChanged;
      if (temp != null)
      {
        Action<object> action = obj => Parse((ITextSnapshot)obj);
        _task = new Task(action, e.After.TextBuffer.CurrentSnapshot); // start background parsing
        _task.Start();
      }
      System.Diagnostics.Debug.WriteLine("buffer_changed end");
    }

    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    /// <summary>
    /// Parse the given span(s) and return all the tags that intersect the specified spans.
    /// </summary>
    /// <param name="spans">ordered collection of non-overlapping spans</param>
    /// <returns>unordered enumeration of tags</returns>
    public IEnumerable<ITagSpan<ProtobufTokenTag>> GetTags(NormalizedSnapshotSpanCollection spans)
    {
      return _tagList;
    }
  }
}
