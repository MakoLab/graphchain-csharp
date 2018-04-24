using System;
using System.Collections.Generic;
using System.Text;
using elephant.core.model.rdf;
using elephant.core.model.rdf.impl;
using elephant.core.service.cryptography;
using elephant.core.service.hashing;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace elephant.core.unittests
{
    [TestClass]
    public class HashingTests
    {
        [TestMethod]
        public void GetBytesFromString()
        {
            var str = "AB";
            var bytes = Encoding.UTF8.GetBytes(str);
            Assert.AreEqual(2, bytes.Length);
            Assert.AreEqual(65, bytes[0]);
            Assert.AreEqual(66, bytes[1]);
        }

        [TestMethod]
        public void HashingSimpleStrings()
        {
            var str = "GraphChain";
            var url = new Uri("http://ontologies.makolab.com/gc/").ToString();
            var hashCalculator = new Sha256HashCalculator();
            var hashStr = hashCalculator.CalculateHash(str);
            var hashUrl = hashCalculator.CalculateHash(url);
            Assert.AreEqual("64db22876fc25365e482c0fb09dc51109b3a91c0865323582709f4164eac8c0f", hashStr);
            Assert.AreEqual("4613be6647372e30fced2fc1899bd035b8192a720112c939df5141bd01c78d30", hashUrl);
        }

        [TestMethod]
        public void HashingTriple()
        {
            var loggerMock1 = new Mock<ILogger<CircleDotHashingService>>();
            var loggerMock2 = new Mock<ILogger<InterwovenHashingService>>();
            var hashCalculator = new Sha256HashCalculator();
            var triples = new HashSet<Triple>
            {
                new Triple(
                    new Iri("http://ontologies.makolab.com/gc/"),
                    new Iri("http://ontologies.makolab.com/gc/name"),
                    new Literal("Ontology"))
            };
            var hashingService = new CircleDotHashingService(loggerMock1.Object, hashCalculator);
            var interwovenService = new InterwovenHashingService(loggerMock2.Object, hashCalculator);
            var hashTriple = hashingService.CalculateHash(triples);
            var interwovenHash = interwovenService.CalculateHash(triples);
            Assert.AreEqual("9ca7f0185c00422b045dc8256a4872ef713ac57b059f49d1ad78808d4aca3c6a", hashTriple);
            Assert.AreEqual("9ca7f0185c00422b045dc8256a4872ef713ac57b059f49d1ad78808d4aca3c6a", interwovenHash);
        }

        [TestMethod]
        public void HashingTwoTriplesInDifferentOrder()
        {
            var loggerMock = new Mock<ILogger<CircleDotHashingService>>();
            var hashCalculator = new Sha256HashCalculator();
            var triples = new HashSet<Triple>
            {
                new Triple(
                    new Iri("http://ontologies.makolab.com/gc/"),
                    new Iri("http://ontologies.makolab.com/gc/name"),
                    new Literal("Ontology1")),
                new Triple(
                    new Iri("http://ontologies.makolab.com/gc/"),
                    new Iri("http://ontologies.makolab.com/gc/name"),
                    new Literal("Ontology2"))
            };
            var hashingService = new CircleDotHashingService(loggerMock.Object, hashCalculator);
            var hashTriples1 = hashingService.CalculateHash(triples);
            triples = new HashSet<Triple>
            {
                new Triple(
                    new Iri("http://ontologies.makolab.com/gc/"),
                    new Iri("http://ontologies.makolab.com/gc/name"),
                    new Literal("Ontology2")),
                new Triple(
                    new Iri("http://ontologies.makolab.com/gc/"),
                    new Iri("http://ontologies.makolab.com/gc/name"),
                    new Literal("Ontology1"))
            };
            var hashTriples2 = hashingService.CalculateHash(triples);
            Assert.AreEqual(hashTriples1, hashTriples2);
        }

        [TestMethod]
        public void HashTwoBlankNodes()
        {
            var loggerMock = new Mock<ILogger<InterwovenHashingService>>();
            var hashCalculator = new Sha256HashCalculator();
            var hashingService = new InterwovenHashingService(loggerMock.Object, hashCalculator);
            var triples1 = new HashSet<Triple>
            {
                new Triple(
                    new BlankNode("AAA"),
                    new Iri("http://ex.com/prop"),
                    new Iri("http://ex.com/E1")),
                new Triple(
                    new BlankNode("AAA"),
                    new Iri("http://ex.com/prop"),
                    new Iri("http://ex.com/E2"))
            };
            var triples2 = new HashSet<Triple>
            {
                new Triple(
                    new BlankNode("AAA"),
                    new Iri("http://ex.com/prop"),
                    new Iri("http://ex.com/E1")),
                new Triple(
                    new BlankNode("BBB"),
                    new Iri("http://ex.com/prop"),
                    new Iri("http://ex.com/E2"))
            };
            var hash1 = hashingService.CalculateHash(triples1);
            var hash2 = hashingService.CalculateHash(triples2);
            Assert.AreNotEqual(hash1, hash2);
        }
    }
}
