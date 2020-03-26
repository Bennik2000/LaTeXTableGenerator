using System.Diagnostics;
using LaTeXTableGenerator.LaTeX;
using LaTeXTableGenerator.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LaTeXTableGenerator.Test.LaTeX
{
    [TestClass]
    public class LaTeXOutputGeneratorTest
    {
        [TestMethod]
        public void Test_GenerateTable()
        {
            var generator = new LaTeXOutputGenerator();

            var str = generator.GenerateTable(new Table(10, 4));

            Debug.WriteLine(str);
        }
    }
}
