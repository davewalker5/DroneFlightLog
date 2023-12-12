namespace DroneFlightLog.Importer.Entities
{
    public class CsvReaderError
    {
        public int Record { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}
