using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FoodShop.CognitiveServices.Domain
{
    public class RecognitionContext
    {
        public IntentProperty TopScoringIntent { get; set; }

        public List<EntityProperty> Entities { get; set; }

        public List<string> GetEntities(string name)
        {
            return Entities.Where(x => x.Type.Equals(name, StringComparison.InvariantCultureIgnoreCase)).SelectMany(x => (x.Resolution as ListResolution)?.Values.Select(v => v)).ToList();            
        }

        public List<T> GetEntities<T>(string name) where T: struct, Enum
        {
            return Entities.Where(x => x.Type.Equals(name, StringComparison.InvariantCultureIgnoreCase)).SelectMany(x => (x.Resolution as ListResolution)?.Values.Select(v => 
            {
                Enum.TryParse<T>(v, true, out T entity);
                return entity;
            }
            )).ToList();
        }
    }
}
