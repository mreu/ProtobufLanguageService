#region Copyright © 2013 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtobufClassificationFormatDefinition.cs" company="Michael Reukauff">
//   Copyright © 2013 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.Protobuf
{
  using System.ComponentModel.Composition;
  using System.Windows.Media;

  using Microsoft.VisualStudio.Text.Classification;
  using Microsoft.VisualStudio.Utilities;

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = ProtobufFormatDefinitions.Enum)]
  [Name(ProtobufFormatDefinitions.Enum)]
  [UserVisible(true)]
  [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
  internal sealed class ProtobufEnum : ClassificationFormatDefinition
  {
    public ProtobufEnum()
    {
      IsBold = false;
      ForegroundColor = Colors.Olive;
      DisplayName = "Protobuf Enum";
    }
  }

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = ProtobufFormatDefinitions.FieldRule)]
  [Name(ProtobufFormatDefinitions.FieldRule)]
  [UserVisible(true)]
  [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
  internal sealed class ProtobufFieldRule : ClassificationFormatDefinition
  {
    public ProtobufFieldRule()
    {
      IsBold = false;
      ForegroundColor = Colors.Orchid;
      DisplayName = "Protobuf Field Rule";
    }
  }

  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = ProtobufFormatDefinitions.TopLevelCmd)]
  [Name(ProtobufFormatDefinitions.TopLevelCmd)]
  [UserVisible(true)]
  [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
  internal sealed class ProtobufTopLevelCmd : ClassificationFormatDefinition
  {
    public ProtobufTopLevelCmd()
    {
      IsBold = false;
      ForegroundColor = Colors.DodgerBlue;
      DisplayName = "Protobuf Top Level Cmd";
    }
  }
}
