using Cheddar.Client.Services.Interfaces;

namespace Cheddar.Client.ViewModels
{
  public class HeaderViewModel
  {
    private readonly IClockService clock;

    public string timeOfDayAsText { get; private set; } = "";

    public HeaderViewModel(IClockService clock)
    {
      this.clock = clock;
      ConvertTimeOfDayToText();
    }

    public void ConvertTimeOfDayToText()
    {
      int hour = clock.Now.Hour;
      
      if (hour >= 1 && hour <= 11)
      {
        timeOfDayAsText = "Good morning";
      }
      else if (hour >= 12 && hour <= 17)
      {
        timeOfDayAsText = "Good afternoon";
      }
      else
      {
        timeOfDayAsText = "Good evening";
      }
    }
  }
}