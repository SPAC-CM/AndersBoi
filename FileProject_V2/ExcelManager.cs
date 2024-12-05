using FileProject;
using OfficeOpenXml;

namespace FileProject;

public class ExcelManager
{
    /// <summary>
    /// This class handle all interaction with the XL file
    /// </summary>

    //File names
    public string GRI_dataPath;
    public string metaDataPath;

    //Rapport file
    private const string rapportName = "Rapport.xlsx";

    public ExcelManager(string GRI_path, string meta_path)
    {
	this.GRI_dataPath= GRI_path;
	this.metaDataPath = meta_path;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //Find XL dokuments with Links
        try
        {
            //Check for files
            if (!File.Exists(GRI_dataPath) || !File.Exists(metaDataPath))
            {
                Console.WriteLine("XL files not found");
                return;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine("XL not found");
	    return;
        }
    }

    /// <summary>
    /// Give Count of rows in GRI_2017_2020
    /// </summary>
    /// <returns>Number of rows</returns>
    public int GetNumberOfGRI_Rows()
    {
        using (var package = new ExcelPackage(GRI_dataPath))
        {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
	    if(worksheet is not null){
            	return worksheet.Rows.Count();
	    }
	    else{
		    return 0;
	    }
        }
    }


    /// <summary>
    /// Reads two indecies of specific row and from a given file
    /// </summary>
    /// <returns>Two indecies from the file<\returns>
    public URL_Data ReadRowFromExcel(string filePath, int rowIndex)
    {
        using (var package = new ExcelPackage(filePath))
        {
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
	    string BRNum = "";
	    string link = "";
	    bool non_empty_link = false;
	    if(worksheet is not null){
		    
		    //Finds BRNum
		    int BRNumIndex = 1;
		    BRNum = (worksheet.Cells[rowIndex, BRNumIndex].Value?.ToString());
		    
		    //Finds PDF_URL
		    int Pdf_URLIndex = 38;
		    ExcelRange Pdf_Url = worksheet.Cells[rowIndex, Pdf_URLIndex];

		    //If PDF_Url is not empty return it
		    if (Pdf_Url.Value != null && Pdf_Url.Value.ToString() != ""){
			    link = Pdf_Url.Value.ToString();
			    non_empty_link=true;
		    }
		    else
		    {
			//Gets an alternate link
			int cellAMIndex = 39;
			ExcelRange alt_link = worksheet.Cells[rowIndex, cellAMIndex];
			if (alt_link.Value != null && alt_link.Value.ToString() != ""){
				link = alt_link.Value.ToString();
				non_empty_link = true;
			}
		    }
	    }
	    return new URL_Data(BRNum,link,non_empty_link);
	}
    }

    /// <summary>
    /// Reads data from multipul rows in Xl.
    /// Returns what it finds
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="startRow"></param>
    /// <param name="endRow"></param>
    /// <returns>URL_Data in a list</returns>
    public async Task<List<URL_Data>> ReadMultipulRowsWithLinks(string filePath, int startRow, int endRow)
    {
        //Can't read a range where the start point is larger than the end point
        if(startRow > endRow)
        {
            Console.WriteLine("Reading mul link where start is bigger then end");
            return null;
        }
        List<URL_Data> values = new List<URL_Data>();
        
	//Gets all the links in the specified range
        for (int i = startRow; i <= endRow; i++)
        {
                //Avoid header
                if (i == 1)
                    values.Add(new URL_Data("", "", false));
                else
                {
                	values.Add(ReadRowFromExcel(filePath,i));
		}
        }
	return values;
    }

    /// <summary>
    /// Write a rapport if the result of the program to folder
    /// </summary>
    /// <param name="urlDataList">The dataset</param>
    public void WriteRapport(List<URL_Data> urlDataList)
    {
        // Get the current directory
        string currentDirectory = Directory.GetCurrentDirectory();

        // Create a file path relative to the current directory
        string filePath = Path.Combine(currentDirectory, rapportName);

        using (var package = new ExcelPackage())
        {
            // Create a new workbook
            var workbook = package.Workbook;

            // Create a new worksheet named "rapport"
            var worksheet = workbook.Worksheets.Add("rapport");

            //Count downloaded
            int sucDownload = 0;

            // Set up the header row
            worksheet.Cells[1, 1].Value = "BR_Number";
            worksheet.Cells[1, 2].Value = "Download Status";
            worksheet.Cells[1, 3].Value = "URL";

            // Start writing data from row 2
            int rowIndex = 2;

            foreach (var urlData in urlDataList)
            {
                worksheet.Cells[rowIndex, 1].Value = urlData.BR_Nummer;
                worksheet.Cells[rowIndex, 2].Value = urlData.validLink ? "Downloaded" : "Failed to download";
                worksheet.Cells[rowIndex, 3].Value = urlData.URL;

                if(urlData.validLink)
                    sucDownload++;

                rowIndex++;
            }

            // Resume of the data
            worksheet.Cells[1, 5].Value = "Succesful download";
            worksheet.Cells[2, 5].Value = $"{sucDownload} / {rowIndex}"; 

            // Save the workbook
            package.SaveAs(filePath);
        }
    }
}
