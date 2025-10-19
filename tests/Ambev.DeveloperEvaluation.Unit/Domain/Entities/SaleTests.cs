using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains comprehensive unit tests for the Sale Aggregate Root, covering item management,
/// full sale cancellation, partial item cancellation, and update invariants.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that adding an item increases the collection count and correctly updates the TotalAmount.
    /// </summary>
    [Fact(DisplayName = "AddItem should update collection count and recalculate TotalAmount")]
    public void Given_ExistingSale_When_AddItem_Then_TotalAmountShouldBeRecalculated()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 2);
        var initialTotal = sale.TotalAmount.Value;
        var initialCount = sale.Items.Count;

        var newItem = SaleTestData.GenerateValidSaleItem(quantity: 1, unitPrice: 50.00M);
        var newItemTotal = newItem.TotalAmount.Value;

        sale.AddItem(newItem);

        Assert.Equal(initialCount + 1, sale.Items.Count);
        Assert.Equal(initialTotal + newItemTotal, sale.TotalAmount.Value);
    }

    /// <summary>
    /// Tests that attempting to remove a non-existent item throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "RemoveItem for non-existent ID should throw DomainException")]
    public void Given_ExistingSale_When_RemoveNonExistentItem_Then_ShouldThrowDomainException()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 1);
        var nonExistentId = Guid.NewGuid();

        Assert.Throws<DomainException>(() => sale.RemoveItem(nonExistentId));
    }

    /// <summary>
    /// Tests that removing an existing item correctly reduces the collection count and updates the TotalAmount.
    /// </summary>
    [Fact(DisplayName = "RemoveItem for existing item should reduce count and recalculate TotalAmount")]
    public void Given_SaleWithMultipleItems_When_RemoveExistingItem_Then_TotalAmountShouldBeReduced()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 3);
        var itemToRemove = sale.Items.First();
        var initialTotal = sale.TotalAmount.Value;
        var removedItemTotal = itemToRemove.TotalAmount.Value;
        var expectedTotal = initialTotal - removedItemTotal;
        var itemIdToRemove = itemToRemove.Id;

        sale.RemoveItem(itemIdToRemove);

        Assert.DoesNotContain(sale.Items, i => i.Id == itemIdToRemove);
        Assert.Equal(2, sale.Items.Count);
        Assert.Equal(expectedTotal, sale.TotalAmount.Value);
    }

    /// <summary>
    /// Tests that calling the Cancel method on the Sale entity sets the IsCancelled flag to true 
    /// and ensures all associated SaleItems are also cancelled (cascading cancellation).
    /// </summary>
    [Fact(DisplayName = "Cancel should set IsCancelled to true and cancel all SaleItems")]
    public void Given_UncancelledSale_When_Cancel_Then_AllItemsShouldBeCancelledAndSaleIsCancelled()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 3);

        sale.Cancel();

        Assert.True(sale.IsCancelled);
        Assert.All(sale.Items, item => Assert.True(item.IsCancelled));
    }

    /// <summary>
    /// Tests that attempting to call Cancel on an already cancelled Sale throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Cancel on cancelled sale should throw DomainException")]
    public void Given_CancelledSale_When_Cancel_Then_ShouldThrowDomainException()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 1);
        sale.Cancel();

        Assert.Throws<DomainException>(() => sale.Cancel());
    }

    /// <summary>
    /// Tests the cascading rule: if the last remaining item is cancelled via CancelItem, the parent Sale should also be cancelled.
    /// </summary>
    [Fact(DisplayName = "CancelItem should trigger Sale cancellation if it was the last active item")]
    public void Given_SaleWithOnlyOneItem_When_CancelItem_Then_SaleShouldBeCancelled()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 1);
        var itemIdToCancel = sale.Items.First().Id;

        sale.CancelItem(itemIdToCancel);

        Assert.True(sale.IsCancelled);
        Assert.True(sale.Items.First().IsCancelled);
    }

    /// <summary>
    /// Tests that attempting to cancel an item with a non-existent ID throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "CancelItem for non-existent ID should throw DomainException")]
    public void Given_ExistingSale_When_CancelNonExistentItem_Then_ShouldThrowDomainException()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 1);
        var nonExistentId = Guid.NewGuid();

        Assert.Throws<DomainException>(() => sale.CancelItem(nonExistentId));
    }

    /// <summary>
    /// Tests that updating a valid sale successfully replaces customer, branch, and items,
    /// and recalculates the final TotalAmount based on the new item collection.
    /// </summary>
    [Fact(DisplayName = "Update should replace all details and recalculate TotalAmount")]
    public void Given_ValidSale_When_UpdateWithNewData_Then_DetailsAndTotalShouldBeUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 2);

        var newCustomer = SaleTestData.GenerateExternalReference();
        var newBranch = SaleTestData.GenerateExternalReference();

        var newItem1 = SaleTestData.GenerateValidSaleItem(quantity: 1, unitPrice: 100.00M);
        var newItem2 = SaleTestData.GenerateValidSaleItem(quantity: 10, unitPrice: 10.00M);
        var newItems = new List<SaleItem> { newItem1, newItem2 };

        var expectedNewTotal = newItem1.TotalAmount.Value + newItem2.TotalAmount.Value;

        // Act
        sale.Update(newCustomer, newBranch, newItems);

        // Assert
        Assert.Equal(newCustomer.Value, sale.Customer.Value);
        Assert.Equal(newCustomer.Description, sale.Customer.Description);
        Assert.Equal(newBranch.Value, sale.Branch.Value);
        Assert.Equal(newBranch.Description, sale.Branch.Description);

        Assert.Equal(2, sale.Items.Count);
        Assert.Contains(sale.Items, i => i.Product.Value == newItem1.Product.Value 
            && i.Product.Description == newItem1.Product.Description);

        Assert.Equal(expectedNewTotal, sale.TotalAmount.Value);
    }

    /// <summary>
    /// Tests that attempting to update a Sale that is already cancelled throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Update on cancelled Sale should throw DomainException")]
    public void Given_CancelledSale_When_Update_Then_ShouldThrowDomainException()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 1);
        sale.Cancel();

        var newCustomer = SaleTestData.GenerateExternalReference();
        var newBranch = SaleTestData.GenerateExternalReference();
        var newItems = SaleTestData.GenerateValidSaleItems(1);

        Assert.Throws<DomainException>(() => sale.Update(newCustomer, newBranch, newItems));
    }

    /// <summary>
    /// Tests that attempting to update a Sale by including an item that is already cancelled throws a DomainException.
    /// </summary>
    [Fact(DisplayName = "Update with a cancelled item should throw DomainException")]
    public void Given_ValidSale_When_UpdateWithCancelledItem_Then_ShouldThrowDomainException()
    {
        var sale = SaleTestData.GenerateValidSaleWithItems(initialItemCount: 1);

        var cancelledItem = SaleTestData.GenerateValidSaleItem();
        cancelledItem.Cancel();

        var newCustomer = SaleTestData.GenerateExternalReference();
        var newBranch = SaleTestData.GenerateExternalReference();
        var newItems = new List<SaleItem> { cancelledItem };

        Assert.Throws<DomainException>(() => sale.Update(newCustomer, newBranch, newItems));
    }
}