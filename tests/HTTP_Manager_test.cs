using NUnit.Framework;
using FileProject;

namespace HTTP_Manager_Unittest
{
	[TestFixture]
	public class Test_HTTP_Manager{

		private HTTP_Manager manager;
		string[] paths = {"..","..","..","downloads"};
		private string path;
		[SetUp]
		public void SetUp(){
			path = System.IO.Path.Combine(paths);
			manager = new HTTP_Manager();
		}

		[Test]
		public async Task Should_Download_One(){
			URL_Data test_data = new URL_Data("test","http://cdn12.a1.net/m/resources/media/pdf/A1-Umwelterkl-rung-2016-2017.pdf",true);
			var result = await manager.DownloadFileAsync(test_data,1,path);
			Assert.That(result.Item2, Is.EqualTo(true));
			string[] files =  System.IO.Directory.GetFiles(path);
			string test_path = System.IO.Path.Combine(new string[] {path,"test.pdf"});
			Assert.That(files[0],Is.EqualTo(test_path));
		}

		[TearDown]
		public void TearDown(){
			System.IO.Directory.Delete(path,true);
		}

	}
}
