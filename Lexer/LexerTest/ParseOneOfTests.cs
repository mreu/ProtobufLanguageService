#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseOneOfTests.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
    using MichaelReukauff.Lexer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Parse oneof tests.
    /// </summary>
    [TestClass]
    public class ParseOneOfTests
    {
        [TestMethod]
        public void ParseOneOf_NOK01()
        {
            const string Text = "  oneof oneof_name {\r\n  string url = 2;\r\n  string title = 3;\r\n  msg1 snippets = 4;\r\n}";

            var lex = new Lexer(Text) { Matches = Helper.SplitText(Text) };

            lex.ParseTopLevelStatement();

            Assert.AreEqual(0, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(1, lex.Index);

        }

        [TestMethod]
        public void ParseOneOf_OK01()
        {
            const string Text = "  oneof oneof_name {\r\n  string url = 2;\r\n  string title = 3;\r\n  msg1 snippets = 4;\r\n}";

            var lex = new Lexer(Text) { Matches = Helper.SplitText(Text) };

            Assert.IsTrue(lex.ParseOneOf());

            Assert.AreEqual(11, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(23, lex.Index);

            Assert.AreEqual(2, lex.Tokens[0].Position);
            Assert.AreEqual(8, lex.Tokens[1].Position);
            Assert.AreEqual(24, lex.Tokens[2].Position);
            Assert.AreEqual(31, lex.Tokens[3].Position);
            Assert.AreEqual(37, lex.Tokens[4].Position);
            Assert.AreEqual(43, lex.Tokens[5].Position);
            Assert.AreEqual(50, lex.Tokens[6].Position);
            Assert.AreEqual(58, lex.Tokens[7].Position);
            Assert.AreEqual(64, lex.Tokens[8].Position);
            Assert.AreEqual(69, lex.Tokens[9].Position);
            Assert.AreEqual(80, lex.Tokens[10].Position);

            Assert.AreEqual(5, lex.Tokens[0].Length);
            Assert.AreEqual(10, lex.Tokens[1].Length);
            Assert.AreEqual(6, lex.Tokens[2].Length);
            Assert.AreEqual(3, lex.Tokens[3].Length);
            Assert.AreEqual(1, lex.Tokens[4].Length);
            Assert.AreEqual(6, lex.Tokens[5].Length);
            Assert.AreEqual(5, lex.Tokens[6].Length);
            Assert.AreEqual(1, lex.Tokens[7].Length);
            Assert.AreEqual(4, lex.Tokens[8].Length);
            Assert.AreEqual(8, lex.Tokens[9].Length);
            Assert.AreEqual(1, lex.Tokens[10].Length);

            Assert.AreEqual(CodeType.Keyword, lex.Tokens[0].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[1].CodeType);
            Assert.AreEqual(CodeType.Keyword, lex.Tokens[2].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[3].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[4].CodeType);
            Assert.AreEqual(CodeType.Keyword, lex.Tokens[5].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[6].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[7].CodeType);
            Assert.AreEqual(CodeType.SymRef, lex.Tokens[8].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[9].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[10].CodeType);
        }
    }
}
