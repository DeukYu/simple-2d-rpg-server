namespace ServerCore.WAL;

[Serializable]
public class WalEntry
{
    public long TransactionId { get; set; }
    public string Operation { get; set; }   // ex) Update, Insert, Delete..
    public string Data { get; set; }        // ex) { "Name": "John", "Age": 33 }
    public DateTime Timestamp { get; set; }  = DateTime.Now;

}
