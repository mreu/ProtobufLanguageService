using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MichaelReukauff.LexerTest
{
  using MichaelReukauff.Lexer;

  [TestClass]
  public class ParseDefaultTests
  {
    [TestMethod]
    public void Default_OK01()
    {
      const string text = "default=1234]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_int32, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.line);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(4, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.line);
    }

    [TestMethod]
    public void Default_OK02()
    {
      const string text = "default=-1234]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_int32, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void Default_OK03()
    {
      const string text = "default=ABCDE]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_unknown, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Enums, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void Default_OK04()
    {
      const string text = "default=1234567890]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_uint64, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(10, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void Default_OK05()
    {
      const string text = "default=12345]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_uint32, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void Default_OK06()
    {
      const string text = "default=1.2345E+3]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_float, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(9, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void Default_OK07()
    {
      const string text = "default=true]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_bool, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(4, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void Default_OK08()
    {
      const string text = "default=false]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_bool, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void Default_OK09()
    {
      const string text = "default=0x123c]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_uint64, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(6, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
    }

    [TestMethod]
    public void Default_NOK01()
    {
      const string text = "default=-1234a]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_int32, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.line);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(6, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.line);
    }

    [TestMethod]
    public void Default_NOK02()
    {
      const string text = "default=\"-1234a\"]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_int32, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.line);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(1, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.line);
    }

    [TestMethod]
    public void Default_NOK03()
    {
      const string text = "default=-1234a]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_string, hasOption = false };

      Assert.IsFalse(lex.ParseDefault(field));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(6, lex.Errors[0].Length);
      Assert.AreEqual(CodeType.Text, lex.Errors[0].CodeType);
    }

    [TestMethod]
    public void Default_NOK04()
    {
      const string text = "default=\"abcde\"]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_string, hasOption = true };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(7, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.String, lex.Tokens[1].CodeType);

      Assert.AreEqual(0, lex.Errors[0].Position);
      Assert.AreEqual(7, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK05()
    {
      const string text = "default";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_string, hasOption = false };

      Assert.IsFalse(lex.ParseDefault(field));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);

      Assert.AreEqual(7, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK06()
    {
      const string text = "default 0";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_string, hasOption = false };

      Assert.IsFalse(lex.ParseDefault(field));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);

      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK07()
    {
      const string text = "default =";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_string, hasOption = false };

      Assert.IsFalse(lex.ParseDefault(field));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);

      Assert.AreEqual(9, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK08()
    {
      const string text = "default=farz]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_bool, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(4, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK09()
    {
      const string text = "default=-1234567890]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_uint64, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(11, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(11, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK10()
    {
      const string text = "default=-12345]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_uint32, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(6, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(6, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK11()
    {
      const string text = "default=-1.2345E+]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_float, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(9, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(9, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK12()
    {
      const string text = "default=-1]";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_error, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(2, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(2, lex.Errors[0].Length);
    }

    [TestMethod]
    public void Default_NOK13()
    {
      const string text = "default=0x12x";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_uint64, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.line);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.line);
    }

    [TestMethod]
    public void Default_NOK14()
    {
      const string text = "default=0x12x";

      var lex = new Lexer(text) { matches = Helper.SplitText(text) };

      Field field = new Field { fieldType = FieldType.type_int32, hasOption = false };

      Assert.IsTrue(lex.ParseDefault(field));

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(7, lex.Tokens[0].Length);
      Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
      Assert.AreEqual(0, lex.line);
      Assert.AreEqual(8, lex.Tokens[1].Position);
      Assert.AreEqual(5, lex.Tokens[1].Length);
      Assert.AreEqual(CodeType.Number, lex.Tokens[1].CodeType);
      Assert.AreEqual(0, lex.line);
    }
  }
}
