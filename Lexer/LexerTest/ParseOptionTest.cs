using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MichaelReukauff.LexerTest
{
  using MichaelReukauff.Lexer;

  [TestClass]
  public class ParseOptionTest
  {
    [TestMethod]
    public void ParseEnum_OK01()
    {
      const string text = "  option java_package = \"sakldfjhfggh.lkj l\";\r\n";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseOption(true));

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(11, lex.ix);
    }

    [TestMethod]
    public void ParseEnum_NOK01()
    {
      const string text = "  option java_package = \"sakldfjhfggh.lkj l\"\r\n";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseOption(true));

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(10, lex.ix);
    }

    [TestMethod]
    public void ParseEnum_NOK02()
    {
      const string text = "  option java_package = \"sakldfjhfggh.lkj l\r\n";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseOption(true));

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(2, lex.Errors.Count);
      Assert.AreEqual(9, lex.ix);
    }

    [TestMethod]
    public void ParseEnum_NOK03()
    {
      const string text = "  option java_package = \"\r\n";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseOption(true));

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(2, lex.Errors.Count);
      Assert.AreEqual(5, lex.ix);
    }

    [TestMethod]
    public void ParseEnum_NOK04()
    {
      const string text = "  option java_package = \r\n";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseOption(true));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(4, lex.ix);
    }

    [TestMethod]
    public void ParseEnum_NOK05()
    {
      const string text = "  option java_package\r\n";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseOption(true));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.ix);
    }

    [TestMethod]
    public void ParseEnum_NOK06()
    {
      const string text = "  option\r\n";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseOption(true));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.ix);
    }

  }
}
