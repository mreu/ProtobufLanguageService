using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MichaelReukauff.LexerTest
{
  using Lexer;

  [TestClass]
  public class ParseImportTest
  {
    [TestMethod]
    public void Import_OK1()
    {
      const string text = "  import \"blah fasel\";";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(12, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_OK2()
    {
      const string text = "  import public \"blah fasel\";";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(6, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(16, lex.Tokens[2].Position);
      Assert.AreEqual(12, lex.Tokens[2].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[2].CodeType);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_OK3()
    {
      const string text = "  import weak \"blah fasel\";";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(4, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(14, lex.Tokens[2].Position);
      Assert.AreEqual(12, lex.Tokens[2].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[2].CodeType);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_NOK1()
    {
      const string text = "  import \"blah fasel\"";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(12, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_NOK2()
    {
      const string text = "  import \"blah fasel\"";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(12, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_NOK3()
    {
      const string text = "  import \"blah fasel";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(11, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(20, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_NOK4()
    {
      const string text = "  import ";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_NOK5()
    {
      const string text = "  import \"";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(1, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);

      Assert.AreEqual(10, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_NOK6()
    {
      const string text = "  import public";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(6, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(15, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_NOK7()
    {
      const string text = "  import weak";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(4, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.Line);
      Assert.AreEqual(13, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void Import_NOK8()
    {
      const string text = "  import \"abcdef\"\r\naaa"; // missing ;

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseImport();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
      Assert.AreEqual(1, lex.Line);

      Assert.AreEqual(19, lex.Errors[0].Position);
      Assert.AreEqual(3, lex.Errors[0].Length);
    }
  }
}
