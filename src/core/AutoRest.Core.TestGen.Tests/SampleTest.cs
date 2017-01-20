using AutoRest.Core.Utilities;
using AutoRest.Swagger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Xunit;

namespace AutoRest.Core.TestGen.Tests
{
    public sealed class SampleTest
    {
        [Fact]
        public void SimpleSampleTest()
        {
            var fileSystem = new MemoryFileSystem();

            fileSystem.WriteFile("swagger.json", JObject.FromObject(new { }).ToString());
            var swagger = SwaggerParser.Load("swagger.json", fileSystem);

            var sampleSource = new
            {
                title = "Title",
                operationId = "Operation",
                parameters = new
                {
                    a = "3",
                },
                responses = new Dictionary<string, object>
                {
                    {
                        "200",
                        new
                        {
                            headers = new Dictionary<string, string>
                            {
                                { "Content-Type", "application/json" }
                            }
                        }
                    }
                }
            };

            var sampleSourceStr = JObject.FromObject(sampleSource).ToString();
            var x = JsonConvert.DeserializeObject<Sample>(sampleSourceStr);

            fileSystem.WriteFile("sample.json", sampleSourceStr);            
            var simple = Sample.Load("sample.json", fileSystem);

            Assert.Equal(simple.Title, "Title");
            Assert.Equal(simple.OperationId, "Operation");
            Assert.Equal(simple.Parameters["a"], "3");
            Assert.Equal(simple.Responses["200"].Headers["Content-Type"], "application/json");
        }
    }
}
