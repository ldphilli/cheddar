
using Cheddar.Client.Services.Interfaces;

namespace Cheddar.Client.Services
{
  public class ClockService : IClockService
  {
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;

  }
}
