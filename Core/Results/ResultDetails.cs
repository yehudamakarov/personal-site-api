using System.ComponentModel;
using Core.Manager;

namespace Core.Results
{
    public class ResultDetails
    {
        public ResultStatus ResultStatus { get; set; }
        public string Message { get; set; }

        [Description(
            "If the owning Result is a piece of data that has changed, StaleEntity is an object containing: PreviousData,\nNextData,\nand PropertyName."
        )]
        public StaleEntity StaleEntity { get; set; }
    }
}