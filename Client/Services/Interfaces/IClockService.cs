
namespace Cheddar.Client.Services.Interfaces
{
  public interface IClockService
  {
    DateTime Now { get; }
    DateTime UtcNow { get; }
  }
}
