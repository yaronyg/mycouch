﻿using MyCouch.Testing;
using NUnit.Framework;

namespace MyCouch.IntegrationTests.Documents
{
    [TestFixture]
    public class DocumentsTests : IntegrationTestsOf<IDocuments>
    {
        protected override void OnTestInitialize()
        {
            SUT = Client.Documents;
        }

        [Test]
        public void When_post_of_new_document_Using_an_entity_The_document_is_persisted()
        {
            var artist = TestDataFactory.CreateArtist();

            var response = SUT.Post(artist);

            response.Should().BeSuccessfulPost(e => e.ArtistId, e => e.ArtistRev);
        }
    }
}