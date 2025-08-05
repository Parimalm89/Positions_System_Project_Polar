

List<TradePositionModel> posModels = new List<TradePositionModel>();
List<TradeTickModel> tickModels = new List<TradeTickModel>();

bool isStop = false;

Thread thread = new Thread(keyboardevent);
thread.IsBackground = true;
thread.Start();

while (true)
{
    if (!isStop)
    {
        Console.WriteLine("Process started.");
        checkandProcessFile();
    }
    else
    {
        Console.WriteLine("Process stopped.");
    }

    Thread.Sleep(5000);
}


void checkandProcessFile()
{
    if (!isStop)
    {
        readfile();
    }
    else
    {
        posModels.Clear();
        tickModels.Clear();
        Console.Clear();
        Console.WriteLine("Stop Process");
    }
}
void keyboardevent()
{
    while (true)
    {
        if (Console.KeyAvailable)
        {
            if (Console.ReadKey(true).Key == ConsoleKey.Spacebar)
            {
                isStop = !isStop;
                Console.Clear();
            }
        }
        Thread.Sleep(1000);
    }

}
void readfile()
{
   
    string dirPath = @"D:\TestApp";
    var files = Directory.GetFiles(dirPath);

    foreach (var file in files)
    {
        if (File.Exists(file))
        {
            using (var readFile = new StreamReader(file))
            {
                if (readFile != null)
                {
                    while (!readFile.EndOfStream)
                    {
                        if (!isStop)
                        {
                            string data = readFile.ReadLine();
                            if (!string.IsNullOrEmpty(data))
                            {
                                string[] arry = data.Split(',');
                                if (arry.Length > 2)
                                {
                                    if (Path.GetFileName(file).ToLower().Contains("positions"))
                                    {
                                        TradePositionModel posModel = new TradePositionModel();
                                        posModel.Symbol = arry[0];
                                        posModel.position = Convert.ToInt64(arry[1]);
                                        posModel.Tick = Convert.ToDateTime(arry[2]);
                                        posModels.Add(posModel);
                                    }
                                    else if (Path.GetFileName(file).ToLower().Contains("ticks"))
                                    {
                                        TradeTickModel tickModel = new TradeTickModel();
                                        tickModel.Symbol = arry[0];
                                        tickModel.Price = Convert.ToDecimal(arry[1]);
                                        tickModel.Tick = Convert.ToDateTime(arry[2]);
                                        tickModels.Add(tickModel);
                                    }
                                }
                            }
                        }

                    }
                }
            }
               
        }
    }

    if (posModels.Count > 0 && tickModels.Count > 0)
    {

        DateTime? latestTime = null;
        var postMaxTime = posModels.Max(x => x.Tick);
        var tickMaxtime = tickModels.Max(x => x.Tick);

        if (postMaxTime >= tickMaxtime)
            latestTime = postMaxTime;
        else
            latestTime = tickMaxtime;


        var latestPost = posModels.Where(x => Convert.ToDateTime(x.Tick).ToLongTimeString() == Convert.ToDateTime(latestTime).ToLongTimeString()).ToList();
        var latestTick = tickModels.Where(x => Convert.ToDateTime(x.Tick).ToLongTimeString() == Convert.ToDateTime(latestTime).ToLongTimeString()).ToList();

        foreach (var pos in latestPost.ToList())
        {
            if (!isStop)
            {
                var tickItems = latestTick.Where(x => x.Symbol == pos.Symbol);
                if (tickItems.Any())
                {
                    foreach (var item in tickItems.ToList())
                    {
                        string op = item.Symbol + "Position:" + pos.position + "Price:" + item.Price;
                        Console.WriteLine(op);
                    }
                }
            }
        }
    }
}
public class TradePositionModel
{
    public string? Symbol { get; set; }
    public DateTime? Tick { get; set; }
    public long position { get; set; }
}
public class TradeTickModel
{
    public string? Symbol { get; set; }
    public decimal Price { get; set; }
    public DateTime? Tick { get; set; }
}