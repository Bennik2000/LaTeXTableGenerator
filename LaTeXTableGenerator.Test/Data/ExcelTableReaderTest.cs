using LaTeXTableGenerator.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LaTeXTableGenerator.Test.Data
{
    [TestClass]
    public class ExcelTableReaderTest
    {
        [TestMethod]
        public void Test_LoadTable_Assert_CorrectValues()
        {
            var reader = new ExcelTableReader(@"TestResources\ValidTableExcel.xlsx");
            var table = reader.ReadTable().Result;

            Assert.AreEqual("Lorem", table.Rows[0].Cells[0].Text);
            Assert.AreEqual("ipsum", table.Rows[0].Cells[1].Text);
            Assert.AreEqual("dolor", table.Rows[0].Cells[2].Text);

            Assert.AreEqual("sit", table.Rows[1].Cells[0].Text);
            Assert.AreEqual("amet", table.Rows[1].Cells[1].Text);
            Assert.AreEqual("consetetur", table.Rows[1].Cells[2].Text);

            Assert.AreEqual("sadipscing", table.Rows[2].Cells[0].Text);
            Assert.AreEqual("elitr", table.Rows[2].Cells[1].Text);
            Assert.AreEqual("sed", table.Rows[2].Cells[2].Text);

            Assert.AreEqual("diam", table.Rows[3].Cells[0].Text);
            Assert.AreEqual("nonumy", table.Rows[3].Cells[1].Text);
            Assert.AreEqual("eirmod", table.Rows[3].Cells[2].Text);
        }

        [TestMethod]
        public void Test_LoadTable_Assert_CorrectFormat()
        {
            var reader = new ExcelTableReader(@"TestResources\ValidTableExcel.xlsx");
            var table = reader.ReadTable().Result;

            Assert.IsTrue(table.Rows[0].Cells[0].IsBold);
            Assert.IsTrue(table.Rows[0].Cells[1].IsBold);
            Assert.IsTrue(table.Rows[0].Cells[2].IsBold);

            Assert.IsTrue(table.Rows[0].Cells[0].IsItalic);
            Assert.IsTrue(table.Rows[0].Cells[1].IsItalic);
            Assert.IsTrue(table.Rows[0].Cells[2].IsItalic);

            Assert.IsFalse(table.Rows[1].Cells[0].IsBold);
            Assert.IsFalse(table.Rows[1].Cells[1].IsBold);
            Assert.IsFalse(table.Rows[1].Cells[2].IsBold);

            Assert.IsFalse(table.Rows[1].Cells[0].IsItalic);
            Assert.IsFalse(table.Rows[1].Cells[1].IsItalic);
            Assert.IsFalse(table.Rows[1].Cells[2].IsItalic);
        }
    }
}
