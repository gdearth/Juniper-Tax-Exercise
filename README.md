# Juniper-Tax-Exercise

[![.NET](https://github.com/gdearth/Juniper-Tax-Exercise/actions/workflows/dotnet.yml/badge.svg)](https://github.com/gdearth/Juniper-Tax-Exercise/actions/workflows/dotnet.yml)

Juniper coding exercise for Tax Jar.

## Assignment:
We use a lot of external services and API's to accommodate our customer's needs. One of the them is Tax calculation. There are a lot of Tax calculation API's out there and we need to be able to work with them via a common interface we define in a service class.

Your code test is to simply create a Tax Service that can take a Tax Calculator in the class initialization and return the total tax that needs to be collected.
We are going to use Tax Jar as one of our calculators. You will need to write a .Net client to talk to their API, do not use theirs. Can be very simple, needed methods outined below.

**We are only going to be talking to their SalesTax API:**

[https://developers.taxjar.com/api/reference/#sales-tax-api](https://developers.taxjar.com/api/reference/#sales-tax-api)
  
**Here is the API Key:**

**~~Not Including~~**
  
**The client you need to write for Tax Jar only needs to include a couple of methods:**
- Get the Tax rates for a location
- Calculate the taxes for an order


The Tax Service will also have these methods and simply call the Tax Calculator. Eventually we would have several Tax Calculators and the Tax Service would need to decide which to use based on the Customer that is consuming the Tax Service.

Unit Tests on the Tax Jar calculator and Tax Service should be included in your solution.

Please let us know if you have any questions at all.

When you are finished just push your code to GitHub or BitBucket and send us the link.
