#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LexerTests.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  using MichaelReukauff.Lexer;

  [TestClass]
  public class LexerTests
  {
    #region Comment tests
    [TestMethod]
    public void LineComment_without_EOF1()
    {
      const string text = " option farz = true;  // this is a comment until end of line\r\n message...";

      var lex = new Lexer(text) {Matches = Helper.SplitText(text) };

      lex.EatComments();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Index);
      Assert.AreEqual(10, lex.Matches.Count);

      Assert.AreEqual(22, lex.Tokens[0].Position);
      Assert.AreEqual(38, lex.Tokens[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void LineComment_without_EOF2()
    {
      const string text = " option farz = true;  // this is a comment until end of line\r\n message...";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.EatComments();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Index);
      Assert.AreEqual(10, lex.Matches.Count);

      Assert.AreEqual(22, lex.Tokens[0].Position);
      Assert.AreEqual(38, lex.Tokens[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void LineComment_until_EOF()
    {
      const string text = "  option farz = true;  // this is a comment until EOF";

      var lex = new Lexer(text) {Matches = Helper.SplitText(text) };

      lex.EatComments();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Index);
      Assert.AreEqual(5, lex.Matches.Count);

      Assert.AreEqual(23, lex.Tokens[0].Position);
      Assert.AreEqual(30, lex.Tokens[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void BlockComment_In_One_Line()
    {
      const string text = " asdölfkj asdfj asdölkjfas  asdflj /* asöldkfjasdfölkjhas dfhööhasdf */ hö\r\nas dlfjhas dflasd fasdlkfh ";

      var lex = new Lexer(text) {Matches = Helper.SplitText(text) };

      lex.EatComments();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Index);
      Assert.AreEqual(10, lex.Matches.Count);

      Assert.AreEqual(35, lex.Tokens[0].Position);
      Assert.AreEqual(36, lex.Tokens[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void BlockComment_Over_Multiple_Lines()
    {
      const string text = " asdölfkj asdfj asdölkjfas  asdflj /* asöldkfjasdfölkjhas\r\n dfhööhasdf */ hö\r\nas dlfjhas dflasd fasdlkfh ";

      var lex = new Lexer(text) {Matches = Helper.SplitText(text) };

      lex.EatComments();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Index);
      Assert.AreEqual(10, lex.Matches.Count);

      Assert.AreEqual(35, lex.Tokens[0].Position);
      Assert.AreEqual(38, lex.Tokens[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void BlockComment_Over_Multiple_Lines_Until_EOF()
    {
      const string text = " asdölfkj asdfj asdölkjfas  asdflj /* asöldkfjasdfölkjhas\r\n dfhööhasdf */";

      var lex = new Lexer(text) {Matches = Helper.SplitText(text) };

      lex.EatComments();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(0, lex.Index);
      Assert.AreEqual(4, lex.Matches.Count);

      Assert.AreEqual(35, lex.Tokens[0].Position);
      Assert.AreEqual(38, lex.Tokens[0].Length);
      Assert.AreEqual(0, lex.Line);
    }

    [TestMethod]
    public void BlockComment_Over_Multiple_Lines_Until_EOF_Missing_EndComment()
    {
      const string text = " asdölfkj asdfj asdölkjfas  asdflj /* asöldkfjasdfölkjhas\r\n dfhöö \r\nhasdf";

      var lex = new Lexer(text) {Matches = Helper.SplitText(text) };

      lex.EatComments();

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Index);
      Assert.AreEqual(4, lex.Matches.Count);

      Assert.AreEqual(35, lex.Tokens[0].Position);
      Assert.AreEqual(38, lex.Tokens[0].Length);

      Assert.AreEqual(0, lex.Errors[0].Line);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(73, lex.Errors[0].Position);

      Assert.AreEqual(0, lex.Line);
    }
    #endregion

    #region GetString
    [TestMethod]
    public void GetString_OK()
    {
      const string text = "\"abc def\"";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.GetString("xxx"));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(9, lex.Tokens[0].Length);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void GetString_NOK1()
    {
      const string text = "abc def\"";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.GetString("xxx"));

      Assert.AreEqual(0, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Errors[0].Position);
      Assert.AreEqual(3, lex.Errors[0].Length);
      Assert.AreEqual(0, lex.Index);
    }

    [TestMethod]
    public void GetString_NOK2()
    {
      const string text = "\"abc def";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsFalse(lex.GetString("xxx"));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(8, lex.Tokens[0].Length);
      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void GetString_NOK3()
    {
      const string text = "\"abc def\r\ndksfjhs";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      Assert.IsTrue(lex.GetString("xxx"));

      Assert.AreEqual(1, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(0, lex.Tokens[0].Position);
      Assert.AreEqual(8, lex.Tokens[0].Length);
      Assert.AreEqual(8, lex.Errors[0].Position);
      Assert.AreEqual(1, lex.Errors[0].Length);
      Assert.AreEqual(3, lex.Index);
    }
    #endregion GetString

    #region GetFloat
    [TestMethod]
    public void GetFloat_OK1()
    {
      const string text = "123";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(0, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK2()
    {
      const string text = "1.23";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK3()
    {
      const string text = "123E";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(0, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK4()
    {
      const string text = "123E+";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK5()
    {
      const string text = "123E+3";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK6()
    {
      const string text = "1.23E+3";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(4, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK7()
    {
      const string text = "1.";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK8()
    {
      const string text = "1.123E+]";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text.Substring(0, text.Length - 1), ret);
      Assert.AreEqual(3, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK9()
    {
      const string text = "+123";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK10()
    {
      const string text = "-123";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(0, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK11()
    {
      const string text = "+123E";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(1, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK12()
    {
      const string text = "+123E-3";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text, ret);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK13()
    {
      const string text = "+123E-3;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual(text.Substring(0,text.Length-1), ret);
      Assert.AreEqual(2, lex.Index);
    }

    [TestMethod]
    public void GetFloat_OK14()
    {
      const string text = "+123E x3;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      var ret = lex.GetFloatNumber();

      Assert.AreEqual("+123E", ret);
      Assert.AreEqual(2, lex.Index);
    }
    #endregion GetFloat

    #region ParseTopLevel
    [TestMethod]
    public void ParseTopLevel_OK01()
    {
      const string text = "  message SearchResponse {\r\n  required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n}\r\n";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseTopLevelStatement();

      Assert.AreEqual(14, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(27, lex.Index);
    }

    [TestMethod]
    public void ParseTopLevel_OK02()
    {
      const string text = "  enum test\r\n  {\r\n  a=1;\r\n  b=2;\r\n}\r\n";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseTopLevelStatement();

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(17, lex.Index);
    }

    [TestMethod]
    public void ParseTopLevel_OK03()
    {
      const string text = "  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseTopLevelStatement();

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(18, lex.Index);
    }

    [TestMethod]
    public void ParseTopLevel_OK04()
    {
      const string text = "  extend test\r\n  {\r\n optional int32 abc = 12;\r\n}\r\n";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseTopLevelStatement();

      Assert.AreEqual(6, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(14, lex.Index);
    }

    [TestMethod]
    public void ParseTopLevel_OK05()
    {
      const string text = "  import \"lkjlkjlkj\";";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseTopLevelStatement();

      Assert.AreEqual(2, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(5, lex.Index);
    }

    [TestMethod]
    public void ParseTopLevel_OK06()
    {
      const string text = "  package asdf.sdf.sf;\r\n";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseTopLevelStatement();

      Assert.AreEqual(4, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(8, lex.Index);
    }

    [TestMethod]
    public void ParseTopLevel_OK07()
    {
      const string text = "  option abc = true;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseTopLevelStatement();

      Assert.AreEqual(3, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(5, lex.Index);
    }

    [TestMethod]
    public void ParseTopLevel_NOK01()
    {
      const string text = "  abc = true;";

      var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

      lex.ParseTopLevelStatement();

      Assert.AreEqual(0, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(1, lex.Index);
    }
    #endregion ParseTopLevel

    [TestMethod]
    public void Analyze_OK01()
    {
      const string text = "  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n";

      var lex = new Lexer(text);

      lex.Analyze();

      Assert.AreEqual(7, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(18, lex.Index);
    }

    [TestMethod]
    public void Analyze_OK02()
    {
      const string text = "  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n" +
                          "  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n";

      var lex = new Lexer(text);

      lex.Analyze();

      Assert.AreEqual(14, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(36, lex.Index);
    }

    [TestMethod]
    public void Analyze_OK03()
    {
      const string text = "\r\n  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n" +
                          "  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n";

      var lex = new Lexer(text);

      lex.Analyze();

      Assert.AreEqual(14, lex.Tokens.Count);
      Assert.AreEqual(0, lex.Errors.Count);
      Assert.AreEqual(37, lex.Index);
    }

    [TestMethod]
    public void Analyze_NOK01()
    {
      const string text = "\r\n  service test\r\n  {\r\n rpx test1 (test2) returns (test3);\r\n}\r\n" + // rpx instead of rpc
                          "  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n";

      var lex = new Lexer(text);

      lex.Analyze();

      Assert.AreEqual(9, lex.Tokens.Count);
      Assert.AreEqual(11, lex.Errors.Count);
      Assert.AreEqual(37, lex.Index);
    }

    [TestMethod]
    public void Analyze_NOK02()
    {
      const string text = "  x\r\n  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n" +
                          "  service test\r\n  {\r\n rpc test1 (test2) returns (test3);\r\n}\r\n";

      var lex = new Lexer(text);

      lex.Analyze();

      Assert.AreEqual(14, lex.Tokens.Count);
      Assert.AreEqual(1, lex.Errors.Count);
      Assert.AreEqual(38, lex.Index);
    }
  }
}
