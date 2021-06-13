using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FoodShop.CognitiveServices.Domain;
using FoodShop.Domain;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FoodShop.CognitiveServices.Core
{
    public class NaturalLanguageUnderstandingService : INaturalLanguageUnderstandingService
    {
        private const string SLOT = "production";
        private const string INSTANCE = "$instance";

        private readonly IConfiguration _configuration;
        private readonly LUISRuntimeClient _luisClient;

        public NaturalLanguageUnderstandingService(IConfiguration configuration)
        {
            _configuration = configuration;

            var appKey = configuration.GetSection("Luis:appKey")?.Value;            
            var endpoint = configuration.GetSection("Luis:endpoint")?.Value;

            _luisClient = new LUISRuntimeClient(new ApiKeyServiceClientCredentials(appKey)) { Endpoint = endpoint };

        }

        public async Task<RecognitionContext> RecognizeAsync(RecognitionRequest recognitionRequest)
        {
            var predictionRequest = new PredictionRequest
            {
                Query = recognitionRequest.ActivityText,
                Options = new PredictionRequestOptions
                {
                    DatetimeReference = DateTime.Now,
                    PreferExternalEntities = true
                }
            };

            var appId = _configuration.GetSection("Luis:appId")?.Value;
            var prediction = await _luisClient.Prediction.GetSlotPredictionAsync(Guid.Parse(appId), SLOT, predictionRequest, true, true, true);

            return new RecognitionContext
            {
                TopScoringIntent = GetTopIntent(prediction.Prediction),
                Entities = GetEntities(prediction.Prediction.Entities)
            };
        }

        public IntentProperty GetTopIntent(Prediction prediction)
        {
            return new IntentProperty()
            {
                Name = prediction.TopIntent,
                Score = prediction.Intents[prediction.TopIntent].Score.Value
            };
        }

        private List<EntityProperty> GetEntities(IDictionary<string, object> entities)
        {
            var resultEntities = new List<EntityProperty>();

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    if (entity.Key == INSTANCE)
                        continue;

                    var entityValue = entity.Value.ToString();

                    switch (entity.Key)
                    {
                        case EntityTypes.Pizza:
                        case EntityTypes.Burger:
                        case EntityTypes.Drinks:
                            {
                                var resolutions = JsonConvert.DeserializeObject<List<List<string>>>(entityValue);

                                foreach (var resolution in resolutions)
                                {
                                    resultEntities.Add(new EntityProperty { Type = entity.Key, Resolution = new ListResolution { Values = resolution.ToList() } });
                                }
                            }
                            break;
                        case EntityTypes.Money:
                            {
                                var numberWithUnitsModels = JsonConvert.DeserializeObject<List<MoneyModel>>(entityValue);

                                foreach (var numberWithUnitsModel in numberWithUnitsModels)
                                {
                                    if (numberWithUnitsModel != null && numberWithUnitsModel.Units == "Cent")
                                    {
                                        numberWithUnitsModel.Units = "Dollar";
                                        numberWithUnitsModel.Number /= 100;
                                    }

                                    resultEntities.Add(new EntityProperty { Type = EntityTypes.Money, Resolution = numberWithUnitsModel });
                                }

                                break;
                            }

                        case EntityTypes.DateTime:
                            {
                                var dateTimeModels = JsonConvert.DeserializeObject<List<DateTimeModel>>(entityValue);
                                foreach (var dateTimeModel in dateTimeModels)
                                {
                                    var dateTimeResolution = new DateTimeResolution { Values = dateTimeModel.Values.Select(x => new DateTimeResolutionEntity { Value = x.Resolution.First().Value, Timex = x.Timex, Start = x.Resolution.First().Start }).ToList() };

                                    resultEntities.Add(new EntityProperty { Type = EntityTypes.DateTime, Resolution = dateTimeResolution });
                                }

                                break;
                            }
                        case EntityTypes.Size:
                            {
                                var resolutions = JsonConvert.DeserializeObject<List<List<string>>>(entityValue);

                                foreach (var resolution in resolutions)
                                {
                                    resultEntities.Add(new EntityProperty { Type = entity.Key, Resolution = new ListResolution { Values = resolution.ToList() } });
                                }
                                break;
                            }
                        default:
                            {
                                resultEntities.Add(new EntityProperty { Type = entity.Key });
                                break;
                            }
                    }
                }
            }

            return resultEntities;
        }

    }
}
