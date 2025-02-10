# Project Overview

## Installed NuGet Packages
- **protobuf-net** – Used for binary serialization.  
- **System.Xml.ReaderWriter** – Used for XML serialization.  
- **System.Data.SQLite.Core** – Provides SQLite database support.  

## Threading.razor
When the **Start** button is clicked, two streams begin execution:  
- `GenerateOddNumbers()`  
- `GeneratePrimeNegatives()`  

Both functions update a global variable called **number**. Once this variable reaches a count of **25,000,000**, a third stream is initiated:  
- `GenerateEvenNumbers()`  

All three streams run in parallel until the **number** variable reaches **10,000,000**, at which point all streams are terminated. The results are then displayed on screen, and the list of generated numbers is made available for saving.  

Once number generation is complete, the **Save** button is enabled. The save operation uses **batch inserts**, as this method proved to be the most efficient.  

## Results.razor
### Binary File Downloads  
Initially, the `GetAll()` function was used to fetch all records from the database and return them as a list. However, this approach was inefficient for large datasets.  

To optimize performance, the `StreamAllNumbers()` function was developed. It asynchronously streams records from the **Number** table directly into a memory stream.  

As a result, on **Results.razor**, file downloads now use `StreamAllNumbers()` instead of `GetAll()`, significantly improving efficiency when handling large datasets.  
