# SportsStoreDemo
Based on the Sports Store application fomr Pro ASP.NET MVC 5 Apress book ISBN13: 978-1-4302-6529-0
with the following alterations:
1.)	Did not use the ViewBag in the Cart Index controller to communicate return url for View instead I used CartIndexViewModel.
2.)	Added a dismiss button to alert messaged in Admin Layout.
3.)	Added an alert message for delete in Admin Layout
4.)	Added log off button on the header for the Admin view (Index.cshtml).
5.)	Added remove all button to the cart Index.cshtm.
6.)	Updated to use .NET 4.6 and C# 6.0
7.)	Added Dispose for the dbContext in EFProductRepository and sealed the class.
8.)	Change DI resolver to keep EF Repository only for InRequestScope.
