// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.

namespace Fixtures.AcceptanceTestsAzureCompositeModelClient.Models
{
    using AcceptanceTestsAzureCompositeModelClient;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    [JsonObject("shark")]
    public partial class Shark : FishInner
    {
        /// <summary>
        /// Initializes a new instance of the Shark class.
        /// </summary>
        public Shark() { }

        /// <summary>
        /// Initializes a new instance of the Shark class.
        /// </summary>
        public Shark(double length, System.DateTime birthday, string species = default(string), IList<FishInner> siblings = default(IList<FishInner>), int? age = default(int?))
            : base(length, species, siblings)
        {
            Age = age;
            Birthday = birthday;
        }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "age")]
        public int? Age { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "birthday")]
        public System.DateTime Birthday { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
        }
    }
}

