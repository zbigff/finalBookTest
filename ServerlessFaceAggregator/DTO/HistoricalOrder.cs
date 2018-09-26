using System.Collections.Generic;

namespace ServerlessFaceAggregator.DTO
{
    public class HistoricalOrder
    {
        public RecognitionOrder RecognitionOrder { get; set; }
        public IEnumerable<string> RecognizedFiles { get; set; }
    }
}
