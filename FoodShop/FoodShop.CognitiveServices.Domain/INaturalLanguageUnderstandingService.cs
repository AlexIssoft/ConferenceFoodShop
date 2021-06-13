using FoodShop.CognitiveServices.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FoodShop.Domain
{
    public interface INaturalLanguageUnderstandingService
    {
        Task<RecognitionContext> RecognizeAsync(RecognitionRequest recognitionRequest);
    }
}
