using System.Threading.Tasks;
using ED_Router.Model;

namespace ED_Router.Services
{
    public interface ICsvManager
    {
        Task<FlightPlan> ImportCsv(string filePath);
    }
}
