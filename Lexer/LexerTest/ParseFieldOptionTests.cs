using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MichaelReukauff.LexerTest
{
  using Lexer;

  [TestClass]
  public class ParseFieldOptionTests
  {
    [TestMethod]
    public void ParseFiledOption_OK01()
    {
      const string text = "  packed=true];";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseFieldOption());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);

      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(6, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(9, lex.Tokens[1].Position);
      Assert.AreEqual(4, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void ParseFiledOption_OK02()
    {
      const string text = "  aaa=\"farz\"];";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseFieldOption());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(4, lex.Index);

      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(3, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(6, lex.Tokens[1].Position);
      Assert.AreEqual(6, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void ParseFiledOption_OK03()
    {
      const string text = "  aaa=1234];";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseFieldOption());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);

      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(3, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(6, lex.Tokens[1].Position);
      Assert.AreEqual(4, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void ParseFiledOption_OK04()
    {
      const string text = "  aaa=abcdefgh];";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseFieldOption());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);

      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(3, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(6, lex.Tokens[1].Position);
      Assert.AreEqual(8, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void ParseFiledOption_NOK01()
    {
      const string text = "  aaa="; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseFieldOption());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);

      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(3, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
    }

    [TestMethod]
    public void ParseFiledOption_NOK02()
    {
      const string text = "  aaa"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseFieldOption());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);

      Assert.AreEqual(2, lex.Tokens[0].Position);
      Assert.AreEqual(3, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
    }

    [TestMethod]
    public void ParseFiledOption_NOK03()
    {
      const string text = "  1aaa"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseFieldOption());

      Assert.AreEqual(0, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Index);
    }

    [TestMethod]
    public void ParseFiledOption_NOK04()
    {
      const string text = "  cpp=\r\n"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseFieldOption());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.Index);
    }
  }
}
