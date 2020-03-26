using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaTeXTableGenerator.LaTeX;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LaTeXTableGenerator.Test.LaTeX
{
    [TestClass]
    public class LaTeXTextEscapeTest
    {
        [TestMethod]
        public void Test_Escape_Assert_Correct_Escaped()
        {
            var candidates = new Dictionary<string, string>()
            {
                {"Example #1", @"Example \#1"},
                {@"C:\direct", @"C:\textbackslash{}direct"},
                { "Lorem ipsum", "Lorem ipsum" }
            };

            foreach (var candidate in candidates)
            {
                Assert.AreEqual(
                    candidate.Value, 
                    new LaTeXTextEscape().Escape(candidate.Key));
            }
        }
    }
}
