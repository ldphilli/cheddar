
using Cheddar.Client.Services;

namespace Cheddar.Client.Services
{
  public interface IClockService
  {
    DateTime Now { get; }
    DateTime UtcNow { get; }
  }
  
  public class ClockService : IClockService
  {
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;

  }
}
