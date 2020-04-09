using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LaTeXTableGenerator.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LaTeXTableGenerator.Test.Data
{
    [TestClass]
    public class LaTeXTableReaderTest
    {
        [TestMethod]
        public void Test_LoadTable_Assert_CorrectValues()
        {
            var reader = new LaTeXTableReader(@"TestResources\ValidTableLaTeX.tex");

            var table = reader.ReadTable().Result;

            Assert.IsNotNull(table);
        }

        [TestMethod]
        public void Test_ReadValidTable_Assert_CorrectValues()
        {
            var reader = new LaTeXTableReader(@"TestResources\ValidTableLaTeX.tex");

            var table = reader.ReadTable().Result;

            Assert.AreEqual("Lorem ipsum", table.Rows[0].Cells[0].Text);
            Assert.AreEqual("dolor", table.Rows[0].Cells[1].Text);
            Assert.AreEqual("sit", table.Rows[0].Cells[2].Text);

            Assert.AreEqual("amet", table.Rows[1].Cells[0].Text);
            Assert.AreEqual("consetetur", table.Rows[1].Cells[1].Text);
            Assert.AreEqual("sadipscing", table.Rows[1].Cells[2].Text);

            Assert.AreEqual("elitr", table.Rows[2].Cells[0].Text);
            Assert.AreEqual("sed", table.Rows[2].Cells[1].Text);
            Assert.AreEqual("diam", table.Rows[2].Cells[2].Text);

            Assert.AreEqual("nonumy", table.Rows[3].Cells[0].Text);
            Assert.AreEqual("eirmod", table.Rows[3].Cells[1].Text);
            Assert.AreEqual("tempor", table.Rows[3].Cells[2].Text);
        }
    }
}
