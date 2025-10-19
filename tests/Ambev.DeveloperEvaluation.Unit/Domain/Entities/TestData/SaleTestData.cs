using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for the Sale Aggregate Root and its components
/// using the Bogus library. This class ensures all generated objects comply with domain invariants.
/// </summary>
public static class SaleTestData
{

    /// <summary>
    /// Configures the Faker for ExternalReference, ensuring a valid Guid and non-empty description.
    /// </summary>
    private static readonly Faker<ExternalReference> ExternalReferenceFaker = new Faker<ExternalReference>()
        .CustomInstantiator(f => new ExternalReference(
            value: Guid.NewGuid(),
            description: f.Commerce.Department() + " " + f.Random.String2(5)
        ));

    /// <summary>
    /// Configures the Faker for Quantity, ensuring Value is > 0 and <= Quantity.MaxValue (20).
    /// </summary>
    private static readonly Faker<Quantity> QuantityFaker = new Faker<Quantity>()
        .CustomInstantiator(f =>
        {
            // Quantity must be > 0 and <= 20
            int quantity = f.Random.Int(1, Quantity.MaxValue);
            return new Quantity(quantity);
        });

    /// <summary>
    /// Configures the Faker for Money, ensuring Value is > 0.
    /// </summary>
    private static readonly Faker<Money> MoneyFaker = new Faker<Money>()
        .CustomInstantiator(f =>
        {
            // Money must be > 0. Use 0.01 as the minimum safe amount.
            decimal amount = f.Finance.Amount(0.01M, 1000M, 2);
            return new Money(amount);
        });

    /// <summary>
    /// Configures the Faker for SaleItem, using generated VOs in the constructor.
    /// </summary>
    private static readonly Faker<SaleItem> SaleItemFaker = new Faker<SaleItem>()
        .CustomInstantiator(f =>
        {
            var productRef = ExternalReferenceFaker.Generate();
            var quantity = QuantityFaker.Generate();
            var unitPrice = MoneyFaker.Generate();

            // The SaleItem constructor handles IsCancelled=false and calls CalculateTotal().
            return new SaleItem(productRef, quantity, unitPrice);
        })
        .RuleFor(si => si.Id, f => Guid.NewGuid());

    /// <summary>
    /// Configures the Faker for Sale (Aggregate Root).
    /// </summary>
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .CustomInstantiator(f =>
        {
            var customer = ExternalReferenceFaker.Generate();
            var branch = ExternalReferenceFaker.Generate();
            var number = f.Random.Long(100, 99999);

            return new Sale(customer, branch, number);
        })
        .RuleFor(s => s.Id, f => Guid.NewGuid())
        .RuleFor(s => s.CreatedDate, f => f.Date.Past());


    /// <summary>
    /// Generates a randomized ExternalReference.
    /// </summary>
    public static ExternalReference GenerateExternalReference() => ExternalReferenceFaker.Generate();

    /// <summary>
    /// Generates a randomized Quantity, or a controlled value if specified.
    /// </summary>
    public static Quantity GenerateQuantity(int? value = null)
    {
        if (value.HasValue) return new Quantity(value.Value);
        return QuantityFaker.Generate();
    }

    /// <summary>
    /// Generates a randomized Money amount, or a controlled value if specified.
    /// </summary>
    public static Money GenerateMoney(decimal? value = null)
    {
        if (value.HasValue) return new Money(value.Value);
        return MoneyFaker.Generate();
    }

    /// <summary>
    /// Generates a randomized SaleItem, allowing control over quantity and unit price.
    /// </summary>
    public static SaleItem GenerateValidSaleItem(int? quantity = null, decimal? unitPrice = null)
    {
        // If controlled values are specified, manually construct the item
        if (quantity.HasValue || unitPrice.HasValue)
        {
            return new SaleItem(
                GenerateExternalReference(),
                GenerateQuantity(quantity ?? 1),
                GenerateMoney(unitPrice ?? 100.00M)
            );
        }

        // Otherwise, use the Faker for fully randomized data
        return SaleItemFaker.Generate();
    }

    /// <summary>
    /// Generates a list of randomized SaleItems, allowing control over count, quantity, and unit price.
    /// </summary>
    public static List<SaleItem> GenerateValidSaleItems(int count, int? quantity = null, decimal? unitPrice = null)
    {
        // If controlled values are requested, use the explicit generator logic
        if (quantity.HasValue || unitPrice.HasValue)
        {
            var items = new List<SaleItem>();
            for (int i = 0; i < count; i++)
            {
                items.Add(GenerateValidSaleItem(quantity, unitPrice));
            }
            return items;
        }

        // Use the Faker for fully randomized data
        return SaleItemFaker.Generate(count);
    }

    /// <summary>
    /// Generates a valid Sale Aggregate Root without any items initially, allowing control over core properties.
    /// </summary>
    public static Sale GenerateValidSale(long? number = null, ExternalReference? customer = null, ExternalReference? branch = null)
    {
        // If no controlled properties are needed, use the Faker
        if (!number.HasValue && customer == null && branch == null)
            return SaleFaker.Generate();

        // Otherwise, manually construct to override properties
        var customerRef = customer ?? ExternalReferenceFaker.Generate();
        var branchRef = branch ?? ExternalReferenceFaker.Generate();
        var saleNumber = number ?? new Faker().Random.Long(100, 99999);

        return new Sale(customerRef, branchRef, saleNumber);
    }
    /// <summary>
    /// Generates a List of Sale Root without any items initially, allowing control over core properties.
    /// </summary>
    public static List<Sale> GenerateValidSales(int count, long? number = null, ExternalReference? customer = null, ExternalReference? branch = null)
    {
        // If no controlled properties are needed, use the Faker
        if (!number.HasValue && customer == null && branch == null)
            return SaleFaker.Generate(count);

        var sales = new List<Sale>();

        for (int i = 0; i < count; i++)
        {
            sales.Add(GenerateValidSale(number, customer, branch));
        }
        return sales;

    }

    /// <summary>
    /// Generates a fully configured and valid Sale Aggregate Root, initialized with items.
    /// This is the primary method for most integration/extension tests.
    /// </summary>
    public static Sale GenerateValidSaleWithItems(
        int initialItemCount = 2,
        long? number = null,
        int? quantity = null,
        decimal? unitPrice = null,
        ExternalReference? customer = null,
        ExternalReference? branch = null)
    {
        var sale = GenerateValidSale(number, customer, branch);

        var items = GenerateValidSaleItems(
            initialItemCount,
            quantity,
            unitPrice
        );

        foreach (var item in items)
            sale.AddItem(item);

        return sale;
    }
}