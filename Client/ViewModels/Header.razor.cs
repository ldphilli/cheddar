using Cheddar.Shared.Models;
using Cheddar.Client.ViewModels;
using Cheddar.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Json;

namespace Cheddar.Client.ViewModels {
    public class HeaderViewModel {

        public string timeOfDayAsText { get; set; }

        public HeaderViewModel() {
            ConvertTimeOfDayToText();
        }

        public void ConvertTimeOfDayToText() {
            
            int hour = DateTime.Now.Hour;
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