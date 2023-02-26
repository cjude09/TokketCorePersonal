using System;
using System.Collections.Generic;
using System.Text;

namespace Tokket.Shared.Models
{
    public class CommonModel
    {
        /// <summary>
        ///     Any Id based on identified model
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        ///     Important! Original Label of the model to identify
        /// </summary>
        public string LabelIdentifier { get; set; }
        /// <summary>
        ///     The unique identifier for the model such as the partition key
        /// </summary>
        public string UniqueId { get; set; }
        /// <summary>
        ///     A Key that is unique to the model
        /// </summary>
        public string UniqueKey { get; set; }
        /// <summary>
        ///     Main Title
        /// </summary>
        public string Title { get; set; }
        public string SecondTitle { get; set; }
        /// <summary>
        ///     Main Description
        /// </summary>
        public string Description { get; set; }
        public string SecondDescription { get; set; }
        /// <summary>
        ///     Counter for some cases
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        ///     The json serialized of the data/model for an easy deserialization or convenience.
        /// </summary>
        public string JsonData { get; set; }
        public Boolean isSelected { get; set; } = false;
    }
}
