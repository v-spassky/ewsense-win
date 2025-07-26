using Microsoft.VisualStudio.TestTools.UnitTesting;
using ewsense.ui;

namespace ewsense.Tests;

[TestClass]
public class PopupManagerTests
{
    [TestMethod]
    public void Constructor_ShouldCreateInstance()
    {
        var popupManager = new Popup();

        Assert.IsNotNull(popupManager);
    }

    [TestMethod]
    public void ShowKeywordDetectedPopup_ShouldNotThrow()
    {
        var popupManager = new Popup();

        popupManager.ShowKeywordDetectedPopup();
    }
}
