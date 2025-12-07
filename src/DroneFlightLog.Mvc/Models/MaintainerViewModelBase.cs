using DroneFlightLog.Mvc.Entities;

namespace DroneFlightLog.Mvc.Models
{
    public abstract class MaintainerViewModelBase
    {
        public Maintainer Maintainer { get; set; }
        public string Message { get; set; }

        public MaintainerViewModelBase()
        {
            Clear();
        }

        public virtual void Clear()
        {
            Maintainer = new Maintainer();
            Message = "";
        }
    }
}
