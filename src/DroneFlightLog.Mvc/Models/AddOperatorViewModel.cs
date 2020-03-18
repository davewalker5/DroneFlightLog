namespace DroneFlightLog.Mvc.Models
{
    public class AddOperatorViewModel : OperatorViewModelBase
    {
        public string Message { get; set; }

        public AddOperatorViewModel() : base()
        {
            Clear();
        }

        public override void Clear()
        {
            base.Clear();
            Message = "";
        }
    }
}
