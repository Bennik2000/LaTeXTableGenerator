using LaTeXTableGenerator.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
            var testFiles = new List<string>()
            {
                @"TestResources\ValidTableLaTeX_1.tex",
                @"TestResources\ValidTableLaTeX_2.tex",
                @"TestResources\ValidTableLaTeX_3.tex",
            };

            foreach (var testFile in testFiles)
            {
                Console.WriteLine($"Testing file {testFile}");

                var reader = new LaTeXTableReader(testFile);

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

                Console.WriteLine("-> OK");
            }

        }

        [TestMethod]
        public void Test_ReadValidTable_Assert_CorrectBoldItalic()
        {
            var reader = new LaTeXTableReader(@"TestResources\ValidTableLaTeX_4_bold_italic.tex");

            var table = reader.ReadTable().Result;


            Assert.AreEqual("Lorem ipsum", table.Rows[0].Cells[0].Text);
            Assert.IsFalse(table.Rows[0].Cells[0].IsItalic);
            Assert.IsTrue(table.Rows[0].Cells[0].IsBold);

            Assert.AreEqual("dolor", table.Rows[0].Cells[1].Text);
            Assert.IsTrue(table.Rows[0].Cells[1].IsItalic);
            Assert.IsTrue(table.Rows[0].Cells[1].IsBold);

            Assert.AreEqual("sit", table.Rows[0].Cells[2].Text);
            Assert.IsTrue(table.Rows[0].Cells[2].IsItalic);
            Assert.IsFalse(table.Rows[0].Cells[2].IsBold);

        }
    }
}
