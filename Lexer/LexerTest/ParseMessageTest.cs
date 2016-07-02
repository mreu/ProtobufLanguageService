#region Copyright © 2014 Michael Reukauff
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseMessageTest.cs" company="Michael Reukauff">
//   Copyright © 2014 Michael Reukauff
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
#endregion

namespace MichaelReukauff.LexerTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Lexer;

    [TestClass]
    public class ParseMessageTest
    {
        [TestMethod]
        public void ParseMessage_OK01()
        {
            const string text = "  message SearchResponse {\r\n  required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(14, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(26, lex.Index);

            Assert.AreEqual(2, lex.Tokens[0].Position);
            Assert.AreEqual(10, lex.Tokens[1].Position);
            Assert.AreEqual(30, lex.Tokens[2].Position);
            Assert.AreEqual(39, lex.Tokens[3].Position);
            Assert.AreEqual(46, lex.Tokens[4].Position);
            Assert.AreEqual(52, lex.Tokens[5].Position);
            Assert.AreEqual(58, lex.Tokens[6].Position);
            Assert.AreEqual(67, lex.Tokens[7].Position);
            Assert.AreEqual(74, lex.Tokens[8].Position);
            Assert.AreEqual(82, lex.Tokens[9].Position);
            Assert.AreEqual(88, lex.Tokens[10].Position);
            Assert.AreEqual(97, lex.Tokens[11].Position);
            Assert.AreEqual(104, lex.Tokens[12].Position);
            Assert.AreEqual(115, lex.Tokens[13].Position);

            Assert.AreEqual(7, lex.Tokens[0].Length);
            Assert.AreEqual(14, lex.Tokens[1].Length);
            Assert.AreEqual(8, lex.Tokens[2].Length);
            Assert.AreEqual(6, lex.Tokens[3].Length);
            Assert.AreEqual(3, lex.Tokens[4].Length);
            Assert.AreEqual(1, lex.Tokens[5].Length);
            Assert.AreEqual(8, lex.Tokens[6].Length);
            Assert.AreEqual(6, lex.Tokens[7].Length);
            Assert.AreEqual(5, lex.Tokens[8].Length);
            Assert.AreEqual(1, lex.Tokens[9].Length);
            Assert.AreEqual(8, lex.Tokens[10].Length);
            Assert.AreEqual(6, lex.Tokens[11].Length);
            Assert.AreEqual(8, lex.Tokens[12].Length);
            Assert.AreEqual(1, lex.Tokens[13].Length);

            Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[1].CodeType);
            Assert.AreEqual(CodeType.FieldRule, lex.Tokens[2].CodeType);
            Assert.AreEqual(CodeType.Keyword, lex.Tokens[3].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[4].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[5].CodeType);
            Assert.AreEqual(CodeType.FieldRule, lex.Tokens[6].CodeType);
            Assert.AreEqual(CodeType.Keyword, lex.Tokens[7].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[8].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[9].CodeType);
            Assert.AreEqual(CodeType.FieldRule, lex.Tokens[10].CodeType);
            Assert.AreEqual(CodeType.Keyword, lex.Tokens[11].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[12].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[13].CodeType);
        }

        [TestMethod]
        public void ParseMessage_OK02()
        {
            const string text = "  message SearchResponse {\r\n  required string url = 2;\r\n \r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(14, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(27, lex.Index);

            Assert.AreEqual(2, lex.Tokens[0].Position);
            Assert.AreEqual(10, lex.Tokens[1].Position);
            Assert.AreEqual(30, lex.Tokens[2].Position);
            Assert.AreEqual(39, lex.Tokens[3].Position);
            Assert.AreEqual(46, lex.Tokens[4].Position);
            Assert.AreEqual(52, lex.Tokens[5].Position);
            Assert.AreEqual(61, lex.Tokens[6].Position);
            Assert.AreEqual(70, lex.Tokens[7].Position);
            Assert.AreEqual(77, lex.Tokens[8].Position);
            Assert.AreEqual(85, lex.Tokens[9].Position);
            Assert.AreEqual(91, lex.Tokens[10].Position);
            Assert.AreEqual(100, lex.Tokens[11].Position);
            Assert.AreEqual(107, lex.Tokens[12].Position);
            Assert.AreEqual(118, lex.Tokens[13].Position);

            Assert.AreEqual(7, lex.Tokens[0].Length);
            Assert.AreEqual(14, lex.Tokens[1].Length);
            Assert.AreEqual(8, lex.Tokens[2].Length);
            Assert.AreEqual(6, lex.Tokens[3].Length);
            Assert.AreEqual(3, lex.Tokens[4].Length);
            Assert.AreEqual(1, lex.Tokens[5].Length);
            Assert.AreEqual(8, lex.Tokens[6].Length);
            Assert.AreEqual(6, lex.Tokens[7].Length);
            Assert.AreEqual(5, lex.Tokens[8].Length);
            Assert.AreEqual(1, lex.Tokens[9].Length);
            Assert.AreEqual(8, lex.Tokens[10].Length);
            Assert.AreEqual(6, lex.Tokens[11].Length);
            Assert.AreEqual(8, lex.Tokens[12].Length);
            Assert.AreEqual(1, lex.Tokens[13].Length);

            Assert.AreEqual(CodeType.TopLevelCmd, lex.Tokens[0].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[1].CodeType);
            Assert.AreEqual(CodeType.FieldRule, lex.Tokens[2].CodeType);
            Assert.AreEqual(CodeType.Keyword, lex.Tokens[3].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[4].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[5].CodeType);
            Assert.AreEqual(CodeType.FieldRule, lex.Tokens[6].CodeType);
            Assert.AreEqual(CodeType.Keyword, lex.Tokens[7].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[8].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[9].CodeType);
            Assert.AreEqual(CodeType.FieldRule, lex.Tokens[10].CodeType);
            Assert.AreEqual(CodeType.Keyword, lex.Tokens[11].CodeType);
            Assert.AreEqual(CodeType.SymDef, lex.Tokens[12].CodeType);
            Assert.AreEqual(CodeType.Number, lex.Tokens[13].CodeType);
        }

        [TestMethod]
        public void ParseMessage_OK03()
        {
            const string text = "  message SearchResponse {}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(4, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_OK04()
        {
            const string text = "  message SearchResponse {\r\n option abc = true; \r\n required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(17, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(32, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_OK05()
        {
            const string text = "  message SearchResponse {\r\n enum abc { a = 1; b=2;} \r\n required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(20, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(39, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_OK06()
        {
            const string text = "  message SearchResponse {\r\n extensions 100 to 199; \r\n required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(18, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(32, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_OK07()
        {
            const string text = "  message SearchResponse {\r\n extend msg {  optional int32 t1 = 1;} \r\n required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(20, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(37, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_OK08()
        {
            const string text = "  message SearchResponse {\r\n message msg {  optional int32 t1 = 1;} \r\n required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(20, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(37, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_OK09()
        {
            const string text = "  message SearchResponse {\r\n message msg {  optional int32 t1 = 1;} \r\n required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n};";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(20, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(38, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_OK10()
        {
            const string text = "  message SearchResponse {\r\n message msg {  optional int32 t1 = 1;} \r\n required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;\r\n  oneof oneof_name {\r\n  string url = 2;\r\n  string title = 3;\r\n  msg1 snippets = 4;\r\n}};";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(true));

            Assert.AreEqual(31, lex.Tokens.Count);
            Assert.AreEqual(0, lex.Errors.Count);
            Assert.AreEqual(61, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK01()
        {
            const string text = "  message SearchResponse {\r\n  required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseMessage(false));

            Assert.AreEqual(14, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(24, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK02()
        {
            const string text = "  message";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseMessage(false));

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(1, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK03()
        {
            const string text = "  message 1SearchResponse {\r\n  required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseMessage(false));

            Assert.AreEqual(1, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(1, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK04()
        {
            const string text = "  message SearchResponse";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseMessage(false));

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(2, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK05()
        {
            const string text = "  message SearchResponse x\r\n  required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseMessage(false));

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(2, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK06()
        {
            const string text = "  message SearchResponse {";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseMessage(false));

            Assert.AreEqual(2, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(3, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK07()
        {
            const string text = "  message SearchResponse {\r\n  required string url = 2;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsFalse(lex.ParseMessage(false));

            Assert.AreEqual(14, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(24, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK08()
        {
            const string text = "  message SearchResponse {\r\n  required string url ;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(false));

            Assert.AreEqual(13, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(23, lex.Index);
        }

        [TestMethod]
        public void ParseMessage_NOK09()
        {
            const string text = "  message SearchResponse {\r\n\r\n  required string url ;\r\n  optional string title = 3;\r\n  repeated string snippets = 4;}";

            var lex = new Lexer(text) { Matches = Helper.SplitText(text) };

            Assert.IsTrue(lex.ParseMessage(false));

            Assert.AreEqual(13, lex.Tokens.Count);
            Assert.AreEqual(1, lex.Errors.Count);
            Assert.AreEqual(24, lex.Index);
        }
    }
}
