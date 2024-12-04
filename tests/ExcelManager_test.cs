using NUnit.Framework;
using FileProject;

namespace ExcelManager_unitTests
{
	[TestFixture]
	public class Test_ExcelManager{

		private ExcelManager manager;
		private string GRI_Path = System.IO.Path.Combine({"..","..","..","GRI_2017_2020.xlsx"});
		private string Meta_Path = System.IO.Path.Combine({"..","..","..","Metadata2006_2016.xlsx"});
		
		[SetUp]
		public void SetUp(){
			manager = new ExcelManager(GRI_Path,Meta_Path);
		}

		[Test]
		public void Rows_Should_Be_21058(){
			var result = manager.GetNumberOfGRI_Rows();
			var actual = 21058;
			Assert.That(result,Is.EqualTo(actual));
		}

		[Test]
		public void ReadRowFromExcel_Should_Be_X(){
			var result = manager.ReadRowFromExcel(GRI_Path,1);
			var actual1 = "BRnum";
			var actual2 = "Pdf_URL";
			Assert.That(result.BR_Nummer,Is.EqualTo(actual1));
			Assert.That(result.URL,Is.EqualTo(actual2));
			Assert.That(result.validLink,Is.EqualTo(true));
		}

		[Test]
		public async Task ReadMultipleRows_Should_Read_Two_Success(){
			var result = await manager.ReadMultipulRowsWithLinks(GRI_Path,1,3);

			Assert.That(result[0].validLink,Is.EqualTo(false));
			Assert.That(result[1].validLink,Is.EqualTo(true));
			Assert.That(result[2].validLink,Is.EqualTo(true));
		}
	}
}
