using EggProductionProject_MVC.Models;
using System.ComponentModel.DataAnnotations;

namespace EggProductionProject_MVC.ViewModels
{
    public class EggProductionReport
    {
        public string Title { get; set; }
        public List<object> Rate { get; set; }

        public EggProductionReport()
        {
            Rate = new List<object>();
        }
    }
}
