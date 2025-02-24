using Avro.IO;
using Avro.Reflect;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avro.test.Reflect
{
    [TestFixture]
    public class TestRecursive
    {
        private const string _recurse = @"
        {
           ""type"": ""record"",
           ""namespace"": ""AvroSchemaGenerator.Tests"",
           ""name"": ""Recursive"",
           ""fields"": [
              {
                 ""name"": ""Fo"",
                 ""type"": [
                    ""null"",
                    {
                       ""type"": ""record"",
                       ""namespace"": ""AvroSchemaGenerator.Tests"",
                       ""name"": ""SimpleFoo"",
                       ""fields"": [
                          {
                             ""name"": ""Age"",
                             ""type"": ""int""
                          },
                          {
                             ""name"": ""Name"",
                             ""type"": ""string""
                          },
                          {
                             ""name"": ""FactTime"",
                             ""type"": ""long""
                          },
                          {
                             ""name"": ""Point"",
                             ""type"": ""double""
                          },
                          {
                             ""name"": ""Precision"",
                             ""type"": ""float""
                          },
                          {
                             ""name"": ""Attending"",
                             ""type"": ""boolean""
                          },
                          {
                             ""name"": ""Id"",
                             ""type"": [
                                ""null"",
                                ""bytes""
                             ],
                             ""default"": null
                          }
                       ]
                    }
                 ],
                 ""default"": null
              },
              {
                 ""name"": ""Recurse"",
                 ""type"": [
                    ""null"",
                    ""Recursive""
                 ],
                 ""default"": null
              }
           ]
        }";
        [TestCase]
        public void RecurseTest()
        {
            var schema = Schema.Parse(_recurse);
            var recursive = new Recursive
            {
                Fo = new SimpleFoo
                {
                    Age = 67,
                    Attending = true,
                    FactTime = 90909099L,
                    Id = new byte[0] { },
                    Name = "Ebere",
                    Point = 888D,
                    Precision = 787F
                },
                Recurse = new Recursive
                {
                    Fo = new SimpleFoo
                    {
                        Age = 6,
                        Attending = false,
                        FactTime = 90L,
                        Id = new byte[0] { },
                        Name = "Ebere Abanonu",
                        Point = 88D,
                        Precision = 78F
                    },
                }
            };

            var writer = new ReflectWriter<Recursive>(schema);
            var reader = new ReflectReader<Recursive>(schema, schema);

            using (var stream = new MemoryStream(256))
            {
                writer.Write(recursive, new BinaryEncoder(stream));
                stream.Seek(0, SeekOrigin.Begin);
                var recursiveRead = reader.Read(new BinaryDecoder(stream));
                Assert.IsTrue(recursiveRead.Fo.Attending);
                Assert.AreEqual(recursiveRead.Recurse.Fo.Name, "Ebere Abanonu");
            }
        }
    }
    public class Recursive
    {
        public SimpleFoo Fo { get; set; }
        public Recursive Recurse { get; set; }
    }
    public class SimpleFoo
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public long FactTime { get; set; }
        public double Point { get; set; }
        public float Precision { get; set; }
        public bool Attending { get; set; }
        public byte[] Id { get; set; }
    }
}
