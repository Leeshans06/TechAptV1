// Copyright © 2025 Always Active Technologies PTY Ltd
using System.Collections.Concurrent;
using System.Data;
using TechAptV1.Client.Models;
namespace TechAptV1.Client.Services;

/// <summary>
/// Default constructor providing DI Logger and Data Service
/// </summary>
/// <param name="logger"></param>
/// <param name="dataService"></param>
public sealed class ThreadingService
{
    private int _oddNumbers = 0;
    private int _evenNumbers = 0;
    private int _primeNumbers = 0;
    private int _totalNumbers = 0;
    private readonly ILogger<ThreadingService> _logger;
    private readonly DataService _dataService;
    private readonly ConcurrentBag<Number> _numbers = new();
    private bool _isRunning = false;
    private Task? _oddTask;
    private Task? _primeTask;
    private Task? _evenTask;

    public ThreadingService(ILogger<ThreadingService> logger, DataService dataService)
    {
        _logger = logger;
        _dataService = dataService;
    }

    public int GetOddNumbers() => _oddNumbers;
    public int GetEvenNumbers() => _evenNumbers;
    public int GetPrimeNumbers() => _primeNumbers;
    public int GetTotalNumbers() => _totalNumbers;
    public ConcurrentBag<Number> GetGobalList() => _numbers;

    /// <summary>
    /// Start the random number generation process
    /// </summary>
    public async Task Start()
    {
        _logger.LogInformation("Start");
        if (_isRunning) return;
        _isRunning = true;

        // Run tasks 
        _oddTask = Task.Run(() => GenerateOddNumbers());
        _primeTask = Task.Run(() => GeneratePrimeNegatives());
        await Task.WhenAll(_oddTask, _primeTask);

        if (_numbers.Count >= 2500000)
        {
            _evenTask = Task.Run(() => GenerateEvenNumbers());
            await Task.WhenAll(_evenTask);
        }

        _isRunning = false;
    }

    /// <summary>
    /// Generate Odd Numbers and adds to gobal variable Numbers
    /// </summary>
    private void GenerateOddNumbers()
    {
         _logger.LogInformation("Start GenerateOddNumbers :");
        Random rand = new();
        while (_numbers.Count < 2500000) 
        {
            int num = rand.Next(1, int.MaxValue) | 1; 
            var number = new Number { Value = num, IsPrime = IsPrime(num) ? 1 : 0 };
            _numbers.Add(number);
            Interlocked.Increment(ref _oddNumbers);
            Interlocked.Increment(ref _totalNumbers);
        }
        _logger.LogInformation("Finsihed GenerateOddNumbers ;");
    }

    /// <summary>
    /// Generate Prime Negatives and adds to gobal variable Numbers
    /// </summary>
    private void GeneratePrimeNegatives()
    {
         _logger.LogInformation("Start GeneratePrimeNegatives :");
        Random rand = new();
        while (_numbers.Count < 2500000) 
        {
            int num = rand.Next(2, int.MaxValue);
            var isPrime = IsPrime(num); 

            if (isPrime)
            {
                var number = new Number { Value = -num, IsPrime = 1 };
                _numbers.Add(number);               
                Interlocked.Increment(ref _primeNumbers);
                Interlocked.Increment(ref _totalNumbers);
            }
        }
         _logger.LogInformation("Finished GeneratePrimeNegatives ;");
    }

    /// <summary>
    /// Generate Even Numbers and adds to gobal variable Numbers
    /// </summary>
    private void GenerateEvenNumbers()
    {
        _logger.LogInformation("Start GenerateEvenNumbers :");
        Random rand = new();
        while (_numbers.Count < 10000000) 
        {
            int num = rand.Next(2, int.MaxValue) & ~1; // Ensure even number

            var number = new Number { Value = num, IsPrime = IsPrime(num) ? 1 : 0 };
            _numbers.Add(number);           
            Interlocked.Increment(ref _evenNumbers);
            Interlocked.Increment(ref _totalNumbers);
        }
        _logger.LogInformation("Finsihed GenerateEvenNumbers ;");
    }

    /// <summary>
    /// Persist the results to the SQLite database
    /// </summary>
    public async Task Save()
    {
        _logger.LogInformation("Save");

        var numberList = _numbers.ToList();
        numberList.OrderBy(n => n.Value).ToList(); // order by asc order
        await _dataService.InitializeAsync();
        await _dataService.Save(numberList);        
        _logger.LogInformation("Finished - Save Numbers.");
    }

    /// <summary>
    /// Check if the number is prime
    /// </summary>
    /// <param name="number"></param>
    /// <returns>true/false</returns>
    public bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i * i <= number; i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

}
