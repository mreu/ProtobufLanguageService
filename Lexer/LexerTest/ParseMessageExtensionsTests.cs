using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MichaelReukauff.LexerTest
{
  using Lexer;

  [TestClass]
  public class ParseMessageExtensionsTests
  {
    [TestMethod]
    public void MessageField_OK01()
    {
      const string text = "  extensions 1000 to 1234;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseMessageExtensions());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(5, lex.Index);
    }

    [TestMethod]
    public void MessageField_OK02()
    {
      const string text = "  extensions 1000 to max;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseMessageExtensions());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(5, lex.Index);
    }

    [TestMethod]
    public void MessageField_OK03()
    {
      const string text = "  extensions 1000 to 1234, 2000 to 3000;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseMessageExtensions());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(9, lex.Index);
    }

    [TestMethod]
    public void MessageField_OK04()
    {
      const string text = "  extensions 1000 to 1999, 6000 to max;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseMessageExtensions());

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(9, lex.Index);
    }

    [TestMethod]
    public void MessageField_OK05()
    {
      const string text = "  extensions 0x100c to 0x199c;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseMessageExtensions());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(5, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK01()
    {
      const string text = "  extensions 10"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK02()
    {
      const string text = "  extensions 1000 t"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK03()
    {
      const string text = "  extensions 1000 to"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK04()
    {
      const string text = "  extensions 1000 to ma"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK05()
    {
      const string text = "  extensions "; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK06()
    {
      const string text = "  extensions -1000 to 1131"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK07()
    {
      const string text = "  extensions 1000 to -1131"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK08()
    {
      const string text = "  extensions 1000 to 113 x"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(4, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK09()
    {
      const string text = "  extensions 1000 to 113"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(4, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK10()
    {
      const string text = "  extensions 100c to 113c;"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void MessageField_NOK11()
    {
      const string text = "  extensions 0x100x to 113c;"; // incomplete statement

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseMessageExtensions());

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }
  }
}
