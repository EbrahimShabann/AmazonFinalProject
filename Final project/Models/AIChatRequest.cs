// Models/AIChatRequest.cs
using Final_project.Repository;
using Final_project.ViewModel.Customer;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

public class AIChatRequest
{
    public string Message { get; set; }
    public string Model { get; set; } = "meta-llama/llama-4-scout-17b-16e-instruct";
    public double Temperature { get; set; } = 0.7;
}
