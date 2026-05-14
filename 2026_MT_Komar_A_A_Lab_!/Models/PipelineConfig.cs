using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Models
{
    public class PipelineConfig
    {
        [JsonPropertyName("pipeline")]
        public Collection<PipelineItem> Pipeline { get; } =
            [];
    }
}
