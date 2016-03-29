using System.IO;
using System.Linq;
using Irony.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AgidIronyParser.Tests
{
    [TestClass]
    public class AgidTests
    {
        [TestMethod]
        public void ReadAgid()
        {
            var grammar = new InflGrammer();
            var numLines = 0;

            foreach (var root in File.ReadLines("infl.txt").Select(line => GetRoot(line, grammar)))
            {
                numLines++;
                ValidateNodes(root);
            }

            Assert.AreEqual(112505, numLines);
        }

        /// <summary>
        /// Recursively validate nodes.
        /// </summary>
        private static void ValidateNodes(ParseTreeNode node)
        {
            Assert.IsNotNull(node);
            Assert.IsFalse(node.IsError);

            foreach (var child in node.ChildNodes)
            {
                ValidateNodes(child);
            }
        }

        private static ParseTreeNode GetRoot(string sourceCode, Grammar grammar)
        {
            var language = new LanguageData(grammar);
            var parser = new Parser(language);
            var parseTree = parser.Parse(sourceCode);
            return parseTree.Root;
        }
    }
}
