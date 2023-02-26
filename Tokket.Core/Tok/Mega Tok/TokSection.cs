//------------------------------------------------------------
// Copyright (c) Tokket Corporation.  All rights reserved.
//------------------------------------------------------------
namespace Tokket.Core
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>Used in the Mega Tok format. Content can be no longer than 150,000 characters.</summary>
    public class TokSection : BaseModel
    {
        /// <summary>Type of document.</summary>
        [JsonRequired]
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; } = "toksection";

        /// <summary>The user's id.</summary>
        [JsonProperty(PropertyName = "user_id")]
        public string UserId​ { get; set; }

        /// <summary>The tok's id.</summary>
        [JsonProperty(PropertyName = "tok_id")]
        public string TokId​ { get; set; }

        /// <summary>The tok type's id.</summary>
        [JsonProperty(PropertyName = "tok_type_id")]
        public string TokTypeId​ { get; set; }

        /// <summary>The section's title.</summary>
        [JsonProperty(PropertyName = "title")]
        public string Title​ { get; set; }

        /// <summary>The section's image url​.</summary>
        [JsonProperty(PropertyName = "image")]
        public string Image​ { get; set; }

        /// <summary>The section's text content​.</summary>
        [MaxLength(150000)]
        [JsonProperty(PropertyName = "content")]
        public string Content​ { get; set; }

        /// <summary>Length of the section's text content​. Content can be no longer than 150,000 characters.​</summary>
        [JsonProperty(PropertyName = "section_length")]
        public int SectionLength { get; set; }

        /// <summary>The section's number id. Once the section is added ever changes even if a previous section is deleted.​</summary>
        [JsonProperty(PropertyName = "section_number")]
        public int SectionNumber { get; set; }

        [JsonProperty(PropertyName = "question_answer")]
        public List<Qna> QuestionAnswer { get; set; }
    }
}
