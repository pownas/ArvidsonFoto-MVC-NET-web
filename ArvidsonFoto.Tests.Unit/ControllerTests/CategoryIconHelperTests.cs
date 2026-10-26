using ArvidsonFoto.Core.Extensions;

namespace ArvidsonFoto.Tests.Unit.ControllerTests;

public class CategoryIconHelperTests
{
    [Theory]
    [InlineData(false, false, "bi-tag")]
    [InlineData(true, false, "bi-folder")]
    [InlineData(false, true, "bi-tag")]
    [InlineData(true, true, "bi-folder2-open")]
    public void GetIconClass_ReturnsTagForCategoriesWithoutChildren(
        bool hasSubcategories,
        bool isRoot,
        string expectedIcon)
    {
        Assert.Equal(expectedIcon, CategoryIconHelper.GetIconClass(hasSubcategories, isRoot));
    }
}
