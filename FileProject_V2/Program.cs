using FileProject;




int threadDevider = 1000; //A magic number for have many rows i want in a thread

try
{
    //Setup for functions
    ExcelManager excelManager = new ExcelManager();
    HTTP_Manager httpManager = new HTTP_Manager();

    //Find rows
    int rowCount = excelManager.GetNumberOfGRI_Rows();
    //Download 20
    rowCount = 20;

    //Divede rows onto threads
    int numberOfThreads = 1;
    if(rowCount > threadDevider)
        numberOfThreads = rowCount / threadDevider;
    var collectedData = new List<URL_Data> ();


    //Deviede rows into chunks
    List<(int, int)> rowChunks = new List<(int, int)> ();
    for (int i = 0; i < numberOfThreads; i++)
    {
        int endValue = (i + 1) * threadDevider;
        if(endValue > rowCount) //Stop its from having more them max rows
            endValue = rowCount;

        rowChunks.Add((i * threadDevider + 1, endValue));
    }

    Console.WriteLine("Find links to data");

    //Devide tasks
    var tasks = new List<Task<List<URL_Data>>>();
    for (int i = 0; i < rowChunks.Count; i++)
    {
        tasks.Add(excelManager.ReadMultipulRowsWithLinks(ExcelManager.GRI_dataPath, rowChunks[i].Item1, rowChunks[i].Item2));
    }

    //Wait for all rows to be read
    var taskResult = await Task.WhenAll(tasks);
    Console.WriteLine("Download found");

    //Collect results
    foreach (var item in taskResult)
    {
        collectedData.AddRange(item);
    }

    // Find bad results
    for (global::System.Int32 i = 0; i < collectedData.Count; i++)
    {
        if (collectedData[i].BR_Nummer == string.Empty)
            collectedData[i].validLink = false;
        else if(collectedData[i].URL == string.Empty)
            collectedData[i].validLink = false;
    }

    // Download
    // Devide into task
    // Can use rowChunks again because the lenght of the list should be the same lenght.    
    Console.WriteLine("Download data");
    var downloadTasks = new List<Task<(int, bool)>>();
    for (global::System.Int32 i = 0; i < collectedData.Count; i++)
    {
        if (collectedData[i].validLink)
        {
            downloadTasks.Add(httpManager.DownloadFileAsync(collectedData[i], i));
        }
    }
    var downloadResult = await Task.WhenAll(downloadTasks);

    //Update data with download result
    foreach ((int, bool) item in downloadResult)
    {
        collectedData[item.Item1].validLink = item.Item2;
    }

    //Updata download status in colleced data
    Console.WriteLine("Give rapport");
    excelManager.WriteRapport(collectedData);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

Console.WriteLine("Jobs done. Press any key to end...");
Console.ReadKey();

