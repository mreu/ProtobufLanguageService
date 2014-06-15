namespace MichaelReukauff.Lexer
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;

  public class Lexer
  {
    /// <summary>
    /// The whole file
    /// </summary>
    private readonly string _buffer;

    /// <summary>
    /// All matches from regEx
    /// </summary>
    internal List<Match> Matches;

    /// <summary>
    /// List of all token
    /// </summary>
    public List<Token> Tokens { get; private set; }

    /// <summary>
    /// List of all errors (if anyx)
    /// </summary>
    public List<Error> Errors { get; private set; }

    /// <summary>
    /// Index into the current matches
    /// </summary>
    internal int Index;

    /// <summary>
    /// Line number of the current line
    /// </summary>
    internal int Line = 0;

    /// <summary>
    /// Is set to true if a package top kevel statement occurs
    /// </summary>
    internal bool HasPackage;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="text">The text to parse</param>
    public Lexer(string text)
    {
      _buffer = text;

      Tokens = new List<Token>();
      Errors = new List<Error>();
    }

    /// <summary>
    /// Analyze the text
    /// Fills the error and token lists
    /// </summary>
    public void Analyze()
    {
      Matches = Helper.SplitText(_buffer);

      // Eat all comments from the list, so that only pure code is left
      EatComments();

      Index = 0;
      Line = 0;

      while (true)
      {
        if (Index >= Matches.Count)
          break;

        // if a new line is here, eat it and increment line number
        if (Word == "\n")
        {
          Index++;
          Line++;
          continue;
        }

        ParseTopLevelStatement();
      }

      if (Errors.Count > 0)
      {
        SetOffsetinErrors();
      }
    }

    public void AnalyzeForCommentsOnly()
    {
      Matches = Helper.SplitText(_buffer);

      EatComments();
    }

    /// <summary>
    /// Set the line offset in all errors
    /// </summary>
    private void SetOffsetinErrors()
    {
      // get all line breaks
      var res = Matches.Where(x => x.Value == "\n");

      foreach (var error in Errors)
      {
        if (error.Line == 0)
          error.Offset = error.Position;
        else
        {
          var l = res.Skip(error.Line - 1).First();
          error.Offset = error.Position - l.Index - 1;
        }
      }
    }

    /// <summary>
    /// Parse top level statement
    /// <remarks>
    /// Must be message, enum, service, extend, import, package or option
    /// </remarks>
    /// </summary>
    internal void ParseTopLevelStatement()
    {
      switch (Word)
      {
        case "message":
          ParseMessage(true);
          break;
        case "enum":
          ParseEnum(true);
          break;
        case "service":
          ParseService();
          break;
        case "extend":
          ParseExtend();
          break;
        case "import":
          ParseImport();
          break;
        case "package":
          ParsePackage();
          break;
        case "option":
          ParseOption(true);
          break;
        default:
          // Syntax error
          AddNewError("Expected top-level statement (e.g. \"message\").");
          IncrementIndex();
          break;
      }
    }

    #region Parse option
    internal bool ParseOption(bool isTopLevel)
    {
      AddNewToken(isTopLevel ? CodeType.TopLevelCmd : CodeType.Keyword);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected option.");
        return false;
      }

      if (!ParseFieldOption())
        return false;

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \";\"");
        return false;
      }

      if (Word != ";")
      {
        AddNewError("Expected \";\"");
        return false;
      }

      IncrementIndex(true);

      return true;
    }
    #endregion Parse option

    #region Parse service
    internal bool ParseService()
    {
      AddNewToken(CodeType.TopLevelCmd);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected service name");
        return false;
      }

      if (!Helper.IsIdentifier(Word))
      {
        AddNewError("Expected service name");
        return false;
      }

      AddNewToken(CodeType.SymDef);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"{\"");
        return false;
      }

      if (Word != "{")
      {
        AddNewError("Expected \"{\"");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"option\" or \"rpc\".");
        return false;
      }

      if (Word == "option")
        if (!ParseOption(false))
          return false;

      if (Word != "rpc")
      {
        AddNewError("Expected \"rpc\"");
        return false;
      }

      AddNewToken(CodeType.Keyword);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected method name.");
        return false;
      }

      if (!Helper.IsIdentifier(Word))
      {
        AddNewError("Expected method name.");
        return false;
      }

      AddNewToken(CodeType.SymDef);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"(\".");
        return false;
      }

      if (Word != "(")
      {
        AddNewError("Expected \"(\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected name of request message.");
        return false;
      }

      if (!Helper.IsIdentifier(Word))
      {
        AddNewError("Expected name of request message.");
        return false;
      }

      AddNewToken(CodeType.SymRef);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \")\"");
        return false;
      }

      if (Word != ")")
      {
        AddNewError("Expected \")\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"returns\".");
        return false;
      }

      if (Word != "returns")
      {
        AddNewError("Expected \"returns\".");
        return false;
      }

      AddNewToken(CodeType.Keyword);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"returns\".");
        return false;
      }

      if (Word != "(")
      {
        AddNewError("Expected \"(\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected name of response message.");
        return false;
      }

      if (!Helper.IsIdentifier(Word))
      {
        AddNewError("Expected name of response message.");
        return false;
      }

      AddNewToken(CodeType.SymRef);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \")\"");
        return false;
      }

      if (Word != ")")
      {
        AddNewError("Expected \")\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \";\".");
        return false;
      }

      if (Word != ";")
      {
        AddNewError("Expected \";\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"}\".");
        return false;
      }

      if (Word != "}")
      {
        AddNewError("Expected \"}\".");
        return false;
      }

      IncrementIndex(true);

      return true;
    }
    #endregion Parse service

    #region Parse enum
    /// <summary>
    /// Parse enum
    /// </summary>
    /// <param name="isTopLevel"></param>
    /// <returns></returns>
    internal bool ParseEnum(bool isTopLevel)
    {
      var field = new Field { FieldType = FieldType.TypeUnknown, HasOption = false };

      AddNewToken(isTopLevel ? CodeType.TopLevelCmd : CodeType.Keyword);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected enum name.");
        return false;
      }

      if (!Helper.IsIdentifier(Word))
      {
        AddNewError("Expected enum name.");
        return false;
      }

      AddNewToken(CodeType.SymDef);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"{\".");
        return false;
      }

      if (Word != "{")
      {
        AddNewError("Expected \"{\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"option\", \"}\", \";\" or enum constant name.");
        return false;
      }

      do
      {
        if (Word == "option")
        {
          if (!ParseOption(false))
            return false;
        }

        if (!Helper.IsIdentifier(Word))
        {
          AddNewError("Expected enum constant name");
          return false;
        }

        AddNewToken(CodeType.Enums);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"=\".");
          return false;
        }

        if (Word != "=")
        {
          AddNewError("Expected \"=\".");
          return false;
        }

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected numeric value for enum constant.");
          return false;
        }

        if (!Helper.IsInteger(Word))
        {
          AddNewError("Expected numeric value for enum constant.");
          return false;
        }

        AddNewToken(CodeType.Number);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \";\".");
          return false;
        }

        if (!ParseFieldOptions(field))
          return false;

        if (Word != ";")
        {
          AddNewError("Expected \";\".");
          return false;
        }

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"option\", \"}\", \";\" or enum constant name.");
          return false;
        }
      }
      while (Word != "}");

      IncrementIndex(true);

      return true;
    }
    #endregion Parse enum

    #region Parse message
    /// <summary>
    /// Parse a message
    /// </summary>
    /// <returns></returns>
    internal bool ParseMessage(bool isTopLevel)
    {
      AddNewToken(isTopLevel ? CodeType.TopLevelCmd : CodeType.Keyword);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected message name.");
        return false;
      }

      if (!Helper.IsIdentifier(Word))
      {
        AddNewError("Expected message name.");
        return false;
      }

      AddNewToken(CodeType.SymDef);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"{\".");
        return false;
      }

      if (Word != "{")
      {
        AddNewError("Expected \"{\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"required\", \"optional\", or \"repeated\".");
        return false;
      }

      while (Word != "}")
      {
        switch (Word)
        {
          case "message":
            ParseMessage(false);
            break;
          case "enum":
            ParseEnum(false);
            break;
          case "extensions":
            ParseMessageExtensions();
            break;
          case "extend":
            ParseExtend();
            break;
          case "option":
            ParseOption(false);
            break;
          default:
            ParseMessageField();
            break;
        }

        if (Index >= Matches.Count)
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"}\".");
          return false;
        }
      }

      // eat the }
      IncrementIndex(true);

      return true;
    }

    /// <summary>
    /// Parse the extension inside a message
    /// </summary>
    /// <returns></returns>
    internal bool ParseMessageExtensions()
    {
      AddNewToken(CodeType.Keyword);

      do
      {
        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected field number range.");
          return false;
        }

        if (!Helper.IsPositiveInteger(Word))
        {
          AddNewError("Expected integer.");
          return false;
        }

        AddNewToken(CodeType.Number);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected the word \"to\".");
          return false;
        }

        if (Word != "to")
        {
          AddNewError("Expected the word \"to\".");
          return false;
        }

        AddNewToken(CodeType.Keyword);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected field number range.");
          return false;
        }

        if (!Helper.IsPositiveInteger(Word))
        {
          if (Word != "max")
          {
            AddNewError("Expected integer or \"max\".");
            return false;
          }

          AddNewToken(CodeType.Keyword);
        }
        else
          AddNewToken(CodeType.Number);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \";\".");
          return false;
        }
      }
      while (Word == ","); // multiple extension ranges are possible

      if (Word != ";")
      {
        AddNewError("Expected \";\".");
        return false;
      }

      IncrementIndex(true);

      return true;
    }

    /// <summary>
    /// Parse a message field
    /// </summary>
    /// <returns></returns>
    internal bool ParseMessageField()
    {
      var field = new Field();

      if (!Helper.IsFieldRule(Word))
      {
        AddNewError("Expected \"required\", \"optional\", or \"repeated\".");
        IncrementIndex(true);
        return false;
      }

      AddNewToken(CodeType.FieldRule);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected field type.");
        return false;
      }

      field.FieldType = Helper.GetFieldType(Word);
      if (field.FieldType == FieldType.TypeUnknown)
      {
        while (true)
        {
          if (!Helper.IsIdentifier(Word))
            AddNewError("Expected Identifier.");

          AddNewToken(CodeType.SymRef);

          if (!IncrementIndex())
          {
            AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected field name.");
            return false;
          }

          if (Word == ".")
          {
            if (!IncrementIndex())
            {
              AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected Identifier.");
              return false;
            }
          }
          else
            break;
        }
      }
      else
      {
        AddNewToken(CodeType.Keyword);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected field name.");
          return false;
        }
      }

      if (!Helper.IsIdentifier(Word))
      {
        AddNewError("Invalid field name.");
        return false;
      }

      AddNewToken(CodeType.SymDef);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"=\".");
        return false;
      }

      if (Word != "=")
      {
        AddNewError("Expected \"=\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected field number.");
        return false;
      }

      if (!Helper.IsPositiveInteger(Word))
        AddNewError("Expected field number.");
      else
      {
        AddNewToken(CodeType.Number);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \";\"");
          return false;
        }
      }

      ParseFieldOptions(field);

      if (Word != ";")
      {
        AddNewError("Expected \";\"");
        return false;
      }

      IncrementIndex(true);

      return true;
    }

    /// <summary>
    /// Parse field options
    /// </summary>
    /// <param name="field"></param>
    /// <returns></returns>
    internal bool ParseFieldOptions(Field field)
    {
      if (Word != "[") // are there any options ?
        return true;                // no, return

      do
      {
        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"option\" or \"default\".");
          return false;
        }

        switch (Word)
        {
          case "default":
            if (!ParseDefault(field))
              return false;
            break;

          default:
            if (!ParseFieldOption())
              return false;
            break;
        }

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \",\" or \"]\".");
          return false;
        }
      }
      while (Word == ","); // do while until no more options are found

      if (Word != "]")
      {
        AddNewError("Expected \",\" or \"]\".");
      }
      else
      {
        IncrementIndex(true);
      }

      return true;
    }

    /// <summary>
    /// Parse an option field
    /// </summary>
    /// <returns></returns>
    internal bool ParseFieldOption()
    {
      do
      {
        if (!Helper.IsIdentifier(Word))
        {
          AddNewError("Expected valid option.");
          return false;
        }

        AddNewToken(CodeType.Keyword);

        if (!IncrementIndex())
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \".\" or \"=\".");
          return false;
        }
      }
      while (Word == ".");

      if (Word == "\n")
        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \".\".");
          return false;
        }

      if (Word != "=")
      {
        AddNewError("Expected \"=\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected option value.");
        return false;
      }

      if (Helper.IsInteger(Word))
      {
        AddNewToken(CodeType.Number);
      }
      else
      {
        if (Word == "\"")
        {
          GetString("Expected string.");
        }
        else
        {
          AddNewToken(CodeType.Keyword);
        }
      }

      return true;
    }

    /// <summary>
    /// Parse the "default" of a message field
    /// </summary>
    /// <param name="field">The field</param>
    /// <returns>True if ok otherwise false</returns>
    internal bool ParseDefault(Field field)
    {
      if (field.HasOption)
      {
        AddNewError("Already set option \"default\".");
      }

      // eat the word "default"
      AddNewToken(CodeType.Keyword);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"=\".");
        return false;
      }

      if (Word != "=")
      {
        AddNewError("Expected \"=\".");
        return false;
      }

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected default value.");
        return false;
      }

      switch (field.FieldType)
      {
        case FieldType.TypeUnknown: // enum value
          AddNewToken(CodeType.Enums);
          break;

        case FieldType.TypeInt32:
        case FieldType.TypeInt64:
        case FieldType.TypeSint32:
        case FieldType.TypeSint64:
        case FieldType.TypeSfixed32:
        case FieldType.TypeSfixed64:
          if (!Helper.IsInteger(Word))
          {
            AddNewError("Expected number.");
          }

          AddNewToken(CodeType.Number);
          break;

        case FieldType.TypeUint64:
        case FieldType.TypeFixed64:
          if (!Helper.IsPositive64Integer(Word))
          {
            AddNewError("Expected unsigned number.");
          }

          AddNewToken(CodeType.Number);
          break;

        case FieldType.TypeUint32:
        case FieldType.TypeFixed32:
          if (!Helper.IsPositiveInteger(Word))
          {
            AddNewError("Expected unsigned number.");
          }

          AddNewToken(CodeType.Number);
          break;

        case FieldType.TypeFloat:
        case FieldType.TypeDouble:
          int i = Matches[Index].Index; // save position

          var num = GetFloatNumber();

          if (!Helper.IsDoubleOrFloat(num))
          {
            AddNewError(i, Matches[Index].Index + Matches[Index].Length - i, "Expected number.");
          }

          AddNewToken(i, Matches[Index].Index + Matches[Index].Length - i, num, CodeType.Number);
          break;

        case FieldType.TypeBool:
          if (Helper.IsTrueOrFalse(Word))
            AddNewToken(CodeType.Keyword);
          else
            AddNewError("Expected \"true\" or \"false\".");
          break;

        case FieldType.TypeString:
        case FieldType.TypeBytes:
          // tokenize string
          if (!GetString("Expected string."))
            return false;
          break;

        default:
          AddNewToken(CodeType.Number);
          AddNewError("Expected default value.");
          break;
      }
      return true;
    }
    #endregion Parse message

    #region Parse extend
    /// <summary>
    /// Parse the extend message
    /// </summary>
    /// <returns></returns>
    internal bool ParseExtend()
    {
      // eat the word "extend"
      AddNewToken(CodeType.TopLevelCmd);
      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected expandee.");
        return false;
      }

      if (!Helper.IsIdentifier(Word))
      {
        AddNewError("Expected expandee.");
        return false;
      }

      AddNewToken(CodeType.SymRef);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \"{\".");
        return false;
      }

      if (Word != "{")
      {
        AddNewError("Expected \"{\".");
        return false;
      }

      // eat the {
      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Reached end of input in extend definition.");
        return false;
      }

      while (true)
      {
        if (!ParseMessageField())
          return false;

        if (Index >= Matches.Count)
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected\"}\"");
          return false;
        }

        if (Word == "}")
          break;
      }

      IncrementIndex(true);

      return true;
    }
    #endregion Parse extend

    #region Parse Import
    internal bool ParseImport()
    {
      // eat the word "import"
      AddNewToken(CodeType.TopLevelCmd);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected a string naming the file to import.");
        return false;
      }

      if (Word == "public")
      {
        AddNewToken(CodeType.Keyword);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected a string naming the file to import.");
          return false;
        }
      }

      if (Word == "weak")
      {
        AddNewToken(CodeType.Keyword);

        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected a string naming the file to import.");
          return false;
        }
      }

      // tokenize string
      if (!GetString("Expected a string naming the file to import."))
        return false;

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \";\".");
        return false;
      }

      if (Word != ";")
        AddNewError("Expected \";\".");
      else
        IncrementIndex(true);

      return true;
    }
    #endregion Parse Import

    #region Parse Package
    /// <summary>
    /// Parse the package top level statement
    /// </summary>
    internal bool ParsePackage()
    {
      if (HasPackage)
      {
        AddNewError("Multiple package definitions.");
      }

      // eat the word "package"
      AddNewToken(CodeType.TopLevelCmd);

      if (!IncrementIndex(true))
      {
        AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected Identifier.");
        return false;
      }

      while (true)
      {
        if (!Helper.IsIdentifier(Word))
          AddNewError("Expected Identifier.");

        AddNewToken(CodeType.Namespace);
        if (!IncrementIndex())
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \";\".");
          return false;
        }

        if (Word == ".")
        {
          if (!IncrementIndex())
          {
            AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected Identifier.");
            return false;
          }
        }
        else
          break;
      }

      if (Word == "\n")
      {
        if (!IncrementIndex(true))
        {
          AddNewError(Matches[Index - 1].Index + Matches[Index - 1].Length, 1, "Expected \";\".");
          return false;
        }
      }

      if (Word != ";")
        AddNewError("Expected \";\".");
      else
      {
        HasPackage = true;
        IncrementIndex(true);
      }

      return true;
    }
    #endregion Parse Package

    #region Comment handling
    /// <summary>
    /// Eat all comments from the matches, so we get a clean list wihtout any comments
    /// Put the whole comment into one token
    /// </summary>
    internal void EatComments()
    {
      for (int idx = 0; idx < Matches.Count; idx++)
      {
        var match = Matches[idx];

        // is it a line comment starting here
        if (match.Value == "//")
        {
          var token = new Token(Line, match.Index, 0, string.Empty, CodeType.Comment);

          // now search the end of this line
          int i = idx;
          for (; i < Matches.Count; i++)
          {
            if (Matches[i].Value == "\n")
              break;
          }

          // is comment until EOF
          if (i == Matches.Count)
            token.Length = Matches[i - 1].Index + Matches[i - 1].Length - match.Index;
          else
            token.Length = Matches[i].Index - match.Index;

          // set text from buffer
          token.Text = _buffer.Substring(token.Position, token.Length);
          if (token.Text.EndsWith("\r"))
            token.Length--;

          // add token to list
          AddNewToken(token);

          // now remove all found comment token from the list
          Matches.RemoveRange(idx, i - idx);
        }

        // is it a block comment starting here
        if (match.Value == "/*")
        {
          var token = new Token(Line, match.Index, 0, string.Empty, CodeType.Comment);

          // now search the */
          int i = idx;
          for (; i < Matches.Count; i++)
          {
            if (Matches[i].Value == "\n")
              continue;

            if (Matches[i].Value == "*/")
              break;
          }

          // is comment until EOF
          if (i == Matches.Count)
          {
            token.Length = Matches[i - 1].Index + Matches[i - 1].Length - match.Index;

            // and error */ is missed
            AddNewError(Matches[i - 1].Index + Matches[i - 1].Length, 1, "Missing */");
          }
          else
          {
            token.Length = Matches[i].Index + Matches[i].Length - match.Index;
            i++;
          }

          // set text from buffer
          token.Text = _buffer.Substring(token.Position, token.Length);

          // add token to list
          AddNewToken(token);

          // now remove all found comment token from the list
          Matches.RemoveRange(idx, i - idx);
        }
      }
    }
    #endregion Comment handling

    #region Helper methods
    /// <summary>
    /// Increment the index of the match collection by one
    /// </summary>
    /// <returns>True if there are more matches otherwise false</returns>
    internal bool IncrementIndex(bool ignoreNewLine = false)
    {
      Index++;
      if (Index >= Matches.Count)
        return false;

      if (ignoreNewLine)
      {
        while (Word == "\n")
        {
          Index++;
          Line++;

          if (Index >= Matches.Count)
            return false;
        }
      }

      return (Index < Matches.Count);
    }

    /// <summary>
    /// Get the string from the matches collection
    /// </summary>
    /// <param name="error">Error message</param>
    /// <returns>True if string was found and ok otherwise false</returns>
    internal bool GetString(string error)
    {
      if (Word != "\"")
      {
        AddNewError(error);
        return false;
      }

      int i = Index + 1;
      for (; i < Matches.Count; i++) // find end of string or newline whatever comes first
      {
        if (Matches[i].Value == "\"" || Matches[i].Value == "\n")
          break;
      }

      if (i >= Matches.Count) // until eof?
      {
        AddNewToken(Matches[Index].Index, Matches[i - 1].Index + Matches[i - 1].Length - Matches[Index].Index, string.Empty, CodeType.String);
        AddNewError(Matches[i - 1].Index + Matches[i - 1].Length, 1, "Closing \" missing.");
        Index = i;
        return false;
      }

      if (Matches[i].Value != "\"") // end of string found ?
      {
        // end of string not found
        // find other limiting end
        i = Index + 1;
        for (; i < Matches.Count; i++) // find end terminating char of string
        {
          if (Matches[i].Value == "]" || Matches[i].Value == ";" || Matches[i].Value == "\n")
            break;
        }

        AddNewError(Matches[i - 1].Index + Matches[i - 1].Length, 1, "Closing \" missing.");
        AddNewToken(Matches[Index].Index, Matches[i - 1].Index + Matches[i - 1].Length - Matches[Index].Index, string.Empty, CodeType.String);
      }
      else
        AddNewToken(Matches[Index].Index, Matches[i].Index + Matches[i].Length - Matches[Index].Index, string.Empty, CodeType.String);

      Index = i;
      return true;
    }

    /// <summary>
    /// Read a possible floating number from the token collection
    /// float numbers can be in the following formats:
    /// nnnn
    /// n.nnn
    /// n.nnnE+n
    /// n.nnnE-n
    /// plus a sign in front of the number
    /// </summary>
    /// <returns>The floating number as string</returns>
    internal string GetFloatNumber()
    {
      int max = Matches.Count;

      var ret = GetExponent();
      string number = ret.Item1;

      if (!ret.Item2)
      {
        if (Index + 1 >= max)
          return number;

        Index++;

        if (Word == ".")
        {
          number += '.';

          if (Index + 1 >= max)
            return number;

          Index++;

          ret = GetExponent();
          number += ret.Item1;
        }
      }

      return number;
    }

    /// <summary>
    /// Get the E-part of a floating number
    /// </summary>
    /// <returns>The string with the floating number and if a E number was found</returns>
    private Tuple<string, bool> GetExponent()
    {
      string number = Word;
      if (number == "+") // the + sign is a single token, we must get the nect token which is the numeric part of it
      {
        if (Index < Matches.Count)
        {
          Index++;
          number += Word;
        }
      }

      if (Word.EndsWith("E") || Word.EndsWith("e"))
      {
        if (Index + 1 >= Matches.Count)
          return new Tuple<string, bool>(number, true);

        Index++;

        if (Word.StartsWith("+") || Word.StartsWith("-"))
        {
          number += Word;

          if (Index + 1 >= Matches.Count)
            return new Tuple<string, bool>(number, true);

          Index++;

          if (Helper.IsInteger(Word))
            number += Word;
          else
          {
            Index--;
          }
        }

        return new Tuple<string, bool>(number, true);
      }

      return new Tuple<string, bool>(number, false);
    }

    private void AddNewToken(CodeType codeType)
    {
      Tokens.Add(new Token(Line, Matches[Index], codeType));
    }

    private void AddNewToken(Token token)
    {
      Tokens.Add(token);
    }

    private void AddNewToken(int offset, int length, string text, CodeType codeType)
    {
      Tokens.Add(new Token(Line, offset, length, text, codeType));
    }

    private void AddNewError(string message, CodeType codeType = CodeType.Text)
    {
      if (Errors.All(x => x.Position != Matches[Index].Index))
        Errors.Add(new Error(Line, Matches[Index], codeType, message));
    }

    private void AddNewError(int offset, int length, string message, string text = "", CodeType codeType = CodeType.Text)
    {
      if (Errors.All(x => x.Position != offset))
        Errors.Add(new Error(Line, offset, length, text, codeType, message));
    }

    /// <summary>
    /// Get the current token from the matches
    /// </summary>
    public string Word
    {
      get
      {
        return Matches[Index].Value;
      }
    }
    #endregion Helper methods
  }

  class Field
  {
    internal FieldType FieldType = FieldType.TypeUnknown;
    internal bool HasOption = false;
  }
}
