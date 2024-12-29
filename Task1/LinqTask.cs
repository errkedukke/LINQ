using System;
using System.Collections.Generic;
using System.Linq;
using Task1.DoNotChange;

namespace Task1;

public static class LinqTask
{
    public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
    {
        return customers.Where(c => c.Orders.Sum(o => o.Total) > limit);
    }

    public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
        IEnumerable<Customer> customers,
        IEnumerable<Supplier> suppliers
    )
    {
        return customers.Select(c => (
            customer: c,
            suppliers: suppliers.Where(s => s.City == c.City && s.Country == c.Country)
        ));
    }

    public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
        IEnumerable<Customer> customers,
        IEnumerable<Supplier> suppliers
    )
    {
        var groupedSuppliers = suppliers.GroupBy(s => new { s.City, s.Country });

        return customers.Select(c => (
            customer: c,
            suppliers: groupedSuppliers
                .FirstOrDefault(g => g.Key.City == c.City && g.Key.Country == c.Country)?.AsEnumerable() ?? Enumerable.Empty<Supplier>()
        ));
    }

    public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
    {
        return customers.Where(c => c.Orders.Any(o => o.Total > limit));
    }

    public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(IEnumerable<Customer> customers)
    {
        return customers
            .Where(c => c.Orders.Any())
            .Select(c => (customer: c, dateOfEntry: c.Orders.Min(o => o.OrderDate)));
    }

    public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(IEnumerable<Customer> customers)
    {
        return customers
            .Where(c => c.Orders.Any())
            .Select(c => new
            {
                customer = c,
                dateOfEntry = c.Orders.Min(o => o.OrderDate),
                turnover = c.Orders.Sum(o => o.Total)
            })
            .OrderBy(e => e.dateOfEntry.Year)
            .ThenBy(e => e.dateOfEntry.Month)
            .ThenByDescending(e => e.turnover)
            .ThenBy(e => e.customer.CompanyName)
            .Select(e => (e.customer, e.dateOfEntry));
    }

    public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
    {
        return customers.Where(c => !c.PostalCode.All(char.IsDigit)
                                  || string.IsNullOrEmpty(c.Region)
                                  || !c.Phone.Contains("("));
    }

    public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
    {
        return products
            .GroupBy(p => p.Category)
            .Select(categoryGroup => new Linq7CategoryGroup
            {
                Category = categoryGroup.Key,
                UnitsInStockGroup = categoryGroup
                    .GroupBy(p => p.UnitsInStock)
                    .Select(stockGroup => new Linq7UnitsInStockGroup
                    {
                        UnitsInStock = stockGroup.Key,
                        Prices = stockGroup.Select(p => p.UnitPrice).OrderBy(price => price)
                    }).ToList()
            });
    }

    public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
     IEnumerable<Product> products,
     decimal cheap,
     decimal middle,
     decimal expensive
 )
    {
        return products
            .Select(p => new
            {
                Product = p,
                Category = p.UnitPrice <= cheap ? cheap
                          : p.UnitPrice <= middle ? middle
                          : expensive
            })
            .GroupBy(p => p.Category)
            .Select(g => (g.Key, g.Select(x => x.Product)));
    }

    public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(IEnumerable<Customer> customers)
    {
        var groupedByCity = customers.GroupBy(c => c.City);

        var result = groupedByCity.Select(g =>
        {
            var totalIncome = g.SelectMany(c => c.Orders).Sum(o => o.Total);
            var totalOrders = g.SelectMany(c => c.Orders).Count();
            int averageIncome = totalOrders > 0 ? (int)Math.Round(totalIncome / totalOrders) : 0;
            int averageIntensity = totalOrders > 0 ? (int)Math.Round((double)totalOrders / g.Count()) : 0;

            return (city: g.Key, averageIncome, averageIntensity);
        });

        return result;
    }

    public static string Linq10(IEnumerable<Supplier> suppliers)
    {
        return string.Concat(suppliers
            .Select(s => s.Country)
            .Distinct()
            .OrderBy(c => c.Length)
            .ThenBy(c => c));
    }
}
