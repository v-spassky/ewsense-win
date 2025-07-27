using Microsoft.VisualStudio.TestTools.UnitTesting;
using ewsense.storage;

namespace ewsense.Tests;

[TestClass]
public class KeywordRepositoryTests
{
    private KeywordRepository? _repository;

    [TestInitialize]
    public void Setup()
    {
        _repository = new KeywordRepository();
    }

    [TestMethod]
    public void Constructor_ShouldCreateInstance()
    {
        Assert.IsNotNull(_repository);
        Assert.IsNotNull(_repository!.Keywords);
    }

    [TestMethod]
    public void AddKeyword_ValidKeyword_ShouldAddToCollection()
    {
        _repository!.AddKeyword("test");

        Assert.IsTrue(_repository.Keywords.Contains("test"));
    }

    [TestMethod]
    public void AddKeyword_EmptyKeyword_ShouldNotAdd()
    {
        var initialCount = _repository!.Keywords.Count;
        _repository.AddKeyword("");
        _repository.AddKeyword("   ");

        Assert.AreEqual(initialCount, _repository.Keywords.Count);
    }

    [TestMethod]
    public void RemoveKeyword_ExistingKeyword_ShouldRemove()
    {
        _repository!.AddKeyword("test");
        _repository.RemoveKeyword("test");

        Assert.IsFalse(_repository.Keywords.Contains("test"));
    }

    [TestMethod]
    public void HasKeywordEndingWith_ValidText_ShouldReturnCorrectResult()
    {
        _repository!.AddKeyword("hello");
        _repository.AddKeyword("world");

        Assert.IsTrue(_repository.HasKeywordEndingWith("hello"));
        Assert.IsTrue(_repository.HasKeywordEndingWith("say hello"));
        Assert.IsFalse(_repository.HasKeywordEndingWith("goodbye"));
    }
}
