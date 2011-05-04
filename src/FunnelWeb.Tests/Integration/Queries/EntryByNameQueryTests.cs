﻿using System;
using System.Linq;
using FunnelWeb.Model;
using FunnelWeb.Repositories.Queries;
using FunnelWeb.Tests.Helpers;
using NUnit.Framework;

namespace FunnelWeb.Tests.Integration.Queries
{
    [TestFixture]
    public class EntryByNameQueryTests : IntegrationTest
    {
        public EntryByNameQueryTests()
            : base(TheDatabase.CanBeDirty)
        {
        }

        [Test]
        public void MatchesEntryByName()
        {
            var name = "test-" + Guid.NewGuid();

            Database.WithRepository(
                repo =>
                {
                    var entry1 = new Entry { Name = name, Author = "A1" };
                    var revision1 = entry1.Revise();
                    revision1.Body = "Hello";
                    repo.Add(entry1);
                });

            Database.WithRepository(
                repo =>
                {
                    var entry = repo.FindFirst(new EntryByNameQuery(name));
                    Assert.AreEqual("A1", entry.Author);
                });
        }

        [Test]
        public void MatchesEntryByNameThatHasComments()
        {
            var name = "test-" + Guid.NewGuid();

            Database.WithRepository(
                repo =>
                    {
                        var entry = new Entry
                                     {
                                         Name = name,
                                         Author = "A1"
                                     };
                        var revision1 = entry.Revise();
                        revision1.Body = "Hello";
                        repo.Add(entry);
                        var comment = new Comment
                                          {
                                              AuthorName = "Test",
                                              AuthorEmail = "test@test.net",
                                              AuthorUrl = "",
                                              Body = "Comment",
                                              Posted = DateTime.Now,
                                              Entry = entry
                                          };
                        entry.Comments.Add(comment);
                        repo.Add(comment);
                    });

            Database.WithRepository(
                repo =>
                {
                    var entry = repo.FindFirst(new EntryByNameQuery(name));
                    Assert.AreEqual("A1", entry.Author);
                    Assert.AreEqual("Test", entry.Comments.First().AuthorName);
                });
        }
    }
}