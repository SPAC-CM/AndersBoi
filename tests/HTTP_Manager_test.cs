using NUnit.Framework;
using FileProject;

namespace HTTP_Manager_Unittest
{
	[TestFixture]
	public class Test_HTTP_Manager{

		private HTTP_Manager manager;
		[SetUp]
		public void SetUp(){
			manager = new HTTP_Manager();
		}

		[Test]
		public async Task Should_Download_One(){
			string download_path = @"..\..\..\downloads";
			URL_Data test_data = new URL_Data("test","http://cdn12.a1.net/m/resources/media/pdf/A1-Umwelterkl-rung-2016-2017.pdf",true);
			var result = await manager.DownloadFileAsync(test_data,1,download_path);
			Assert.That(result.Item2, Is.EqualTo(true));
			string[] files =  System.IO.Directory.GetFiles(download_path);
			Assert.That(files[0],Is.EqualTo(@"..\..\..\downloads\test.pdf"));
		}

		[TearDown]
		public void TearDown(){
			System.IO.Directory.Delete(@"..\..\..\downloads",true);
		}

	}
}
