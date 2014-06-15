using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MichaelReukauff.LexerTest
{
  using Lexer;

  [TestClass]
  public class ParseFieldOptions
  {
    [TestMethod]
    public void ParseFieldOptions_OK01()
    {
      const string text = "  [default=123, packed=true];";

      var field = new Field { FieldType = FieldType.TypeInt32, HasOption = false };

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseFieldOptions(field));

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(9, lex.Index);

      Assert.AreEqual(3, lex.Tokens[0].Position);
      Assert.AreEqual(11, lex.Tokens[1].Position);
      Assert.AreEqual(16, lex.Tokens[2].Position);
      Assert.AreEqual(23, lex.Tokens[3].Position);

      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(3, lex.Tokens[1].Length);
      Assert.AreEqual(6, lex.Tokens[2].Length);
      Assert.AreEqual(4, lex.Tokens[3].Length);

      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[2].CodeType);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[3].CodeType);
    }

    [TestMethod]
    public void ParseFieldOptions_NOK01()
    {
      const string text = "  [default=123 packed=true];"; // comma is missing

      var field = new Field { FieldType = FieldType.TypeInt32, HasOption = false };

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.ParseFieldOptions(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(4, lex.Index);
    }

    [TestMethod]
    public void ParseFieldOptions_NOK02()
    {
      const string text = "  [";

      var field = new Field { FieldType = FieldType.TypeInt32, HasOption = false };

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseFieldOptions(field));

      Assert.AreEqual(0, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void ParseFieldOptions_NOK03()
    {
      const string text = "  [default x 123 packed=true];";

      var field = new Field { FieldType = FieldType.TypeInt32, HasOption = false };

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseFieldOptions(field));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void ParseFieldOptions_NOK04()
    {
      const string text = "  [default = 123";

      var field = new Field { FieldType = FieldType.TypeInt32, HasOption = false };

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.ParseFieldOptions(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(4, lex.Index);
    }
  }
}
