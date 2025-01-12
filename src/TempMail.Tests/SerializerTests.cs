using Newtonsoft.Json;
using NUnit.Framework;
using SmorcIRL.TempMail.Helpers;

namespace SmorcIRL.TempMail.Tests
{
    public class SerializerTests
    {
        [Test]
        public void CaseResolverTest()
        {
            var json = Serializer.Serialize(new TestCaseModel
            {
                Property_1 = "1",
                Property_2 = "2",
                Property_3 = "3",
            });
            
            Assert.That(json.Contains("property_1"), Is.True);
            Assert.That(json.Contains("property_2"), Is.True);
            Assert.That(json.Contains("property_3_Alias"), Is.True);
            
            var model = Serializer.Deserialize<TestCaseModel>("{Property_1: 1, property_2: 2, property_3_Alias: \"3\"}");
            
            Assert.That("1", Is.EqualTo(model.Property_1));
            Assert.That("2", Is.EqualTo(model.Property_2));
            Assert.That("3", Is.EqualTo(model.Property_3));
        }
        
        private class TestCaseModel
        {
            public string Property_1 { get; set; }
            
            public string Property_2 { get; set; }
            
            [JsonProperty("Property_3_Alias")]
            public string Property_3 { get; set; }
        }
    }
}